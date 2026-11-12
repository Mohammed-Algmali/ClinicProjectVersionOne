

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.ViewModel.HealthCareSystem.HumanResources
{
    public class CreateEmployeeViewModel : ViewModelBase
    {
        // خدمة الاتصال بالسيرفر
        private readonly EmployeeService _employeeService;

        // =========================================================================
        // 1. متغيرات الحالة العامة (Status & Loading)
        // =========================================================================
        private bool _isLoading = false;
        private string _statusMessage = string.Empty;
        private bool _isSuccess = false;

        // =========================================================================
        // 2. الحقول الأساسية للمستخدم (UserBase Fields)
        // =========================================================================
        private string _userFullName = string.Empty;
        private string _userName = string.Empty;
        private string _userPhone = string.Empty;
        private string _userEmail = string.Empty;
        private string _userPassword = string.Empty;
        private int _selectedRoleId = 1;
        private string _birthDate = string.Empty;
        private string _userAddress = string.Empty;

        // =========================================================================
        // 3. الحقول المهنية والطبية (Employee Fields)
        // =========================================================================
        private string _medicalLicenseNumber = string.Empty;
        private string _qualification = string.Empty;
        private string _specialization = string.Empty;
        private string _emergencyContactPhone = string.Empty;
        private string _notes = string.Empty;

        // =========================================================================
        // 4. الخصائص العامة المربوطة بالواجهة (Properties)
        // =========================================================================

        public string UserFullName
        {
            get => _userFullName;
            set
            {
                _userFullName = value ?? string.Empty;
                OnPropertyChanged();
              
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string UserName
        {
            get => _userName;
            set
            {
                _userName = value?.Trim().ToLower() ?? string.Empty;
                OnPropertyChanged();
               
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string UserPhone
        {
            get => _userPhone;
            set
            {
                _userPhone = value ?? string.Empty;
                OnPropertyChanged();
               
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string UserEmail
        {
            get => _userEmail;
            set
            {
                _userEmail = value ?? string.Empty;
                OnPropertyChanged();
               
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string UserPassword
        {
            get => _userPassword;
            set
            {
                _userPassword = value ?? string.Empty;
                OnPropertyChanged();
               
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public int SelectedRoleId
        {
            get => _selectedRoleId;
            set
            {
                _selectedRoleId = value;
                OnPropertyChanged();
             
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public ObservableCollection<string> GendersList { get; } = new ObservableCollection<string> { "ذكر", "أنثى" };

        // 2. المتغير الخاص بالقيمة المختارة (افتراضياً: ذكر)
        private string _gender = "ذكر";

        // 3. الخاصية المربوطة بالـ ComboBox
        public string Gender
        {
            get => _gender;
            set
            {
                if (_gender != value)
                {
                    _gender = value;
                    OnPropertyChanged(); // إشعار الواجهة بتحديث الاختيار
                }
            }
        }

        public string BirthDate
        {
            get => _birthDate;
            set
            {
                _birthDate = value ?? string.Empty;
                OnPropertyChanged();
              
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string UserAddress
        {
            get => _userAddress;
            set
            {
                _userAddress = value ?? string.Empty;
                OnPropertyChanged();
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string MedicalLicenseNumber
        {
            get => _medicalLicenseNumber;
            set
            {
                _medicalLicenseNumber = value ?? string.Empty;
                OnPropertyChanged();
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string Qualification
        {
            get => _qualification;
            set
            {
                _qualification = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Specialization
        {
            get => _specialization;
            set
            {
                _specialization = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string EmergencyContactPhone
        {
            get => _emergencyContactPhone;
            set
            {
                _emergencyContactPhone = value ?? string.Empty;
                OnPropertyChanged();
             
            }
        }

        public string Notes
        {
            get => _notes;
            set
            {
                _notes = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                SaveEmployeeCommand?.RaiseCanExecuteChanged();
            }
        }

        public string StatusMessage
        {
            get => _statusMessage;
            set
            {
                _statusMessage = value;
                OnPropertyChanged();
            }
        }

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
        // 5. القوائم المنسدلة والأوامر (Commands & Collections)
        // =========================================================================
       // public ObservableCollection<RoleResponseModel> RolesList { get; set; } = new ObservableCollection<RoleResponseModel>();

        public RelayCommand SaveEmployeeCommand { get; }

        // أمر الإلغاء والخروج من الشاشة
        public RelayCommand CancelCommand { get; }
        public RelayCommand ResetFormCommand { get; }

        // =========================================================================
        // 6. دالة البناء (Constructor)
        // =========================================================================
        public CreateEmployeeViewModel()
        {
            _employeeService = new EmployeeService();

            SaveEmployeeCommand = new RelayCommand(
                executeAsync: async (param) => await ExecuteSaveEmployeeAsync(),
                canExecute: (param) => CanSaveEmployee()
            );

            ResetFormCommand = new RelayCommand(
                execute: (param) => ResetForm()
            );


            // أمر الإلغاء: يقوم بتفريغ الحقول وإغلاق النافذة الحالية
            CancelCommand = new RelayCommand((param) =>
            {
                // 1. تفريغ كل الحقول والمدخلات
                ResetForm();

                // 2. إذا كانت الشاشة نافذة مستقلة (Window)، يقوم بإغلاقها فوراً
                if (param is Window window)
                {
                    window.Close();
                }
            });
        }

        /// <summary>
        /// شرط تفعيل زر الحفظ (نفس أسلوب المنشأة بالضبط)
        /// </summary>
        private bool CanSaveEmployee()
        {
            return !HasErrors &&
                   !IsLoading &&
                   !string.IsNullOrWhiteSpace(UserFullName) &&
                   !string.IsNullOrWhiteSpace(UserName) &&
                   !string.IsNullOrWhiteSpace(UserPhone) &&
                   !string.IsNullOrWhiteSpace(UserPassword);
        }

        /// <summary>
        /// إرسال طلب الحفظ للـ API ومعالجة الاستجابة
        /// </summary>
        private async Task ExecuteSaveEmployeeAsync()
        {
            StatusMessage = string.Empty;
            IsLoading = true;

            // تجهيز كائن الطلب الموحد
            var registrationRequest = new EmployeeRegistrationRequestModel
            {
                // 1. بيانات الحساب الأساسية
                User = new UserBaseModel
                {
                    UserFullName = this.UserFullName.Trim(),
                    UserName = this.UserName.Trim(),
                    UserPhone = this.UserPhone.Trim(),
                    UserEmail = this.UserEmail.Trim(),
                    UserPassword = this.UserPassword,
                    RoleId = this.SelectedRoleId,
                    Gender = this.Gender,
                    BirthDate = this.BirthDate,
                    UserAdress = this.UserAddress.Trim()
                },

                // 2. البيانات المهنية والطبية
                Employee = new CreateEmployeeModel
                {
                    MedicalLicenseNumber = this.MedicalLicenseNumber.Trim(),
                    Qualification = string.IsNullOrWhiteSpace(this.Qualification) ? null : this.Qualification.Trim(),
                    Specialization = string.IsNullOrWhiteSpace(this.Specialization) ? null : this.Specialization.Trim(),
                    EmergencyContactPhone = string.IsNullOrWhiteSpace(this.EmergencyContactPhone) ? null : this.EmergencyContactPhone.Trim(),
                    Notes = string.IsNullOrWhiteSpace(this.Notes) ? null : this.Notes.Trim()
                }
            };

            // إرسال الكائن الجامع للخدمة
            var (success, data, error) = await _employeeService.CreateEmployeeAsync(registrationRequest);

            IsLoading = false;

            if (success)
            {
                IsSuccess = true;
                StatusMessage = $"تم تسجيل الموظف ({this.UserFullName}) بنجاح في النظام!";

                MessageBox.Show($"تم حفظ بيانات الموظف ({this.UserFullName}) بنجاح!",
                                "نجاح العملية",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                ResetForm();
            }
            else
            {
                IsSuccess = false;
                StatusMessage = error;
            }
        }

        /// <summary>
        /// تفريغ الحقول وإعادة تعيين الأخطاء (نفس أسلوب المنشأة)
        
        /// </summary>
        


        public void ResetForm()
        {
            UserFullName = string.Empty;
            UserName = string.Empty;
            UserPhone = string.Empty;
            UserEmail = string.Empty;
            UserPassword = string.Empty;
            SelectedRoleId = 0;
            Gender = "ذكر";
            BirthDate = string.Empty;
            UserAddress = string.Empty;

            MedicalLicenseNumber = string.Empty;
            Qualification = string.Empty;
            Specialization = string.Empty;
            EmergencyContactPhone = string.Empty;
            Notes = string.Empty;

            // مسح أي رسائل أخطاء سابقة
            ClearErrors(nameof(UserFullName));
            ClearErrors(nameof(UserName));
            ClearErrors(nameof(UserPhone));
            ClearErrors(nameof(UserEmail));
            ClearErrors(nameof(UserPassword));
            ClearErrors(nameof(SelectedRoleId));
            ClearErrors(nameof(UserAddress));

            SaveEmployeeCommand?.RaiseCanExecuteChanged();
        }
    }
}