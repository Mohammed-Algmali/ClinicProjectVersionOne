/*using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MyClinicProject_VersionOne.ViewModel
    {
        // ============================================================
        // ViewModelBase
        // الكلاس الأساسي لجميع الـ ViewModels في المشروع.
        //
        // الهدف منه:
        // 1. تطبيق INotifyPropertyChanged.
        // 2. إبلاغ واجهة WPF عند تغير قيمة في الـ ViewModel.
        // 3. تجنب تكرار نفس كود PropertyChanged في كل ViewModel.
        //
        // سيتم توريث هذا الكلاس من ViewModels مثل:
        // UserViewModel
        // PatientViewModel
        // DoctorViewModel
        // وغيرها.
        // ============================================================

        public class ViewModelBase : INotifyPropertyChanged
        {
            // الحدث الذي تستخدمه WPF لمتابعة تغير الخصائص.
            public event PropertyChangedEventHandler? PropertyChanged;

            // دالة لإبلاغ الواجهة بأن قيمة إحدى الخصائص قد تغيرت.
            protected void OnPropertyChanged(
                [CallerMemberName] string? propertyName = null)
            {
                PropertyChanged?.Invoke(
                    this,
                    new PropertyChangedEventArgs(propertyName));
            }
        }
    }

*/using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace MyClinicProject_VersionOne.ViewModel
{
    // =====================================================================
    // هذا هو الكلاس الأساسي لجميع الشاشات في النظام.
    // وظيفته:
    // 1. إشعار الواجهة عند تغير أي قيمة (INotifyPropertyChanged).
    // 2. إدارة وتخزين رسائل الأخطاء للواجهات (INotifyDataErrorInfo).
    // 3. توفير دالة سريعة للتحقق (ValidateProperty).
    // =====================================================================
    public class ViewModelBase : INotifyPropertyChanged, INotifyDataErrorInfo
    {
        // -------------------------------------------------------------
        // القسم الأول: إشعارات تحديث الواجهة (PropertyChanged)
        // -------------------------------------------------------------
        public event PropertyChangedEventHandler? PropertyChanged;

        // دالة يتم استدعاؤها في كل Setter لإشعار WPF بتحديث النص على الشاشة
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // -------------------------------------------------------------
        // القسم الثاني: إدارة قائمة الأخطاء (INotifyDataErrorInfo)
        // -------------------------------------------------------------

        // قاموس يحفظ: اسم الحقل (مثلاً Phone) -> قائمة رسائل الأخطاء الخاصة به
        private readonly Dictionary<string, List<string>> _propertyErrors = new();

        // حدث تستمع له WPF لترسم الإطار الأحمر وتظهر نص الخطأ
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        // خاصية تعود بـ true إذا كان يوجد أي خطأ في الشاشة حالياً
        public bool HasErrors => _propertyErrors.Any();

        // دالة تستدعيها WPF لتعرف ما هي أخطاء حقل معين
        public IEnumerable GetErrors(string? propertyName)
        {
            if (string.IsNullOrEmpty(propertyName) || !_propertyErrors.ContainsKey(propertyName))
                return Enumerable.Empty<string>();

            return _propertyErrors[propertyName];
        }

        // دالة لإضافة رسالة خطأ لحقل معين
        protected void AddError(string propertyName, string errorMessage)
        {
            if (!_propertyErrors.ContainsKey(propertyName))
            {
                _propertyErrors[propertyName] = new List<string>();
            }

            if (!_propertyErrors[propertyName].Contains(errorMessage))
            {
                _propertyErrors[propertyName].Add(errorMessage);
                OnErrorsChanged(propertyName);
            }
        }

        // دالة لمسح أخطاء حقل معين عند تصحيح المدخلات
        protected void ClearErrors(string propertyName)
        {
            if (_propertyErrors.Remove(propertyName))
            {
                OnErrorsChanged(propertyName);
            }
        }

        // دالة داخلية لإشعار WPF بتغير حالة الخطأ
        private void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            OnPropertyChanged(nameof(HasErrors)); // إشعار زر الحفظ ليعرف هل يتعطل أم يشتغل
        }

        // -------------------------------------------------------------
        // القسم الثالث: الدالة السحرية التي تستدعيها بسطر واحد
        // -------------------------------------------------------------
        // تأخذ اسم الحقل، ورسالة الخطأ القادمة من ClinicValidator
        // إذا كانت هناك رسالة خطأ تضيفها، وإذا كانت null تمسح الخطأ
        protected bool ValidateProperty(string propertyName, string? errorMessage)
        {
            ClearErrors(propertyName); // مسح الخطأ القديم إن وُجد

            if (!string.IsNullOrEmpty(errorMessage))
            {
                AddError(propertyName, errorMessage); // تسجيل الخطأ الجديد
                return false;
            }
            return true;
        }
    }
}