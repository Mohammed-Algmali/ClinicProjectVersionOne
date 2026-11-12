

using MyClinicProject_VersionOne.Model.Enums;
using System;
using System.Text.Json.Serialization;
namespace MyClinicProject_VersionOne.Model.BaseModel
{
    public class FacilityBaseModel
    {
        [JsonPropertyName("FacilityName")]
        public string FacilityName { get; set; } = string.Empty;

        [JsonPropertyName("FacilityType")]
        public string FacilityType { get; set; } = string.Empty; // يفضل إرسالها كنص string

        [JsonPropertyName("FacilityLicenseNumber")]
        public string FacilityLicenseNumber { get; set; } = string.Empty;

        // مطابقة لاسم العمود في كلاس بايثون: LicenseIssueDate
        [JsonPropertyName("LicenseIssueDate")]
        public DateTime? FacilityLicenseIssueDate { get; set; }

        // مطابقة لاسم العمود في كلاس بايثون: LicenseExpiryDate
        [JsonPropertyName("LicenseExpiryDate")]
        public DateTime? FacilityLicenseExpiryDate { get; set; }

        [JsonPropertyName("FacilityCode")]
        public string FacilityCode { get; set; } = string.Empty;

        [JsonPropertyName("FacilityCountry")]
        public string FacilityCountry { get; set; } = "اليمن";

        [JsonPropertyName("FacilityCity")]
        public string FacilityCity { get; set; } = string.Empty;

        [JsonPropertyName("FacilityPhone")]
        public string FacilityPhone { get; set; } = string.Empty;

        [JsonPropertyName("FacilityEmail")]
        public string? FacilityEmail { get; set; }

        [JsonPropertyName("FacilityAddress")]
        public string FacilityAddress { get; set; } = string.Empty;

        // مطابقة لاسم العمود في كلاس بايثون: FacilityWebSite
        [JsonPropertyName("FacilityWebSite")]
        public string? FacilityWebsiteUrl { get; set; }
    }
}
