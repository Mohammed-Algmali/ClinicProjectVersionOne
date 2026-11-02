# users/router.py
from fastapi import APIRouter, Depends, status
from sqlalchemy.orm import Session

from typing import List
from app.core.database import get_db
from app.modules.users.schemas import RoleCreate, RoleResponse
from app.modules.users.services import RoleService

# ==============================================================================
# 1. تهيئة موجه مسارات الأدوار
# ==============================================================================
router = APIRouter(
    prefix="/roles",
    tags=["إدارة الأدوار والصلاحيات (Roles)"]
)
# prefix="/roles" تعني أن كل المسارات في هذا الملف ستبدأ تلقائياً بـ /roles


# ==============================================================================
# 2. المسار الأول: إنشاء دور وظيفي جديد
# ==============================================================================
@router.post(
    "/",
    response_model=RoleResponse,
    status_code=status.HTTP_201_CREATED,
    summary="إنشاء دور وظيفي جديد"
)
def create_new_role(
    data: RoleCreate,
    db: Session = Depends(get_db)
):
    """
    مسار استقبال طلب إضافة دور جديد من الواجهة وتمريره للـ Service
    """
    # --------------------------------------------------------------------------
    # تفصيل ما يحدث في هذا المسار:
    # 1. data: RoleCreate           = فحص البيانات القادمة من الواجهة تلقائياً
    # 2. db: Session = Depends(...) = فتح جلسة اتصال بقاعدة البيانات لهذا الطلب
    # 3. status.HTTP_201_CREATED   = إرجاع رمز 201 القياسي (تعني تم الإنشاء بنجاح)
    # 4. response_model=RoleResponse = فلترة النتيجة المعادة لتطابق المخطط المحدد
    # --------------------------------------------------------------------------

    service = RoleService(db)
    # تهيئة طبقة الخدمات وتمرير جلسة قاعدة البيانات لها

    return service.create_new_role(data)
    # استدعاء دالة الإنشاء من Service وإرجاع النتيجة





# ==============================================================================
# المسار الثاني: جلب وعرض قائمة جميع الأدوار
# ==============================================================================
@router.get(
    "/",
    response_model=List[RoleResponse],
    status_code=status.HTTP_200_OK,
    summary="عرض جميع الأدوار المتاحة في النظام",
)
def get_all_roles(db: Session = Depends(get_db)):
  """مسار إرجاع قائمة بجميع الأدوار لملء القوائم المنسدلة في شاشات الإدارة"""
  service = RoleService(db)
  # تهيئة طبقة الخدمات وتمرير جلسة قاعدة البيانات لها

  return service.list_all_roles()
  # جلب قائمة الأدوار كاملة من Service وإرجاعها للواجهة كمصفوفة JSON