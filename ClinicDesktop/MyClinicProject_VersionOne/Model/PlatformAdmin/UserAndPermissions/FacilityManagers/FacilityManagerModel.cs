

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.PlatformAdmin.UserAndPermissions.FacilityManagers
{
    public class FacilityManagerModel:UserBaseModel
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; } = string.Empty;


        [JsonPropertyName("UserPassword")]
        public string? UserPassword { get; set; } 


        [JsonPropertyName("RoleId")]
        public int RoleId { get; set; }

    }
}
