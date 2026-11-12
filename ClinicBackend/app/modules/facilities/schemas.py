# facilities/schemas.py
from datetime import datetime
from typing import Optional
from pydantic import BaseModel
from app.modules.users.schemas import ManagerCreate


# ==============================================================================
# 1. بيانات المنشأة الأساسية
# ==============================================================================
class FacilityBase(BaseModel):
    """
    البيانات الخاصة بالمنشأة الطبية فقط
    """
 
    FacilityName: str                                   # اسم المنشأة (مثل: مستشفى الأمل التخصصي، مجمع الشفاء الطبي)
    FacilityType: str                                   # نوعها (مستشفى، عيادة، مجمع طبي، مركز أشعة
    FacilityPhone: Optional[str] = None                 # رقم الهاتف الرسمي الخاص بالمنشأة
    FacilityEmail: Optional[str] = None                 # البريد الإلكتروني الرسمي للمنشأة
    FacilityCountry: Optional[str] = "اليمن"            # الدولة
    FacilityCity: Optional[str]                         # المدينة
    FacilityAddress: Optional[str] = None               # العنوان أو الشارع التفصيلي
    FacilityLicenseNumber: Optional[str] = None         # رقم الترخيص الطبي الصادر من وزارة الصحة
    FacilityCode:str
    FacilityWebSite:str
    LicenseExpiryDate:datetime
    LicenseIssueDate:datetime


class FacilityCreate(FacilityBase):
    """
    نموذج إنشاء المنشأة
    """
    pass


# ==============================================================================
# 2. النموذج الموحد للتسجيل المشترك (Nested Request)
# ==============================================================================
class FacilityRegisterRequest(BaseModel):
    """
    يجمع بيانات كائن المستخدم وكائن المنشأة في طلب واحد منظم
    """
    Manager: ManagerCreate                         # كائن يحتوي على: UserFullName, Username, Password, UserPhone, UserEmail
    Facility: FacilityCreate                       # كائن يحتوي على تفاصيل المنشأة الطبية المذكورة أعلاه


# ==============================================================================
# 3. شكل الاستجابة بعد نجاح الحفظ (Response Model)
# ==============================================================================
class FacilityRegisterResponse(BaseModel):
    """
    البيانات المعادة للواجهة بعد نجاح عملية الحفظ والتسجيل
    """
    FacilityId: int                                # الرقم التعريفي الأساسي للمنشأة بعد إنشائها في قاعدة البيانات
    FacilityName: str
    FacilityPhone: Optional[str] = None
    ManagerFullName: str
    FacilityCode: str                              # رمز المنشأة (مثل: MED-001)
    FacilityStatus: str                            # حالة المنشأة الحالية (Active)
    class Config:
        from_attributes = True

class FacilityUpdateRequest(FacilityBase):
    pass
    # FacilityId: int                        # ⬅️ أضف هذا السطر
   
    # FacilityStatus: Optional[str] = "Active"
    # CreatedAt: Optional[datetime] = None

class FacilityResponse(FacilityBase):
    """
    يرث كل حقول المنشأة من FacilityBase
    ويضيف فقط الحقول التي تولدها قاعدة البيانات تلقائياً
    """
    FacilityId: int
    FacilityCode: str
    FacilityStatus: str = "Active"
    CreatedAt: Optional[datetime] = None
    class Config:
        from_attributes = True
   


class FacilityListItemResponse(BaseModel):
  FacilityId: int
  FacilityName: str
  FacilityCode: Optional[str] = None
  ManagerFullName: str                            # اسم المدير
  FacilityCity: Optional[str] = None
  FacilityAddress: Optional[str] = None
  FacilityStatus: str

  class Config:
    from_attributes = True