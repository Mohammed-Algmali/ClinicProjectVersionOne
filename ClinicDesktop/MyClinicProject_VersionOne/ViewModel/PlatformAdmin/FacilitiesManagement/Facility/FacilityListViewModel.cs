


using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace MyClinicProject_VersionOne.ViewModel.PlatformAdmin.FacilitiesManagement.Facility
{
    public class FacilityListViewModel : ViewModelBase
    {
        private readonly FacilityService _facilityService;

        // القائمة التي سترتبط بالـ DataGrid وتحدّث الواجهة تلقائياً
        public ObservableCollection<FacilityListModel> Facilities { get; set; }

        private string _statusMessage = string.Empty;
        public string StatusMessage
        {
            get => _statusMessage;
            set { _statusMessage = value; OnPropertyChanged(); }
        }

        // مؤشر التحميل (لإظهار ProgressBar أو رسالة انتظار إن أردت)
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

        public ICommand EditFacilityCommand { get; }

        public FacilityListViewModel()
        {
            _facilityService = new FacilityService();
            Facilities = new ObservableCollection<FacilityListModel>();

            // لاحظ إضافة _ هنا
            RefreshCommand = new RelayCommand(async _ => await LoadFacilitiesAsync());

            // ربط الأمر بدالة تستقبل رقم المنشأة
            EditFacilityCommand = new RelayCommand(async param => await ExecuteEditFacilityAsync(param));

            // التحميل التلقائي عند فتح الواجهة
            _ = LoadFacilitiesAsync();
        }

        // دالة جلب البيانات من السيرفس وتعبئة القائمة

        public async Task LoadFacilitiesAsync()
        {
            IsLoading = true;

            // استدعاء الدالة من السيرفس
            var (success, data, errorMessage) = await _facilityService.GetAllFacilitiesAsync();

            IsLoading = false;

            if (success && data != null)
            {
                // تفريغ القائمة القديمة وإضافة البيانات الجديدة
                Facilities.Clear();
                foreach (var item in data)
                {
                    Facilities.Add(item);
                }
            }
            else
            {
                // عرض رسالة الخطأ في حال فشل الاتصال
                MessageBox.Show(errorMessage, "تنبيه", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }



        private async Task ExecuteEditFacilityAsync(object parameter)
        {
            if (parameter == null || !int.TryParse(parameter.ToString(), out int facilityId))
                return;

            IsLoading = true;

            // 1. استقبال المتغيرات الثلاثة (النجاح، البيانات، رسالة الخطأ)
            var (success, data, errorMessage) = await _facilityService.GetFacilityByIdAsync(facilityId);

            IsLoading = false;

            // 2. التحقق من فشل الطلب أو عدم وجود بيانات
            if (!success || data == null)
            {
                MessageBox.Show($"تعذر جلب بيانات المنشأة:\n{errorMessage}",
                                "خطأ",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
                return;
            }

            var editViewModel = new EditFacilityViewModel(data);
            var editWindow = new EditFacilityView
            {
                DataContext = editViewModel,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = System.Windows.Application.Current.MainWindow
            };

            if (editWindow.ShowDialog() == true)
            {
                await LoadFacilitiesAsync();
            }
        }



        // أداة لإلغاء عملية الانتظار السابقة إذا استمر المستخدم في الكتابة بسرعة
        private CancellationTokenSource? _searchCts;

        private string _searchText = string.Empty;

        /// <summary>
        /// الخاصية المربوطة بمربع البحث في الواجهة (TextBox)
        /// يتم استدعاء الـ set تلقائياً مع كل حرف يكتبه أو يمسحه المستخدم
        /// </summary>
        public string SearchText
        {
            get => _searchText;
            set
            {
                // إذا لم يتغير النص فعلياً لا نفعل شيئاً
                if (_searchText == value) return;

                _searchText = value;
                OnPropertyChanged(); // إبلاغ الواجهة بتحديث قيمة النص

                // استدعاء دالة البحث في الخلفية دون تجميد الواجهة
                _ = ExecuteSearchAsync(value);
            }
        }

        /// <summary>
        /// دالة البحث الذكي (Debounce): تنتظر توقف المستخدم عن الكتابة ثم ترسل الطلب للخادم
        /// </summary>
        private async Task ExecuteSearchAsync(string query)
        {
            // 1. إلغاء المؤقت السابق (إذا كتب المستخدم حرفاً جديداً قبل انتهاء 350ms)
            _searchCts?.Cancel();

            // 2. إنشاء مؤقت جديد لهذا الحرف مع رمز إلغاء خاص به
            _searchCts = new CancellationTokenSource();
            var token = _searchCts.Token;

            try
            {
                // 3. انتظار 350 مللي ثانية؛ إذا كتب المستخدم حرفاً آخر خلال هذه المدة
                //    سيتم إيقاف هذه الدالة فوراً والانتقال إلى catch (TaskCanceledException)
                await Task.Delay(350, token);

                // 4. إذا مسح المستخدم مربع البحث وأصبح فارغاً، نعرض جميع المنشآت مجدداً
                if (string.IsNullOrWhiteSpace(query))
                {
                    await LoadFacilitiesAsync();
                    return;
                }

                // 5. تشغيل مؤشر التحميل وإرسال طلب البحث إلى FastAPI
                IsLoading = true;
                var (success, result, error) = await _facilityService.SearchFacilitiesForAdminAsync(
                    search: query,
                    page: 1,
                    pageSize: 20
                );
                IsLoading = false;

                // 6. إذا نجح الطلب، نحدّث جدول الـ DataGrid بالنتائج الجديدة فقط
                if (success && result != null)
                {
                    Facilities.Clear();
                    foreach (var item in result.Data)
                    {
                        Facilities.Add(item);
                    }
                }
                else
                {
                    // في حال فشل الطلب نعرض رسالة الخطأ القادمة من السيرفر
                    StatusMessage = error;
                }
            }
            catch (TaskCanceledException)
            {
                // هذا الاستثناء طبيعي جداً: يحدث عندما نلغي الانتظار السابق لأن المستخدم ما زال يكتب،
                // لذلك نتجاهله بصمت ولا نعتبره خطأً.
            }
        }
    }
}
