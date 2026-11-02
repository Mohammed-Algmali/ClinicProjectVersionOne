# prescriptions/models.py
from datetime import datetime
from typing import Optional, List
from sqlalchemy import (
    BigInteger,
    String,
    DateTime,
    ForeignKey,
    func,
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


# ==============================================================================
# 1. جدول دليل الأدوية (tbMedicines)
# ==============================================================================
class Medicine(Base):
    """
    دليل الأدوية والمستحضرات الصيدلانية المعتمدة في النظام
    """
    __tablename__ = "tbMedicines"

    Medicen_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للدواء في الدليل الدوائي

    Name: Mapped[str] = mapped_column(String(100), nullable=False, index=True)
    # الاسم العلمي أو التجاري للدواء (مثل: Amoxicillin أو Panadol)

    Strangth: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # تركيز وقوة المادة الفعالة في الدواء (مثل: 500mg أو 250mg/5ml)

    Dosage_from: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # الشكل الصيدلاني للدواء (مثل: أقراص Tablets، شراب Syrup، حقن Injection، مرهم Ointment)

    # العلاقات
    prescription_items: Mapped[List["PrescriptionItem"]] = relationship("PrescriptionItem", back_populates="medicine")


# ==============================================================================
# 2. جدول الوصفات الطبية / الروشتات (tbPrescriptions)
# ==============================================================================
class Prescription(Base):
    """
    جدول الروشتات الصادرة عن الطبيب في الزيارة
    """
    __tablename__ = "tbPrescriptions"

    Prescription_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للروشتة الطبية

    Visit_is: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbVisits.visit_id"), nullable=False, index=True)
    # معرف الزيارة السريرية التي قام فيها الطبيب بكتابة هذه الوصفة للمريض

    PrescriptionNote: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات الطبيب العامة وتوجيهاته الإضافية للمريض أو الصيدلي

    # العلاقات
    items: Mapped[List["PrescriptionItem"]] = relationship("PrescriptionItem", back_populates="prescription", cascade="all, delete-orphan")


# ==============================================================================
# 3. جدول بنود وتفاصيل الأدوية في الروشتة (tbPrescriptionItems)
# ==============================================================================
class PrescriptionItem(Base):
    """
    جدول الأدوية الفردية وجرعاتها داخل كل روشتة
    """
    __tablename__ = "tbPrescriptionItems"

    Prescription_item_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لبند الدواء داخل الروشتة

    Prescription_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbPrescriptions.Prescription_id"), nullable=False, index=True)
    # معرف الروشتة التابع لها هذا الدواء

    Medicen_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbMedicines.Medicen_id"), nullable=False, index=True)
    # معرف الدواء المختار من دليل الأدوية

    Dosage: Mapped[str] = mapped_column(String(50), nullable=False)
    # كمية ومقدار الجرعة في المرة الواحدة (مثل: قرص واحد، ملعقة 5 مل، أمبولة واحدة)

    Frequncy: Mapped[str] = mapped_column(String(50), nullable=False)
    # تكرار أخذ الدواء يومياً (مثل: مرتين يومياً كل 12 ساعة، 3 مرات بعد الأكل، أو عند اللزوم)

    Duration: Mapped[str] = mapped_column(String(50), nullable=False)
    # فترة العلاج المستمرة (مثل: لمدة 5 أيام، لمدة أسبوع، أو علاج دائم للأمراض المزمنة)

    Instruction: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # تعليمات وطريقة الاستخدام الخاصة (مثل: قبل الإفطار بنصف ساعة، رج الزجاجة جيداً، تجنب منتجات الألبان)

    # العلاقات
    prescription: Mapped["Prescription"] = relationship("Prescription", back_populates="items")
    medicine: Mapped["Medicine"] = relationship("Medicine", back_populates="prescription_items")