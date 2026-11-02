
using System.Windows;
using System.Windows.Controls;



namespace MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.PlatformTeam
{
    /// <summary>
    /// Interaction logic for PlatformTeamListView.xaml
    /// </summary>
    public partial class PlatformTeamListView : UserControl
    {
        public PlatformTeamListView()
        {
            InitializeComponent();
            this.DataContext = new PlatformTeamListViewModel();
        }

      

        private void BtnPlatformTeamAddUser_Click(object sender, RoutedEventArgs e)
        {
            CreatePlatformTeamView view = new CreatePlatformTeamView();
            view.ShowDialog();
        }

        private void BtnEditUser_Click(object sender, RoutedEventArgs e)
        {
            EditPlatformTeamView view = new EditPlatformTeamView();
            view.Show();
        }
    }
}
