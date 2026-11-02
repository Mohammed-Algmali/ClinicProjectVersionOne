# auth/router.py
from datetime import timedelta
from fastapi import APIRouter, Depends, HTTPException, status
from fastapi.security import OAuth2PasswordRequestForm
from sqlalchemy.orm import Session

from app.core.database import get_db
from app.modules.users.models import User
from app.modules.auth.schemas import Token, LoginRequest
from app.modules.auth.utils import verify_password, create_access_token, ACCESS_TOKEN_EXPIRE_MINUTES

# ==============================================================================
# 1. تهيئة موجه مسارات المصادقة (Auth Router)
# ==============================================================================
router = APIRouter(
    prefix="/auth",
    tags=["المصادقة والتوثيق (Authentication)"]
)
# إنشاء مسار رئيسي بادئته /auth ومصنف في واجهة Swagger


# ==============================================================================
# 2. مسار تسجيل الدخول المتوافق مع Swagger UI (OAuth2 Form)
# ==============================================================================
@router.post(
    "/token",
    response_model=Token,
    summary="تسجيل الدخول لتوثيق Swagger UI"
)
def login_for_swagger_token(
    form_data: OAuth2PasswordRequestForm = Depends(),
    db: Session = Depends(get_db)
):
    """
    مسار مخصص لزر الـ Authorize في واجهة التوثيق Swagger
    """
    # 1. البحث عن المستخدم بواسطة اسم المستخدم المدخل
    user = db.query(User).filter(User.Username == form_data.username).first()
    # جلب سجل المستخدم من جدول Users

    # 2. التحقق من وجود المستخدم وصحة كلمة المرور
    if not user or not verify_password(form_data.password, user.Password):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="اسم المستخدم أو كلمة المرور غير صحيحة",
            headers={"WWW-Authenticate": "Bearer"},
        )
        # إرجاع خطأ 401 في حال عدم التطابق

    # 3. التأكد من أن حساب المستخدم نشط وغير معطل
    if user.UserStatus != "Active":
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="هذا الحساب موقوف أو غير نشط. يرجى مراجعة إدارة النظام."
        )
        # منع تسجيل الدخول للحسابات المجمدة

    # 4. توليد رمز الدخول المشفر (JWT)
    access_token_expires = timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    # حساب مدة صلاحية الرمز

    access_token = create_access_token(
        data={
            "sub": user.Username,
            "user_id": user.UserId,
            "role_id": user.RoleId
        },
        expires_delta=access_token_expires
    )
    # تشفير المعرفات الأساسية داخل التوكن

    return {
        "access_token": access_token,
        "token_type": "bearer"
    }
    # إرجاع التوكن للواجهة


# ==============================================================================
# 3. مسار تسجيل الدخول القياسي عبر JSON (للفروونت إند والموبايل)
# ==============================================================================
@router.post(
    "/login",
    response_model=Token,
    summary="تسجيل الدخول القياسي عبر JSON"
)
def login(
    login_data: LoginRequest,
    db: Session = Depends(get_db)
):
    """
    مسار تسجيل الدخول المباشر المخصص للفرونت إند (React / Flutter / Vue)
    """
    # 1. البحث عن المستخدم
    user = db.query(User).filter(User.Username == login_data.username).first()
    # الاستعلام عن الحساب بالاسم

    # 2. فحص كلمة المرور
    if not user or not verify_password(login_data.password, user.Password):
        raise HTTPException(
            status_code=status.HTTP_401_UNAUTHORIZED,
            detail="اسم المستخدم أو كلمة المرور غير صحيحة"
        )
        # رفض الطلب إذا كانت البيانات غير متطابقة

    # 3. فحص حالة الحساب
    if user.UserStatus != "Active":
        raise HTTPException(
            status_code=status.HTTP_403_FORBIDDEN,
            detail="هذا الحساب موقوف أو غير نشط."
        )
        # حظر الحسابات غير المفعلة

    # 4. توليد التوكن
    access_token = create_access_token(
        data={
            "sub": user.Username,
            "user_id": user.UserId,
            "role_id": user.RoleId
        }
    )
    # إنشاء رمز JWT ساري المفعول

    return {
        "access_token": access_token,
        "token_type": "bearer"
    }
    # إرجاع رمز الوصول