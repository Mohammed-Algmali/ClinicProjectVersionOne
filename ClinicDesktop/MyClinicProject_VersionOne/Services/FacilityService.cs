
using System;
using System.Collections.Generic;

using System.Threading.Tasks;
namespace MyClinicProject_VersionOne.Services
{
    public class FacilityService:BaseService
    {
      

        public async Task<(bool Success, FacilityListModel?Data, string ErrorMessage)> CreateFacilityAsync(FacilityRegistrationRequestModel newFacility) 
        {
            return await SendPostRequestAsync<FacilityListModel, FacilityRegistrationRequestModel>("/facilities/register", newFacility);
        }


        public async Task<(bool Success, List<FacilityListModel>? Data, string ErrorMessage)> GetAllFacilitiesAsync()
        {
            return await SendGetRequestAsync<List<FacilityListModel>>("/facilities/");
        }

        public async Task<(bool Success, FacilityResponseModel? Data, string ErrorMessage)> GetFacilityByIdAsync(int facilityId)
        {
            return await SendGetRequestAsync<FacilityResponseModel>($"/facilities/{facilityId}");
        }


        public async Task<(bool Success, FacilityResponseModel? Data, string ErrorMessage)> UpdateFacilityAsync(int facilityId, CreateFacilityModel model)


        {
            return await SendPutRequestAsync<FacilityResponseModel, CreateFacilityModel>($"/facilities/{facilityId}", model);



        }


        /// <param name="search">نص البحث (اسم المنشأة، الكود، الهاتف، أو رقم الترخيص)</param>
        /// <param name="facilityType">فلتر نوع المنشأة (مستشفى، عيادة، مجمع طبي...)</param>
        /// <param name="status">فلتر حالة المنشأة (Active، Suspended، Inactive)</param>
        /// <param name="city">فلتر المدينة أو المحافظة</param>
        /// <param name="page">رقم الصفحة المطلوبة للعرض</param>
        /// <param name="pageSize">عدد النتائج في كل صفحة</param>
        /// <returns>كائن يحتوي على إجمالي النتائج وقائمة المنشآت المطابقة في الصفحة الحالية</returns>
        public async Task<(bool Success, FacilitySearchResponseModel? Data, string ErrorMessage)> SearchFacilitiesForAdminAsync(
            string? search = null,
            string? facilityType = null,
            string? status = null,
            string? city = null,
            int page = 1,
            int pageSize = 20)
        {
            // 1. تهيئة معاملات الترقيم الأساسية في الرابط
            var queryParams = new List<string>
            {
                $"page={page}",
                $"page_size={pageSize}"
            };

            // 2. إضافة فلاتر البحث الاختيارية فقط إذا كانت غير فارغة (مع ترميز النص العربي لحماية الرابط)
            if (!string.IsNullOrWhiteSpace(search))
                queryParams.Add($"search={Uri.EscapeDataString(search.Trim())}");

            if (!string.IsNullOrWhiteSpace(facilityType))
                queryParams.Add($"facility_type={Uri.EscapeDataString(facilityType)}");

            if (!string.IsNullOrWhiteSpace(status))
                queryParams.Add($"status={Uri.EscapeDataString(status)}");

            if (!string.IsNullOrWhiteSpace(city))
                queryParams.Add($"city={Uri.EscapeDataString(city)}");

            // 3. تجميع الرابط النهائي وإرسال طلب GET إلى مسار البحث الإداري
            string endpoint = $"/facilities/admin/search?{string.Join("&", queryParams)}";
            return await SendGetRequestAsync<FacilitySearchResponseModel>(endpoint);
        }
    }

    
}
