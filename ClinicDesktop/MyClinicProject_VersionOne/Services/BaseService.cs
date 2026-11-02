using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyClinicProject_VersionOne.Services
{
    public abstract  class BaseService
    {
        //  استخدام كائن الاتصال المركزي المشترك لكل الخدمات
        protected readonly HttpClient _client = ApiClient.Instance.Client;

        //
        private readonly JsonSerializerOptions _jsonOption = new() { PropertyNameCaseInsensitive = true };


        protected async Task<(bool Success,T? Data, string ErrorMessage)> SendPostRequestAsync<T,TRequest>(string endpoint,TRequest payload)
        {
            try
            {
                string jsonPayload = JsonSerializer.Serialize(payload);
                var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

                var response = await _client.PostAsync(endpoint, content);

                string responseBody = await response.Content.ReadAsStringAsync();

                // -------------------------------------------------------------
                // الحالة 1: نجاح الإنشاء (Status Code: 200 OK أو 201 Created)
                // -------------------------------------------------------------
                if (response.IsSuccessStatusCode)
                {
                    var data = JsonSerializer.Deserialize<T>(responseBody, _jsonOption);
                    return (true, data, String.Empty);
                }

                //اذا حدث خطا من الخادم نقوم ب تحليل رساله الخطا
                string errorMessage = ExtractErrorMessage(response.StatusCode, responseBody);
                return (false, default, errorMessage);
            }

            catch (HttpRequestException)
            {
                return (false, default, "تعذر الاتصال بالخادم. يرجى التأكد من تشغيل FastAPI.");
            }
            catch (TaskCanceledException)
            {
                return (false, default, "انتهت مهلة انتظار الخادم دون استجابة.");
            }
            catch (Exception ex)
            {
                return (false, default, $"حدث خطأ غير متوقع: {ex.Message}");
            }
        }

        //
        // داله استخراج وتوحيد رساله الخطاء بناء على كود الحاله
        private string ExtractErrorMessage(HttpStatusCode statusCode,string responseBody)
        {

            try
            {
                using var doc = JsonDocument.Parse(responseBody);
                if (doc.RootElement.TryGetProperty("detail", out var detailElement))
                {
                    string? apiDetail = detailElement.GetString();
                    if (!string.IsNullOrEmpty(apiDetail))
                        return apiDetail;
                }
               

            }

            catch
            { }

            return statusCode switch
            {
                HttpStatusCode.Forbidden => "ليس لديك صلاحية الكافيه لتنفيذ هذا  الاجراء ",
                HttpStatusCode.Unauthorized=>"انتهت صلاحيه الجلسه يرجى تسجيل الدخول مجددا",
                HttpStatusCode.Conflict=>"البيانات المدخله موجوده مسبقا في النظام او حدث تعارض",
                HttpStatusCode.BadRequest =>"الطلب غير صالح يرجى التحقق من المدخلات",
                HttpStatusCode.UnprocessableEntity =>"البيانات المدخله غير مطابقه لشروط الخادم",
                 _=>$"statusCode(int)فضل حفظ البيانات رمز الخادم"
            };
                   


            
        }

    }
}
