using System;
using System.Text.RegularExpressions;

namespace MyClinicProject_VersionOne.Helpers
{
    /// <summary>
    /// كلاس مركزي ثابت ( Class) يحتوي على جميع شروط وقواعد التحقق للمنظومة الطبية.
    /// المبدأ المعماري:
    /// كل دالة تأخذ القيمة المدخلة، وتُرجع:
    /// - (null): إذا كانت القيمة صحيحة ومطابقة للشروط.
    /// - (نص الخطأ): إذا كانت القيمة غير صالحة، ليتم عرضه فوراً في الواجهة.
    /// </summary>
    public class Validations

    {
        // =========================================================================
        // 1. تحققات بيانات المرضى والموظفين الأساسية
        // =========================================================================

        /// <summary>
        /// التحقق من الاسم الكامل (للمريض، الطبيب، الموظف):
        /// - ألا يكون فارغاً.
        /// - لا يقل عن 3 أحرف.
        /// - يحتوي على حروف ومسافات فقط (يمنع الأرقام والرموز).
        /// </summary>
      
            // 1. دالة التحقق من الاسم الكامل
        public static string? ValidateFullName(string? name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return "اسم المريض مطلوب ولا يمكن تركه فارغاً.";

            name = name.Trim();

            if (name.Length < 3)
                return "يجب أن يتكون الاسم من 3 أحرف على الأقل.";

            if (!Regex.IsMatch(name, @"^[\p{L}\s]+$"))
                return "الاسم يجب أن يحتوي على أحرف ومسافات فقط.";

                return null; // لا توجد أخطاء
        }

            // 2. دالة التحقق من رقم الهاتف
        public static string? ValidatePhone(string? phone)
        {
            if (string.IsNullOrWhiteSpace(phone))
                return "رقم الهاتف مطلوب.";

            phone = phone.Trim();

            if (!Regex.IsMatch(phone, @"^[0-9]+$"))
                return "يجب إدخال أرقام فقط.";

            if (phone.Length != 9)
                return $"يجب إدخال 9 أرقام بالضبط (أدخلت {phone.Length} أرقام).";

            if (!phone.StartsWith("7"))
                return "يجب أن يبدأ رقم الجوال بالرقم 7.";

                return null; // لا توجد أخطاء
        }

        //داله التحقق من صحه الايميل
        public static string? ValidateEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return "البريد الالكتروني مطلوب";
            }
            email = email.Trim();

            string emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";

            if (!Regex.IsMatch(email, emailPattern))
            {
                return "صيغه البريد الالكتروني غير صحيحه (name@example.com)";
            }

            return null;//الايميل سليم تماما
        }


        /// <summary>
        /// التحقق من اسم المستخدم المخصص لتسجيل الدخول:
        /// - لا يقل عن 4 ولا يزيد عن 20 حرفاً.
        /// - يبدأ بحرف إنجليزي.
        /// - يحتوي على حروف إنجليزية، أرقام، و (_) فقط بدون مسافات.
        /// </summary>
        public static string? ValidateUsername(string? username)
        {
            if (string.IsNullOrWhiteSpace(username))
                return "اسم المستخدم مطلوب لتسجيل الدخول.";

            username = username.Trim();

            if (username.Length < 4)
                return "اسم المستخدم قصير جداً (يجب أن يكون 4 أحرف على الأقل).";

            if (username.Length > 20)
                return "اسم المستخدم طويل جداً (الحد الأقصى 20 حرفاً).";

            // منع المسافات بشكل صريح
            if (username.Contains(" "))
                return "لا يمكن أن يحتوي اسم المستخدم على مسافات.";

            // شرط أن يبدأ بحرف إنجليزي
            if (!char.IsLetter(username[0]))
                return "يجب أن يبدأ اسم المستخدم بحرف إنجليزي.";

            // النمط: يبدأ بحرف، متبوعاً بأحرف إنجليزية أو أرقام أو شرطة سفلية فقط
            string pattern = @"^[a-zA-Z][a-zA-Z0-9_]*$";
            if (!Regex.IsMatch(username, pattern))
                return "يُسمح فقط بالأحرف الإنجليزية، الأرقام، والشرطة السفلية (_).";

            return null; // صالح ومقبول
        }

        /// <summary>
        /// التحقق من عنوان السكن للمريض أو الموظف:
        /// - ألا يكون فارغاً (إذا كان إلزامياً).
        /// - لا يقل عن 4 أحرف لضمان كتابة عنوان حقيقي (مثلاً: ذمار - حي الجامعة).
        /// - لا يتجاوز 150 حرفاً.
        /// </summary>
        public static string? ValidateAddress(string? address, bool isRequired = true)
        {
            if (string.IsNullOrWhiteSpace(address))
                return isRequired ? "عنوان السكن مطلوب ولا يمكن تركه فارغاً." : null;

            address = address.Trim();

            if (address.Length < 4)
                return "العنوان قصير جداً، يرجى كتابة المدينة والحي أو الشارع (4 أحرف على الأقل).";

            if (address.Length > 150)
                return "العنوان طويل جداً (الحد الأقصى 150 حرفاً).";

            // التأكد من خلوه من الرموز البرمجية أو المشبوهة
            if (!Regex.IsMatch(address, @"^[\p{L}0-9\s\-\/،,\.]+$"))
                return "العنوان يحتوي على رموز غير مسموح بها.";

            return null; // صالح ومقبول
        }


        /// <summary>
        /// التحقق من قوة كلمة المرور لحسابات النظام (أطباء، موظفين، مدراء):
        /// - لا تقل عن 6 خانات.
        /// - تحتوي على حروف وأرقام معاً لضمان الأمان.
        /// - خالية من المسافات.
        /// </summary>
        public static string? ValidatePassword(string? password)
        {
            if (string.IsNullOrWhiteSpace(password))
                return "كلمة المرور مطلوبة ولا يمكن تركها فارغة.";

            if (password.Length < 6)
                return "كلمة المرور ضعيفة (يجب ألا تقل عن 6 خانات).";

            if (password.Length > 50)
                return "كلمة المرور طويلة جداً (الحد الأقصى 50 خانة).";

            if (password.Contains(" "))
                return "لا يمكن أن تحتوي كلمة المرور على مسافات.";

            // شرط وجود حرف واحد على الأقل
            if (!Regex.IsMatch(password, @"[a-zA-Z]"))
                return "يجب أن تحتوي كلمة المرور على حرف إنجليزي واحد على الأقل.";

            // شرط وجود رقم واحد على الأقل
            if (!Regex.IsMatch(password, @"[0-9]"))
                return "يجب أن تحتوي كلمة المرور على رقم واحد على الأقل.";

            return null; // كلمة المرور مقبولة وقوية
        }



        // ============================================================
        // 1. التحقق من تاريخ ميلاد المريض (في الماضي فقط)
        // ============================================================
        /// <summary>
        /// - إلزامي ولا يمكن تركه فارغاً.
        /// - لا يمكن أن يكون تاريخاً مستقبلياً أو تاريخ اليوم.
        /// - لا يتجاوز 120 سنة كعمر منطقي.
        /// </summary>
        public static string? ValidateBirthDate(DateTime? date, bool isRequired = true)
        {
            if (!date.HasValue)
                return isRequired ? "تاريخ الميلاد مطلوب، يرجى اختياره من التقويم." : null;

            DateTime selectedDate = date.Value.Date;
            DateTime today = DateTime.Today;

            if (selectedDate >= today)
                return "تاريخ الميلاد غير صالح (يجب أن يكون في الماضي قبل تاريخ اليوم).";

            if (selectedDate < today.AddYears(-120))
                return "تاريخ الميلاد غير منطقي (تجاوز الحد الأقصى للعمر).";

            return null; // صالح
        }

        // ============================================================
        // 2. التحقق من تاريخ الموعد أو الحجز أو الوردية (في المستقبل)
        // ============================================================
        /// <summary>
        /// - إلزامي للمواعيد.
        /// - لا يمكن حجز موعد في يوم قد مضى.
        /// - لا يتجاوز سنة واحدة كحد أقصى للحجز المسبق.
        /// </summary>
        public static string? ValidateAppointmentDate(DateTime? date, bool isRequired = true)
        {
            if (!date.HasValue)
                return isRequired ? "تاريخ الموعد مطلوب." : null;

            DateTime selectedDate = date.Value.Date;
            DateTime today = DateTime.Today;

            if (selectedDate < today)
                return "لا يمكن اختيار تاريخ في الماضي لموعد أو حجز قادم.";

            if (selectedDate > today.AddYears(1))
                return "لا يمكن حجز موعد لأكثر من سنة مقدماً.";

            return null; // صالح
        }



        // =========================================================================
        // تحققات إدارة الأدوار والصلاحيات (Role Validations)
        // =========================================================================

        /// <summary>
        /// 1. التحقق من اسم الدور الوظيفي (مثل: طبيب عيادة، موظف استقبال، مدير):
        /// - لا يمكن أن يكون فارغاً.
        /// - لا يقل عن 3 أحرف ولا يزيد عن 50 حرفاً.
        /// - يحتوي على حروف ومسافات فقط (عربي وإنجليزي).
        /// </summary>
        public static string? ValidateTextField(string? value,string fielName , int minLength = 4)
        {
            if (string.IsNullOrWhiteSpace(value))
                return  $"{fielName}  مطلوب ولا يمكن تركه فارغاً.";

            value = value.Trim();

            if (value.Length < minLength)
                return $"حقل{fielName} يجب الا يقل {minLength} احرف.";

            if (value.Length > 50)
                return $"اسم {fielName} طويل جداً (الحد الأقصى 50 حرفاً).";

            // السماح بالحروف العربية والإنجليزية والمسافات فقط
            if (!Regex.IsMatch(value, @"^[\p{L}\s]+$"))
                return $"اسم {fielName} يجب أن يحتوي على أحرف ومسافات فقط بدون أرقام أو رموز.";

            return null; // صالح
        }

        /// <summary>
        /// 2. التحقق من وصف الدور والمسؤوليات:
        /// - اختياري، ولكن إذا كُتب يجب ألا يقل عن 5 أحرف وألا يتجاوز 250 حرفاً.
        /// </summary>
        public static string? ValidateRoleDescription(string? description, bool isRequired = false)
        {
            if (string.IsNullOrWhiteSpace(description))
                return isRequired ? "وصف مهام الدور مطلوب." : null;

            description = description.Trim();

            if (description.Length < 5)
                return "الوصف قصير جداً، يرجى كتابة وصف توضيحي لمهام الدور (5 أحرف على الأقل).";

            if (description.Length > 250)
                return "الوصف طويل جداً (الحد الأقصى 250 حرفاً).";

            return null; // صالح
        }

        /// <summary>
        /// 3. التحقق من مستوى الدور (Level / Hierarchy):
        /// - يجب أن يكون رقماً صحيحاً.
        /// - محصور بين 1 (أعلى صلاحية كمدير) إلى 10 (أقل صلاحية كاستقبال/متدرب).
        /// </summary>
        public static string? ValidateRoleLevel(string? levelText, int min = 1, int max = 10)
        {
            if (string.IsNullOrWhiteSpace(levelText))
                return "مستوى الدور مطلوب.";

            if (!int.TryParse(levelText.Trim(), out int level))
                return "يجب إدخال رقم صحيح للمستوى بدون كسور أو حروف.";

            if (level < min || level > max)
                return $"يجب أن يكون مستوى الدور رقماً بين {min} و {max}.";

            return null; // صالح
        }
    }

}

