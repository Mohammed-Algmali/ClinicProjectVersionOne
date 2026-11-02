using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyClinicProject_VersionOne.Command
{
    /// <summary>
    /// كلاس تنفيذ الأوامر (Command Pattern Implementation):
    /// الهدف المعماري منه في نمط MVVM:
    /// 1. تطبيق واجهة ICommand القياسية الخاصة بـ WPF.
    /// 2. ربط عناصر الواجهة (مثل الأزرار Button) بالدوال الموجودة داخل الـ ViewModel بدون كود في View.
    /// 3. دعم العمليات غير المتزامنة (Async / Await) الضرورية عند استدعاء خادم FastAPI.
    /// 4. حماية الخادم وقاعدة البيانات من الضغط المكرر على الزر (Double Click Protection).
    /// </summary>
    public class RelayCommand : ICommand
    {
        // =========================================================================
        // 1. المتغيرات الخاصة لتخزين الدوال وحالة التنفيذ
        // =========================================================================

        // مؤشر للدوال العادية المتزامنة (Synchronous) التي لا تحتاج إلى انتظار
        private readonly Action<object?>? _execute;

        // مؤشر للدوال غير المتزامنة (Async Tasks) التي تتصل بالـ API وتستخدم await
        private readonly Func<object?, Task>? _executeAsync;

        // دالة شرطية ترجع (true أو false) لتحديد هل الزر مفعل أم معطل
        private readonly Predicate<object?>? _canExecute;

        // صمام أمان منطقي: يصبح true أثناء اتصال البرنامج بالسيرفر لمنع تكرار النقر
        private bool _isExecuting;


        // =========================================================================
        // 2. دوال البناء (Constructors)
        // =========================================================================

        /// <summary>
        /// Constructor مخصص للدوال العادية (مثل مسح الحقول، إغلاق نافذة، تنقل محلي).
        /// </summary>
        /// <param name="execute">الدالة المراد تنفيذها عند الضغط</param>
        /// <param name="canExecute">الشرط الاختياري لتفعيل الزر</param>
        public RelayCommand(Action<object?> execute, Predicate<object?>? canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Constructor مخصص للعمليات غير المتزامنة (مثل تسجيل الدخول، حفظ مريض عبر FastAPI).
        /// </summary>
        /// <param name="executeAsync">دالة الـ Task غير المتزامنة</param>
        /// <param name="canExecute">الشرط الاختياري لتفعيل الزر</param>
        public RelayCommand(Func<object?, Task> executeAsync, Predicate<object?>? canExecute = null)
        {
            _executeAsync = executeAsync ?? throw new ArgumentNullException(nameof(executeAsync));
            _canExecute = canExecute;
        }


        // =========================================================================
        // 3. التحقق من صلاحية الضغط على الزر (CanExecute)
        // =========================================================================

        /// <summary>
        /// دالة تستدعيها WPF لتعرف هل تجعل الزر مفعلاً (قابل للنقر) أم معطلاً (رمادي).
        /// </summary>
        public bool CanExecute(object? parameter)
        {
            // أولاً: إذا كان الزر مضغوطاً حالياً والطلب قيد المعالجة في السيرفر، نعطل الزر فوراً
            if (_isExecuting) return false;

            // ثانياً: إذا لم يكن هناك شرط محدد، فالزر مفعل دائماً، وإلا نتحقق من الشرط
            return _canExecute == null || _canExecute(parameter);
        }


        // =========================================================================
        // 4. تنفيذ الأمر عند نقر الزر (Execute)
        // =========================================================================

        /// <summary>
        /// الدالة التي تعمل فعلياً عند ضغط المستخدم على الزر في الواجهة.
        /// </summary>
        public async void Execute(object? parameter)
        {
            // إذا كان الأمر من النوع غير المتزامن (Async)
            if (_executeAsync != null)
            {
                try
                {
                    // تفعيل حالة التنفيذ وتعطيل الزر في الواجهة لمنع النقر المتكرر
                    _isExecuting = true;
                    RaiseCanExecuteChanged();

                    // انتظار انتهاء العملية غير المتزامنة مع السيرفر
                    await _executeAsync(parameter);
                }
                finally
                {
                    // كتلة finally تضمن إعادة تفعيل الزر دائماً حتى لو حدث خطأ أو انقطع الاتصال
                    _isExecuting = false;
                    RaiseCanExecuteChanged();
                }
                return;
            }

            // إذا كان أمراً عادياً متزامناً
            _execute?.Invoke(parameter);
        }


        // =========================================================================
        // 5. إشعار الواجهة بتحديث حالة الزر (RaiseCanExecuteChanged)
        // =========================================================================

        /// <summary>
        /// دالة نداء يدوية نستدعيها من الـ ViewModel (مثل: LoginCommand.RaiseCanExecuteChanged()).
        /// وظيفتها: إجبار WPF على إعادة فحص دالة CanExecute فوراً لتفعيل الزر أو تعطيله.
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// الحدث القياسي في WPF لمتابعة حالة الأوامر وربطها بمحرك CommandManager.
        /// </summary>
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}