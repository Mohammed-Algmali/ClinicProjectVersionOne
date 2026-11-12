
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;

namespace MyClinicProject_VersionOne.ViewModel.HealthCareSystem.HumanResources.Shift
{
    public class CreateShiftViewModel:ViewModelBase
    {// خدمة الاتصال المسؤولة عن إرسال طلبات الورديات إلى خادم FastAPI
        private readonly ShiftService _shiftService;

        // =========================================================================
        // 1. المتغيرات الخاصة لتخزين القيم في الذاكرة (Backing Fields)
        // =========================================================================
        private bool _isLoading = false;             // حالة الاتصال بالسيرفر
        private string _statusMessage = string.Empty; // نص رسالة النتيجة للمستخدم
        private bool _isSuccess = false;              // هل نجحت العملية (للتحكم بلون الرسالة)

        private string _shiftName = string.Empty;     // اسم الوردية
        private string _startTime = "08:00:00";       // وقت بداية الدوام
        private string _endTime = "14:00:00";         // وقت نهاية الدوام
        private int _gracePeriodMinutes = 15;         // فترة السماح بالتأخير بالدقائق
        private double _totalHours = 6.0;             // إجمالي الساعات المحسوبة
        private string _selectedShiftType = "صباحية"; // نوع الوردية المختار افتراضياً


        // =========================================================================
        // 2. الخصائص العامة المربوطة بعناصر الواجهة (Properties)
        // =========================================================================

        /// <summary>
        /// خاصية اسم الوردية (مثل: الوردية الصباحية، مناوبة طوارئ ليلية)
        /// </summary>
        public string ShiftName
        {
            get => _shiftName;
            set
            {
                _shiftName = value ?? string.Empty;
                OnPropertyChanged(); // إشعار الواجهة بتحديث النص فوراً
                SaveShiftCommand?.RaiseCanExecuteChanged(); // إعادة تقييم زر الحفظ
            }
        }

        /// <summary>
        /// وقت بدء العمل الرسمي: يقوم بتحديث القيمة وإعادة حساب الساعات الإجمالية فوراً
        /// </summary>
        public string StartTime
        {
            get => _startTime;
            set
            {
                _startTime = value ?? string.Empty;
                OnPropertyChanged();
                CalculateTotalHours(); // إعادة الحساب الرياضي للساعات تلقائياً

            }
        }

        /// <summary>
        /// وقت انتهاء العمل الرسمي
        /// </summary>
        public string EndTime
        {
            get => _endTime;
            set
            {
                _endTime = value ?? string.Empty;
                OnPropertyChanged();
                CalculateTotalHours(); // إعادة الحساب الرياضي للساعات تلقائياً

            }
        }

        /// <summary>
        /// الدقائق المسموحة للموظف للتأخير الصباحي قبل بدء تسجيل خصم في الحضور
        /// </summary>
        public int GracePeriodMinutes
        {
            get => _gracePeriodMinutes;
            set
            {
                _gracePeriodMinutes = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// إجمالي عدد ساعات العمل (حقل للقراءة فقط يُحسب آلياً)
        /// </summary>
        public double TotalHours
        {
            get => _totalHours;
            set
            {
                _totalHours = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// نوع الوردية المختار من القائمة المنسدلة
        /// </summary>
        public string SelectedShiftType
        {
            get => _selectedShiftType;
            set
            {
                if (_selectedShiftType != value)
                {
                    _selectedShiftType = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// مؤشر التحميل: يصبح true أثناء اتصال البرنامج بالسيرفر لتعطيل الزر
        /// </summary>
        public bool IsLoading
        {
            get => _isLoading;
            set
            {
                _isLoading = value;
                OnPropertyChanged();
                SaveShiftCommand?.RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// نص رسالة التنبيه التي تظهر أسفل النافذة (سواء بنجاح الحفظ أو الخطأ القادم من السيرفر)
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
        /// مؤشر لتحديد لون الرسالة (أخضر عند النجاح، أحمر عند الفشل)
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
        // 3. القوائم المنسدلة والأوامر (Collections & Commands)
        // =========================================================================

        // القائمة التي تغذي الـ ComboBox بالخيارات الثلاثة الثابتة
        public ObservableCollection<string> ShiftTypesList { get; set; }

        // أمر حفظ الوردية المربوط بزر "حفظ الوردية" في XAML
        public RelayCommand SaveShiftCommand { get; }

        public RelayCommand CancelCommand { get; }
        // أمر تفريغ الحقول وإعادة التهيئة
        public RelayCommand ResetFormCommand { get; }


        // =========================================================================
        // 4. دالة البناء (Constructor)
        // =========================================================================
        public CreateShiftViewModel()
        {
            // 1. تهيئة خدمة الاتصال مع مسارات الورديات
            _shiftService = new ShiftService();

            // 2. تعبئة قائمة أنواع الورديات لتظهر فور فتح الشاشة
            ShiftTypesList = new ObservableCollection<string> { "صباحية", "مسائية", "ليلية" };

            // 3. ربط أمر الحفظ بدالة التنفيذ غير المتزامنة وشرط التفعيل
            SaveShiftCommand = new RelayCommand(
                executeAsync: async (param) => await ExecuteSaveShiftAsync(),
                canExecute: (param) => CanSaveShift()
            );

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

            // 4. ربط أمر الإلغاء بدالة تفريغ الحقول
            ResetFormCommand = new RelayCommand(
                execute: (param) => ResetForm()
            );
        }


        // =========================================================================
        // 5. دوال المعالجة ومنطق الأعمال (Methods)
        // =========================================================================

        /// <summary>
        /// شرط تفعيل زر الحفظ:
        /// لا يسمح بالضغط على الزر إلا إذا كان اسم الوردية مكتوباً والبرنامج ليس في حالة انتظار
        /// </summary>
        private bool CanSaveShift()
        {
            return !IsLoading && !string.IsNullOrWhiteSpace(ShiftName);
        }

        /// <summary>
        /// الدالة الرئيسية لتجميع بيانات الوردية وإرسالها إلى FastAPI
        /// </summary>
        private async Task ExecuteSaveShiftAsync()
        {
            StatusMessage = string.Empty;
            IsLoading = true; // قفل الزر وإظهار حالة التحميل

            // 1. تجميع البيانات داخل كائن الطلب المطابق لمخطط بايثون (ShiftCreate)
            var newShift = new ShiftCreateRequestModel
            {
                ShiftName = this.ShiftName.Trim(),
                StartTime = this.StartTime.Trim(),
                EndTime = this.EndTime.Trim(),
                GracePeriodMinutes = this.GracePeriodMinutes,
                TotalHours = this.TotalHours,
                ShiftType = this.SelectedShiftType,
               
            };

            // 2. إرسال الطلب عبر خدمة الورديات واستلام النتيجة
            var (success, data, error) = await _shiftService.CreateShiftAsync(newShift);

            IsLoading = false; // إعادة تفعيل الزر

            // 3. معالجة النتيجة وعرض الرسائل المناسبة
            if (success && data != null)
            {
                IsSuccess = true;
                StatusMessage = $"تم حفظ الوردية ({data.ShiftName}) بنجاح في النظام!";

                MessageBox.Show($"تم حفظ الوردية الجديدة ({data.ShiftName}) بنجاح!",
                                "نجاح العملية",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);

                // تفريغ الحقول لتهيئة الشاشة لإدخال وردية أخرى
                ResetForm();
            }
            else
            {
                IsSuccess = false;
                StatusMessage = error;
            }
        }



        /// <summary>
        /// دالة الحساب الذكي لإجمالي ساعات العمل:
        /// تطرح وقت البدء من وقت الانتهاء تلقائياً،
        /// مع معالجة ذكية للورديات الليلية التي تمتد بعد منتصف الليل إلى اليوم التالي (+24 ساعة).
        /// </summary>
        private void CalculateTotalHours()
        {
            if (TimeSpan.TryParse(StartTime, out var start) && TimeSpan.TryParse(EndTime, out var end))
            {
                // إذا كان وقت الانتهاء بعد البدء نحسب الفارق، وإلا نضيف 24 ساعة للورديات الليلية
                var duration = end >= start
                    ? (end - start).TotalHours
                    : (end.Add(TimeSpan.FromHours(24)) - start).TotalHours;

                TotalHours = Math.Round(duration, 2);
            }
        }



        /// <summary>
        /// دالة إعادة ضبط الحقول للقيم الافتراضية
        /// </summary>
        public void ResetForm()
        {
            ShiftName = string.Empty;
            StartTime = "08:00:00";
            EndTime = "14:00:00";
            GracePeriodMinutes = 15;
            TotalHours = 6.0;
            SelectedShiftType = "صباحية";
            StatusMessage = string.Empty;
        }
    }
}
    

