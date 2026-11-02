# system_core/models.py
from datetime import datetime
from typing import Optional
from sqlalchemy import (
    BigInteger,
    Integer,
    String,
    Text,
    DateTime,
    Boolean,
    ForeignKey,
    func,
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


# ==============================================================================
# 1. جدول الإشعارات (tbNotifications)
# ==============================================================================
class Notification(Base):
    """
    جدول تنبيهات وإشعارات المستخدمين داخل النظام
    """
    __tablename__ = "tbNotifications"

    notification_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للإشعار

    user_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, index=True)
    # معرف المستخدم (طبيب، مريض، موظف استقبال) المستلم لهذا التنبيه

    title: Mapped[str] = mapped_column(String(100), nullable=False)
    # عنوان التنبيه المختصر (مثل: تأكيد موعد، صدور نتيجة تحليل، روشتة جديدة)

    message: Mapped[str] = mapped_column(String(255), nullable=False)
    # النص التفصيلي لرسالة الإشعار التوجيهية

    type: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # تصنيف نوع الإشعار (مثل: Appointment, Laboratory, Billing, SystemAlert)

    # الحقول الإضافية الموصى بها لإدارة الإشعارات برمجياً:
    is_read: Mapped[bool] = mapped_column(Boolean, default=False)
    # هل قام المستخدم بقراءة الإشعار أم لا يزال جديداً (Unread)

    created_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إرسال التنبيه


# ==============================================================================
# 2. جدول سجلات التدقيق والرقابة الأمنية (tbAuditLogs)
# ==============================================================================
class AuditLog(Base):
    """
    سجل تتبع عمليات النظام والرقابة على تعديلات البيانات (Audit Trail)
    """
    __tablename__ = "tbAuditLogs"

    audit_logs_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسجل العملية الرقابية

    user_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, index=True)
    # معرف المستخدم أو الموظف الذي قام بتنفيذ الإجراء

    action: Mapped[str] = mapped_column(String(50), nullable=False)
    # نوع العملية المنفذة (مثل: CREATE إضافة، UPDATE تعديل، DELETE حذف، LOGIN تسجيل دخول)

    table_name: Mapped[str] = mapped_column(String(100), nullable=False, index=True)
    # اسم الجدول الذي تم التعديل عليه (مثل: tbVisits، patient، tbPrescriptions)

    recored_number: Mapped[Optional[int]] = mapped_column(BigInteger, nullable=True)
    # الرقم التعريفي للسجل (Record ID) الذي طرأ عليه التغيير

    old_value: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    # القيمة السابقة للبيانات قبل إجراء التعديل (لمعرفة ماذا تغير)

    new_value: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    # القيمة الجديدة التي تم حفظها بعد التعديل

    create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # التاريخ والوقت الدقيق بالثواني للحظة تنفيذ الإجراء