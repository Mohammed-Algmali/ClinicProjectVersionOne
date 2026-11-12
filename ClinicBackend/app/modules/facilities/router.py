from fastapi import APIRouter, Depends, status,Path
from sqlalchemy.orm import Session
from typing import List, Optional

from app.core.database import get_db
from app.modules.facilities.schemas import (
    FacilityRegisterRequest,
    FacilityRegisterResponse,
    FacilityListItemResponse,
    FacilityUpdateRequest,
    FacilityBase,FacilityResponse
)
from app.modules.facilities.services import FacilityService,get_facility_service
from app.modules.auth.dependencies import get_current_user
from app.modules.facilities.repository import FacilityRepository

# ==============================================================================
# 1. تهيئة موجه مسارات المنشآت الطبية
# ==============================================================================
router = APIRouter(
    prefix="/facilities",
    tags=["المنشآت الطبية (Facilities)"]
)
# تحديد البادئة /facilities لجميع المسارات في هذا الموديول


# ==============================================================================
# 2. مسار تسجيل منشأة جديدة مع حساب مديرها الإداري
# ==============================================================================
@router.post(
    "/register",
    response_model=FacilityRegisterResponse,
    status_code=status.HTTP_201_CREATED,
    summary="تسجيل منشأة جديدة مع حساب المدير في عملية واحدة"
)
def register_facility(
    data: FacilityRegisterRequest,
    db: Session = Depends(get_db)
):
    """
    استقبال طلب التسجيل المشترك (بيانات المنشأة + بيانات المدير) وتمريره للـ Service
    """
    service = FacilityService(db)
    # تهيئة طبقة منطق العمل وتمرير جلسة قاعدة البيانات لها

    return service.register_facility_with_manager(data)
    # تنفيذ العملية الذرية وإرجاع الاستجابة المكتملة بدون فروع




# في facilities/router.py
@router.get("/", response_model=List[FacilityListItemResponse])
def get_all_facilities(
    db: Session = Depends(get_db)
):
    service = FacilityService(db)
    return service.get_facilities_with_manager_list()



@router.put(
    "/{facility_id}",
    response_model=FacilityResponse,
    status_code=status.HTTP_200_OK,
    summary="تحديث بيانات المنشأة",
    description="يقوم بتعديل البيانات التشغيلية والموقع ومعلومات التواصل للمنشأة المحددة."
)
def update_facility(
    facility_id: int = Path(..., ge=1, description="رقم المنشأة"),
    data: FacilityUpdateRequest = ...,
    current_user = Depends(get_current_user),
    db: Session = Depends(get_db)  # ⬅️ استخدام get_db المباشر مثل بقية دوالك
):
    service = FacilityService(db)
    return service.update_facility(facility_id=facility_id, data=data, current_user=current_user)



@router.get(
    "/admin/search",
    summary="بحث واستعراض المنشآت لمدير المنصة",
    description="يتيح لمدير المنصة البحث بالاسم، الكود، الفلترة بالحالة، والترقيم."
)
def search_facilities_endpoint(
    search: Optional[str] = None,
    facility_type: Optional[str] = None,
    status: Optional[str] = None,
    city: Optional[str] = None,
    page: int = 1,
    page_size: int = 20,
    current_user = Depends(get_current_user),  # التوكن الحالي
    service: FacilityService = Depends(get_facility_service)
):
    return service.search_facilities_as_super_admin(
        current_user=current_user,
        search=search,
        facility_type=facility_type,
        status=status,
        city=city,
        page=page,
        page_size=page_size
    )


@router.get(
    "/{facility_id}",
    response_model=FacilityResponse,
    summary="جلب بيانات منشأة واحدة بالمعرف"
)
def get_facility_by_id(
    facility_id: int = Path(..., ge=1, description="رقم المنشأة"),
    db: Session = Depends(get_db)
):
    facility = FacilityRepository(db).get_by_id(facility_id)
    if not facility:
        raise HTTPException(
            status_code=status.HTTP_404_NOT_FOUND,
            detail=f"المنشأة برقم '{facility_id}' غير موجودة."
        )
    return FacilityResponse.from_orm(facility)