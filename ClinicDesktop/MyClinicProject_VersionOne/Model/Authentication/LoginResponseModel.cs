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

        /// <summary>
        /// نوع الرمز (غالباً يكون "bearer").
        /// </summary>
        [JsonPropertyName("token_type")]
        public string TokenType { get; set; } = "bearer";

        /// <summary>
        /// الدور الوظيفي للمستخدم (Role):
        /// يفيدنا في توجيه المستخدم بعد الدخول (مثلاً: إذا كان "Doctor" نفتح شاشة الطبيب، وإذا كان "Receptionist" نفتح شاشة الاستقبال).
        /// </summary>
        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty;

        /// <summary>
        /// الاسم الشخصي الكامل للمستخدم (مثل: د. أحمد علي) لعرضه كرسالة ترحيبية في الواجهة الرئيسية.
        /// </summary>
        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;
    }
}
