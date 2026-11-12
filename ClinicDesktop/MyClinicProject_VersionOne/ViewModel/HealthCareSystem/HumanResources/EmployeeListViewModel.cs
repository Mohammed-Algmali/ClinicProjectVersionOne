

using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.ViewModel.HealthCareSystem.HumanResources
{
    public class EmployeeListViewModel : ViewModelBase
    {// خدمة الاتصال بالخادم الخاصة بالموظفين
        private readonly EmployeeService _employeeService;

        // القائمة المراقبة المرتبطة بالـ DataGrid لتحديث الواجهة تلقائياً
        public ObservableCollection<EmployeeListModel> Employees { get; set; }

        // مؤشر التحميل (لإظهار شريط الانتظار أو تعطيل الشاشة أثناء جلب البيانات)
        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged(nameof(IsLoading));
            }
        }

        // أمر إعادة تحميل البيانات (Refresh Command)
        public RelayCommand RefreshCommand { get; }

        /// <summary>
        /// المشيد (Constructor): تهيئة الخدمة، القائمة، والأوامر
        /// </summary>
        public EmployeeListViewModel()
        {
            _employeeService = new EmployeeService();
            Employees = new ObservableCollection<EmployeeListModel>();

            // ربط أمر التحديث مع تمرير المعامل _ للـ RelayCommand
            RefreshCommand = new RelayCommand(async _ => await LoadEmployeesAsync());

            // تحميل الموظفين تلقائياً بمجرد فتح الشاشة
            _ = LoadEmployeesAsync();
        }

        /// <summary>
        /// دالة غير متزامنة لجلب قائمة موظفي المنشأة من الـ API وتعبئة الجدول
        /// </summary>
        public async Task LoadEmployeesAsync()
        {
            IsLoading = true;

            // استدعاء دالة جلب الموظفين من السيرفس (ترسل التوكن تلقائياً لجلب موظفي المنشأة فقط)
            var (success, data, errorMessage) = await _employeeService.GetEmployeesAsync();

            IsLoading = false;

            if (success && data != null)
            {
                // تفريغ القائمة القديمة وإضافة البيانات القادمة من السيرفر
                Employees.Clear();
                foreach (var item in data)
                {
                    Employees.Add(item);
                }
            }
            else
            {
                // عرض رسالة تنبيه واضحة في حال تعثر الاتصال أو حدوث خطأ
                MessageBox.Show(errorMessage, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

    }
}