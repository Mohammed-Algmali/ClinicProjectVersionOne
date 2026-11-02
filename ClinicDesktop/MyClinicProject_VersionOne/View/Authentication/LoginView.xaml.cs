

using System.Windows;

namespace MyClinicProject_VersionOne.View.Authentication
{
    /// <summary>
    /// Interaction logic for LoginView.xaml
    /// </summary>
    public partial class LoginView : Window
    {
        public LoginView()
        {
            InitializeComponent();

            this.DataContext = new LoginViewModel();

            // الاستماع للـ ViewModel لمعرفة متى ينجح تسجيل الدخول
            if (DataContext is LoginViewModel vm)
            {
                vm.OnLoginSuccess = () =>
                {
                    // 1. إنشاء نسخة من لوحة التحكم الرئيسية (MainWindow)
                    DashboardWindow dashboard = new DashboardWindow();

                    // 2. [سطر أمان هام جداً]:
                    // جعل لوحة التحكم هي النافذة الرئيسية للتطبيق حتى لا يغلق الويندوز البرنامج كاملاً
                    Application.Current.MainWindow = dashboard;

                    // 3. إظهار لوحة التحكم للمستخدم
                    dashboard.Show();

                    // 4. إغلاق نافذة تسجيل الدخول الحالية بأمان
                    this.Close();
                };
            }
        }
    }
}
