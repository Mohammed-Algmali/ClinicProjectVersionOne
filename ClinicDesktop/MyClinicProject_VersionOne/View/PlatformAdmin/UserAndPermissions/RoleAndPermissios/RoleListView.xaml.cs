using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.RoleAndPermissios
{
    /// <summary>
    /// Interaction logic for RoleListView.xaml
    /// </summary>
    public partial class RoleListView : UserControl
    {
        public RoleListView()
        {
            InitializeComponent();
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            CreateRoleView view = new CreateRoleView();
            view.ShowDialog();
        }
    }
}
