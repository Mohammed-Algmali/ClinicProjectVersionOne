from enum import Enum

class UserStatus(str, Enum):
    """حالات حساب المستخدم داخل النظام"""
    ACTIVE = "active"          # نشط ويعمل
    INACTIVE = "inactive"      # غير نشط
    SUSPENDED = "suspended"    # موقف مؤقتاً من قبل الإدارة
    LOCKED = "locked"          # مقفل بسبب محاولات دخول خاطئة متكررة

class RoleName(str, Enum):
    """الأدوار القياسية المعتمدة في نظام الـ SaaS"""
    SAAS_ADMIN = "saas admin"          # أنت (مطور ومالك المنصة بالكامل)
    CLINIC_MANAGER = "clinic manager"  # مدير العيادة المشتركة (المستأجر)
    DOCTOR = "doctor"                  # الطبيب
    RECEPTION = "reception"            # موظف الاستقبال
    PATIENT = "patient"                # المريض

class RoleDomain(str, Enum):
    """نطاقات الصلاحية لعزل البيانات في نظام تعدد المستأجرين"""
    GLOBAL = "global"            # صلاحية مطلقة على مستوى السيرفر بالكامل (خاص بـ SaaS Admin)
    CLINIC = "clinic"            # صلاحية محصورة داخل حدود العيادة المشتركة فقط
    OWN_PATIENT = "own patient"  # صلاحية محصورة للطبيب على مرضاه الذين يتابعهم فقط
    SELF = "self"                # صلاحية محصورة للمستخدم على بيانات ملفه الشخصي فقط (كالمريض)

class Gender(str, Enum):
    """الجنس المعتمد في البيانات الشخصية والطبية"""
    MALE = "male"
    FEMALE = "female"
