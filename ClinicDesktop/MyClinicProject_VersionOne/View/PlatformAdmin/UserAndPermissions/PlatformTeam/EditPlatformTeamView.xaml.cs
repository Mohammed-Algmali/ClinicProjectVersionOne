
using System.Windows;


namespace MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.PlatformTeam
{
    /// <summary>
    /// Interaction logic for EditPlatformTeamView.xaml
    /// </summary>
    public partial class EditPlatformTeamView : Window
    {
        public EditPlatformTeamView()
        {
            InitializeComponent();
            this.DataContext = new EditPlatformTeamViewModel();

        }

        private void btnEditPlatformTeamExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
