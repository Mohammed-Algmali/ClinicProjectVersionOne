# users/services.py
from fastapi import HTTPException, status
from sqlalchemy.orm import Session
from app.modules.users.repository import RoleRepository
from app.modules.users.schemas import RoleCreate, RoleUpdate


class RoleService:
    """عقل النظام وإدارة منطق العمليات الخاص بالأدوار والصلاحيات"""

    def __init__(self, db: Session):
        self.repo = RoleRepository(db)
        # تهيئة مستودع الأدوار وتمرير جلسة قاعدة البيانات له

    def create_new_role(self, data: RoleCreate):
        """1. دالة فحص الشروط ثم إنشاء الدور الجديد"""
        # الخطوة 1: فحص قاعدة العمل (هل الاسم مكرر؟)
        existing_role = self.repo.get_by_name(data.RoleName)
        if existing_role:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=f"الدور الوظيفي '{data.RoleName}' مسجل مسبقاً في النظام، يرجى اختيار اسم آخر.",
            )

        # الخطوة 2: تحويل البيانات لقاموس
        role_dict = data.dict()

        # الخطوة 3: الحفظ في قاعدة البيانات
        return self.repo.create(role_dict)

    def list_all_roles(self) -> list:
        """2. دالة استرجاع قائمة جميع الأدوار المسجلة في النظام"""
        return self.repo.get_all()

    def get_role_by_id(self, role_id: int):
        """3. دالة فحص وجلب دور محدد بواسطة معرّفه"""
        role = self.repo.get_by_id(role_id)
        if not role:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"الدور الوظيفي ذو الرقم التعريفي ({role_id}) غير موجود في النظام.",
            )
        return role

    def update_role(self, role_id: int, data: RoleUpdate):
        """4. دالة فحص شروط التعديل وتحديث الدور"""
        # 1. التأكد أولاً من وجود الدور
        role = self.get_role_by_id(role_id)

        # 2. التأكد من عدم تكرار الاسم الجديد إن تم تغييره
        if data.RoleName and data.RoleName != role.RoleName:
            existing_role = self.repo.get_by_name(data.RoleName)
            if existing_role:
                raise HTTPException(
                    status_code=status.HTTP_400_BAD_REQUEST,
                    detail=f"لا يمكن التعديل: اسم الدور '{data.RoleName}' محجوز مسبقاً لدور آخر.",
                )

        # 3. استخراج الحقول المرسلة فقط
        update_dict = data.dict(exclude_unset=True)

        # 4. إرسال التعديل للمستودع لحفظه
        return self.repo.update(role, update_dict)