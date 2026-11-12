using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace MyClinicProject_VersionOne.Services
{
    /// <summary>
    /// خدمة المصادقة والتحقق (Authentication Service):
    /// وظيفته:
    /// 1. إرسال بيانات الدخول إلى مسار "/auth/login" في FastAPI.
    /// 2. معالجة رد الخادم وفك تشفير كود الـ JSON إلى كائن LoginResponseModel.
    /// 3. تثبيت التوكن داخل ApiClient المشترك فور نجاح الدخول.
    /// 4. معالجة سيناريوهات الأخطاء (انقطاع الاتصال بالسيرفر، كلمة مرور غير صحيحة، انتهاء المهلة).
    /// </summary>
    public class AuthService : BaseService
    {
        /// <summary>
        /// خدمة المصادقة وتسجيل الدخول:
        /// ترث من BaseService للاستفادة من الدوال العامة الموحدة للاتصال بالخادم.
        /// </summary>

        /// <summary>
        /// دالة تسجيل الدخول غير المتزامنة:
        /// تأخذ اسم المستخدم وكلمة المرور وترسلهما لمسار /auth/login،
        /// وتُرجع نتيجة العملية (نجاح/فشل، كائن البيانات، نص الخطأ بالعربي).
        /// </summary>
        public async Task<(bool Success, LoginResponseModel? Data, string ErrorMessage)> LoginAsync(string username, string password)
        {
            // 1. تجهيز كائن الطلب وتعبئته بالبيانات المدخلة من واجهة المستخدم
            var requestData = new LoginRequestModel
            {
                Username = username,
                Password = password
            };

            // 2. إرسال الطلب عبر دالة الأب BaseService إلى مسار الدخول في FastAPI
            var result = await SendPostRequestAsync<LoginResponseModel, LoginRequestModel>("/auth/login", requestData);

            // 3. التحقق من نجاح العملية والحصول على بيانات الرد
            if (result.Success && result.Data != null)
            {
                // حفظ التوكن وبيانات المستخدم في كلاس الجلسة UserSession وحقنها في ترويسة الطلبات
                UserSession.SetSession(result.Data);
            }

            // 4. إرجاع النتيجة الكاملة للـ ViewModel ليتصرف بناءً عليها (فتح النافذة أو إظهار الخطأ)
            return (result.Success, result.Data, result.ErrorMessage);
        }
    }  
}