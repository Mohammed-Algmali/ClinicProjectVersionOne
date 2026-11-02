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
using System.Windows.Shapes;

namespace MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.PlatformTeam
{
    /// <summary>
    /// Interaction logic for CreatePlatformTeamView.xaml
    /// </summary>
    public partial class CreatePlatformTeamView : Window
    {
        public CreatePlatformTeamView()
        {
            InitializeComponent();
        }

        private void btnCreatePlatformTeamExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
