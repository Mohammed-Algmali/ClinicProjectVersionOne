from sqlalchemy.orm import Session
from fastapi import HTTPException, status
from typing import List, Optional
from datetime import datetime

# استيراد المستودعات المطلوبة
from app.modules.human_resources.repository import EmployeeRepository,ShiftRepository
from app.modules.users.repository import UserRepository, RoleRepository
from app.modules.human_resources.models import Employee
from app.modules.users.models import User,Role
from app.modules.medical_records.repository import MedicalFileRepository


# استيراد المخططات (Schemas)
from app.modules.human_resources.schemas import (
    EmployeeRegisterRequest,
    EmployeeRegisterResponse,EmployeeListItemResponse,ShiftResponse,ShiftCreate
)

# استيراد دالة تشفير كلمات المرور
from app.modules.auth.utils import get_password_hash


class EmployeeService:
    """
    خدمة إدارة الموظفين والأطباء:
    تتولى التحقق من القواعد الإدارية وتنفيذ الحفظ المشترك في جدولي Users و tbEmployees.
    """
    def __init__(self, db: Session):
        self.db = db
        # تهيئة المستودعات عبر جلسة قاعدة البيانات المشتركة
        self.employee_repo = EmployeeRepository(db)
        self.user_repo = UserRepository(db)
        self.role_repo = RoleRepository(db)
        self.medical_file_repo = MedicalFileRepository(db)

    def register_employee(
        self, 
        data: EmployeeRegisterRequest, 
        current_user_facility_id: Optional[int] = None
    ) -> EmployeeRegisterResponse:
        """
        تسجيل موظف أو طبيب جديد في المنشأة:
        1. التحقق من عدم تكرار اسم المستخدم.
        2. التحقق من وجود الدور الوظيفي المختار (RoleId).
        3. حفظ بيانات المستخدم في جدول Users وربطه بالمنشأة (FacilityId).
        4. حفظ البيانات التخصصية في جدول tbEmployees وربطه بـ (UserId).
        5. الحفظ النهائي المتزامن (Commit).
        """

        # ----------------------------------------------------------------------
        # 1. التحقق من صحة القواعد الإدارية
        # ----------------------------------------------------------------------
        # أ. التأكد من أن اسم المستخدم غير مكرر
        if self.user_repo.get_by_username(data.User.Username):
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=f"اسم المستخدم '{data.User.Username}' محجوز مسبقاً لمستخدم آخر."
            )

        # ب. التأكد من وجود الدور الوظيفي في النظام (طبيب، ممرض، استقبال...)
        role = self.role_repo.get_by_id(data.User.RoleId)
        if not role:
            raise HTTPException(
                status_code=status.HTTP_404_NOT_FOUND,
                detail=f"الدور الوظيفي ذو الرقم ({data.User.RoleId}) غير مسجل في النظام."
            )

        # ج. تحديد رقم المنشأة (إما الممرر أو من توكن المدير الحالي)
        facility_id = data.User.FacilityId or current_user_facility_id
        if not facility_id:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="يجب تحديد رقم المنشأة الطبية التابع لها هذا الموظف."
            )

        try:
            # ----------------------------------------------------------------------
            # 2. إنشاء حساب المستخدم في جدول Users
            # ----------------------------------------------------------------------
            user_dict = data.User.dict()
            
            # تشفير كلمة المرور
            plain_password = user_dict.pop("Password")
            user_dict["Password"] = get_password_hash(plain_password)
            
            # ربط المستخدم بمنشأة المدير وحالته
            user_dict["FacilityId"] = facility_id
            user_dict["UserStatus"] = "Active"

            # إنشاء المستخدم وحفظه مؤقتاً في نفس المعاملة
            new_user = self.user_repo.create_without_commit(user_dict)
            
            # توليد UserId فوراً لاستخدامه في الخطوة التالية
            self.db.flush()

            # ----------------------------------------------------------------------
            # 3. إنشاء سجل الموظف في جدول tbEmployees
            # ----------------------------------------------------------------------
            emp_dict = data.Employee.dict()
            
            # ربط سجل الموظف برقم المستخدم المتولد
            emp_dict["UserId"] = new_user.UserId
            emp_dict["EmployeeStatus"] = "Active"

            # إنشاء الموظف مؤقتاً
            new_employee = self.employee_repo.create_without_commit(emp_dict)



            generated_record_num = f"MRN-{datetime.now().year}-{new_user.UserId:04d}"


            # ج. تجهيز بيانات الملف الطبي
            medical_file_dict = {
                "user_id": new_user.UserId,
                "record_num": generated_record_num,
                "status": "Active",
                "notes": "ملف طبي مهني تم إنشاؤه تلقائياً للموظف عند التعيين",
               
            }
            new_medical_file = self.medical_file_repo.create_without_commit(medical_file_dict)

            # ----------------------------------------------------------------------
            # 4. تثبيت الحفظ الموحد للجدولين معاً (Commit)
            # ----------------------------------------------------------------------
            self.db.commit()
            self.db.refresh(new_user)
            self.db.refresh(new_employee)
            self.db.refresh(new_medical_file)


            # ----------------------------------------------------------------------
            # 5. بناء وإرجاع كائن الاستجابة
            # ----------------------------------------------------------------------
            return EmployeeRegisterResponse(
                EmployeeId=new_employee.EmployeeId,
                UserId=new_user.UserId,
                FacilityId=new_user.FacilityId,
                UserFullName=new_user.UserFullName,
                Username=new_user.Username,
                RoleId=new_user.RoleId,
                Specialization=new_employee.Specialization,
                MedicalLicenseNumber=new_employee.MedicalLicenseNumber,
                EmployeeStatus=new_employee.EmployeeStatus,
                EmployeeCreatedAt=new_employee.EmployeeCreatedAt
            )

        except HTTPException:
            # التراجع الفوري إذا كان خطأ تم رميه عمداً
            self.db.rollback()
            raise

        except Exception as error:
            # التراجع عن كافة العمليات في حال حدوث أي خطأ غير متوقع
            self.db.rollback()
            raise HTTPException(
                status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
                detail=f"حدث خطأ أثناء حفظ بيانات الموظف: {str(error)}"
            )




        
    def get_all_employees(self, facility_id: Optional[int] = None) -> List[EmployeeListItemResponse]:
        """
        دالة منطق الأعمال (Business Logic Layer):
        وظيفتها:
        1. استدعاء المستودع لجلب نتائج قاعدة البيانات الخام.
        2. تحويل النتائج (Data Mapping) إلى قائمة من نماذج Pydantic (EmployeeListItemResponse)
           لتخرج بصيغة JSON نظيفة ومحمية إلى واجهات WPF.
        """
        # 1. جلب السجلات من قاعدة البيانات عبر المستودع
        results = self.employee_repo.get_employees_with_user_and_role(facility_id)

        # 2. تحويل البيانات باستخدام الـ List Comprehension (الأسرع والأكثر كفاءة في بايثون)
        return [
            EmployeeListItemResponse(
                EmployeeId=r.EmployeeId,
                UserId=r.UserId,
                UserFullName=r.UserFullName,
                Username=r.Username,
                UserPhone=r.UserPhone,
                UserEmail=r.UserEmail,
                # وضع قيمة افتراضية "غير محدد" في حال كان الدور الوظيفي فارغاً
                RoleName=r.RoleName if r.RoleName else "غير محدد",
                Specialization=r.Specialization,
                MedicalLicenseNumber=r.MedicalLicenseNumber,
                EmployeeStatus=r.EmployeeStatus,
                EmployeeCreatedAt=r.EmployeeCreatedAt,
            )
            for r in results  # المرور على كل موظف وإضافته للقائمة
        ]


class ShiftService:
    """
    خدمة إدارة فترات الدوام والورديات:
    تتولى التحقق من القواعد الإدارية وتنفيذ الحفظ في جدول tbShifts.
    """
    def __init__(self, db: Session):
        self.db = db
        # تهيئة المستودع عبر جلسة قاعدة البيانات المشتركة
        self.shift_repo = ShiftRepository(db)

    def create_shift(
        self, 
        data: ShiftCreate, 
        current_user_facility_id: Optional[int] = None
    ) -> ShiftResponse:
        """
        إنشاء وتسجيل وردية عمل جديدة في المنشأة:
        1. تحديد رقم المنشأة (FacilityId).
        2. التحقق من عدم تكرار اسم الوردية في نفس المنشأة.
        3. حفظ بيانات الوردية في جدول tbShifts.
        4. بناء وإرجاع كائن الاستجابة (ShiftResponse).
        """

        # ----------------------------------------------------------------------
        # 1. التحقق من صحة القواعد الإدارية
        # ----------------------------------------------------------------------
        # أ. تحديد رقم المنشأة (إما الممرر في البيانات أو من توكن المدير الحالي)
        facility_id = data.FacilityId or current_user_facility_id
        if not facility_id:
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail="يجب تحديد رقم المنشأة الطبية التابع لها هذه الوردية."
            )

        # ب. التأكد من أن اسم الوردية غير مكرر داخل نفس المنشأة
        if self.shift_repo.get_by_name(data.ShiftName, facility_id):
            raise HTTPException(
                status_code=status.HTTP_400_BAD_REQUEST,
                detail=f"اسم الوردية '{data.ShiftName}' مسجل مسبقاً في هذه المنشأة."
            )

        try:
            # ----------------------------------------------------------------------
            # 2. إعداد وحفظ الوردية في جدول tbShifts
            # ----------------------------------------------------------------------
            shift_dict = data.dict()
            
            # ربط الوردية بمنشأة المدير وحالتها
            shift_dict["FacilityId"] = facility_id
            shift_dict["IsOvernight"] = (data.EndTime < data.StartTime)
            shift_dict["ShiftStatus"] = "Active"

            # إنشاء الوردية وحفظها في قاعدة البيانات عبر المستودع
            new_shift = self.shift_repo.create_shift(shift_dict)

            # ----------------------------------------------------------------------
            # 3. بناء وإرجاع كائن الاستجابة
            # ----------------------------------------------------------------------
            return ShiftResponse(
                ShiftId=new_shift.ShiftId,
                FacilityId=new_shift.FacilityId,
                ShiftName=new_shift.ShiftName,
                StartTime=new_shift.StartTime,
                EndTime=new_shift.EndTime,
                GracePeriodMinutes=new_shift.GracePeriodMinutes,
                TotalHours=new_shift.TotalHours,
                ShiftType=new_shift.ShiftType
            )

        except Exception as ex:
            raise InternalServerException("حفظ الوردية", error)