
using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.HealthCareSystem.HumanResources
{
    public class EmployeeRegistrationRequestModel
    {
        [JsonPropertyName("User")]
        public UserBaseModel User { get; set; } = new UserBaseModel();

        // يطابق Employee: EmployeeDetailsCreate في بايثون
        [JsonPropertyName("Employee")]
        public CreateEmployeeModel Employee { get; set; } = new CreateEmployeeModel();
    }
}
