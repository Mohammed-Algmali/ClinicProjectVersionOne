# app/core/exceptions.py
from typing import Any, Optional
from fastapi import status


# 1. كلاس الأب الأساسي لجميع استثناءات المنظومة
class AppException(Exception):
  """كلاس الأب العام لأي خطأ منطقي في النظام الطبي"""

  def __init__(
      self,
      message: str,
      status_code: int = status.HTTP_400_BAD_REQUEST,
      error_code: str = "BUSINESS_ERROR",
  ):
    self.message = message
    # نص رسالة الخطأ الموجهة للمستخدم

    self.status_code = status_code
    # رمز حالة HTTP المقابل للخطأ

    self.error_code = error_code
    # كود رمزي للخطأ يفيد مطور الواجهة في معرفة نوع المشكلة


# 2. كلاس خطأ: عنصر غير موجود (404 Not Found)
class NotFoundException(AppException):
  """يُرمى عندما لا نجد عنصراً في قاعدة البيانات"""

  def __init__(self, resource_name: str, identifier: Any):
    super().__init__(
        message=f"{resource_name} ذو المعرف ({identifier}) غير موجود في النظام.",
        status_code=status.HTTP_404_NOT_FOUND,
        error_code="NOT_FOUND",
    )


# 3. كلاس خطأ: تكرار البيانات المحجوزة (Duplicate / Conflict 400)
class DuplicateException(AppException):
  """يُرمى عند تكرار اسم دور، أو اسم مستخدم، أو كود منشأة مأخوذ مسبقاً"""

  def __init__(self, resource_name: str, field_value: Any):
    super().__init__(
        message=(
            f"{resource_name} بالقيمة '{field_value}' مسجل مسبقاً في النظام،"
            " يرجى اختيار قيمة أخرى."
        ),
        status_code=status.HTTP_400_BAD_REQUEST,
        error_code="DUPLICATE_ENTRY",
    )


# 4. كلاس خطأ: انتهاك شرط طبي أو إداري (Business Validation Error)
class BusinessRuleException(AppException):
  """يُرمى عند مخالفة شرط خاص (مثل: موعد في الماضي أو إلغاء بعد فوات الأوان)"""

  def __init__(self, message: str):
    super().__init__(
        message=message,
        status_code=status.HTTP_422_UNPROCESSABLE_ENTITY,
        error_code="BUSINESS_RULE_VIOLATION",
    )



# ==============================================================================
# كلاس خطأ: فشل العمليات الداخلية أو قاعدة البيانات (Internal Server Error 500)
# ==============================================================================
class InternalServerException(AppException):
    """
    يُرمى عند حدوث خطأ غير متوقع أثناء الحفظ في قاعدة البيانات أو معالجة العمليات
    """
    def __init__(self, operation_name: str, error_detail: Optional[Any] = None):
        # إضافة تفاصيل الخطأ التقني إن وُجد
        err_msg = f": {str(error_detail)}" if error_detail else "."

        super().__init__(
            message=f"حدث خطأ أثناء {operation_name}{err_msg}",
            status_code=status.HTTP_500_INTERNAL_SERVER_ERROR,
            error_code="INTERNAL_SERVER_ERROR",
        )