# facilities/services.py
from fastapi import HTTPException, status,Depends
from sqlalchemy.orm import Session
from app.core.database import get_db
# 1. استدعاء المستودعات المسؤولة عن جداول قاعدة البيانات
from app.modules.facilities.repository import FacilityRepository
from app.modules.users.repository import UserRepository, RoleRepository

from app.core.exceptions import DuplicateException, NotFoundException,InternalServerException
from typing import List, Optional
# 2. استدعاء المخططات (Schemas) لتدقيق البيانات القادمة
from app.modules.facilities.schemas import FacilityRegisterRequest,FacilityRegisterResponse,FacilityListItemResponse,FacilityUpdateRequest,FacilityResponse

# 3. استدعاء أداة التشفير لكلمات المرور
from app.modules.auth.utils import get_password_hash

class FacilityService:
    """
    طبقة منطق العمل للمنشآت بعد إلغاء الفروع
    """
    def __init__(self, db: Session):
        self.db = db
        self.facility_repo = FacilityRepository(db)
        self.user_repo = UserRepository(db)
        self.role_repo = RoleRepository(db)

    
    def get_facilities_with_manager_list(self) -> List[FacilityListItemResponse]:
        results = self.facility_repo.get_facilities_with_manager()
        return [
            FacilityListItemResponse(
                FacilityId=r.FacilityId,
                FacilityName=r.FacilityName,
                FacilityCode=r.FacilityCode,
                ManagerFullName=r.ManagerFullName,
                FacilityCity=r.FacilityCity,
                FacilityAddress=r.FacilityAddress,
                FacilityStatus=r.FacilityStatus,
            )
            for r in results
        ]

    def register_facility_with_manager(self, data: FacilityRegisterRequest) -> FacilityRegisterResponse:

        """
        تسجيل منشأة جديدة مع حساب مديرها فقط (بدون أي فروع)
        """
        # 1. التحقق من عدم تكرار اسم مستخدم المدير
        if self.user_repo.get_by_username(data.Manager.Username):
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=f"اسم المستخدم '{data.Manager.Username}' محجوز مسبقاً لمستخدم آخر."
            )

        # 2. التحقق من عدم تكرار رمز المنشأة
      

        # 3. التحقق من وجود الدور الإداري للمدير
        role = self.role_repo.get_by_id(data.Manager.RoleId)
        if not role:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"الدور الوظيفي ذو الرقم ({data.Manager.RoleId}) غير مسجل في النظام."
            )

        try:
            # 4. تشفير كلمة المرور وإنشاء حساب المدير
            facility_dict = data.Facility.dict()
          
            facility_dict["FacilityStatus"] = "Active"
            
            # ننشئ المنشأة هنا أولاً   
            new_facility = self.facility_repo.create_without_commit(facility_dict)
            self.facility_repo.flush()  # يولد رقم FacilityId للمنشأة فوراً

            # -------------------------------------------------------------
            # 2. الآن نجهز المدير ونعطيه رقم المنشأة (new_facility.FacilityId)
            # -------------------------------------------------------------
            hashed_pwd = get_password_hash(data.Manager.Password)
            manager_dict = data.Manager.dict()
            manager_dict["Password"] = hashed_pwd
            
            # نربط المدير بالمنشأة التي أنشئت في الخطوة 1
            manager_dict["FacilityId"] = new_facility.FacilityId
            
            # ننشئ حساب المدير الآن بعد أن أصبح رقم المنشأة جاهزاً
            new_manager = self.user_repo.create_manager_user(manager_dict)

            # -------------------------------------------------------------
            # 3. تثبيت الحفظ الموحد للطرفين في قاعدة البيانات
            # -------------------------------------------------------------
            self.facility_repo.commit()
            self.facility_repo.refresh(new_facility)
            self.user_repo.refresh(new_manager)
            
            return FacilityRegisterResponse(
                FacilityId=new_facility.FacilityId,
                FacilityName=new_facility.FacilityName,
                ManagerFullName=new_manager.UserFullName,
                FacilityPhone=new_facility.FacilityPhone,
                FacilityCode=new_facility.FacilityCode,
                FacilityStatus=new_facility.FacilityStatus
            )
                
            

        except HTTPException:
            self.facility_repo.rollback()
            raise

        except Exception as error:
            self.facility_repo.rollback()
            raise InternalServerException("حفظ المنشأة والمدير", error)
    


    # ⬅️ تم إضافة current_user للتحقق من الأمان
    def update_facility(self, facility_id: int, data: FacilityUpdateRequest, current_user) -> FacilityResponse:
        """
        تحديث بيانات المنشأة مع التحقق من صلاحية المستخدم
        """
        # 1. التحقق من الأمان (مدير المنصة أو مدير هذه المنشأة فقط)
        is_admin = (current_user.RoleId == 1)
        is_owner = (current_user.FacilityId == facility_id)
        if not (is_admin or is_owner):
            raise HTTPException(
                status_code=status.HTTP_403_FORBIDDEN,
                detail="غير مصرح لك بتعديل بيانات منشأة لا تتبعك."
            )

        # 2. التحقق من وجود المنشأة (تم إصلاح خطأ المتغير error)
        facility = self.facility_repo.get_by_id(facility_id)
        if not facility:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"المنشأة برقم '{facility_id}' غير موجودة في النظام."
            )
        # 3. تحديث البيانات
        update_dict = data.dict(exclude_unset=True)
        for field, value in update_dict.items():
            if hasattr(facility, field):
                setattr(facility, field, value)
        # 4. تثبيت التعديل
        try:
            self.facility_repo.commit()
            self.facility_repo.refresh(facility)
        except Exception as error:
            self.facility_repo.rollback()
            raise InternalServerException("تعديل بيانات المنشأة", error)

        # 5. إرجاع النتيجة
        return FacilityResponse.from_orm(facility)


    # ⬅️ دالة التزويد بالـ Service المحقونة بالـ db (لحذف خطأ الـ ImportError)
   
    


    def search_facilities_as_super_admin(
        self,
        current_user, # كائن المستخدم المسجل حالياً
        search: Optional[str] = None,
        facility_type: Optional[str] = None,
        status: Optional[str] = None,
        city: Optional[str] = None,
        page: int = 1,
        page_size: int = 20
    ):
        # 1. التحقق من صلاحية مدير المنصة (الأمان)
        # يمكنك التحقق من RoleName أو RoleLevel (مثلاً Level 1 = SuperAdmin)
        if current_user.RoleId != 1 and getattr(current_user, "RoleName", "") != "SuperAdmin":
            raise HTTPException(
                status_code=403, 
                detail="هذه العملية مخصصة حصرياً لمدير المنصة الرئيسي."
            )

        # 2. حساب أوفسيت الترقيم
        skip = (page - 1) * page_size

        # 3. جلب النتائج من المستودع
        facilities, total = self.facility_repo.search_facilities_for_admin(
            search_term=search,
            facility_type=facility_type,
            facility_status=status,
            city=city,
            skip=skip,
            limit=page_size
        )

        return {
            "total": total,
            "page": page,
            "page_size": page_size,
            "data": facilities
        }



def get_facility_service(db: Session = Depends(get_db)) -> FacilityService:
        return FacilityService(db)

    