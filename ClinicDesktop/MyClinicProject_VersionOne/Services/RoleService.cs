
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyClinicProject_VersionOne.Services
{
    /// <summary>
    /// خدمة إدارة الأدوار الوظيفية (Role Service):
    /// وظيفته:
    /// التواصل مع مسارات الأدوار في FastAPI لحفظ واسترجاع الأدوار والصلاحيات.
    /// الميزة الهامة:
    /// تعتمد تلقائياً على التوكن المحفوظ في ApiClient دون الحاجة لإعادة تمريره يدوياً.
    /// </summary>
    public class RoleService:BaseService
    {


      /*  // استخدام كائن الاتصال المركزي المشترك
        private readonly HttpClient _client = ApiClient.Instance.Client;

        /// <summary>
        /// دالة إنشاء دور وظيفي جديد عبر إرسال POST إلى FastAPI
        /// تُرجع: (نجاح/فشل، بيانات الدور المحفوظ، رسالة الخطأ بالعربي)
        /// </summary>
        public async Task<(bool Success, RoleListModel? Data, string ErrorMessage)> CreateRoleAsync(CreateRoleModel newRole)
        {
            try
            {
                // 1. تسلسل كائن الدور إلى صيغة JSON بحروف صغيرة (name, description, level)
                string jsonPayload = JsonSerializer.Serialize(newRole);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                // 2. إرسال الطلب إلى مسار إنشاء الأدوار في FastAPI (مثلاً: /roles أو /api/roles)
                HttpResponseMessage response = await _client.PostAsync("/roles/", content);

                string responseBody = await response.Content.ReadAsStringAsync();

                // -------------------------------------------------------------
                // الحالة 1: نجاح الإنشاء (Status Code: 200 OK أو 201 Created)
                // -------------------------------------------------------------
                if (response.IsSuccessStatusCode)
                {
                    var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                    var createdRole = JsonSerializer.Deserialize<RoleListModel>(responseBody, options);

                    return (true, createdRole, string.Empty);
                }
                // -------------------------------------------------------------
                // الحالة 2: عدم امتلاك صلاحية (403 Forbidden)
                // -------------------------------------------------------------
                else if (response.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    return (false, null, "ليس لديك صلاحية كمدير نظام لإنشاء أدوار جديدة.");
                }
                // -------------------------------------------------------------
                // الحالة 3: انتهاء الجلسة أو عدم وجود توكن (401 Unauthorized)
                // -------------------------------------------------------------
                else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return (false, null, "انتهت صلاحية الجلسة، يرجى تسجيل الدخول مجدداً.");
                }
                // -------------------------------------------------------------
                // الحالة 4: تكرار اسم الدور (409 Conflict أو 400 Bad Request)
                // -------------------------------------------------------------
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict ||
                         response.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    return (false, null, "اسم الدور موجود مسبقاً في النظام، يرجى اختيار اسم آخر.");
                }
                // -------------------------------------------------------------
                // الحالة 5: خطأ في صيغة المدخلات (422 Unprocessable Entity)
                // -------------------------------------------------------------
                else if (response.StatusCode == System.Net.HttpStatusCode.UnprocessableEntity)
                {
                    return (false, null, "البيانات المدخلة غير مطابقة لشروط الخادم.");
                }
                else
                {
                    return (false, null, $"فشل حفظ الدور. رمز الخادم: {(int)response.StatusCode}");
                }
            }
            catch (HttpRequestException)
            {
                return (false, null, "تعذر الاتصال بالخادم. يرجى التأكد من تشغيل FastAPI.");
            }
            catch (TaskCanceledException)
            {
                return (false, null, "انتهت مهلة انتظار الخادم دون استجابة.");
            }
            catch (Exception ex)
            {
                return (false, null, $"حدث خطأ غير متوقع: {ex.Message}");
            }
        }*/

        public async Task<(bool Success, RoleListModel? Data,string ErrorMessage)> CreateRoleAsync(CreateRoleModel newRole)
        {
            return await SendPostRequestAsync<RoleListModel, CreateRoleModel>("/roles/", newRole);
        }
    }
    
}


