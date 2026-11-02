# facilities/schemas.py
from datetime import datetime
from typing import Optional
from pydantic import BaseModel, EmailStr
from users.schemas import UserCreate


# ==============================================================================
# 1. بيانات المنشأة الأساسية
# ==============================================================================
class FacilityBase(BaseModel):
    """
    البيانات الخاصة بالمنشأة الطبية فقط
    """
    FacilityName: str
    # اسم المنشأة (مثل: مستشفى الأمل التخصصي، مجمع الشفاء الطبي)

    FacilityType: str
    # نوعها (مستشفى، عيادة، مجمع طبي، مركز أشعة)

    FacilityPhone: Optional[str] = None
    # رقم الهاتف الرسمي الخاص بالمنشأة

    FacilityEmail: Optional[EmailStr] = None
    # البريد الإلكتروني الرسمي للمنشأة

    FacilityCountry: Optional[str] = "اليمن"
    # الدولة

    FacilityCity: Optional[str] = "الحديدة"
    # المدينة

    FacilityAddress: Optional[str] = None
    # العنوان أو الشارع التفصيلي

    FacilityLicenseNumber: Optional[str] = None
    # رقم الترخيص الطبي الصادر من وزارة الصحة


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
    user: UserCreate
    # كائن يحتوي على: UserFullName, Username, Password, UserPhone, UserEmail

    facility: FacilityCreate
    # كائن يحتوي على تفاصيل المنشأة الطبية المذكورة أعلاه


# ==============================================================================
# 3. شكل الاستجابة بعد نجاح الحفظ (Response Model)
# ==============================================================================
class FacilityResponse(FacilityBase):
    """
    البيانات المعادة للواجهة بعد نجاح عملية الحفظ والتسجيل
    """
    FacilityId: int
    # الرقم التعريفي الأساسي للمنشأة بعد إنشائها في قاعدة البيانات

    UserId: int
    # معرف المستخدم (المدير) الذي أصبحت المنشأة مرتبطة به

    FacilityStatus: str
    # حالة المنشأة الحالية (Active)

    FacilityCreatedAt: datetime
    # تاريخ وتوقيت إنشاء السجل في النظام

    class Config:
        orm_mode = True
        from_attributes = True