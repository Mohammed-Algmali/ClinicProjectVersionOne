from datetime import date, datetime, time
from decimal import Decimal
from typing import Optional, List
from sqlalchemy import (
    BigInteger,
    Integer,
    String,
    Text,
    DateTime,
    Date,
    Time,
    Boolean,
    Numeric,
    ForeignKey,
    func,
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base





class BiometricCredential(Base):
    """
    نموذج بيانات البصمات الحيوية والذكاء الاصطناعي (tbBiometrics):
    المبدأ الهندسي والأمني:
    1. دعم تقنيات تسجيل الحضور الذكي غير التلامسي (Contactless Check-in) عبر الذكاء الاصطناعي (التعرف على الوجه).
    2. معيار الخصوصية الطبي: لا نقوم بحفظ الصور الأصلية لوجوه الموظفين لحماية خصوصيتهم،
       بل نخزن فقط مصفوفات التشفير الرقمية (128-D أو 512-D Face Embeddings) الناتجة عن نماذج الذكاء الاصطناعي.
    3. قابلية التوسع (Extensibility): يدعم النظام أنواع بصمات متعددة لنفس الموظف (وجه، إصبع، بطاقة RFID).
    """
    __tablename__ = "tbBiometrics"

    # =========================================================================
    # 1. المفتاح الأساسي والربط بالموظف
    # =========================================================================

    BiometricId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي الفريد لكل قالب بصمة في النظام

    UserId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId", ondelete="CASCADE"), nullable=False, index=True)
    # المفتاح الأجنبي الذي يربط قالب البصمة بالموظف صاحبها في جدول tbEmployees
    # ondelete="CASCADE": يضمن حذف بيانات البصمات تلقائياً عند حذف سجل الموظف


    # =========================================================================
    # 2. نوع البصمة والبيانات الرقمية المشفرة
    # =========================================================================

    BiometricType: Mapped[str] = mapped_column(String(50), nullable=False)
    # يحدد تصنيف البصمة:
    # - "face"        : بصمة وجه تم التقاطها عبر كاميرا العيادة الذكية
    # - "fingerprint" : بصمة إصبع مسحوبة من جهاز القارئ المكتبي (Biometric Scanner)
    # - "rfid"        : كود البطاقة الذكية للموظف

    TemplateData: Mapped[str] = mapped_column(Text, nullable=False)
    # محتوى البصمة المشفر كبيانات نصية ضخمة (Payload):
    # - في حالة الوجه: يُخزن نص JSON يحتوي على مصفوفة الإحداثيات الرياضية (Face Encodings Vector)
    # - في حالة الإصبع: يُخزن الـ Template المشفر (Base64 String) القادم من مكتبة الـ SDK الخاصة بالجهاز
    # - في حالة RFID: يُخزن الكود الرقمي التسلسلي المشفر للبطاقة


    # =========================================================================
    # 3. التحكم بالحالة وسجلات التدقيق (Status & Audit)
    # =========================================================================

    IsActive: Mapped[bool] = mapped_column(Boolean, default=True)
    # خيار التفعيل والتعطيل:
    # يتيح للإدارة إيقاف بصمة موظف مؤقتاً (مثلاً: عند إيقافه عن العمل) دون الحاجة لحذف بياناته


    CreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت تسجيل البصمة لأول مرة في النظام بواسطة التوقيت المرجعي لقاعدة البيانات


    # =========================================================================
    # 4. العلاقات (Relationships)
    # =========================================================================

    user: Mapped["User"] = relationship(
        "User",
        back_populates="biometrics"
    )
    # علاقة ثنائية عكسية تتيح للسيرفر معرفة صاحب البصمة فور مطابقة الوجه بالكاميرا
