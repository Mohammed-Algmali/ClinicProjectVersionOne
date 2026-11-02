using MyClinicProject_VersionOne.Model.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.ViewModel.PlatformAdmin.FacilitiesManagement.Facility
{
    //هذا الكلاس المسؤول عن انشاء منشاه
    // ============================================================
    // CreateFacilityViewModel
    //
    // الـ ViewModel الخاص بصفحة إنشاء منشاه جديد.
    //
    // مسؤول عن:
    // 1. الاحتفاظ ببيانات المنشاه الجديد.
    // 2. توفير قائمة الانواع للواجهة.
    // 3. 
    // 4. استقبال أمر الحفظ من زر الحفظ.
    // 5. استقبال أمر الإلغاء من زر الإلغاء.
    //
    // لاحقًا:
    // سيتم ربط SaveCommand مع 
    // لإرسال بيانات المنشاه إلى FastAPI.
    // ============================================================
    public class CreateFacilityViewModel : ViewModelBase
    {
        // خدمة الأدوار المسؤولة عن التواصل مع FastAPI
        private readonly FacilityService _facilityService;

        private bool _isLoading = false;
        private string _statusMessage = string.Empty;
        private bool _isSuccess = false;

        private string _facilityName = string.Empty;
        private string _facilityLicenseNumber = string.Empty;
        private FacilityTypes _selectedFacilityType;
        private DateTime _facilityLicenseIssueDate;
        private DateTime _facilityLicenseExpiryDate;
        private string _facilityCode = String.Empty;
        private string _facilityCountry = String.Empty;
        private string _facilityCity = String.Empty;
        private string _facilityPhone = String.Empty;
        private string _facilityEmail = String.Empty;
        private string _facilityAddress = String.Empty;
        private string _facilityWebsiteUrl = String.Empty;





        // =========================================================================
        // 2. بيانات مدير المنشأة الجديد (Facility Manager Fields) - 
        // =========================================================================
        private string _managerFullName = string.Empty;
        private string _managerUsername = string.Empty;
        private string _managerPhone = string.Empty;
        private string _managerEmail = string.Empty;
        private string _managerTemporaryPassword = string.Empty;
        private int _selectedManagerRoleId = 1; // رقم معرّف دور مدير المنشأة افتراضياً

        public string FacilityName
        {
            get => _facilityName;
            set
            {
                _facilityName = value ?? string.Empty;
                OnPropertyChanged();

                //
                // استدعاء دالة التحقق من اسم  بسطر واحد
                ValidateProperty(nameof(FacilityName), Validations.ValidateTextField(_facilityName,"اسم المنشاه"));

                // إعادة تقييم زر الحفظ
              
            }
        }


        public string FacilityLicenseNumber
        {
            get => _facilityLicenseNumber;
            set
            {
                _facilityLicenseNumber = value ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(FacilityLicenseNumber), Validations.ValidateTextField(_facilityLicenseNumber, "اسم المنشاه"));
            }
        }


        public FacilityTypes SelectedFacilityType
        {
            get => _selectedFacilityType;//جلب القيمه الحاليه وعرضها في الواجهه

            set
            {  //فحص ما اذا كانت الخيار الجديد يتخلف عن الخيار القديم منع تكرار المعالجه
                if (_selectedFacilityType != value)
                {
                    _selectedFacilityType = value;  //تحديث القيمه في الذاكره بالقيمه الجديدة
                    OnPropertyChanged(); //اطلاق جرس الانذار لاعلام الواجهه بالتحديث واعاده رسم الحقل فورا
                }

            }
        }



        public DateTime FacilityLicenseIssueDate
        {
            get => _facilityLicenseIssueDate;

            set
            {
                _facilityLicenseIssueDate = value;
                OnPropertyChanged();
            }
        }

        public DateTime FacilityLicenseExpiryDate
        {
            get => _facilityLicenseExpiryDate;

            set
            {
                _facilityLicenseExpiryDate = value;
                OnPropertyChanged();
            }
        }

        public string FacilityCode
        {
            get => _facilityCode;
            set
            {
                _facilityCode = value ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(FacilityCode), Validations.ValidateTextField(_facilityCode, "اسم المنشاه"));
            }
        }

        public string FacilityCountry
        {
            get => _facilityCountry;
            set
            {
                _facilityCountry = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string FacilityCity
        {
            get => _facilityCity;
            set
            {
                _facilityCity = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string FacilityPhone
        {
            get => _facilityPhone;
            set
            {
                _facilityPhone = value ?? string.Empty;
                OnPropertyChanged();

                ValidateProperty(nameof(FacilityPhone), Validations.ValidatePhone(_facilityPhone));
            }
        }


        public string FacilityEmail
        {
            get => _facilityEmail;
            set
            {
                _facilityEmail = value ?? string.Empty;
                OnPropertyChanged();

                ValidateProperty(nameof(FacilityEmail), Validations.ValidateEmail(_facilityEmail));
            }
        }

        public string FacilityAddress
        {
            get => _facilityAddress;
            set
            {
                _facilityAddress = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string FacilityWebsiteUrl
        {
            get => _facilityWebsiteUrl;
            set
            {
                _facilityWebsiteUrl = value ?? string.Empty;
                OnPropertyChanged();
            }
        }



        public string ManagerFullName
        {
            get => _managerFullName;
            set
            {
                _managerFullName = value ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(ManagerFullName), Validations.ValidateFullName(_managerFullName));
                
            }
        }

        public string ManagerUsername
        {
            get => _managerUsername;
            set
            {
                _managerUsername = value?.Trim().ToLower() ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(ManagerUsername), Validations.ValidateUsername(_managerUsername));
               
            }
        }

        public string ManagerPhone
        {
            get => _managerPhone;
            set
            {
                _managerPhone = value ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(ManagerPhone), Validations.ValidatePhone(_managerPhone));
              
            }
        }

        public string ManagerEmail
        {
            get => _managerEmail;
            set
            {
                _managerEmail = value ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(ManagerEmail), Validations.ValidateEmail(_managerEmail));
              
            }
        }

        public string ManagerTemporaryPassword
        {
            get => _managerTemporaryPassword;
            set
            {
                _managerTemporaryPassword = value ?? string.Empty;
                OnPropertyChanged();
                ValidateProperty(nameof(ManagerTemporaryPassword), Validations.ValidatePassword(_managerTemporaryPassword));
               
            }
        }

        public int SelectedManagerRoleId
        {
            get => _selectedManagerRoleId;
            set
            {
                _selectedManagerRoleId = value;
                OnPropertyChanged();
            }
        }


        /// حالة التحميل: تكون true أثناء اتصال البرنامج بالسيرفر لتعطيل الزر
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                //SaveRoleCommand.RaiseCanExecuteChanged();
            }
        }


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

        public bool IsSuccess
        {
            get => _isSuccess;
            set
            {
                _isSuccess = value;
                OnPropertyChanged();
            }
        }


        // 3. أوامر الأزرار (Commands)
        // =========================================================================
        public RelayCommand SaveFacilityCommand { get; }
















        //القائمه التي ستقرا منها ال ComboBox لتعرضها
        public ObservableCollection<FacilityTypes> FacilityType { get; set; }


        //المتغير الخاص لتخزين القيمه المحدده الحاليه في الذاكرة
       

        //الخاصيه الذكيه المربوطه ب العنصر المختار في ComboBox
       

        // داله البناء
        public CreateFacilityViewModel()
        {
            _facilityService = new FacilityService();

            //جلب جميه القيم المكتوبه داخل FacilityTypes 
            //وتحويلها الي قائمه منسدله مدعومه برمجيا باستخدام داله Cast
            var enumValues = Enum.GetValues(typeof(FacilityTypes)).Cast<FacilityTypes>();

            //وضع القيم وتخزينها داخل الخاصيه FacilityType لكي تظهر فورا للمستخدم عند فتح القائمه
            FacilityType = new ObservableCollection<FacilityTypes>(enumValues);

             SaveFacilityCommand= new RelayCommand(
                executeAsync: async (param) => await ExecuteSaveFacilityAsync(),
                canExecute: (param) => CanSaveFacility()
            );
        }
        private bool CanSaveFacility()
        {
            return !HasErrors &&
                   !IsLoading &&
                   !string.IsNullOrWhiteSpace(FacilityName) &&
                   !string.IsNullOrWhiteSpace(FacilityPhone);
        }



        private async Task ExecuteSaveFacilityAsync()
        {
            StatusMessage = string.Empty;
            IsLoading = true;

            // تجهيز كائن الطلب مع تحويل المستوى لرقم صحيح
            var registrationRequest = new FacilityRegistrationRequestModel
            {
                Facility = new CreateFacilityModel
                {
                    FacilityName = this.FacilityName.Trim(),
                    FacilityCode = this.FacilityCode.Trim(),
                    FacilityType = this.SelectedFacilityType,
                    FacilityLicenseNumber = this.FacilityLicenseNumber,
                    FacilityLicenseIssueDate = this.FacilityLicenseIssueDate,
                    FacilityLicenseExpiryDate = this.FacilityLicenseExpiryDate,
                    FacilityCountry = this.FacilityCountry.Trim(),
                    FacilityCity = this.FacilityCity.Trim(),
                    FacilityAddress = this.FacilityAddress.Trim(),
                    FacilityPhone = this.FacilityPhone.Trim(),
                    FacilityEmail = string.IsNullOrWhiteSpace(this.FacilityEmail) ? null : this.FacilityEmail.Trim(),
                    FacilityWebsiteUrl = string.IsNullOrWhiteSpace(this.FacilityWebsiteUrl) ? null : this.FacilityWebsiteUrl.Trim()
                },
                Manager = new FacilityManagerModel
                {
                    UserFullName = this.ManagerFullName.Trim(),
                    UserName = this.ManagerUsername.Trim(),
                    UserPassword = this.ManagerTemporaryPassword,
                    UserPhone = this.ManagerPhone.Trim(),
                    UserEmail = this.ManagerEmail.Trim(),
                    RoleId = this.SelectedManagerRoleId // رقم الدور المحدد
                }
            };

                // إرسال الطلب عبر خدمة الأدوار
                var (success, data, error) = await _facilityService.CreateFacilityAsync(registrationRequest);

            IsLoading = false;

            if (success && data != null)
            {
                IsSuccess = true;
                StatusMessage = $"تم إنشاء الدور ({data.FacilityName}) بنجاح في النظام برقم معرّف #{data.FacilityName}!";

                MessageBox.Show($"تم حفظ الدور الوظيفي الجديد ({data.FacilityName}) بنجاح!",
                                "نجاح العملية",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // تفريغ الحقول بعد الحفظ الناجح
            }
            else
            {
                IsSuccess = false;
                StatusMessage = error;
            }
        }



    }
}
