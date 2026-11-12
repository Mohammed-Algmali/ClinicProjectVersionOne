from typing import Any, Dict
from app.modules.facilities.models import  Facility
from sqlalchemy.orm import Session
from app.core.BaseRepository import BaseRepository
from app.modules.users.models import User
from sqlalchemy import select, or_, func


class FacilityRepository(BaseRepository[Facility]):
  """مستودع جدول المنشآت الطبية:

  يرث تلقائياً من BaseRepository دوال الـ CRUD الأساسية لكلاس Facility.
  """

  def __init__(self, db: Session):
    super().__init__(Facility, db)
    # تمرير موديل Facility وجلسة db لكلاس الأب BaseRepository

  # =========================================================================
  # 1. دالة إنشاء المنشأة الطبية (ضمن معاملة مركبة)
  #
  # • الوظيفة: حفظ سجل المنشأة وربطه بـ UserId وتوليد FacilityId في الذاكرة.
  # =========================================================================

    # استدعاء دالة الأب لتوليد رقم FacilityId فوراً مع إبقاء المعاملة مفتوحة

  def get_facilities_with_manager(self):
    # استعلام يجلب المنشأة مع المدير المرتبط بها من جدول Users
    statement = select(
        Facility.FacilityId,
        Facility.FacilityName,
        Facility.FacilityCode,
        Facility.FacilityCity,
        Facility.FacilityAddress,
        Facility.FacilityStatus,
        User.UserFullName.label("ManagerFullName"),
    ).outerjoin(User, User.FacilityId == Facility.FacilityId)

    return self.db.execute(statement).all()
  



  def search_facilities_for_admin(
        self,
        search_term: Optional[str] = None,
        facility_type: Optional[str] = None,
        facility_status: Optional[str] = None,
        city: Optional[str] = None,
        skip: int = 0,
        limit: int = 20
    ):
        query = select(Facility)

        # 1. البحث بالنص (الاسم، الكود، الهاتف، رقم الترخيص)
        if search_term:
            term = f"%{search_term.strip()}%"
            query = query.where(
                or_(
                    Facility.FacilityName.ilike(term),
                    Facility.FacilityCode.ilike(term),
                    Facility.FacilityPhone.ilike(term),
                    Facility.FacilityLicenseNumber.ilike(term)
                )
            )

        # 2. فلاتر إضافية لمدير المنصة
        if facility_type:
            query = query.where(Facility.FacilityType == facility_type)
        if facility_status:
            query = query.where(Facility.FacilityStatus == facility_status)
        if city:
            query = query.where(Facility.FacilityCity == city)

        # 3. الترتيب والترقيم (Pagination)
        total_count = self.db.execute(select(func.count()).select_from(query.subquery())).scalar()
        facilities = self.db.execute(query.order_by(Facility.FacilityId.desc()).offset(skip).limit(limit)).scalars().all()

        return facilities, total_count
