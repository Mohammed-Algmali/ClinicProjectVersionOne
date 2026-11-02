# appointments/models.py
from datetime import datetime
from typing import Optional
from sqlalchemy import (
    BigInteger,
    String,
    DateTime,
    ForeignKey,
    func,
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


class Appointment(Base):
    """
    جدول مواعيد وحجوزات المرضى مع الأطباء
    """
    __tablename__ = "tbAppointments"

    appointment_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للموعد المحجوز

    medical_file_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbMedicalFiles.medical_file_id"), nullable=False, index=True)
    # معرف الملف الطبي للمريض صاحب الحجز

    Employee_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbEmployees.EmployeeId"), nullable=False, index=True)
    # معرف الطبيب المحجوز عنده الموعد

    appointment_datetime: Mapped[datetime] = mapped_column(DateTime, nullable=False, index=True)
    # تاريخ ووقت الموعد المحجوز بالساعة والدقيقة

    appointment_status: Mapped[str] = mapped_column(String(50), default="Scheduled")
    # حالة الموعد: مجدول (Scheduled)، مكتمل وحضر (Completed)، ملغي (Cancelled)، أو لم يحضر (No-Show)

    notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # سبب الحجز أو ملاحظات موظف الاستقبال والمريض

    create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إجراء الحجز في النظام