
using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.PlatformAdmin.UserAndPermissions.RoleAndPermissios
{
    public class CreateRoleModel {
        [JsonPropertyName("RoleName")]
        public string RoleName { get; set; } = string.Empty;

        // تم تغييرها إلى "RoleDescription"
        [JsonPropertyName("RoleDescription")]
        public string? RoleDescription { get; set; }

        /*   [JsonPropertyName("RoleStatus")]
           public string RoleStatus { get; set; } = "Active";*/

        [JsonPropertyName("RoleLevel")]
        public int RoleLevel { get; set; } 

       
    }
}
