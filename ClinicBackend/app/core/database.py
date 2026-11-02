from sqlalchemy import create_engine
from sqlalchemy.orm import sessionmaker, DeclarativeBase
from app.core.config import settings

# 1. إنشاء محرك الاتصال الفعلي بين تطبيق بايثون وقاعدة البيانات باستخدام الرابط المعرف
engine = create_engine(settings.DATABASE_URL)

# 2. إنشاء مصنع الجلسات (Session Factory) المسؤول عن تنظيم وإرسال الأوامر لقاعدة البيانات
# - autocommit=False: عدم حفظ التغييرات تلقائياً حتى تنفيذ أمر commit صريح لحماية البيانات.
# - autoflush=False: عدم إرسال التغييرات فوراً أثناء تنفيذ الكود والانتظار حتى طلب ذلك برمجياً.
# - bind=engine: ربط كافة الجلسات بمحرك الاتصال الذي أنشأناه.
SessionLocal = sessionmaker(
    autocommit=False,
    autoflush=False,
    bind=engine
)

# 3. الكلاس الأب لجميع الجداول (Models) التي سننشئها لاحقاً لتمثيلها في قاعدة البيانات
class Base(DeclarativeBase):
    pass

# 4. دالة وسيطة (Dependency) لفتح وإغلاق جلسة قاعدة البيانات تلقائياً مع كل طلب (Request)
def get_db():
    db = SessionLocal()
    try:
        yield db
    finally:
        db.close()
