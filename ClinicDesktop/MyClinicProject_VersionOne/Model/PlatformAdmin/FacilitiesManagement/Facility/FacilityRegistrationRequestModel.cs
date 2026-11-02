
using System.Text.Json.Serialization;


namespace MyClinicProject_VersionOne.Model.PlatformAdmin.FacilitiesManagement.Facility
{
    public class FacilityRegistrationRequestModel
    {
        [JsonPropertyName("Facility")]
        public CreateFacilityModel Facility { get; set; } = new CreateFacilityModel();

        [JsonPropertyName("Manager")]
        public FacilityManagerModel Manager { get; set; } = new FacilityManagerModel();
    

    }
}
