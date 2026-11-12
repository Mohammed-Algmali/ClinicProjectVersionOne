using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyClinicProject_VersionOne.ViewModel.PlatformAdmin.FacilitiesManagement.Facility
{
    public class EditFacilityViewModel:ViewModelBase
    {
        private readonly FacilityService _facilityService = new FacilityService();

        // =====================================================================
        // 1. الخصائص (Properties)
        // =====================================================================
        public int FacilityId { get; private set; }

        private string _facilityCode;
        public string FacilityCode
        {
            get => _facilityCode;
            set { _facilityCode = value; OnPropertyChanged(); }
        }

        private String _createdAt;
        public string CreatedAt
        {
            get => _createdAt;
            set { _createdAt = value; OnPropertyChanged(); }
        }

        private string _facilityName;
        public string FacilityName
        {
            get => _facilityName;
            set { _facilityName = value; OnPropertyChanged(); }
        }

        private string _selectedFacilityType;
        public string SelectedFacilityType
        {
            get => _selectedFacilityType;
            set { _selectedFacilityType = value; OnPropertyChanged(); }
        }

        public List<string> FacilityTypesList { get; } = new List<string>
        {
            "مستشفى", "عيادة", "مجمع طبي", "مركز أشعة", "مختبر"
        };

        private string _facilityLicenseNumber;
        public string FacilityLicenseNumber
        {
            get => _facilityLicenseNumber;
            set { _facilityLicenseNumber = value; OnPropertyChanged(); }
        }

        private String _licenseIssueDate;
        public String LicenseIssueDate
        {
            get => _licenseIssueDate;
            set { _licenseIssueDate = value; OnPropertyChanged(); }
        }

        private String _licenseExpiryDate;
        public string LicenseExpiryDate
        {
            get => _licenseExpiryDate;
            set { _licenseExpiryDate = value; OnPropertyChanged(); }
        }

        private string _facilityCountry;
        public string FacilityCountry
        {
            get => _facilityCountry;
            set { _facilityCountry = value; OnPropertyChanged(); }
        }

        private string _facilityCity;
        public string FacilityCity
        {
            get => _facilityCity;
            set { _facilityCity = value; OnPropertyChanged(); }
        }

        private string _facilityAddress;
        public string FacilityAddress
        {
            get => _facilityAddress;
            set { _facilityAddress = value; OnPropertyChanged(); }
        }

        private string _facilityPhone;
        public string FacilityPhone
        {
            get => _facilityPhone;
            set { _facilityPhone = value; OnPropertyChanged(); }
        }

        private string _facilityEmail;
        public string FacilityEmail
        {
            get => _facilityEmail;
            set { _facilityEmail = value; OnPropertyChanged(); }
        }

        private string _facilityWebSite;
        public string FacilityWebSite
        {
            get => _facilityWebSite;
            set { _facilityWebSite = value; OnPropertyChanged(); }
        }

        // خصائص حالة التحميل والرسائل
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set { _isLoading = value; OnPropertyChanged(); }
        }

        private bool _isSuccess;
        public bool IsSuccess
        {
            get => _isSuccess;
            set { _isSuccess = value; OnPropertyChanged(); }
        }

        private string _statusMessage;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // =====================================================================
        // 2. الأوامر (Commands)
        // =====================================================================
        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // =====================================================================
        // 3. الباني (Constructor) — يستقبل البيانات المجلوبة ويعبّئ الحقول
        // =====================================================================
        public EditFacilityViewModel(FacilityResponseModel facility)
        {
            if (facility != null)
            {
                FacilityId = facility.FacilityId;
                FacilityCode = facility.FacilityCode;
                CreatedAt = facility.CreatedAt;

                FacilityName = facility.FacilityName;
                SelectedFacilityType = facility.FacilityType;
                FacilityLicenseNumber = facility.FacilityLicenseNumber;
                //LicenseIssueDate = facility.FacilityLicenseIssueDate;
               // LicenseExpiryDate = facility.FacilityLicenseExpiryDate;

                FacilityCountry = facility.FacilityCountry;
                FacilityCity = facility.FacilityCity;
                FacilityAddress = facility.FacilityAddress;
                FacilityPhone = facility.FacilityPhone;
                FacilityEmail = facility.FacilityEmail;
                //FacilityWebSite = facility.FacilityWebSite;
            }

            SaveCommand = new RelayCommand(async param => await ExecuteUpdateFacilityAsync(param));
            CancelCommand = new RelayCommand(param => ExecuteCancel(param));
        }

        // =====================================================================
        // 4. تنفيذ التحديث (PUT) وإغلاق النافذة
        // =====================================================================
        private async Task ExecuteUpdateFacilityAsync(object windowObj)
        {
            StatusMessage = string.Empty;
            IsLoading = true;

            var updateRequest = new CreateFacilityModel
            {
                FacilityName = this.FacilityName?.Trim(),
                FacilityType = this.SelectedFacilityType,
                FacilityLicenseNumber = this.FacilityLicenseNumber?.Trim(),
                // FacilityLicenseIssueDate = this.LicenseIssueDate,
                // FacilityLicenseExpiryDate = this.LicenseExpiryDate,
                FacilityCountry = this.FacilityCountry?.Trim(),
                FacilityCity = this.FacilityCity?.Trim(),
                FacilityAddress = this.FacilityAddress?.Trim(),
                FacilityPhone = this.FacilityPhone?.Trim(),
                FacilityEmail = string.IsNullOrWhiteSpace(this.FacilityEmail) ? null : this.FacilityEmail.Trim(),
                FacilityWebsiteUrl = string.IsNullOrWhiteSpace(this.FacilityWebSite) ? null : this.FacilityWebSite.Trim()
            };

            var (success, data, error) = await _facilityService.UpdateFacilityAsync(this.FacilityId, updateRequest);

            IsLoading = false;

            if (success && data != null)
            {
                IsSuccess = true;
                StatusMessage = $"تم تحديث بيانات المنشأة ({data.FacilityName}) بنجاح!";

                MessageBox.Show(StatusMessage, "نجاح العملية", MessageBoxButton.OK, MessageBoxImage.Information);

                if (windowObj is Window win)
                {
                    win.DialogResult = true;
                    win.Close();
                }
            }
            else
            {
                IsSuccess = false;
                StatusMessage = error;
                MessageBox.Show($"فشل التحديث:\n{error}", "خطأ", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void ExecuteCancel(object windowObj)
        {
            if (windowObj is Window win)
            {
                win.DialogResult = false;
                win.Close();
            }
        }
    }
}
    
