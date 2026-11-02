using System;

using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.ViewModel.Authentication
{
    /// <summary>
    /// نموذج العرض لشاشة تسجيل الدخول (Login ViewModel):
    /// يربط عناصر واجهة تسجيل الدخول بخدمة المصادقة وقواعد التحقق.
    /// يرث من ViewModelBase للحصول على ميزات فحص الأخطاء والإشعارات التلقائية.
    /// </summary>
    public class LoginViewModel : ViewModelBase
    {
        // خدمة الاتصال بالـ API
        private readonly AuthService _authService;

        // =========================================================================
        // 1. المتغيرات الخاصة (Backing Fields)
        // =========================================================================
        private string _username = string.Empty;
        private string _password = string.Empty;
        private bool _isLoading = false;
        private string _errorMessage = string.Empty;

        // =========================================================================
        // 2. الخصائص العامة المربوطة بالواجهة XAML
        // =========================================================================

        /// <summary>
        /// خاصية اسم المستخدم: تحوله لأحرف صغيرة وتفحص صحته عبر ClinicValidator
        /// </summary>
        public string Username
        {
            get => _username;
            set
            {
                // تحويل الاسم لأحرف صغيرة دائماً وتجريده من الفراغات
                _username = value?.Trim().ToLower() ?? string.Empty;
                OnPropertyChanged(); // إشعار الواجهة بتحديث القيمة

                // استدعاء دالة فحص اسم المستخدم بسطر واحد
                ValidateProperty(nameof(Username), Validations.ValidateUsername(_username));

                // إعادة تقييم زر الدخول (هل يُفعّل أم يتعطل؟)
                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// خاصية كلمة المرور
        /// </summary>
        public string Password
        {
            get => _password;
            set
            {
                _password = value ?? string.Empty;
                OnPropertyChanged();

                // استدعاء دالة فحص كلمة المرور بسطر واحد
                ValidateProperty(nameof(Password), Validations.ValidatePassword(_password));

                LoginCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// خاصية حالة التحميل: تكون true أثناء إرسال الطلب للخادم لإيقاف الزر وإظهار نص جاري التحميل
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                LoginCommand.RaiseCanExecuteChanged(); // قفل الزر أثناء التحميل
            }
        }

        /// <summary>
        /// رسالة الخطأ القادمة من السيرفر (مثلاً: كلمة المرور غير صحيحة، أو السيرفر غير متصل)
        /// </summary>
        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                _errorMessage = value;
                OnPropertyChanged();
            }
        }

        // =========================================================================
        // 3. أمر تسجيل الدخول المربوط بالزر (Login Command)
        // =========================================================================
        public RelayCommand LoginCommand { get; }

        // حدث (Action) سنستخدمه لإغلاق نافذة الدخول وفتح الشاشة الرئيسية فور نجاح العملية
        public Action? OnLoginSuccess { get; set; }

        // =========================================================================
        // 4. دالة البناء (Constructor)
        // =========================================================================
        public LoginViewModel()
        {
            _authService = new AuthService();

            // تجهيز الأمر بربط دالة التنفيذ مع دالة شرط التفعيل
            LoginCommand = new RelayCommand(
                executeAsync: async (param) => await ExecuteLoginAsync(),
                canExecute: (param) => CanLogin()
            );
        }

        /// <summary>
        /// شرط تفعيل زر الدخول:
        /// لا يمكن النقر على الزر إلا إذا كانت الحقول مكتملة وخالية من الأخطاء ولسنا في حالة تحميل
        /// </summary>
        private bool CanLogin()
        {
            return !HasErrors &&
                   !IsLoading &&
                   !string.IsNullOrWhiteSpace(Username) &&
                   !string.IsNullOrWhiteSpace(Password);
        }

        /// <summary>
        /// دالة تنفيذ تسجيل الدخول الفعلية واستدعاء الخادم
        /// </summary>
        private async Task ExecuteLoginAsync()
        {
            // مسح أي رسالة خطأ قديمة وبدء مؤشر التحميل
            ErrorMessage = string.Empty;
            IsLoading = true;

            // إرسال الطلب إلى خادم FastAPI عبر AuthService
            var (success, data, error) = await _authService.LoginAsync(Username, Password);

            // إيقاف مؤشر التحميل بعد استلام الرد
            IsLoading = false;

            if (success && data != null)
            {
                // إظهار رسالة ترحيبية بالاسم والدور الوظيفي
          /*      MessageBox.Show($"تم تسجيل الدخول بنجاح!\nمرحباً بك: {data.FullName}\nالدور الوظيفي: {data.Role}",
                                "نجاح المصادقة",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
                
                */
                    // حفظ بيانات الجلسة
                  
                  
                // إطلاق حدث النجاح للانتقال للنافذة التالية
                OnLoginSuccess?.Invoke();
            }
            else
            {
                // عرض رسالة الخطأ في الواجهة باللون الأحمر
                ErrorMessage = error;
            }
        }
    }

}
