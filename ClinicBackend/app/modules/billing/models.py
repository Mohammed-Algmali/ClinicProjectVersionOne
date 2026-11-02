# billing/models.py
from datetime import datetime
from decimal import Decimal
from typing import Optional, List
from sqlalchemy import (
    BigInteger,
    Integer,
    String,
    Text,
    DateTime,
    Numeric,
    ForeignKey,
    func,
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


# ==============================================================================
# 1. جدول فواتير المرضى (tbPatientInvoices)
# ==============================================================================
class PatientInvoice(Base):
    """
    جدول فواتير المرضى الرئيسية للخدمات الطبية والزيارات
    """
    __tablename__ = "tbPatientInvoices"

    invoice_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للفاتورة

    invoice_number: Mapped[str] = mapped_column(String(50), unique=True, nullable=False, index=True)
    # رقم الفاتورة المالي/الضريبي المطبوع للمريض (مثل: INV-2026-0001)

    patient_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("patient.patient_id"), nullable=False, index=True)
    # معرف المريض الصادرة باسمه الفاتورة

    visit_id: Mapped[Optional[int]] = mapped_column(BigInteger, ForeignKey("tbVisits.visit_id"), nullable=True, index=True)
    # معرف الزيارة الطبية المرتبطة بهذه الفاتورة

    total_amount: Mapped[Decimal] = mapped_column(Numeric(10, 2), default=0.0, nullable=False)
    # إجمالي قيمة الخدمات قبل الخصم

    discount_amount: Mapped[Decimal] = mapped_column(Numeric(10, 2), default=0.0, nullable=False)
    # مبلغ الخصم الممنوح للمريض إن وُجد

    tax_amount: Mapped[Decimal] = mapped_column(Numeric(10, 2), default=0.0, nullable=False)
    # قيمة الضريبة إن كانت مطبقة

    net_amount: Mapped[Decimal] = mapped_column(Numeric(10, 2), default=0.0, nullable=False)
    # الصافي المطلوب سداده (الإجمالي - الخصم + الضريبة)

    paid_amount: Mapped[Decimal] = mapped_column(Numeric(10, 2), default=0.0, nullable=False)
    # إجمالي المبلغ الذي تم سداده فعلياً حتى الآن

    remaining_amount: Mapped[Decimal] = mapped_column(Numeric(10, 2), default=0.0, nullable=False)
    # المبلغ المتبقي على المريض (آجل/دين)

    payment_status: Mapped[str] = mapped_column(String(50), default="Unpaid")
    # حالة سداد الفاتورة: غير مدفوعة (Unpaid)، مدفوعة جزئياً (Partially Paid)، أو مدفوعة بالكامل (Paid)

    invoice_date: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إصدار الفاتورة

    # العلاقات
    items: Mapped[List["PatientInvoiceItem"]] = relationship("PatientInvoiceItem", back_populates="invoice", cascade="all, delete-orphan")
    payments: Mapped[List["PatientPayment"]] = relationship("PatientPayment", back_populates="invoice", cascade="all, delete-orphan")


# ==============================================================================
# 2. جدول بنود وتفاصيل الفاتورة (tbPatientInvoiceItems)
# ==============================================================================
class PatientInvoiceItem(Base):
    """
    جدول تفاصيل الخدمات المسعرة داخل فاتورة المريض
    """
    __tablename__ = "tbPatientInvoiceItems"

    invoice_item_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لبند الفاتورة

    invoice_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbPatientInvoices.invoice_id"), nullable=False, index=True)
    # معرف الفاتورة التابع لها هذا البند

    item_type: Mapped[str] = mapped_column(String(50), nullable=False)
    # نوع البند المحاسبي: كشفية (Consultation)، تحليل مختبر (Lab)، دواء (Medicine)، أو عملية/ضماد (Procedure)

    item_reference_id: Mapped[Optional[int]] = mapped_column(BigInteger, nullable=True)
    # معرف الخدمة الفعلي في جدولها الأصلي (مثل: lab_test_id أو Medicen_id)

    item_name: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم الخدمة المطبوع في الفاتورة (مثل: كشف طبيب استشاري، فحص دم شامل CBC، دواء بنادول)

    unit_price: Mapped[Decimal] = mapped_column(Numeric(10, 2), nullable=False)
    # سعر الوحدة الواحدة من الخدمة أو الدواء

    quantity: Mapped[int] = mapped_column(Integer, default=1, nullable=False)
    # الكمية أو عدد مرات تقديم الخدمة

    total_price: Mapped[Decimal] = mapped_column(Numeric(10, 2), nullable=False)
    # الإجمالي لهذا البند (سعر الوحدة × الكمية)

    # العلاقات
    invoice: Mapped["PatientInvoice"] = relationship("PatientInvoice", back_populates="items")


# ==============================================================================
# 3. جدول سندات القبض والدفعات المالية للمريض (tbPatientPayments)
# ==============================================================================
class PatientPayment(Base):
    """
    جدول سندات استلام المبالغ المالية من المريض (Receipts)
    """
    __tablename__ = "tbPatientPayments"

    payment_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسند القبض

    invoice_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbPatientInvoices.invoice_id"), nullable=False, index=True)
    # معرف الفاتورة التي تم السداد لحسابها

    receipt_number: Mapped[str] = mapped_column(String(50), unique=True, nullable=False, index=True)
    # رقم سند القبض الورقي/الإلكتروني المسلم للمريض (مثل: REC-2026-054)

    amount_paid: Mapped[Decimal] = mapped_column(Numeric(10, 2), nullable=False)
    # المبلغ المدفوع في هذه الدفعة

    payment_method: Mapped[str] = mapped_column(String(50), default="Cash")
    # طريقة الدفع: نقداً (Cash)، بطاقة شبكة/ائتمان (Card)، أو محفظة إلكترونية/تحويل بنكي (Transfer)

    transaction_reference: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # رقم العملية البنكية أو إشعار التحويل في حال الدفع الإلكتروني

    cashier_user_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False)
    # موظف الصندوق أو الكاشير الذي استلم المبلغ وأصدر السند

    payment_date: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ وتوقيت استلام المبلغ بالثواني

    notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات على عملية القبض (مثل: دفعة أولى تحت الحساب)

    # العلاقات
    invoice: Mapped["PatientInvoice"] = relationship("PatientInvoice", back_populates="payments")