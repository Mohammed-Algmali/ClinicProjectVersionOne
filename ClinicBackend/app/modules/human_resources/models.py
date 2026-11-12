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

    TotalHours: Mapped[float] = mapped_column(Numeric(4, 2), default=8.0)
    # العلاقات
    ShiftType: Mapped[str] = mapped_column(String(50))

    employee_shifts: Mapped[List["EmployeeShift"]] = relationship("EmployeeShift", back_populates="shift")

    # 1. [العلاقة المصححة]: علاقة الوردية بالمنشأة (N إلى 1)
    # -------------------------------------------------------------
    facility: Mapped["Facility"] = relationship(
        "Facility", 
        back_populates="shifts"
    )


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
    UserId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, unique=True, index=True)
    # ربط الموظف بحسابه الشخصي في جدول Users للدخول إلى النظام
    MedicalLicenseNumber: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # رقم ترخيص مزاولة المهنة الطبي للأطباء والتمريض

    Specialization: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # التخصص الطبي الدقيق مثل: باطنية، جراحة عامة، مسالك بولية

    Qualification: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # المؤهل الدراسي الأكاديمي وسنة التخرج

    EmergencyContactPhone: Mapped[Optional[str]] = mapped_column(String(20), nullable=True)
    # رقم هاتف الشخص المعتمد في حالات الطوارئ

    Notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    # ملاحظات إضافية حول الموظف أو شروط العقد

    EmployeeStatus: Mapped[str] = mapped_column(String(50), default="Active")
    # الحالة الوظيفية: نشط، في إجازة، أو منهي الخدمة
    EmployeeCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت إضافة سجل الموظف في النظام
    # علاقة تمكنك من كتابة: employee.user.UserFullName
    user: Mapped["User"] = relationship("User", back_populates="employee")





    # NationalId: Mapped[Optional[str]] = mapped_column(String(50), unique=True, nullable=True)
    # # رقم الهوية الوطنية أو الإقامة أو جواز السفر
   

    # EmploymentType: Mapped[str] = mapped_column(String(50), default="Full-Time")
    # # نظام التعاقد: دوام كامل، دوام جزئي، أو طبيب زائر

    # EmployeeHireDate: Mapped[datetime] = mapped_column(DateTime, nullable=False)
    # # تاريخ مباشرة العمل الفعلي في المنشأة

    # ContractEndDate: Mapped[Optional[date]] = mapped_column(Date, nullable=True)
    # # تاريخ انتهاء عقد العمل لمتابعة إجراءات التجديد

   
    

    

    # العلاقات
    
    schedules: Mapped[List["EmployeeShift"]] = relationship("EmployeeShift", back_populates="employee", cascade="all, delete-orphan")
    attendances: Mapped[List["Attendance"]] = relationship(
        "Attendance",
        back_populates="employee",
        cascade="all, delete-orphan" # إذا حُذف الموظف تُحذف سجلات حضوره تلقائياً لحماية قاعدة البيانات
    )

  
   
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



class Attendance(Base):
    """
    نموذج جدول سجلات الحضور والانصراف الفعلي في العيادة (tbAttendance)
    مصمم بنفس معمارية وأسلوب جدول المناوبات (SQLAlchemy 2.0)
    """
    __tablename__ = "tbAttendance"

    AttendanceId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسجل الحضور والانصراف اليومي
    EmployeeId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbEmployees.EmployeeId"), nullable=False, index=True)
    # الموظف أو الطبيب صاحب سجل الحضور
    EmployeeShiftId: Mapped[Optional[int]] = mapped_column(BigInteger, ForeignKey("tbEmployeeShifts.EmployeeShiftId"), nullable=True, index=True)
    # المناوبة المجدولة المرتبطة بهذا الحضور (لحساب التأخير وساعات العمل تلقائياً)
    AttendanceDate: Mapped[date] = mapped_column(Date, default=date.today, nullable=False, index=True)
    # تاريخ يوم العمل الفعلي

    CheckInTime: Mapped[time] = mapped_column(Time, nullable=False)
    # وقت تسجيل الحضور الفعلي في العيادة

    CheckOutTime: Mapped[Optional[time]] = mapped_column(Time, nullable=True)
    # وقت تسجيل الانصراف الفعلي (يقبل None طالما الموظف قيد العمل)

    AttendanceStatus: Mapped[str] = mapped_column(String(50), default="Present")
    # حالة الحضور: Present (حاضر)، Late (متأخر)، Excused (إجازة/عذر)
    LateMinutes: Mapped[int] = mapped_column(Integer, default=0)
    # عدد دقائق التأخير المحسوبة بناءً على وقت بداية المناوبة

    VerificationMethod: Mapped[str] = mapped_column(String(50), default="face") 
    # يوضح كيف حضر اليوم: "face" (وجه)، أو "fingerprint" (إصبع)، أو "manual" (يدوياً)
    WorkHours: Mapped[float] = mapped_column(Numeric(4, 2), default=0.00)

    
    # إجمالي ساعات العمل الفعلية المنجزة في الوردية
    Notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات على الحضور مثل: إذن خروج مبكر أو مبرر تأخير

    CreatedAt: Mapped[datetime] = mapped_column(DateTime, default=datetime.utcnow)
    # تاريخ ووقت إنشاء السجل في النظام لأغراض الرقابة والتدقيق
    # =========================================================================
    # العلاقات (Relationships)
    # =========================================================================
    employee: Mapped["Employee"] = relationship("Employee", back_populates="attendances")


