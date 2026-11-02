# users/models.py
from datetime import datetime
from typing import Optional, List
from sqlalchemy import BigInteger, Integer, String, DateTime, ForeignKey, func,Text
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


class Role(Base):
    """
    جدول الأدوار والصلاحيات في النظام.
    - يحدد نوع الحساب ومستوى الوصول (مثل: مدير نظام Admin، طبيب Doctor، ممرض Nurse، موظف استقبال Receptionist، مريض Patient).
    """
    __tablename__ = "tbRoles"

    RoleId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    RoleName: Mapped[str] = mapped_column(String(100), nullable=False)
    RoleDescription: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    RoleStatus: Mapped[str] = mapped_column(String(50), default="Active")
    RoleLevel: Mapped[Optional[int]] = mapped_column(Integer, nullable=True)  # الرتبة أو المستوى الإداري
    RoleCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # علاقة: الدور الواحد يمكن أن يرتبط به عدة مستخدمين
    users: Mapped[List["User"]] = relationship("User", back_populates="role")


class User(Base):
    """
    جدول المستخدمين الأساسي لكافة أطراف النظام.
    - يحتوي على بيانات الحساب والتحقق وتفاصيل الاتصال لكافة الفئات (أطباء، إداريين، مرضى).
    - يرتبط بدور محدد (Role) وتتفرع منه ملفات الموظفين أو المرضى أو المنشآت.
    """
    __tablename__ = "Users"

    UserId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    RoleId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbRoles.RoleId"), nullable=False, index=True)
    UserFullName: Mapped[str] = mapped_column(String(100), nullable=False)
    Username: Mapped[str] = mapped_column(String(50), unique=True, nullable=False, index=True)
    UserPhone: Mapped[Optional[str]] = mapped_column(String(20), nullable=True)
    UserEmail: Mapped[Optional[str]] = mapped_column(String(100), unique=True, nullable=True)
    Password: Mapped[str] = mapped_column(String(100), nullable=False)
    UserStatus: Mapped[str] = mapped_column(String(50), default="Active")
    UserAddress: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    CreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # علاقة: المستخدم يمتلك دوراً واحداً
    role: Mapped["Role"] = relationship("Role", back_populates="users")

        # قائمة بجميع المستندات والملفات التي تخص هذا المستخدم
    documents: Mapped[List["Document"]] = relationship(
        "Document",
        foreign_keys="[Document.user_id]",
        back_populates="user",
        cascade="all, delete-orphan",
    )




class Document(Base):
    """
    جدول المستندات الموحد المربوط بالمستخدمين (Universal Documents)
    - يربط جميع أنواع الملفات (أشعة، تقارير، هويات، عقود، شهادات) بملف المستخدم مباشرة.
    """
    __tablename__ = "tbDocuments"

    document_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للمستند

    user_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, index=True)
    # معرف المستخدم صاحب المستند (سواء كان مريضاً، طبيباً، أو موظفاً)

    title: Mapped[str] = mapped_column(String(150), nullable=False)
    # اسم أو عنوان المستند (مثل: صورة الهوية، تقرير رنين مغناطيسي، شهادة مزاولة المهنة)

    document_type: Mapped[str] = mapped_column(String(50), nullable=False)
    # تصنيف نوع المستند (مثل: MedicalReport تقرير طبي، XRay أشعة، Contract عقد، Identity هوية)

    file_url: Mapped[str] = mapped_column(String(255), nullable=False)
    # مسار أو رابط حفظ الملف على السيرفر لتنزيله وعرضه

    file_mime_type: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # صيغة وامتداد الملف البرمجي (مثل: application/pdf، image/jpeg، image/png)

    file_size_kb: Mapped[Optional[int]] = mapped_column(Integer, nullable=True)
    # حجم الملف بالكيلوبايت

    visit_id: Mapped[Optional[int]] = mapped_column(BigInteger, ForeignKey("tbVisits.visit_id"), nullable=True, index=True)
    # معرف الزيارة الطبية إن كان المستند مرتبطاً بكشف طبي محدد (مثل أشعة أثناء زيارة)

    uploaded_by: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False)
    # معرف الموظف أو المستخدم الذي قام برفع الملف فعلياً إلى النظام لتوثيق المسؤولية

    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    # شرح أو ملاحظات إضافية حول محتوى الملف

    uploaded_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت رفع الملف في النظام

    # 1. علاقة المستند مع صاحبه (المريض/الطبيب)
    user: Mapped["User"] = relationship(
        "User", foreign_keys=[user_id], back_populates="documents"
    )