# laboratories/models.py
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
# 1. جدول دليل الفحوصات والتحاليل (tbLabTests)
# ==============================================================================
class LabTest(Base):
    """
    دليل الفحوصات والتحاليل الطبية الشاملة في المختبر
    """
    __tablename__ = "tbLabTests"

    lab_test_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للتحليل الطبي

    test_name: Mapped[str] = mapped_column(String(100), nullable=False, index=True)
    # اسم التحليل الطبي الرسمي (مثل: Complete Blood Count - CBC، سكر صائم، وظائف كلى)

    test_descrption: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # وصف التحليل، دواعي إجرائه، ونوع العينة المطلوبة (دم، بول، مسحة)

    test_code: Mapped[Optional[str]] = mapped_column(String(50), unique=True, nullable=True)
    # الكود المخبري المختصر أو الكود الدولي للفحص (LOINC Code)

    # العلاقات
    components: Mapped[List["LabTestComponent"]] = relationship("LabTestComponent", back_populates="lab_test", cascade="all, delete-orphan")
    order_items: Mapped[List["LabOrderItem"]] = relationship("LabOrderItem", back_populates="lab_test")


# ==============================================================================
# 2. جدول مكونات وعناصر التحليل (tbLabTestComponents)
# ==============================================================================
class LabTestComponent(Base):
    """
    جدول العناصر والمؤشرات المقاسة داخل الفحص الواحد
    """
    __tablename__ = "tbLabTestComponents"

    lab_test_component_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لمكون الفحص

    lab_test_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbLabTests.lab_test_id"), nullable=False, index=True)
    # معرف التحليل الرئيسي الذي يتبع له هذا المكون

    component_name: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم العنصر المقاس (مثل: الهيموجلوبين Hemoglobin، كريات الدم البيضاء WBC)

    component_code: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # كود أو اختصار المكون الطبي (مثل: Hb, Platelets)

    unit: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # وحدة القياس المعيارية للنتيجة (مثل: g/dL، mg/dL، mm/hr)

    default_reference_range: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # المعدل والمجال الطبيعي القياسي للشخص السليم (مثل: 13.5 - 17.5 للذكور)

    # العلاقات
    lab_test: Mapped["LabTest"] = relationship("LabTest", back_populates="components")
    results: Mapped[List["LabResult"]] = relationship("LabResult", back_populates="component")


# ==============================================================================
# 3. جدول طلبات الفحص المخبري (tbLabOrders)
# ==============================================================================
class LabOrder(Base):
    """
    جدول أوامر الفحوصات المخبرية الصادرة من الطبيب
    """
    __tablename__ = "tbLabOrders"

    lab_order_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لطلب التحليل

    visit_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbVisits.visit_id"), nullable=False, index=True)
    # معرف الزيارة السريرية التي تم فيها طلب هذه الفحوصات للمريض

    order_date: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إصدار طلب الفحص من الطبيب

    description: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات الطبيب الاسترشادية لفني المختبر (مثل: المريض صائم 8 ساعات، فحص عاجل STAT)

    order_status: Mapped[str] = mapped_column(String(50), default="Pending")
    # حالة الطلب: قيد الانتظار (Pending)، قيد المعالجة (In-Progress)، أو مكتمل (Completed)

    # العلاقات
    items: Mapped[List["LabOrderItem"]] = relationship("LabOrderItem", back_populates="lab_order", cascade="all, delete-orphan")


# ==============================================================================
# 4. جدول بنود وفحوصات الطلب (tbLabOrderItems)
# ==============================================================================
class LabOrderItem(Base):
    """
    جدول التحاليل الفردية المطلوبة داخل أمر الفحص الواحد
    """
    __tablename__ = "tbLabOrderItems"

    labl_order_item_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لبند الفحص المطلوب

    lab_order_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbLabOrders.lab_order_id"), nullable=False, index=True)
    # معرف طلب الفحص الشامل التابع له هذا البند

    lab_test_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbLabTests.lab_test_id"), nullable=False, index=True)
    # معرف الفحص المطلوب إجراؤه من دليل التحاليل

    order_status: Mapped[str] = mapped_column(String(50), default="Pending")
    # حالة الفحص المحدد: بانتظار العينة (Sample Required)، جاري التحليل، أو صُدّقت النتيجة

    # العلاقات
    lab_order: Mapped["LabOrder"] = relationship("LabOrder", back_populates="items")
    lab_test: Mapped["LabTest"] = relationship("LabTest", back_populates="order_items")
    results: Mapped[List["LabResult"]] = relationship("LabResult", back_populates="order_item", cascade="all, delete-orphan")


# ==============================================================================
# 5. جدول النتائج المخبرية (tbLabResults)
# ==============================================================================
class LabResult(Base):
    """
    جدول النتائج المخبرية المسجلة لكل عنصر فحص
    """
    __tablename__ = "tbLabResults"

    lab_result_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسجل النتيجة المخبرية

    lab_order_item_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbLabOrderItems.labl_order_item_id"), nullable=False, index=True)
    # معرف بند الفحص المرتبط بهذه النتيجة

    lab_test_component_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbLabTestComponents.lab_test_component_id"), nullable=False, index=True)
    # معرف المكون أو المؤشر المحدد الذي تم قياس نتيجته

    result_value: Mapped[str] = mapped_column(String(50), nullable=False)
    # القيمة الرقمية أو الوصفية المقاسة فعلياً في المختبر (مثل: 14.2 أو Negative أو Positive)

    unit: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # وحدة قياس النتيجة الفعلية المستخدمة في جهاز التحليل

    reference_range: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # النطاق الطبيعي الخاص بعمر وجنس المريض وقت إجراء الفحص

    result_status: Mapped[str] = mapped_column(String(50), default="Normal")
    # تقييم النتيجة المخبرية: طبيعي (Normal)، مرتفع (High)، منخفض (Low)، أو حرج (Critical)

    result_date: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إدخال واعتماد النتيجة من قبل أخصائي المختبر

    # العلاقات
    order_item: Mapped["LabOrderItem"] = relationship("LabOrderItem", back_populates="results")
    component: Mapped["LabTestComponent"] = relationship("LabTestComponent", back_populates="results")