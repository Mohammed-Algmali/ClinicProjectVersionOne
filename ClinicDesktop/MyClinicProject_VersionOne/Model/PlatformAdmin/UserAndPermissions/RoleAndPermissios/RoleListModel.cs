
using System;
using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.PlatformAdmin.UserAndPermissions.RoleAndPermissios
{
    public class RoleListModel:CreateRoleModel
    {
        [JsonPropertyName("role_id")]
        public int RoleId { get; set; }

      

        [JsonPropertyName("role_status")]
        public string RoleStatus { get; set; }

       

        [JsonPropertyName("role_createat")]
        public DateTime RoleCreateAt { get; set; }
    }
}
