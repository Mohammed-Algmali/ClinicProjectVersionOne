

using MyClinicProject_VersionOne.Model.Enums;
using System;
using System.Text.Json.Serialization;
namespace MyClinicProject_VersionOne.Model.BaseModel
{
    public class FacilityBaseModel
    {
        [JsonPropertyName("facilityname")]
        public string FacilityName { get; set; } = String.Empty;


        [JsonPropertyName("typefacility")] 
        public FacilityTypes FacilityType { get; set; } 


        [JsonPropertyName("facilitylicensenumber")]
        public string FacilityLicenseNumber { get; set; } = String.Empty;


        [JsonPropertyName("facilitylicenseissuedate")]
        public DateTime FacilityLicenseIssueDate { get; set; } 


        [JsonPropertyName("facilitylicenseexpirydate")]
        public DateTime FacilityLicenseExpiryDate { get; set; } 


        [JsonPropertyName("facilitycode")]
        public string FacilityCode { get; set; } = String.Empty;



        [JsonPropertyName("facilitycountry")]
        public string FacilityCountry { get; set; } = String.Empty;


        [JsonPropertyName("facilitycity")]
        public string FacilityCity { get; set; } = String.Empty;

        [JsonPropertyName("facilityphone")]
        public string FacilityPhone { get; set; } = String.Empty;


        [JsonPropertyName("facilityemail")]
        public string FacilityEmail { get; set; } = String.Empty;



        [JsonPropertyName("facilityaddress")]
        public string FacilityAddress { get; set; } = string.Empty;

        [JsonPropertyName("facilitywebsiteuri")]
        public string FacilityWebsiteUrl { get; set; } = String.Empty;
    }
}
