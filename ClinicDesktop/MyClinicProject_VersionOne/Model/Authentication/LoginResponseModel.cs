using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.Authentication
{
    /// <summary>
    /// نموذج استلام البيانات (Response Model):
    /// وظيفته:
    /// استقبال الرد القادم من FastAPI بعد نجاح عملية التحقق،
    /// وتحويل كود الـ JSON إلى كائن C# يسهل استخدامه داخل الواجهات.
    /// </summary>
    public class LoginResponseModel
    {
        /// <summary>
        /// رمز التحقق الرقمي (JWT Token):
        /// تستخدم خاصية [JsonPropertyName] للربط مع الاسم القادم من بايثون "access_token".
        /// هذا الرمز هو تصريح المرور الذي سنرسله مع كل الطلبات التالية (إضافة مريض، حجز موعد).
        /// </summary>
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "bearer";

        /// <summary>
        /// رقم الدور الوظيفي (RoleId):
        /// 1 = مدير النظام (Super Admin)
        /// 2 = مدير المنشأة (Facility Manager)
        /// 3 = طبيب (Doctor)
        /// ... إلخ
        /// </summary>
        [JsonPropertyName("role_id")]
        public int RoleId { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("facility_id")]
        public long? FacilityId { get; set; }
    }
}
