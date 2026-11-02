
using System;

using System.Text.Json.Serialization;
namespace MyClinicProject_VersionOne.Model.PlatformAdmin.FacilitiesManagement.Facility
{
    public  class FacilityListModel:FacilityBaseModel
    {
        [JsonPropertyName("FacilityId")]
        public int FacilityId { get; set; } // المفتاح الأساسي المتولد في قاعدة البيانات

        [JsonPropertyName("IsActive")]
        public bool IsActive { get; set; } = true; // حالة المنشأة (مفعلة / موقوفة)

        [JsonPropertyName("CreatedAt")]
        public DateTime? CreatedAt { get; set; } // تاريخ تسجيل المنشأة في النظام
    }
    
}
