
using System;

using System.Text.Json.Serialization;
namespace MyClinicProject_VersionOne.Model.PlatformAdmin.FacilitiesManagement.Facility
{
    public  class FacilityListModel
    {
        /* [JsonPropertyName("FacilityId")]
         public int FacilityId { get; set; } // المفتاح الأساسي المتولد في قاعدة البيانات

         [JsonPropertyName("IsActive")]
         public bool IsActive { get; set; } = true; // حالة المنشأة (مفعلة / موقوفة)

         [JsonPropertyName("CreatedAt")]
         public DateTime? CreatedAt { get; set; } // تاريخ تسجيل المنشأة في النظام*/



        [JsonPropertyName("FacilityId")]
        public int FacilityId { get; set; }

        [JsonPropertyName("FacilityCode")]
        public string? FacilityCode { get; set; }

        [JsonPropertyName("FacilityName")]
        public string FacilityName { get; set; } = string.Empty;

        [JsonPropertyName("ManagerFullName")]
        public string ManagerFullName { get; set; } = string.Empty; // اسم المدير القادم من جدول المستخدمين

        [JsonPropertyName("FacilityCity")]
        public string? FacilityCity { get; set; }

        [JsonPropertyName("FacilityAddress")]
        public string? FacilityAddress { get; set; }

        [JsonPropertyName("FacilityStatus")]
        public string FacilityStatus { get; set; } = "Active";
    }
    
}



