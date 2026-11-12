from typing import Optional
from sqlalchemy import select
from sqlalchemy.orm import Session
from app.core.BaseRepository import BaseRepository
from app.modules.users.models import Role ,User



class RoleRepository(BaseRepository[Role]):
  """مستودع جدول الأدوار (tbRoles):

  يرث تلقائياً من BaseRepository دوال (create, update, delete, get_by_id,
  get_all).
  """

  def __init__(self, db: Session):
    super().__init__(Role, db)
    # نمرر لكلاس الأب: جدول Role، وجلسة قاعدة البيانات db

  # =========================================================================
  # دالة إضافية خاصة بالأدوار فقط: البحث عن دور بواسطة اسمه
  # • فائدتها: فحص عدم تكرار اسم الدور (مثلاً: منع وجود دورين باسم "Doctor").
  # =========================================================================
  def get_by_name(self, role_name: str) -> Optional[Role]:
    statement = select(Role).where(Role.RoleName == role_name)
    # الاستعلام: SELECT * FROM tbRoles WHERE RoleName = role_name

    return self.db.scalar(statement)
    # ترجع كائن الدور إن وُجد، أو ترجع None إن كان الاسم متاحاً








class UserRepository(BaseRepository[User]):
  """مستودع جدول المستخدمين:

  يرث تلقائياً من BaseRepository دوال (get_by_id, get_all, create, update,
  delete).
  """

  def __init__(self, db: Session):
    super().__init__(User, db)
    # تمرير موديل User وجلسة db لكلاس الأب BaseRepository

  # =========================================================================
  # دالة إنشاء المستخدم لمدير المنشأة (ضمن معاملة مركبة)
  #
  # • الوظيفة: إنشاء حساب المدير وتوليد UserId فوراً في الذاكرة دون Commit نهائي.
  # • متى تستخدمها؟ في العمليات المترابطة التي تحتاج لربط المستخدم بجدول آخر (مثل المنشأة).
  # =========================================================================
  def create_manager_user(self, user_dict: Dict[str, Any]) -> User:
    return self.create_without_commit(user_dict)
    # تستدعي الدالة الموروثة من كلاس الأب لتنفيذ db.add و db.flush
    # النتيجة: يولد رقم UserId فوراً في الكائن قبل الحفظ النهائي

  def get_by_username(self, user_name: str) -> Optional[User]:
   
      statement = select(User).where(User.Username == user_name)
    # بناء استعلام الاختيار بالشرط

      return self.db.scalar(statement)
