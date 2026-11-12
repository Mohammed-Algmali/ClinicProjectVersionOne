

namespace MyClinicProject_VersionOne.Services
{
    public static class UserSession
    {
        // رمز الأمان الرقمي (JWT Token) المستخدم للمصادقة مع الخادم
        public static string Token { get; private set; } = string.Empty;

        // رقم الدور الوظيفي للمستخدم (مثل: 1 = مدير نظام، 2 = مدير منشأة، 3 = طبيب)
        public static int RoleId { get; private set; }

        // الاسم الكامل للمستخدم لعرضه في شريط الترحيب بالواجهة
        public static string FullName { get; private set; } = string.Empty;

        // رقم المنشأة الطبية التي يتبع لها المستخدم الحالي (يقبل null إذا كان مدير نظام عام)
        public static long? FacilityId { get; private set; }

        // خاصية للتحقق هل المستخدم مسجل دخوله حالياً أم لا (إذا وجد توكن فهو مسجل)
        public static bool IsAuthenticated => !string.IsNullOrEmpty(Token);

        /// <summary>
        /// دالة تثبيت بيانات الجلسة:
        /// تُستدعى فور نجاح عملية تسجيل الدخول لتخزين بيانات الرد وضبط التوكن في عميل الـ API.
        /// </summary>
        public static void SetSession(LoginResponseModel response)
        {
            // تخزين التوكن في الذاكرة
            Token = response.AccessToken;

            // تخزين رقم الدور لتحديد الصلاحيات والشاشات المسموحة
            RoleId = response.RoleId;

            // تخزين الاسم الشخصي للمستخدم
            FullName = response.FullName ?? string.Empty;

            // تخزين رقم المنشأة التابع لها
            FacilityId = response.FacilityId;

            // حقن التوكن داخل ترويسة الطلبات في كائن الاتصال المركزي لترفق مع كل طلب قادم
            ApiClient.Instance.SetToken(Token);
        }

        /// <summary>
        /// دالة إنهاء الجلسة (تسجيل الخروج):
        /// تقوم بتفريغ كافة البيانات المحفوظة وإزالة التوكن من عميل الاتصال لمنع أي طلبات لاحقة.
        /// </summary>
        public static void ClearSession()
        {
            // مسح التوكن
            Token = string.Empty;

            // إعادة تصفير رقم الدور
            RoleId = 0;

            // مسح اسم المستخدم
            FullName = string.Empty;

            // مسح رقم المنشأة
            FacilityId = null;

            // إزالة التوكن من ترويسة الـ HttpClient في ApiClient
            ApiClient.Instance.ClearToken();
        }
    }
}
