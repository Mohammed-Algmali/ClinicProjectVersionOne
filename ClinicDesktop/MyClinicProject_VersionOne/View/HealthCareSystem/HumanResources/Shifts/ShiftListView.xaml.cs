
using System.Windows.Controls;


namespace MyClinicProject_VersionOne.View.HealthCareSystem.HumanResources.Shifts
{
    /// <summary>
    /// Interaction logic for ShiftListView.xaml
    /// </summary>
    public partial class ShiftListView : UserControl
    {
        public ShiftListView()
        {
            InitializeComponent();

            this.DataContext = new ShiftListViewModel();
        }

        private void btnCreateShift_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            CreateShiftView view = new CreateShiftView();
            view.ShowDialog();
        }
    }
}
