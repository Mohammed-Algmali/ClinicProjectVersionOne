# # auth/utils.py
# from datetime import datetime, timedelta
# from typing import Optional, Any, Dict
# from jose import jwt, JWTError
# from passlib.context import CryptContext

# # ==============================================================================
# # 1. إعدادات الأمان والتشفير (Security Configurations)
# # ==============================================================================
# SECRET_KEY = "CHANGE_THIS_TO_A_VERY_SECURE_RANDOM_SECRET_KEY_IN_PRODUCTION"
# # المفتاح السري لتوقيع رموز الـ JWT وتشفيرها لمنع تزويرها

# ALGORITHM = "HS256"
# # خوارزمية التشفير القياسية المعتمدة لتوقيع الرمز

# ACCESS_TOKEN_EXPIRE_MINUTES = 60 * 24
# # مدة صلاحية رمز الدخول بالدقائق (هنا محددة بـ 24 ساعة)

# pwd_context = CryptContext(schemes=["bcrypt"], deprecated="auto")
# # تهيئة محرك التشفير باستخدام خوارزمية bcrypt العالمية الآمنة


# # ==============================================================================
# # 2. دوال التعامل مع كلمات المرور (Password Hashing)
# # ==============================================================================
# def verify_password(plain_password: str, hashed_password: str) -> bool:
#     """
#     مقارنة كلمة المرور المدخلة من المستخدم مع الهاش المشفر في قاعدة البيانات
#     """
#     return pwd_context.verify(plain_password, hashed_password)
#     # ترجع True إذا كانت كلمة المرور صحيحة، أو False إذا كانت خاطئة


# def get_password_hash(password: str) -> str:
#     """
#     تشفير كلمة المرور الجديدة قبل حفظها في قاعدة البيانات
#     """
#     return pwd_context.hash(password)
#     # ترجع نصاً مشفراً غير قابل للفك عبر خوارزمية bcrypt


# # ==============================================================================
# # 3. دوال توليد وفك تشفير الـ JWT (Token Operations)
# # ==============================================================================
# def create_access_token(data: Dict[str, Any], expires_delta: Optional[timedelta] = None) -> str:
#     """
#     توليد رمز JWT مشفر يحتوي على بيانات المستخدم وتاريخ انتهاء الصلاحية
#     """
#     to_encode = data.copy()
#     # أخذ نسخة من البيانات المراد تخزينها داخل الرمز (مثل: user_id و username)

#     if expires_delta:
#         expire = datetime.utcnow() + expires_delta
#         # استخدام مدة صلاحية مخصصة إن تم تمريرها
#     else:
#         expire = datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
#         # حساب وقت انتهاء الصلاحية بناءً على الإعدادات الافتراضية

#     to_encode.update({"exp": expire})
#     # إضافة وقت انتهاء الصلاحية داخل حمولة الرمز (Payload)

#     encoded_jwt = jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)
#     # تشفير البيانات وإنتاج نص الرمز النهائي

#     return encoded_jwt


# def decode_access_token(token: str) -> Optional[Dict[str, Any]]:
#     """
#     فك تشفير رمز الـ JWT والتأكد من صحته وصلاحيته
#     """
#     try:
#         payload = jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
#         # فك تشفير الرمز باستخدام نفس المفتاح السري والخوارزمية

#         return payload
#         # إرجاع البيانات المخزنة داخل الرمز إذا كان الرمز سليماً وساري المفعول

#     except JWTError:
#         return None
#         # إرجاع None في حال كان الرمز مزوراً أو منتهي الصلاحية

# auth/utils.py
from datetime import datetime, timedelta
from typing import Optional, Any, Dict
from jose import jwt, JWTError
import bcrypt

# ==============================================================================
# 1. إعدادات الأمان والتشفير (Security Configurations)
# ==============================================================================
SECRET_KEY = "CHANGE_THIS_TO_A_VERY_SECURE_RANDOM_SECRET_KEY_IN_PRODUCTION"
ALGORITHM = "HS256"
ACCESS_TOKEN_EXPIRE_MINUTES = 60 * 24


# ==============================================================================
# 2. دوال التشفير باستخدام bcrypt مباشرة (آمنة وبدون قيود passlib)
# ==============================================================================
def get_password_hash(password: str) -> str:
    """
    تشفير كلمة المرور وتحويلها إلى Hash عبر خوارزمية bcrypt
    """
    # تحويل النص إلى بايت واقتصاصه لأول 72 بايت كحد أقصى وفق معيار bcrypt
    pwd_bytes = password.encode("utf-8")[:72]
    # توليد الملح (Salt) العشوائي
    salt = bcrypt.gensalt()
    # تشفير الكلمة وإرجاعها كنص
    return bcrypt.hashpw(pwd_bytes, salt).decode("utf-8")


def verify_password(plain_password: str, hashed_password: str) -> bool:
    """
    مقارنة كلمة المرور المدخلة مع الهاش المخزن
    """
    pwd_bytes = plain_password.encode("utf-8")[:72]
    hashed_bytes = hashed_password.encode("utf-8")
    return bcrypt.checkpw(pwd_bytes, hashed_bytes)


# ==============================================================================
# 3. دوال الـ JWT (كما هي بدون تغيير)
# ==============================================================================
def create_access_token(data: Dict[str, Any], expires_delta: Optional[timedelta] = None) -> str:
    to_encode = data.copy()
    if expires_delta:
        expire = datetime.utcnow() + expires_delta
    else:
        expire = datetime.utcnow() + timedelta(minutes=ACCESS_TOKEN_EXPIRE_MINUTES)
    to_encode.update({"exp": expire})
    return jwt.encode(to_encode, SECRET_KEY, algorithm=ALGORITHM)


def decode_access_token(token: str) -> Optional[Dict[str, Any]]:
    try:
        return jwt.decode(token, SECRET_KEY, algorithms=[ALGORITHM])
    except JWTError:
        return None