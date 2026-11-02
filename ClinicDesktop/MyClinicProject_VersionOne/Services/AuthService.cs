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
    public class AuthService
    {
        // استدعاء كائن الاتصال المشترك (Singleton)
        private readonly HttpClient _client = ApiClient.Instance.Client;

        /// <summary>
        /// دالة تسجيل الدخول غير المتزامنة (Async Login):
        /// تُرجع Tuple يحتوي على ثلاثة عناصر:
        /// 1. Success: (true/false) هل نجحت العملية؟
        /// 2. Data: كائن البيانات القادم من السيرفر (التوكن، الدور الوظيفي، الاسم).
        /// 3. ErrorMessage: نص الخطأ بالعربي في حال فشلت العملية لعرضه في الواجهة.
        /// </summary>
        public async Task<(bool Success, LoginResponseModel? Data, string ErrorMessage)> LoginAsync(string username, string password)
        {
            try
            {
                // 1. تجميع البيانات داخل كائن المودل
                var requestData = new LoginRequestModel
                {
                    Username = username,
                    Password = password
                };

                // 2. تحويل كائن C# إلى نص بتنسيق JSON
                string jsonPayload = JsonSerializer.Serialize(requestData);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 3. إرسال طلب POST إلى مسار الدخول في FastAPI
                HttpResponseMessage response = await _client.PostAsync("/auth/login", content);

                // 4. قراءة محتوى الرد القادم من الخادم
                string responseBody = await response.Content.ReadAsStringAsync();

                // -------------------------------------------------------------
                // الحالة الأولى: نجاح الدخول (رمز الاستجابة 200 OK)
                // -------------------------------------------------------------
                if (response.IsSuccessStatusCode)
                {
                    // خيار تجاهل حالة الأحرف الكبيرة والصغيرة في خصائص JSON
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var result = JsonSerializer.Deserialize<LoginResponseModel>(responseBody, options);

                    if (result != null)
                    {
                        // تثبيت التوكن في النسخة المركزية لـ ApiClient ليستخدمه كل التطبيق
                        ApiClient.Instance.SetToken(result.AccessToken);

                        return (true, result, string.Empty);
                    }
                }
                // -------------------------------------------------------------
                // الحالة الثانية: بيانات الدخول غير صحيحة (رمز 401 Unauthorized)
                // -------------------------------------------------------------
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, null, "اسم المستخدم أو كلمة المرور غير صحيحة.");
                }
                // -------------------------------------------------------------
                // الحالة الثالثة: خطأ في صيغة البيانات (رمز 422 Unprocessable Entity)
                // -------------------------------------------------------------
                else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    return (false, null, "البيانات المرسلة غير متوافقة مع متطلبات الخادم.");
                }
                // -------------------------------------------------------------
                // أي كود استجابة آخر
                // -------------------------------------------------------------
                else
                {
                    return (false, null, $"فشل تسجيل الدخول. رمز الخادم: {(int)response.StatusCode}");
                }

                return (false, null, "تعذر قراءة بيانات الاستجابة من الخادم.");
            }
            // في حال كان خادم FastAPI متوقفاً أو عنوان IP غير متاح
            catch (HttpRequestException)
            {
                return (false, null, "تعذر الاتصال بالخادم. تأكد من تشغيل خادم FastAPI أولاً.");
            }
            // في حال استغرق الخادم أكثر من 10 ثوانٍ للرد
            catch (TaskCanceledException)
            {
                return (false, null, "انتهت مهلة الانتظار دون استجابة من الخادم.");
            }
            // أي استثناء غير متوقع
            catch (Exception ex)
            {
                return (false, null, $"حدث خطأ غير متوقع: {ex.Message}");
            }
        }
    }
}