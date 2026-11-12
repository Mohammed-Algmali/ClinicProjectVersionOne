using System.Windows.Controls;
using System;
using System.Windows;
using MyClinicProject_VersionOne.Model.Enums;
namespace MyClinicProject_VersionOne.View.Sidebar
{
    /// <summary>
    /// Interaction logic for Sidebar.xaml
    /// </summary>
    public partial class Sidebar : UserControl
    {
        public Sidebar()
        {
            InitializeComponent();

            //PlatformOwner
            //HospitalManager
            
            DataContext = new SidebarViewModel(AccountType.HospitalManager);
        }




        //هذا الكود يجب التاكد منه او 
        
    }
   
}
