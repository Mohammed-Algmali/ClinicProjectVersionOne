# users/services.py
from fastapi import HTTPException, status
from sqlalchemy.orm import Session
from app.modules.users.repository import RoleRepository,UserRepository
from app.modules.users.schemas import RoleCreate, RoleUpdate,ManagerCreate
from app.core.exceptions import DuplicateException, NotFoundException
from app.modules.auth.utils import get_password_hash



class RoleService:

    def __init__(self, db: Session):
        self.repo = RoleRepository(db)

    def create_new_role(self, data: RoleCreate):
        # 1. استخدام كلاس التكرار بسطر واحد نظيف!
        if self.repo.get_by_name(data.RoleName):
            raise DuplicateException("الدور الوظيفي", data.RoleName)

        return self.repo.create(data.dict())
        
    def list_all_roles(self):
      """2.

      دالة جلب قائمة بجميع الأدوار المسجلة في النظام
      """
      # --------------------------------------------------------------------------
      # استدعاء دالة get_all الموروثة مباشرة من كلاس الأب BaseRepository
      # --------------------------------------------------------------------------
      return self.repo.get_all()
      # تنفيذ الاستعلام لجلب كل سجلات جدول tbRoles كقائمة كائنات وإرجاعها للراوتر





class UserService:
    def __init__(self, db: Session):
        self.repo = UserRepository(db)
        # تهيئة مستودع الأدوار وتمرير جلسة قاعدة البيانات له

    def create_new_user(self, data: ManagerCreate):
        """1. دالة فحص الشروط ثم إنشاء الدور الجديد"""
        # الخطوة 1: فحص قاعدة العمل (هل الاسم مكرر؟)
        if self.repo.get_by_username(data.Username):
            raise DuplicateException("اسم المستخدم ",data.Username)

        
    # الخطوة 2: تشفير كلمة المرور عبر خوارزمية Bcrypt الآمنة
        # --------------------------------------------------------------------------
        hashed_password = get_password_hash(data.Password)
        # تحويل النص الصريح (مثل 123456) إلى شفرة غير قابلة للفك تبدأ بـ $2b$

        # --------------------------------------------------------------------------
        # الخطوة 3: تحويل البيانات لقاموس واستبدال كلمة المرور بالقيمة المشفرة
        # --------------------------------------------------------------------------
        user_dict = data.dict()
        # تحويل بيانات Pydantic إلى قاموس عادي

        user_dict["Password"] = hashed_password
        # وضع الهاش المشفر بدلاً من كلمة المرور الصريحة لحماية بيانات المستخدم

       

        # --------------------------------------------------------------------------
        # الخطوة 4: الحفظ النهائي في جدول Users بقاعدة البيانات
        # --------------------------------------------------------------------------
        return self.repo.create(user_dict)
        # إرجاع كائن المستخدم المنشأ بعد حفظه في قاعدة البيانات