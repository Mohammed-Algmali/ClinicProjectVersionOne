/*using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
=
namespace MyClinicProject_VersionOne.Services
{
    // ============================================================
    // UserService
    //
    // هذا الكلاس مسؤول عن جميع العمليات المتعلقة بالمستخدمين.
    //
    // حاليًا يحتوي على عمليتين:
    //
    // 1. CreateUserAsync
    //    إرسال مستخدم جديد إلى FastAPI.
    //
    // 2. GetUsersAsync
    //    جلب قائمة المستخدمين من FastAPI.
    //
    // UserService لا يتعامل مباشرة مع HttpClient.
    // بل يستخدم ApiClient المسؤول عن الاتصال العام بالـ API.
    //
    // المسار:
    //
    // UserViewModel
    //       ↓
    // UserService
    //       ↓
    // ApiClient
    //       ↓
    // FastAPI
    // ============================================================

    public class UserService
    {
        // --------------------------------------------------------
        // متغير خاص من نوع ApiClient.
        //
        // نستخدمه لإرسال واستقبال البيانات من FastAPI.
        // --------------------------------------------------------
        private readonly ApiClient _apiClient;


        // ========================================================
        // Constructor
        //
        // يتم استدعاؤه عند إنشاء UserService.
        //
        // نستقبل ApiClient من الخارج ونحتفظ به
        // لاستخدامه في عمليات المستخدمين.
        // ========================================================

        public UserService(ApiClient apiClient)
        {
            _apiClient = apiClient;
        }


        // ========================================================
        // CreateUserAsync
        //
        // هذه الدالة مسؤولة عن إنشاء مستخدم جديد.
        //
        // تستقبل:
        // CreateUserModel
        //
        // ثم ترسل البيانات إلى FastAPI باستخدام:
        //
        // POST /users/
        //
        // Async لأن الاتصال مع API يحتاج إلى await.
        // ========================================================

        public async Task CreateUserAsync(CreateUserModel user)
        {
            // ----------------------------------------------------
            // إرسال بيانات المستخدم إلى FastAPI.
            //
            // PostAsJsonAsync تقوم بتحويل كائن C#
            // إلى JSON وإرساله إلى API.
            // ----------------------------------------------------

            HttpResponseMessage response =
                await _apiClient.Client.PostAsJsonAsync(
                    "/users/",
                    user);


            // ----------------------------------------------------
            // التحقق من نتيجة الطلب.
            //
            // إذا أعاد FastAPI خطأ مثل:
            //
            // 400
            // 401
            // 422
            // 500
            //
            // لن نعتبر العملية ناجحة.
            // ----------------------------------------------------

            if (!response.IsSuccessStatusCode)
            {
                // قراءة رسالة الخطأ التي أرسلها FastAPI.
                string errorContent =
                    await response.Content.ReadAsStringAsync();


                // إرسال الخطأ إلى ViewModel عن طريق Exception.
                throw new HttpRequestException(
                    $"HTTP {(int)response.StatusCode} " +
                    $"{response.StatusCode}\n\n" +
                    errorContent);
            }
        }


        // ========================================================
        // GetUsersAsync
        //
        // هذه الدالة مسؤولة عن جلب جميع المستخدمين
        // من FastAPI.
        //
        // تستخدم:
        //
        // GET /users/
        //
        // النتيجة ستكون قائمة من UserModel.
        // ========================================================

        public async Task<List<UserModel>> GetUsersAsync()
        {
            // ----------------------------------------------------
            // إرسال طلب GET إلى FastAPI لجلب المستخدمين.
            // ----------------------------------------------------

            HttpResponseMessage response =
                await _apiClient.Client.GetAsync("/users/");


            // ----------------------------------------------------
            // التحقق من نجاح الطلب.
            //
            // إذا كان هناك خطأ، نقرأ رسالة FastAPI
            // ونرسلها إلى ViewModel.
            // ----------------------------------------------------

            if (!response.IsSuccessStatusCode)
            {
                // قراءة تفاصيل الخطأ من FastAPI.
                string errorContent =
                    await response.Content.ReadAsStringAsync();


                // إنشاء Exception يحتوي على رقم الخطأ
                // ورسالة FastAPI.
                throw new HttpRequestException(
                    $"HTTP {(int)response.StatusCode} " +
                    $"{response.StatusCode}\n\n" +
                    errorContent);
            }


            // ----------------------------------------------------
            // قراءة JSON القادم من FastAPI وتحويله إلى:
            //
            // List<UserModel>
            //
            // إذا لم يرجع API أي بيانات، نرجع قائمة فارغة
            // بدلًا من null.
            // ----------------------------------------------------

            return await response.Content
                .ReadFromJsonAsync<List<UserModel>>()
                ?? new List<UserModel>();
        }
    }
}*/