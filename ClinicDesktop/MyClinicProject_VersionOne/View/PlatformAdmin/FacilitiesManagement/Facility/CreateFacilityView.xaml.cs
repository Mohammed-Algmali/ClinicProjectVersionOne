
using System.Windows;


namespace MyClinicProject_VersionOne.View.PlatformAdmin.FacilitiesManagement.Facility
{
    /// <summary>
    /// Interaction logic for CreateFacilityView.xaml
    /// </summary>
    public partial class CreateFacilityView : Window
    {
        public CreateFacilityView()
        {
            InitializeComponent();
            this.DataContext = new CreateFacilityViewModel();
        }

        private void btnCreateFacilityExit_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
