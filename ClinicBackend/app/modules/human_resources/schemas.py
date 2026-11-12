
from datetime import datetime,time
from typing import Optional
from pydantic import BaseModel
from app.modules.users.schemas import EmployeeUserCreate ,EmployeeUserUpdate




class EmployeeDetailsCreate(BaseModel):
    MedicalLicenseNumber: Optional[str] = None    # ترخيص مزاولة المهنة
    Specialization: Optional[str] = None          # التخصص (باطنية، جراحة...)
    Qualification: Optional[str] = None           # المؤهل العلمي
    EmergencyContactPhone: Optional[str] = None   # هاتف الطوارئ
    Notes: Optional[str] = None

# ==============================================================================
# 4. المخطط المجمع الكامل الذي سيتم إرساله من الواجهات (Request Body)
# ==============================================================================
class EmployeeRegisterRequest(BaseModel):
    User: EmployeeUserCreate                      # بيانات الحساب الأساسية
    Employee: EmployeeDetailsCreate               # البيانات المهنية والطبية



class EmployeeListItemResponse(BaseModel):
    """
    البيانات المعادة لكل موظف لعرضها في جدول الـ DataGrid
    """
    EmployeeId: int                         # الرقم الوظيفي
    UserFullName: str                       # الاسم الكامل
    Username: str                           # اسم الدخول
    UserPhone: str                          # الهاتف
    UserEmail: str                          # البريد
    RoleName: str                           # اسم الدور (طبيب، ممرض، استقبال)
    Specialization: Optional[str] = None    # التخصص الطبي
    MedicalLicenseNumber: Optional[str] = None # ترخيص المزاولة
    EmployeeStatus: str                     # الحالة (Active / Inactive)
  

    class Config:
        from_attributes = True

class EmployeeRegisterResponse(BaseModel):


    """
    مخطط الرد بعد نجاح تسجيل الموظف:
    يجمع البيانات الأساسية التي تولدت في قاعدة البيانات للموظف وحسابه.
    """
    EmployeeId: int                         # الرقم الوظيفي المتولد في tbEmployees
    UserId: int                             # رقم المستخدم المتولد في جدول Users
    FacilityId: Optional[int] = None        # رقم المنشأة التي أُضيف إليها
    UserFullName: str                       # اسم الموظف الكامل
    Username: str                           # اسم الدخول
    RoleId: int                             # رقم الدور (طبيب، استقبال...)
    Specialization: Optional[str] = None    # التخصص (إذا كان طبيباً)
    MedicalLicenseNumber: Optional[str] = None # رقم الترخيص المهني
    EmployeeStatus: str = "Active"          # حالة الحساب (Active)
    EmployeeCreatedAt: datetime             # تاريخ ووقت الإضافة

    class Config:
        from_attributes = True  # للسماح بقراءة البيانات مباشرة من كائنات SQLAlchemy ORM



class ShiftBase(BaseModel):
    """
    الخصائص المشتركة لأي وردية عمل في المنشأة الطبية
    """
    ShiftName: str 
    StartTime: time 
    EndTime: time 
    GracePeriodMinutes: int 
    TotalHours:float
    ShiftType:str


# ==============================================================================
# 2. مخطط إنشاء وردية جديدة (ShiftCreate - Request Body)
# ==============================================================================
class ShiftCreate(ShiftBase):
    """
    البيانات التي تُرسل من شاشة 'إضافة وردية جديدة' في WPF
    ملاحظة: رقم المنشأة (FacilityId) اختياري هنا؛ لأن السيرفر يستخرجه تلقائياً من توكن المستخدم
    """
    FacilityId: Optional[int] = None


class ShiftResponse(ShiftBase):
    """
    البيانات المعادة للواجهة بعد حفظ الوردية أو عند جلب قائمة الورديات
    تتضمن المفتاح الأساسي المتولد في قاعدة البيانات
    """
    ShiftId: int
    FacilityId: int

    class Config:
        from_attributes = True  # للسماح بقراءة البيانات مباشرة من كائنات SQLAlchemy ORM