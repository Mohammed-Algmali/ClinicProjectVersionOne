using MyClinicProject_VersionOne.Model.Enums;
using System;
using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.PlatformAdmin.UserAndPermissions.PlatformTeam
{
    public class PlatformUser
    {

        [JsonPropertyName("user_id")]
        public int UserId { get; set; }

        [JsonPropertyName("role_id")]
        public int RoleId { get; set; }

        [JsonPropertyName("full_name")]
        public string FullName { get; set; } = string.Empty;

        [JsonPropertyName("user_name")]
        public string UserName { get; set; } = string.Empty;

        [JsonPropertyName("user_email")]
        public string? UserEmail { get; set; }

        [JsonPropertyName("user_phone")]
        public string UserPhone { get; set; } = string.Empty;

        [JsonPropertyName("user_address")]
        public string UserAddress { get; set; } = string.Empty;

        [JsonPropertyName("user_status")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserStatus UserStatus { get; set; }

        [JsonPropertyName("user_created_at")]
        public DateTime UserCreatedAt { get; set; }
    }
}
