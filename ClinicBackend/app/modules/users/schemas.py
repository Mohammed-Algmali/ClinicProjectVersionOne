from datetime import datetime
from typing import Optional
from pydantic import BaseModel


class RoleBase(BaseModel):
  RoleName: str
  # اسم الدور بالإنجليزية أو العربية (مثل: Admin, Doctor, Receptionist, Patient)

  RoleDescription: Optional[str] = None
  # شرح وتوصيف لصلاحيات ومسؤوليات هذا الدور

 
  RoleLevel: Optional[int] = 1
  # المستوى الإداري للدور (مثلاً: 1 للمدير العام، 2 للأطباء، 3 للاستقبال، 4 للمرضى)

class RoleCreate(RoleBase):
  """البيانات المطلوبة عند إرسال طلب إنشاء دور جديد من الشاشة"""

  pass

class RoleUpdate(BaseModel):
  """البيانات القابلة للتعديل في الدور (جميع الحقول اختيارية)"""

  RoleName: Optional[str] = None
  # تعديل اسم الدور إن رغب المستخدم

  RoleDescription: Optional[str] = None
  # تعديل الوصف والصلاحيات

  RoleStatus: Optional[str] = None
  # تعديل حالة الدور (Active أو Inactive)

  RoleLevel: Optional[int] = None
  # تعديل الرتبة أو المستوى الإداري



class RoleResponse(RoleBase):
  """البيانات التي يعيدها السيرفر للواجهة بعد نجاح الحفظ"""

  RoleId: int
  # المعرف الفريد للدور الناتج من قاعدة البيانات

 
  # حالة الدور (Active / Inactive)


  RoleCreatedAt: datetime
  # تاريخ وتوقيت إنشاء الدور

  class Config:
    
    from_attributes = True




class UserBase(BaseModel):
  RoleId:int
  UserFullName:str
  Username:str
  UserEmail:str
  UserPhone:str
  gender: str = "Male"              # ذكر / أنثى
  birth_date: Optional[str] = None
  UserAddress: Optional[str] = None

  

class ManagerCreate(UserBase):
  Password:str
  


class PatientCreate(UserBase):
  FacilityId: Optional[int] = None  # رقم المنشأة (إذا تُرك فارغاً يأخذه النظام تلقائياً من المدير الحالي)
 

class EmployeeUserCreate(UserBase):
    Password: str
    FacilityId: Optional[int] = None  # رقم المنشأة (إذا تُرك فارغاً يأخذه النظام تلقائياً من المدير الحالي)
    
class EmployeeUserUpdate(UserBase):
    UserStatus: Optional[str] = None


class UserResponse(UserBase):
  UserId:int
  CreatedAt:datetime
  class Config:
    from_attributes = True


  

