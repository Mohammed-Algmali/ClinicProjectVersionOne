

using System.Windows;

namespace MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.RoleAndPermissios
{
    /// <summary>
    /// Interaction logic for CreateRoleView.xaml
    /// </summary>
    public partial class CreateRoleView : Window
    {
        public CreateRoleView()
        {
            InitializeComponent();

            this.DataContext = new CreateRoleViewModel();
        }

        private void txtPatientCreateExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
