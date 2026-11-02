using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.Authentication
{

    /// <summary>
    /// نموذج نقل البيانات (Data Transfer Object - DTO):
    /// وظيفته:
    /// تجميع اسم المستخدم وكلمة المرور من واجهة WPF 
    /// وتحويلهما إلى صيغة JSON ليتم إرسالهما في جسم الطلب (Request Body) إلى خادم FastAPI.
    /// </summary>
    
   /// <summary>
        /// نموذج إرسال بيانات الدخول
        /// يطابق تماماً مخطط LoginRequest في auth/schemas.py
        /// </summary>
        public class LoginRequestModel
        {
            // إجبار C# على كتابة الاسم بحروف صغيرة "username" في الـ JSON
            [JsonPropertyName("username")]
            public string Username { get; set; } = string.Empty;

            // إجبار C# على كتابة الاسم بحروف صغيرة "password" في الـ JSON
            [JsonPropertyName("password")]
            public string Password { get; set; } = string.Empty;
        }
    }




