using System.Collections.Generic;
using System.Text.Json.Serialization;
namespace MyClinicProject_VersionOne.Model.PlatformAdmin.FacilitiesManagement.Facility
{
    public class FacilitySearchResponseModel
    {
        
        
            /// <summary>
            /// العدد الإجمالي للمنشآت المطابقة لنتائج البحث في قاعدة البيانات
            /// </summary>
            [JsonPropertyName("total")]
            public int Total { get; set; }

            /// <summary>
            /// رقم الصفحة الحالية المعروضة
            /// </summary>
            [JsonPropertyName("page")]
            public int Page { get; set; }

            /// <summary>
            /// عدد السجلات المعروضة في الصفحة الواحدة
            /// </summary>
            [JsonPropertyName("page_size")]
            public int PageSize { get; set; }

            /// <summary>
            /// قائمة المنشآت المرجعة في هذه الصفحة لعرضها داخل جدول DataGrid
            /// </summary>
            [JsonPropertyName("data")]
            public List<FacilityListModel> Data { get; set; } = new List<FacilityListModel>();
        
    }

}
