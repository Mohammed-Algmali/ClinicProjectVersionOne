

using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.ViewModel.PlatformAdmin.UserAndPermissions.RoleAndPermissios
{
    public class CreateRoleViewModel:ViewModelBase
    { 
        // خدمة الأدوار المسؤولة عن التواصل مع FastAPI
        private readonly RoleService _roleService;

        // =========================================================================
        // 1. المتغيرات الخاصة (Backing Fields)
        // =========================================================================
        private string _roleName = string.Empty;
        private string _description = string.Empty;
        private string _levelText ; // القيمة الافتراضية لأعلى صلاحية
        private bool _isLoading = false;
        private string _statusMessage = string.Empty;
        private bool _isSuccess = false;

        // =========================================================================
        // 2. الخصائص العامة المربوطة بالواجهة XAML
        // =========================================================================

        /// <summary>
        /// خاصية اسم الدور الوظيفي (مثل: طبيب، ممرض، موظف استقبال)
        /// </summary>
        public string RoleName
        {
            get => _roleName;
            set
            {
                _roleName = value ?? string.Empty;
                OnPropertyChanged(); // إشعار الواجهة بتحديث القيمة

                // استدعاء دالة التحقق من اسم الدور بسطر واحد
                ValidateProperty(nameof(RoleName), Validations.ValidateTextField(_roleName,"اسم الدور"));

                // إعادة تقييم زر الحفظ
                SaveRoleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// خاصية وصف الدور والمسؤوليات الموكلة له
        /// </summary>
        public string Description
        {
            get => _description;
            set
            {
                _description = value ?? string.Empty;
                OnPropertyChanged();

                // التحقق من الوصف (اختياري)
                ValidateProperty(nameof(Description), Validations.ValidateRoleDescription(_description, isRequired: false));

                SaveRoleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// خاصية مستوى الدور (نستقبلها كنص من الواجهة لتسهيل التحقق ومنع إدخال الحروف)
        /// </summary>
        public string LevelText
        {
            get => _levelText;
            set
            {
                _levelText = value ?? string.Empty;
                OnPropertyChanged();

                // التحقق من أن المستوى رقم صحيح بين 1 و 10
                ValidateProperty(nameof(LevelText), Validations.ValidateRoleLevel(_levelText, min: 1, max: 10));

                SaveRoleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// حالة التحميل: تكون true أثناء اتصال البرنامج بالسيرفر لتعطيل الزر
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                SaveRoleCommand.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// رسالة تنبيه نصية تظهر أسفل الزر (سواء بنجاح الحفظ أو نص الخطأ من السيرفر)
        /// </summary>
        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// متغير لتحديد لون الرسالة (أخضر عند النجاح، أحمر عند الخطأ)
        /// </summary>
        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                _isSuccess = value;
                OnPropertyChanged();
            }
        }

        // =========================================================================
        // 3. أوامر الأزرار (Commands)
        // =========================================================================
        public RelayCommand SaveRoleCommand { get; }
        public RelayCommand ResetFormCommand { get; }

        // =========================================================================
        // 4. دالة البناء (Constructor)
        // =========================================================================
        public CreateRoleViewModel()
        {
            _roleService = new RoleService();

            // تجهيز أمر الحفظ
            SaveRoleCommand = new RelayCommand(
                executeAsync: async (param) => await ExecuteSaveRoleAsync(),
                canExecute: (param) => CanSaveRole()
            );

            // أمر تفريغ الحقول لإدخال دور جديد
            ResetFormCommand = new RelayCommand(
                execute: (param) => ResetForm()
            );
        }

        /// <summary>
        /// شرط تفعيل زر الحفظ:
        /// لا يمكن الضغط إلا إذا كانت الحقول خالية من الأخطاء والاسم والمستوى غير فارغين
        /// </summary>
        private bool CanSaveRole()
        {
            return !HasErrors &&
                   !IsLoading &&
                   !string.IsNullOrWhiteSpace(RoleName)&&
                   !string.IsNullOrWhiteSpace(LevelText);
        }

        /// <summary>
        /// تنفيذ إرسال بيانات الدور إلى FastAPI وحفظه
        /// </summary>
        private async Task ExecuteSaveRoleAsync()
        {
            StatusMessage = string.Empty;
            IsLoading = true;

            // تجهيز كائن الطلب مع تحويل المستوى لرقم صحيح
            var newRole = new CreateRoleModel
            {
                RoleName = this.RoleName.Trim(),
                RoleDescription = string.IsNullOrWhiteSpace(this.Description) ? null : this.Description.Trim(),
              
                RoleLevel = int.TryParse(this.LevelText, out int lvl) ? lvl : 1
            };

            // إرسال الطلب عبر خدمة الأدوار
            var (success, data, error) = await _roleService.CreateRoleAsync(newRole);

            IsLoading = false;

            if (success && data != null)
            {
                IsSuccess = true;
                StatusMessage = $"تم إنشاء الدور ({data.RoleName}) بنجاح في النظام برقم معرّف #{data.RoleId}!";

                MessageBox.Show($"تم حفظ الدور الوظيفي الجديد ({data.RoleName}) بنجاح!",
                                "نجاح العملية",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // تفريغ الحقول بعد الحفظ الناجح
                ResetForm();
            }
            else
            {
                IsSuccess = false;
                StatusMessage = error;
            }
        }

        /// <summary>
        /// دالة تفريغ الحقول لإعادة التعيين
        /// </summary>
        public void ResetForm()
        {
            RoleName = string.Empty;
            Description = string.Empty;
            LevelText = "1";
            ClearErrors(nameof(RoleName));
            ClearErrors(nameof(Description));
            ClearErrors(nameof(LevelText));
        }
    }
}