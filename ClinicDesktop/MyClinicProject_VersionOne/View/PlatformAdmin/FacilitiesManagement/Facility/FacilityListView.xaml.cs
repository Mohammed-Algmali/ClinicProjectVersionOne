
using MyClinicProject_VersionOne.ViewModel.PlatformAdmin.FacilitiesManagement.Facility;
using System.Windows.Controls;



namespace MyClinicProject_VersionOne.View.PlatformAdmin.FacilitiesManagement.Facility
{
    /// <summary>
    /// Interaction logic for FacilityListView.xaml
    /// </summary>
    public partial class FacilityListView : UserControl
    {
        public FacilityListView()
        {
            InitializeComponent();

            this.DataContext = new FacilityListViewModel();
        }

        private void btnCreateFacility_Checked(object sender, System.Windows.RoutedEventArgs e)
        {
            CreateFacilityView view = new CreateFacilityView();
            view.ShowDialog();
        }
    }
}
