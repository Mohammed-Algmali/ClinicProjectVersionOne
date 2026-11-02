
namespace MyClinicProject_VersionOne.Model.Sidebar
{   
    // نموذج يمثل عنصر واحد داخل القائمه Sidebar
    public class SidebarItemModel
    {
        //(عنوان العنصر الذي يظهر للمستخدم (مثل الرئيسيه ,الاعدادات 
        public string Title { get; set; }

        //(الايقونه الخاصه ب العنصر home  ,settings 
        public string Icon { get; set; }

        //اسم الصفحه او الرابط الذي سيفتح عند الضغط على هذا العنصر
        public string PageName { get; set; }
    }
}
