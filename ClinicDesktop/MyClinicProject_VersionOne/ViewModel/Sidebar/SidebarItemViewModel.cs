//استدعاء مكتبه القوائم الذكبه لتمكين تحديث عناصر الواجهه تلقائيا عند الاضافه او الحذف
using System.Collections.ObjectModel;
//استدعاء مكتبه مكونات النظام لتمكين ارسال اشارات و تنبيهات لواجهه المستخدم عند تغير القيم
using System.ComponentModel;
//استدعاء مكتبه خدمات بيئه التشغيل للحصول على اسم الخاصيه الحاليه تلقائيا دون كتابتها يدويا 
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace MyClinicProject_VersionOne.ViewModel.Sidebar
{
    //يعمل ك حلقه وصل بين البيانات و XAML
    // نموذج العرض لعنصر القائمه الجانبيه يدعم القوائم الفرعيه والتحديث التلقائي للواجهه
    public class SidebarItemViewModel : INotifyPropertyChanged
    {
        //متغير داخلي لحقظ حاله فتح او اغلاق القائمه الفرعيه
        private bool _isExpanded;

        //عنوان العنصر المعروض في القائمه مثل اداره المرضى
        public string Title { get; set; }

        //اسم او كود الايقونه الخاصه ب العنصر
        public string Icon { get; set; }

        //اسم الصفحه التي سيتم فتحها عند الضغط على العنصر
        public string PageName { get; set; }

        // الأمر الذي يتم تنفيذه عند الضغط على عنصر القائمة
        public ICommand? Command { get; set; }

        // قائمه تحتوي على العناصر الفرعيه  "مثل قائمه التقارير تحتوي على "تقرير مالي" و تقرير يومي" و
        public ObservableCollection<SidebarItemViewModel> Children { get; set; }
            = new ObservableCollection<SidebarItemViewModel>();

        //خاصيه للتحكم في فتح و اغلاق القائمه الفرعيه في الواجهه وتحديثها تلقائيا  
        public bool IsExpanded
        {
            get => _isExpanded;

            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }

        // الحدث المسؤول عن اعلام واجهه المستخدم بتغير قيم الخصائص
        public event PropertyChangedEventHandler? PropertyChanged;


        //داله مساعده لتنبيه الواجهه تلقائيا باسم الخاصيه التي تغيرت قيمتها
        // داله مسؤوله عن ارسال اشعار لواجهه المستخدم لتحديث الشاشه فورا عند تغير اي قيما
        //[CallerMemberName] "   دون الحاجه الي كتابتها يدويا" IsExpended ميزه ذكيه اجلب اسم الخاصيه التي تغيرت تلقائيا "مثل "
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Invoke التحقق اولا اذا كانتالواجهه مربوطه وتستمع للتغيرات ثم ارسل التنبيه
            PropertyChanged?.Invoke(
                this,//اخبر الواجهه عن الكائن الحالي الذي حدث فيه التغيير
                new PropertyChangedEventArgs(propertyName));//ارسل اسم الخاصيه التي تغيرت لتقوم الشاشه بتحديثها فورا

        }
    }
}
