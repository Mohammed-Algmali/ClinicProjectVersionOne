

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.BaseModel
{
    public class UserBaseModel
    {
        [JsonPropertyName("FullName")]
        public string UserFullName { get; set; } = string.Empty;

       
        [JsonPropertyName("Phone")]
        public string UserPhone { get; set; } = string.Empty;

        [JsonPropertyName("Email")]
        public string UserEmail { get; set; } = string.Empty;
    }
}
