using System;

using System.Net.Http;
using System.Net.Http.Headers;

namespace MyClinicProject_VersionOne.Services
{
    /// <summary>
    /// كلاس إدارة الاتصال الشبكي (HTTP Client Manager):
    /// المبدأ الهندسي:
    /// يُطبّق نمط التصميم الفردي (Singleton Pattern).
    /// الهدف:
    /// 1. إنشاء نسخة واحدة فقط من HttpClient في الذاكرة لتجنب استنزاف موارد النظام (Socket Exhaustion).
    /// 2. تخزين رمز الدخول (Token) في مكان واحد مركزي ليكون متاحاً لكل خدمات العيادة (Patients, Doctors, Shifts).
    /// </summary>
    public class ApiClient
    {
        // =========================================================================
        // 1. تطبيق نمط Singleton (النسخة الوحيدة المشتركة)
        // =========================================================================

        // كائن Lazy يضمن عدم إنشاء النسخة في الذاكرة إلا عند أول طلب لها (Thread-Safe)
        private static readonly Lazy<ApiClient> _instance =
            new Lazy<ApiClient>(() => new ApiClient());

        /// <summary>
        /// نقطة الوصول العامة للنسخة المشتركة من أي مكان في المشروع
        /// الاستدعاء: ApiClient.Instance
        /// </summary>
        public static ApiClient Instance => _instance.Value;


        // =========================================================================
        // 2. الكائن المسؤول عن إرسال واستقبال الطلبات
        // =========================================================================
        private readonly HttpClient _httpClient;

        /// <summary>
        /// دالة البناء (Constructor) خاصة (Private):
        /// لمنع أي مبرمج من كتابة: new ApiClient() من الخارج بالخطأ.
        /// </summary>
        private ApiClient()
        {
            _httpClient = new HttpClient
            {
                // عنوان الخادم الأساسي (FastAPI) أثناء العمل المحلي
                BaseAddress = new Uri("http://127.0.0.1:8000"),

                // مهلة زمنية 10 ثوانٍ؛ إذا لم يستجب السيرفر يتم إلغاء الطلب فوراً حتى لا تتجمد الواجهة
                Timeout = TimeSpan.FromSeconds(10)
            };

            // إبلاغ الخادم بأننا نطلب دائماً استجابة بصيغة JSON
            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));




            // إضافة الـ LoggingHandler لاعتراض الطلبات وطباعتها
            var handler = new LoggingHandler(new HttpClientHandler());

            _httpClient = new HttpClient(handler)
            {
                BaseAddress = new Uri("http://127.0.0.1:8000"),
                Timeout = TimeSpan.FromSeconds(10)
            };

            _httpClient.DefaultRequestHeaders.Accept.Clear();
            _httpClient.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
        }


        // =========================================================================
        // 3. إدارة رمز المصادقة (Bearer Token)
        // =========================================================================

        /// <summary>
        /// دالة لتثبيت التوكن بعد نجاح تسجيل الدخول.
        /// بعد استدعاء هذه الدالة، سيتم إرسال هذا التوكن تلقائياً في ترويسة (Header) كل طلب لاحق.
        /// </summary>
        public void SetToken(string token)
        {
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", token);
        }

        /// <summary>
        /// دالة لحذف التوكن عند تسجيل الخروج (Logout) لمنع إرسال أي طلبات غير مصرح بها.
        /// </summary>
        public void ClearToken()
        {
            _httpClient.DefaultRequestHeaders.Authorization = null;
        }


        // =========================================================================
        // 4. إتاحة HttpClient للخدمات
        // =========================================================================

        /// <summary>
        /// خاصية تتيح للخدمات (AuthService, PatientService...) استخدام كائن HttpClient لإرسال الطلبات.
        /// </summary>
        public HttpClient Client => _httpClient;
    }
}