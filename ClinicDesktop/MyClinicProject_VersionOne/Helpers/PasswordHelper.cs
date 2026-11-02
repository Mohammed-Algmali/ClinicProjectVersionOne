using System.Windows;
using System.Windows.Controls;

namespace MyClinicProject_VersionOne.Helpers
{
   

    /// <summary>
    /// كلاس مساعِد (Attached Behavior) لحل معضلة ربط PasswordBox بنمط MVVM.
    /// وظيفته:
    /// يعمل كجسر آمن ينقل كلمة المرور بين PasswordBox في الواجهة وبين الـ ViewModel في الاتجاهين (Two-Way Binding).
    /// </summary>
    public static class PasswordHelper
    {
        // =========================================================================
        // 1. الخاصية الملحقة لكلمة المرور (Password Property)
        // =========================================================================

        /// <summary>
        /// خاصية Dependency Property بديلة ننشئها نحن لتقبل الـ Binding مع الـ ViewModel.
        /// </summary>
        public static readonly DependencyProperty PasswordProperty =
            DependencyProperty.RegisterAttached(
                "Password",                                     // اسم الخاصية
                typeof(string),                                  // نوع البيانات (نص)
                typeof(PasswordHelper),                          // الكلاس المالك
                new FrameworkPropertyMetadata(
                    string.Empty,                                // القيمة الافتراضية
                    OnPasswordPropertyChanged));                 // دالة تعمل عندما تتغير القيمة من الـ ViewModel

        // =========================================================================
        // 2. خاصية التفعيل (Attach Property)
        // =========================================================================

        /// <summary>
        /// مفتاح تشغيل يوضع في XAML: helpers:PasswordHelper.Attach="True"
        /// عند تفعيله، يبدأ الكلاس بمراقبة ضغطات المفاتيح داخل صندوق كلمة المرور.
        /// </summary>
        public static readonly DependencyProperty AttachProperty =
            DependencyProperty.RegisterAttached(
                "Attach",
                typeof(bool),
                typeof(PasswordHelper),
                new PropertyMetadata(false, Attach));

        // =========================================================================
        // 3. خاصية صمام الأمان لمنع التكرار اللانهائي (IsUpdating Property)
        // =========================================================================

        /// <summary>
        /// متغير منطقي سري (خاص بالكلاس):
        /// يمنع حدوث "حلقة لا نهائية (Infinite Loop)".
        /// عندما نغير كلمة المرور برمجياً من الـ ViewModel، يمنع هذا المتغير إطلاق حدث الواجهة مرة أخرى.
        /// </summary>
        private static readonly DependencyProperty IsUpdatingProperty =
            DependencyProperty.RegisterAttached(
                "IsUpdating",
                typeof(bool),
                typeof(PasswordHelper));

        // -------------------------------------------------------------------------
        // دوال القراءة والتعيين القياسية المطلوبة لخصائص WPF الملحقة (Getters & Setters)
        // -------------------------------------------------------------------------
        public static void SetAttach(DependencyObject dp, bool value) => dp.SetValue(AttachProperty, value);
        public static bool GetAttach(DependencyObject dp) => (bool)dp.GetValue(AttachProperty);

        public static string GetPassword(DependencyObject dp) => (string)dp.GetValue(PasswordProperty);
        public static void SetPassword(DependencyObject dp, string value) => dp.SetValue(PasswordProperty, value);

        private static bool GetIsUpdating(DependencyObject dp) => (bool)dp.GetValue(IsUpdatingProperty);
        private static void SetIsUpdating(DependencyObject dp, bool value) => dp.SetValue(IsUpdatingProperty, value);


        // =========================================================================
        // 4. الدوال التنفيذية للربط والتحديث
        // =========================================================================

        /// <summary>
        /// تعمل هذه الدالة عندما تتغير قيمة كلمة المرور القادمة من الـ ViewModel (مثلاً: تفريغ الحقل بعد الحفظ).
        /// فتقوم بتحديث الصندوق في الواجهة بالقيمة الجديدة.
        /// </summary>
        private static void OnPasswordPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                // إيقاف الاستماع مؤقتاً لتجنب إطلاق حدث التغيير مرتين
                box.PasswordChanged -= HandlePasswordChanged;

                // إذا لم نكن في وضع التحديث الداخلي، نقوم بنقل القيمة الجديدة إلى الصندوق
                if (!GetIsUpdating(box))
                {
                    box.Password = (string)e.NewValue;
                }

                // إعادة تفعيل الاستماع للأحداث
                box.PasswordChanged += HandlePasswordChanged;
            }
        }

        /// <summary>
        /// تعمل عند تفعيل الخاصية في XAML (Attach="True"):
        /// تقوم بالاشتراك في حدث PasswordChanged الخاص بصندوق كلمة المرور.
        /// </summary>
        private static void Attach(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                if ((bool)e.NewValue)
                {
                    // بدء مراقبة كتابة المستخدم
                    box.PasswordChanged += HandlePasswordChanged;
                }
                else
                {
                    // إيقاف المراقبة عند إغلاق الشاشة لتفريغ الذاكرة
                    box.PasswordChanged -= HandlePasswordChanged;
                }
            }
        }

        /// <summary>
        /// تعمل فور قيام المستخدم بكتابة أو مسح أي حرف داخل صندوق كلمة المرور:
        /// تأخذ النص المشفر من الواجهة وترسله مباشرة للـ ViewModel لتطبيق التحققات وتخزينه.
        /// </summary>
        private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox box)
            {
                // تفعيل صمام الأمان لمنع الحلقة اللانهائية
                SetIsUpdating(box, true);

                // إرسال النص الجديد إلى الخاصية الملحقة المربوطة بالـ ViewModel
                SetPassword(box, box.Password);

                // إغلاق صمام الأمان
                SetIsUpdating(box, false);
            }
        }
    }
}