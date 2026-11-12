
using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.PlatformAdmin.FacilitiesManagement.Facility
{
    public class FacilityResponseModel:FacilityBaseModel
    {
        [JsonPropertyName("FacilityId")]
        public int FacilityId { get; set; } 

        [JsonPropertyName("FacilityStatus")]
        public string FacilityStatus { get; set; } = string.Empty;


        [JsonPropertyName("CreatedAt")]
        public string CreatedAt { get; set; } = string.Empty;
    }

}
