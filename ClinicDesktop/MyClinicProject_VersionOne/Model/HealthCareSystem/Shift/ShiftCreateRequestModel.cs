

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.HealthCareSystem.Shift
{
    public class ShiftCreateRequestModel
    {
        [JsonPropertyName("ShiftName")]
        public string ShiftName { get; set; } = string.Empty;

        // يُرسل بصيغة وقت منسق "HH:mm:ss" ليقرأه بايثون كـ time مباشرة
        [JsonPropertyName("StartTime")]
        public string StartTime { get; set; } = string.Empty;

        [JsonPropertyName("EndTime")]
        public string EndTime { get; set; } = string.Empty;

        [JsonPropertyName("GracePeriodMinutes")]
        public int GracePeriodMinutes { get; set; } = 15;

        // إجمالي ساعات العمل (مثال: 6.0 أو 8.5)
        [JsonPropertyName("TotalHours")]
        public double TotalHours { get; set; }

        // نوع الوردية (صباحي، مسائي، ليلي)
        [JsonPropertyName("ShiftType")]
        public string ShiftType { get; set; } = string.Empty;

      
    }
}
    

