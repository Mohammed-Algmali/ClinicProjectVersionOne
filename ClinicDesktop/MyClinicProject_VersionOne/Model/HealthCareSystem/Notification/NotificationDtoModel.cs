using System;


namespace MyClinicProject_VersionOne.Model.HealthCareSystem.Notification
{
    public class NotificationDtoModel
    {

        public long NotificationId { get; set; }

        public string Icon { get; set; } = "🔔";

        public string Title { get; set; } = string.Empty;

        public string ShortMessage { get; set; } = string.Empty;

        public string FullMessage { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string CategoryColor { get; set; } = "#2196F3";

        public string RelatedPage { get; set; } = string.Empty;

        public long? RelatedRecordId { get; set; }

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }

        public string DisplayDate =>
            CreatedAt.ToString("yyyy-MM-dd");

        public string DisplayTime =>
            CreatedAt.ToString("hh:mm tt");

    }
}
