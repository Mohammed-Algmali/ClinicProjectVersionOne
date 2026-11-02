# human_resources/models.py
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


# ==============================================================================
# 1. جدول الوظائف (tbJobPositions)
# ==============================================================================
class JobPosition(Base):
    """
    جدول المسميات والهياكل الوظيفية في المنشأة الطبية
    """
    __tablename__ = "tbJobPositions"

    JobPositionId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للمسمى الوظيفي

    DepartmentId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbDepartments.DepartmentId"), nullable=False, index=True)
    # معرف القسم الطبي أو الإداري التابعة له هذه الوظيفة

    JobTitle: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم الوظيفة الرسمي مثل: طبيب استشاري، ممرض، موظف استقبال

    JobCode: Mapped[Optional[str]] = mapped_column(String(50), unique=True, nullable=True)
    # كود الوظيفة المختصر مثل: DOC-01 لسهولة الربط والأرشفة

    JobDescription: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    # الوصف الوظيفي والمسؤوليات والمهام المطلوبة من الموظف

    MinSalary: Mapped[Optional[Decimal]] = mapped_column(Numeric(10, 2), nullable=True)
    # الحد الأدنى لسلم الراتب المعتمد لهذه الوظيفة

    MaxSalary: Mapped[Optional[Decimal]] = mapped_column(Numeric(10, 2), nullable=True)
    # الحد الأعلى لسلم الراتب المعتمد لهذه الوظيفة

    IsActive: Mapped[bool] = mapped_column(Boolean, default=True)
    # حالة الوظيفة: هل هي مفعلة ومتاحة للتوظيف حالياً أم مجمدة

    # العلاقات
    employees: Mapped[List["Employee"]] = relationship("Employee", back_populates="job_position")


# ==============================================================================
# 2. جدول الورديات (tbShifts)
# ==============================================================================
class Shift(Base):
    """
    جدول قوالب فترات الدوام والورديات اليومية
    """
    __tablename__ = "tbShifts"

    ShiftId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للوردية

    FacilityId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Facilities.FacilityId"), nullable=False, index=True)
    # معرف المستشفى أو المركز الطبي الذي يطبق نظام هذه الوردية

    ShiftName: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم الوردية مثل: الصباحية، المسائية، أو الليلية طوارئ

    StartTime: Mapped[time] = mapped_column(Time, nullable=False)
    # وقت بدء العمل الرسمي للوردية

    EndTime: Mapped[time] = mapped_column(Time, nullable=False)
    # وقت انتهاء العمل الرسمي للوردية

    GracePeriodMinutes: Mapped[int] = mapped_column(Integer, default=15)
    # الدقائق المسموحة للتأخير الصباحي قبل بدء الخصم

    IsOvernight: Mapped[bool] = mapped_column(Boolean, default=False)
    # هل تمتد الوردية لليوم التالي (مناوبة مبيت تمتد بعد منتصف الليل)

    ShiftStatus: Mapped[str] = mapped_column(String(50), default="Active")
    # حالة الوردية: نشطة ومستخدمة في الجداول أم معطلة

    # العلاقات
    employee_shifts: Mapped[List["EmployeeShift"]] = relationship("EmployeeShift", back_populates="shift")


# ==============================================================================
# 3. جدول الموظفين (tbEmployees)
# ==============================================================================
class Employee(Base):
    """
    جدول بيانات الموظفين والأطباء والتعاقد
    """
    __tablename__ = "tbEmployees"

    EmployeeId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # الرقم الوظيفي والمعرف الأساسي للموظف

    DepartmentId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbDepartments.DepartmentId"), nullable=False, index=True)
    # القسم الذي يعمل به الموظف داخل المنشأة

    UserId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, unique=True, index=True)
    # ربط الموظف بحسابه الشخصي في جدول Users للدخول إلى النظام

    JobPositionId: Mapped[Optional[int]] = mapped_column(BigInteger, ForeignKey("tbJobPositions.JobPositionId"), nullable=True, index=True)
    # المسمى الوظيفي المربوط بجدول الوظائف

    NationalId: Mapped[Optional[str]] = mapped_column(String(50), unique=True, nullable=True)
    # رقم الهوية الوطنية أو الإقامة أو جواز السفر

    MedicalLicenseNumber: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # رقم ترخيص مزاولة المهنة الطبي للأطباء والتمريض

    Specialization: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # التخصص الطبي الدقيق مثل: باطنية، جراحة عامة، مسالك بولية

    Qualification: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # المؤهل الدراسي الأكاديمي وسنة التخرج

    EmploymentType: Mapped[str] = mapped_column(String(50), default="Full-Time")
    # نظام التعاقد: دوام كامل، دوام جزئي، أو طبيب زائر

    EmployeeHireDate: Mapped[datetime] = mapped_column(DateTime, nullable=False)
    # تاريخ مباشرة العمل الفعلي في المنشأة

    ContractEndDate: Mapped[Optional[date]] = mapped_column(Date, nullable=True)
    # تاريخ انتهاء عقد العمل لمتابعة إجراءات التجديد

    EmergencyContactName: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # اسم قريب يتم التواصل معه عند الطوارئ

    EmergencyContactPhone: Mapped[Optional[str]] = mapped_column(String(20), nullable=True)
    # رقم هاتف الشخص المعتمد في حالات الطوارئ

    Notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    # ملاحظات إضافية حول الموظف أو شروط العقد

    EmployeeStatus: Mapped[str] = mapped_column(String(50), default="Active")
    # الحالة الوظيفية: نشط، في إجازة، أو منهي الخدمة

    EmployeeCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إضافة سجل الموظف في النظام

    # العلاقات
    job_position: Mapped[Optional["JobPosition"]] = relationship("JobPosition", back_populates="employees")
    schedules: Mapped[List["EmployeeShift"]] = relationship("EmployeeShift", back_populates="employee", cascade="all, delete-orphan")


# ==============================================================================
# 4. جدول المناوبات (tbEmployeeShifts)
# ==============================================================================
class EmployeeShift(Base):
    """
    جدول توزيع المناوبات المجدولة للموظفين والأطباء
    """
    __tablename__ = "tbEmployeeShifts"

    EmployeeShiftId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسجل المناوبة المجدولة

    EmployeeId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbEmployees.EmployeeId"), nullable=False, index=True)
    # الموظف أو الطبيب المكلف بالعمل في هذه المناوبة

    ShiftId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbShifts.ShiftId"), nullable=False, index=True)
    # نوع الوردية المكلف بها (صباحي، مسائي، ليلي)

    ScheduleDate: Mapped[date] = mapped_column(Date, nullable=False, index=True)
    # تاريخ اليوم المحدد للوردية في الجدول الأسبوعي أو الشهري

    Status: Mapped[str] = mapped_column(String(50), default="Scheduled")
    # حالة المناوبة: مجدولة، مكتملة، مستبدلة مع زميل، أو ملغاة

    Notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات على المناوبة مثل: تبديل وردية أو تكليف إضافي

    # العلاقات
    employee: Mapped["Employee"] = relationship("Employee", back_populates="schedules")
    shift: Mapped["Shift"] = relationship("Shift", back_populates="employee_shifts")