from enum import Enum

class ClinicStatus(str, Enum):
    """حالات العيادة المشتركة (المستأجر) داخل نظام الـ SaaS"""
    OPEN = "open"                # العيادة تعمل وتستقبل المرضى والمواعيد بانتظام
    CLOSED = "closed"            # العيادة مغلقة (مثلاً بسبب انتهاء الاشتراك أو الإغلاق المؤقت)
    MAINTENANCE = "maintenance"  # العيادة في حالة صيانة وتحديث للبيانات
