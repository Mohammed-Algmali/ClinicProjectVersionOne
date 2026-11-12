

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.HealthCareSystem.HumanResources
{
    public class CreateEmployeeModel
    {


        [JsonPropertyName("MedicalLicenseNumber")]
        public string MedicalLicenseNumber { get; set; } = string.Empty;

        /// <summary>
        /// //////////Qualification
        /// </summary>
        [JsonPropertyName("Qualification")]
        public string? Qualification { get; set; }

        /// <summary>
        /// //////////Specialization
        /// </summary>

        [JsonPropertyName("Specialization")]
        public string? Specialization { get; set; }

        /// <summary>
        /// //////////EmergencyContactPhone
        /// </summary>


        [JsonPropertyName("EmergencyContactPhone")]
        public string? EmergencyContactPhone { get; set; }

        /// <summary>
        /// //////////Notes
        /// </summary>

        [JsonPropertyName("Notes")]
        public string? Notes { get; set; }
    }
}


