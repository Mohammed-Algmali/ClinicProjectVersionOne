using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyClinicProject_VersionOne.Helpers
{
    public static class InputHelper
    {
        // ============================================================
        // 1. دالة حقل الاسم الكامل: السماح بالحروف والمسافات فقط
        // ============================================================
        public static readonly DependencyProperty IsTextOnlyProperty =
            DependencyProperty.RegisterAttached("IsTextOnly", typeof(bool), typeof(InputHelper),
                new PropertyMetadata(false, (d, e) =>
                {
                    if (d is TextBox textBox && (bool)e.NewValue)
                    {
                        // اعتراض كل حرف يكتبه المستخدم وفحصه قبل ظهوره على الشاشة
                        textBox.PreviewTextInput += (s, args) =>
                        {
                            // \p{L} تعني أي حرف عربي أو إنجليزي، و \s تعني مسافة
                            args.Handled = !Regex.IsMatch(args.Text, @"^[\p{L}\s]+$");
                        };
                    }
                }));

        public static void SetIsTextOnly(DependencyObject obj, bool value) => obj.SetValue(IsTextOnlyProperty, value);
        public static bool GetIsTextOnly(DependencyObject obj) => (bool)obj.GetValue(IsTextOnlyProperty);


        // ============================================================
        // 2. دالة حقل الهاتف: السماح بـ 9 أرقام فقط ومنع المسافات
        // ============================================================
        public static readonly DependencyProperty IsPhoneOnlyProperty =
            DependencyProperty.RegisterAttached("IsPhoneOnly", typeof(bool), typeof(InputHelper),
                new PropertyMetadata(false, (d, e) =>
                {
                    if (d is TextBox textBox && (bool)e.NewValue)
                    {
                        textBox.MaxLength = 9; // قفل الحقل عند 9 أرقام

                        // السماح بالأرقام من 0 إلى 9 فقط
                        textBox.PreviewTextInput += (s, args) =>
                        {
                            args.Handled = !Regex.IsMatch(args.Text, @"^[0-9]+$");
                        };

                        // منع ضغط مسافة المسطرة تماماً
                        textBox.PreviewKeyDown += (s, args) =>
                        {
                            if (args.Key == Key.Space) args.Handled = true;
                        };
                    }
                }));

        public static void SetIsPhoneOnly(DependencyObject obj, bool value) => obj.SetValue(IsPhoneOnlyProperty, value);
        public static bool GetIsPhoneOnly(DependencyObject obj) => (bool)obj.GetValue(IsPhoneOnlyProperty);




        // خاصية لحقول اسم المستخدم (حروف إنجليزية، أرقام، شرطة سفلية فقط وبدون مسافات)
        public static readonly DependencyProperty IsUsernameOnlyProperty =
            DependencyProperty.RegisterAttached("IsUsernameOnly", typeof(bool), typeof(InputHelper),
                new PropertyMetadata(false, (d, e) =>
                {
                    if (d is TextBox textBox && (bool)e.NewValue)
                    {
                        textBox.MaxLength = 20;

                // اعتراض لوحة المفاتيح: السماح فقط بالحروف الإنجليزية والأرقام والشرطة السفلية
                textBox.PreviewTextInput += (s, args) =>
                        {
                            args.Handled = !Regex.IsMatch(args.Text, @"^[a-zA-Z0-9_]+$");
                        };

                // منع مسافة المسطرة نهائياً
                textBox.PreviewKeyDown += (s, args) =>
                        {
                            if (args.Key == Key.Space) args.Handled = true;
                        };
                    }
                }));

        public static void SetIsUsernameOnly(DependencyObject obj, bool value) => obj.SetValue(IsUsernameOnlyProperty, value);
        public static bool GetIsUsernameOnly(DependencyObject obj) => (bool)obj.GetValue(IsUsernameOnlyProperty);


        // ============================================================
        // خاصية حقل العنوان (حروف، أرقام، مسافات، وفواصل العناوين الشائعة)
        // ============================================================
        public static readonly DependencyProperty IsAddressOnlyProperty =
            DependencyProperty.RegisterAttached("IsAddressOnly", typeof(bool), typeof(InputHelper),
                new PropertyMetadata(false, (d, e) =>
                {
                    if (d is TextBox textBox && (bool)e.NewValue)
                    {
                        textBox.MaxLength = 150; // حد أقصى مناسب لعنوان مفصل

                // السماح بالحروف، الأرقام، المسافات، والرموز: (- / ، , .)
                textBox.PreviewTextInput += (s, args) =>
                        {
                            string pattern = @"^[\p{L}0-9\s\-\/،,\.]+$";
                            args.Handled = !Regex.IsMatch(args.Text, pattern);
                        };
                    }
                }));

        public static void SetIsAddressOnly(DependencyObject obj, bool value) => obj.SetValue(IsAddressOnlyProperty, value);
        public static bool GetIsAddressOnly(DependencyObject obj) => (bool)obj.GetValue(IsAddressOnlyProperty);
    }
}