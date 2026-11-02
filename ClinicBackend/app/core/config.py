from pydantic_settings import BaseSettings, SettingsConfigDict

class Settings(BaseSettings):
    """
    هذا الكلاس يحتوي على إعدادات ومتغيرات المشروع الأساسية.
    يقوم بفصل تهيئة النظام عن الكود البرمجي وقراءتها من ملف البيئة (.env).
    """
    
    # يتوقع النظام هنا وجود إدخال باسم (DATABASE_URL) يمثل رابط الاتصال بقاعدة بيانات PostgreSQL
    DATABASE_URL: str

    # المفتاح السري المستخدم لتشفير وفك تشفير الـ JWT Tokens لحماية الحسابات
   # SECRET_KEY: str

    # خوارزمية التشفير القياسية المستخدمة في حماية الـ Token
   # ALGORITHM: str = "HS256"

    # مدة صلاحية الـ Token بالدقائق قبل أن يضطر المستخدم لتسجيل الدخول مجدداً
   # ACCESS_TOKEN_EXPIRE_MINUTES: int = 30

    # هذا القاموس يحدد لـ pydantic_settings كيفية التعامل وقراءة ملف المتغيرات البيئية
    model_config = SettingsConfigDict(
        # قراءة الإعدادات والمتغيرات من ملف (.env) المحلي
        env_file=".env",
        
        # ضبط ترميز ملف البيئة إلى UTF-8 لضمان قراءة الرموز والنصوص بشكل صحيح دون أخطاء
        env_file_encoding="utf_8",
        
        # تجاهل أي متغيرات إضافية موجودة في ملف .env وغير معرفة داخل الكلاس لمنع انهيار السيرفر
        extra="ignore"
    )

# إنشاء نسخة مفردة وثابتة (Singleton) من الإعدادات لاستدعائها في كافة أنحاء النظام
settings = Settings()
