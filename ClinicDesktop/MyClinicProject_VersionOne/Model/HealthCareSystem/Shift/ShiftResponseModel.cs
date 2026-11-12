

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.HealthCareSystem.Shift
{
    public class ShiftResponseModel:ShiftCreateRequestModel
    {
        [JsonPropertyName("ShiftId")]
        public int ShiftId { get; set; }

        [JsonPropertyName("FacilityId")]
        public int FacilityId { get; set; }
    }
}
