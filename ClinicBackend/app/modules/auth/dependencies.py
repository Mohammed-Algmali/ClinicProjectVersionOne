from fastapi import Depends, HTTPException, status
from fastapi.security import OAuth2PasswordBearer
from jose import JWTError, jwt
from sqlalchemy.orm import Session

from app.core.database import get_db
from app.modules.users.models import User
# استيراد إعدادات ومفاتيح التشفير من ملف utils أو config لديك
from app.modules.auth.utils import SECRET_KEY, ALGORITHM

# يُحدد لـ FastAPI من أين يتوقع استلام التوكن (من مسار /auth/login)
oauth2_scheme = OAuth2PasswordBearer(tokenUrl="/auth/login")



def get_current_user(
    token: str = Depends(oauth2_scheme),
    db: Session = Depends(get_db)
) -> User:
    """
    التبعية المركزية لحماية المسارات:
    1. تستخرج التوكن من ترويسة الطلب (Authorization: Bearer ...).
    2. تفك تشفيره وتتحقق من صحته ومدة صلاحيته.
    3. تستخرج UserId وتبحث عن المستخدم في قاعدة البيانات.
    4. تُرجع كائن المستخدم (User) حاملاً رقم منشأته FacilityId وصلاحياته.
    """
    credentials_exception = HTTPException(
        status_code=status.HTTP_401_UNAUTHORIZED,
        detail="تعذر التحقق من صحة تصريح المرور (التوكن غير صالح أو منتهي الصلاحية).",
        headers={"WWW-Authenticate": "Bearer"},
    )

    try:
        # فك تشفير التوكن وقراءة البيانات المشفرة بداخله (Payload)
        payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
        
        # استخراج اسم المستخدم أو رقمه (بحسب ما وضعته عند توليد التوكن)
        username: str = payload.get("sub")
        user_id: int = payload.get("user_id")

        if username is None and user_id is None:
            raise credentials_exception

    except JWTError:
        raise credentials_exception

    # البحث عن المستخدم في قاعدة البيانات للتأكد من وجوده وأنه نشط
    user = None
    if user_id:
        user = db.query(User).filter(User.UserId == user_id).first()
    elif username:
        user = db.query(User).filter(User.Username == username).first()

    if user is None:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail="المستخدم المرتبط بهذا التوكن غير موجود في النظام."
        )

    if user.UserStatus != "Active":
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="حساب المستخدم غير نشط أو موقوف مؤقتاً."
        )

    # إرجاع كائن المستخدم بكافة بياناته ومنشأته
    return user