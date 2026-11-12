using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace MyClinicProject_VersionOne.ViewModel.HealthCareSystem.Notification
{
    public class NotificationDtoViewModel : UserControl, INotifyPropertyChanged
    {
        public ObservableCollection<NotificationDtoModel> Notifications { get; set; }
        public ObservableCollection<NotificationDtoModel> NotificationsList { get; set; }

        public string UnreadCountText { get; set; }
        private NotificationDtoModel? _selectedNotification;

        public NotificationDtoModel? SelectedNotification
        {
            get => _selectedNotification;

            set
            {
                _selectedNotification = value;
                OnPropertyChanged(nameof(SelectedNotification));
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        public NotificationDtoViewModel()
        {


            Notifications = new ObservableCollection<NotificationDtoModel>();

            DataContext = this;

            LoadSampleNotifications();
            UpdateUnreadCount();
        }

        private void LoadSampleNotifications()
        {
            Notifications.Add(new NotificationDtoModel
            {
                NotificationId = 1,
                Icon = "👤",
                Title = "تسجيل مريض جديد",
                ShortMessage = "تم تسجيل مريض جديد باسم أحمد محمد في النظام.",
                FullMessage = "تم تسجيل مريض جديد عبر منصة المرضى. يرجى مراجعة بيانات المريض واستكمال الإجراءات المطلوبة.",
                Category = "المستخدمون",
                CategoryColor = "#2798DF",
                RelatedPage = "PatientDetailsView",
                RelatedRecordId = 1005,
                CreatedAt = DateTime.Now.AddMinutes(-10),
                IsRead = false
            });

            Notifications.Add(new NotificationDtoModel
            {
                NotificationId = 2,
                Icon = "🏥",
                Title = "تسجيل منشأة جديدة",
                ShortMessage = "تم تسجيل منشأة جديدة باسم عيادة النور الطبية.",
                FullMessage = "تم تسجيل منشأة جديدة في النظام. يرجى مراجعة بيانات المنشأة وتفعيل الحساب.",
                Category = "المنشآت",
                CategoryColor = "#2DBB96",

                RelatedPage = "FacilityDetailsView",
                RelatedRecordId = 204,
                CreatedAt = DateTime.Now.AddHours(-1),
                IsRead = false
            });

            Notifications.Add(new NotificationDtoModel
            {
                NotificationId = 3,
                Icon = "📅",
                Title = "تأكيد موعد",
                ShortMessage = "تم تأكيد موعد جديد للمريضة فاطمة.",
                FullMessage = "تم تأكيد موعد المريضة فاطمة بتاريخ 2026-10-28 الساعة 10:00 صباحًا.",
                Category = "المواعيد",
                CategoryColor = "#F2A51A",

                RelatedPage = "AppointmentDetailsView",
                RelatedRecordId = 3001,
                CreatedAt = DateTime.Now.AddHours(-2),
                IsRead = true
            });

            Notifications.Add(new NotificationDtoModel
            {
                NotificationId = 4,
                Icon = "💳",
                Title = "تنبيه الاشتراك",
                ShortMessage = "انتهت صلاحية الاشتراك الخاص بك، يرجى تجديده.",
                FullMessage = "انتهت صلاحية الاشتراك الخاص بالمنشأة. يرجى فتح صفحة الاشتراكات لتجديد الباقة.",
                Category = "الاشتراكات",
                CategoryColor = "#7654D9",

                RelatedPage = "SubscriptionView",
                RelatedRecordId = 15,
                CreatedAt = DateTime.Now.AddHours(-4),
                IsRead = false
            });
        }

        private void NotificationsList_SelectionChanged(
            object sender,
            SelectionChangedEventArgs e)
        {
            if (SelectedNotification == null)
                return;

            SelectedNotification.IsRead = true;

            UpdateUnreadCount();
        }

        private void UpdateUnreadCount()
        {
            int unreadCount = Notifications.Count(x => !x.IsRead);

            UnreadCountText =
                $"{unreadCount} غير مقروء";
        }

        private void Category_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (sender is not Button button)
                return;

            string category = button.Tag?.ToString() ?? "all";

            if (category == "all")
            {
                NotificationsList = Notifications;
                return;
            }

            string categoryName = category switch
            {
                "users" => "المستخدمون",
                "appointments" => "المواعيد",
                "laboratory" => "المختبر",
                "subscriptions" => "الاشتراكات",
                "system" => "النظام",
                _ => string.Empty
            };

            var filteredNotifications =
                Notifications
                    .Where(x => x.Category == categoryName)
                    .ToList();

            NotificationsList =
                new ObservableCollection<NotificationDtoModel>(
                    filteredNotifications);
        }

        private void OpenRelatedRecord_Click(
            object sender,
            RoutedEventArgs e)
        {
            if (SelectedNotification == null)
            {
                MessageBox.Show(
                    "يرجى اختيار إشعار أولًا.",
                    "تنبيه",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return;
            }

            long? recordId =
                SelectedNotification.RelatedRecordId;

            switch (SelectedNotification.RelatedPage)
            {
                case "PatientDetailsView":
                    MessageBox.Show(
                        $"سيتم فتح تفاصيل المريض رقم {recordId}",
                        "فتح السجل",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                case "AppointmentDetailsView":
                    MessageBox.Show(
                        $"سيتم فتح تفاصيل الموعد رقم {recordId}",
                        "فتح السجل",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                case "SubscriptionView":
                    MessageBox.Show(
                        "سيتم فتح صفحة الاشتراكات.",
                        "فتح السجل",
                        MessageBoxButton.OK,
                        MessageBoxImage.Information);
                    break;

                default:
                    MessageBox.Show(
                        "لا توجد واجهة مرتبطة بهذا الإشعار.",
                        "تنبيه",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    break;
            }
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(
                this,
                new PropertyChangedEventArgs(propertyName));
        }
    }
}
