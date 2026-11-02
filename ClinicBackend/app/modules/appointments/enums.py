from enum import Enum

class VisitStatus(str, Enum):
    """حالات الزيارة الحالية داخل عيادة الطبيب"""
    WAITING = "waiting"          # المريض في صالة الانتظار ولم يدخل بعد
    INPROGRESS = "inprogress"    # المريض داخل غرفة الكشف مع الطبيب الآن
    COMPLETED = "completed"      # انتهى الطبيب من الكشف واعمد التشخيص
    CANCELLED = "cancelled"      # تم إلغاء الزيارة

class VisitType(str, Enum):
    """نوع الزيارة المحددة للمريض"""
    INITIAL = "initial"          # زيارة أولية (كشف لأول مرة)
    EMERGENCY = "emergency"      # حالة طارئة مستعجلة
    FOLLOW_UP = "follow up"      # مراجعة ومتابعة (الاستشارة)

class AppointmentStatus(str, Enum):
    """حالات الموعد المحجوز من قبل المريض"""
    PENDING = "pending"          # موعد معلق في انتظار التأكيد
    CONFIRMED = "confirmed"      # موعد مؤكد ومحجوز
    CHECK_IN = "check in"        # تم تسجيل وصول المريض للمستشفى وفي انتظار دوره
    COMPLETED = "completed"      # موعد مكتمل ومنتهي بنجاح
    CANCELLED = "cancelled"      # موعد ملغي من المريض أو العيادة
    NO_SHOW = "no show"          # لم يحضر صاحب الموعد في الوقت المحدد
