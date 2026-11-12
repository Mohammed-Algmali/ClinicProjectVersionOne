from fastapi import APIRouter, Depends, status,Path
from sqlalchemy.orm import Session
from typing import List
from app.modules.human_resources.models import Employee

from app.core.database import get_db

# استيراد المخططات (Schemas)
from app.modules.human_resources.schemas import (
    EmployeeRegisterRequest,
    EmployeeRegisterResponse,EmployeeListItemResponse,ShiftResponse,ShiftCreate
)

# استيراد كلاس الخدمة
from app.modules.human_resources.services import EmployeeService,ShiftService

# استيراد التبعية الخاصة بفحص التوكن والمستخدم الحالي
from app.modules.auth.dependencies import get_current_user
from app.modules.users.models import User

# إنشاء الموجّه مع البادئة والوسوم لتوثيق Swagger
employees_router = APIRouter(
    prefix="/employees",
    tags=["الموارد البشرية والموظفون (Employees)"]
)

def get_employee_service(db: Session = Depends(get_db)) -> EmployeeService:
    return EmployeeService(db)

# ==============================================================================
# 1. مسار إضافة موظف أو طبيب جديد
# ==============================================================================
@employees_router.post(
    "/",
    response_model=EmployeeRegisterResponse,
    status_code=status.HTTP_201_CREATED,
    summary="تسجيل موظف أو طبيب جديد"
)
def register_new_employee(
    payload: EmployeeRegisterRequest,
    service: EmployeeService = Depends(get_employee_service),
    current_user: User = Depends(get_current_user)  # استخراج المستخدم والمنشأة من التوكن
):
    """
    تسجيل موظف جديد وربطه تلقائياً بمنشأة المدير المسجل دخوله حالياً.
    """
    return service.register_employee(
        data=payload,
        current_user_facility_id=current_user.FacilityId
    )

@employees_router.get(
    "/",
    response_model=List[EmployeeListItemResponse],
    status_code=status.HTTP_200_OK,
    summary="جلب قائمة موظفي العيادة"
)
def get_all_employees(
    service: EmployeeService = Depends(get_employee_service),
    current_user: User = Depends(get_current_user)
):

    """
    استرجاع موظفي منشأة المستخدم الحالي فقط عبر التوكن
    """
    return service.get_all_employees(facility_id=current_user.FacilityId)












#///////////////////////////////////////////////////
# 2. إنشاء موجّه الورديات ببادئة /shifts
shift_router = APIRouter(
    prefix="/shifts",
    tags=["إدارة فترات الدوام والورديات (Shifts)"]
)

def get_shift_service(db: Session = Depends(get_db)) -> ShiftService:
    return ShiftService(db)

# مسار إضافة وردية عمل جديدة
@shift_router.post(
    "/",
    response_model=ShiftResponse,
    status_code=status.HTTP_201_CREATED,
    summary="إنشاء وردية عمل جديدة"
)
def create_new_shift(
    payload: ShiftCreate,
    service: ShiftService = Depends(get_shift_service),
    current_user: User = Depends(get_current_user)
):
    return service.create_shift(
        data=payload,
        current_user_facility_id=current_user.FacilityId
    )