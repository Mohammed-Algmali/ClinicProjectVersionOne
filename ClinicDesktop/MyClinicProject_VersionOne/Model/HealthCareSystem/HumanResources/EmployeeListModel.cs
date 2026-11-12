

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.HealthCareSystem.HumanResources
{
    public class EmployeeListModel
    {
        [JsonPropertyName("EmployeeId")]
        public long EmployeeId { get; set; }

        // الاسم الكامل للموظف/الطبيب
        [JsonPropertyName("UserFullName")]
        public string UserFullName { get; set; } = string.Empty;

        // اسم المستخدم لتسجيل الدخول
        [JsonPropertyName("Username")]
        public string Username { get; set; } = string.Empty;

        // رقم الهاتف الشخصي
        [JsonPropertyName("UserPhone")]
        public string? UserPhone { get; set; }

        // البريد الإلكتروني
        [JsonPropertyName("UserEmail")]
        public string? UserEmail { get; set; }

        // اسم الدور الوظيفي بالعربي (طبيب، موظف استقبال، ممرض...)
        [JsonPropertyName("RoleName")]
        public string RoleName { get; set; } = string.Empty;

        // التخصص الطبي (للأطباء فقط مثل: باطنية، جراحة...)
        [JsonPropertyName("Specialization")]
        public string? Specialization { get; set; }

        // رقم ترخيص مزاولة المهنة
        [JsonPropertyName("MedicalLicenseNumber")]
        public string? MedicalLicenseNumber { get; set; }

        // الحالة الوظيفية (Active / Inactive)
        [JsonPropertyName("EmployeeStatus")]
        public string EmployeeStatus { get; set; } = "Active";

       
    }
}
