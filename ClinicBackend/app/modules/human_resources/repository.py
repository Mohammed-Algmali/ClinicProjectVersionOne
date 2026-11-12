from typing import Dict, Any, List
from sqlalchemy.orm import Session
from sqlalchemy import select
from app.core.BaseRepository import BaseRepository
from app.modules.human_resources.models import Employee ,Shift# تأكد من مسار الموديل
from app.modules.users.models import User,Role



class EmployeeRepository(BaseRepository[Employee]):
    def __init__(self, db: Session):
        super().__init__(Employee,db)

 

    def get_employees_with_user_and_role(self, facility_id: Optional[int] = None):
        """
        دالة استعلام منخفضة المستوى (Data Access Layer):
        المبدأ الهندسي:
        1. تطبيق معيار SQLAlchemy 2.0 الحديث القائم على صياغة (select statements).
        2. جلب الحقول المطلوبة للعرض فقط (Projection) بدلاً من جلب كامل الجداول لتحسين سرعة الاستجابة وتقليل استهلاك الذاكرة.
        3. دمج بيانات الموظف الطبية مع حسابه في جدول المستخدمين ودوره الوظيفي في استعلام واحد (Single Trip to DB).
        """
        # بناء جملة الاستعلام وتحديد الأعمدة المطلوبة بدقة
        statement = (
            select(
                Employee.EmployeeId,             # الرقم الوظيفي الأساسي
                User.UserId,                     # معرف المستخدم العام
                User.UserFullName,               # الاسم الكامل للموظف
                User.Username,                   # اسم تسجيل الدخول
                User.UserPhone,                  # رقم الهاتف للتواصل
                User.UserEmail,                  # البريد الإلكتروني الرسمي
                Role.RoleName,                   # اسم الدور (طبيب، ممرض، موظف استقبال)
                Employee.Specialization,         # التخصص الطبي (إن كان طبيباً)
                Employee.MedicalLicenseNumber,   # رقم ترخيص مزاولة المهنة
                Employee.EmployeeStatus,         # حالة الحساب (Active / Inactive)
                Employee.EmployeeCreatedAt,      # تاريخ ووقت إضافة الموظف
            )
            # دمج إلزامي (Inner Join): لأن كل موظف يجب أن يمتلك حساباً في جدول المستخدمين
            .join(User, User.UserId == Employee.UserId)
            
            # دمج خارجي (Outer Join): لضمان ظهور الموظف حتى لو لم يُحدد له دور وظيفي بعد
            .outerjoin(Role, Role.RoleId == User.RoleId)
        )

        # عزل البيانات أمنياً (Multi-Tenancy):
        # إذا تم تمرير معرف المنشأة، يتم قصر الاستعلام على موظفي هذه المنشأة فقط
        if facility_id:
            statement = statement.where(User.FacilityId == facility_id)

        # تنفيذ الاستعلام وإرجاع جميع الأسطر الناتجة كقائمة كائنات سريعة القراءة
        return self.db.execute(statement).all()


class ShiftRepository(BaseRepository[Shift]):
    def __init__(self, db: Session):
        super().__init__(Shift, db)

    def create_shift(self, shift_dict: Dict[str, Any]) -> Shift:
        """إنشاء وحفظ سجل الوردية مع تنفيذ commit مباشرة لأنها تخص جدولاً واحداً"""
        return self.create(shift_dict)
    # ========================================================
    def get_by_name(self, shift_name: str, facility_id: int) -> Optional[Shift]:
        """فحص هل اسم الوردية موجود مسبقاً في هذه المنشأة لمنع التكرار"""
        statement = (
            select(Shift)
            .where(Shift.FacilityId == facility_id)
            .where(Shift.ShiftName == shift_name.strip())
        )
        return self.db.scalars(statement).first()