from typing import Optional
from sqlalchemy import select
from sqlalchemy.orm import Session
from app.modules.users.models import Role
class RoleRepository:
    """
    مستودع العمليات المباشرة على جدول الأدوار tbRoles
    """
    def __init__(self, db: Session):
        self.db = db
        # استقبال جلسة قاعدة البيانات وحفظها في self.db لتستخدمها جميع دوال الكلاس

    def create(self, role_dict: dict) -> Role:
        """
        1. دالة إنشاء دور جديد وحفظه في جدول tbRoles
        """
        new_role = Role(**role_dict)
        # تفريغ مفاتيح وقيم القاموس مباشرة داخل أعمدة جدول Role

        self.db.add(new_role)
        # إدراج الكائن في قائمة انتظار الحفظ (Staging)

        self.db.commit()
        # تثبيت العملية نهائياً في قاعدة البيانات (INSERT)

        self.db.refresh(new_role)
        # إعادة قراءة السجل لجلب RoleId وتاريخ الإنشاء المتولدة تلقائياً

        return new_role
        # إرجاع كائن الدور المكتمل

    def get_by_name(self, role_name: str) -> Optional[Role]:
   
        statement = select(Role).where(Role.RoleName == role_name)
    # بناء استعلام الاختيار بالشرط

        return self.db.scalar(statement)
    # تنفيذ الاستعلام وإرجاع أول نتيجة مباشرة، أو إرجاع None إن لم يوجد




    def get_by_id(self, role_id: int) -> Optional[Role]:
      """3.

      دالة البحث عن الدور بواسطة رقمه التعريفي الأساسي (Primary Key)
      """
      statement = select(Role).where(Role.RoleId == role_id)
      # بناء الاستعلام: SELECT * FROM tbRoles WHERE RoleId = role_id

      return self.db.scalar(statement)
      # تنفيذ الاستعلام وإرجاع كائن الدور، أو إرجاع None إذا لم يوجد دور بهذا الرقم

    
    def get_all(self) -> List[Role]:
      """4.

      دالة جلب قائمة بجميع الأدوار المسجلة في النظام
      """
      statement = select(Role)
      # بناء الاستعلام: SELECT * FROM tbRoles (بدون شروط لجلب كل السجلات)

      return list(self.db.scalars(statement).all())
      # --------------------------------------------------------------------------
      # لاحظ الفرق الدقيق بين scalar و scalars:
      # - scalar()  (بالمفرد): ترجع كائناً واحداً فقط (مثل البحث بالاسم أو الرقم).
      # - scalars() (بالجمع):  تتعامل مع مجموعة نتائج متعددة.
      # - .all()              : تجمع النتائج كلها في قائمة واحدة List.
      # --------------------------------------------------------------------------



    def update(self, role: Role, update_data: dict) -> Role:
      """5.

      دالة تحديث بيانات دور موجود مسبقاً
      """
      for key, value in update_data.items():
        setattr(role, key, value)
        # ----------------------------------------------------------------------
        # ماذا يفعل setattr؟
        # يمر على كل مفتاح وقيمة في القاموس ويطبقهما على الكائن، مثلاً:
        # setattr(role, 'RoleDescription', 'وصف جديد')
        # تكافئ تماماً كتابة: role.RoleDescription = 'وصف جديد'
        # لكنها برمجياً ذكية وتعدل أي حقل تم إرساله ديناميكياً دون تكرار الكود
        # ----------------------------------------------------------------------

      self.db.commit()
      # SQLAlchemy ذكي جداً، يلاحظ التغييرات وينفذ أمر:
      # UPDATE tbRoles SET RoleDescription = ... WHERE RoleId = ...

      self.db.refresh(role)
      # إعادة تحميل الكائن للتأكد من تطابق بياناته مع قاعدة البيانات

      return role
      # إرجاع كائن الدور بعد التعديل