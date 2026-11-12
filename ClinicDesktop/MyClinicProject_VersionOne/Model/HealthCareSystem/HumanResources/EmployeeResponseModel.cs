

using System;
using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.HealthCareSystem.HumanResources
{
    
        /// <summary>
        /// نموذج استلام استجابة تسجيل الموظف من خادم FastAPI
        /// يطابق تماماً كلاس EmployeeRegisterResponse في بايثون
        /// </summary>
        public class EmployeeResponseModel
        {
            // الرقم الوظيفي المتولد في جدول tbEmployees (Primary Key)
            [JsonPropertyName("EmployeeId")]
            public int EmployeeId { get; set; }

            // رقم المستخدم المتولد في جدول Users (Foreign Key)
            [JsonPropertyName("UserId")]
            public int UserId { get; set; }

            // رقم المنشأة التي أُضيف إليها الموظف
            [JsonPropertyName("FacilityId")]
            public int? FacilityId { get; set; }

            // اسم الموظف الكامل
            [JsonPropertyName("UserFullName")]
            public string UserFullName { get; set; } = string.Empty;

            // اسم المستخدم لتسجيل الدخول
            [JsonPropertyName("Username")]
            public string Username { get; set; } = string.Empty;

            // رقم الدور الوظيفي (طبيب، استقبال، ممرض)
            [JsonPropertyName("RoleId")]
            public int RoleId { get; set; }

            // التخصص الطبي إن وُجد
            [JsonPropertyName("Specialization")]
            public string? Specialization { get; set; }

            // رقم ترخيص مزاولة المهنة
            [JsonPropertyName("MedicalLicenseNumber")]
            public string? MedicalLicenseNumber { get; set; }

            // حالة الموظف في النظام (Active)
            [JsonPropertyName("EmployeeStatus")]
            public string EmployeeStatus { get; set; } = "Active";

            // تاريخ ووقت إضافة الموظف لقاعدة البيانات
            [JsonPropertyName("EmployeeCreatedAt")]
            public DateTime EmployeeCreatedAt { get; set; }
        }
}
