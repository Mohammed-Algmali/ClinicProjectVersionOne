
using System.Windows.Controls;

namespace MyClinicProject_VersionOne.View.HealthCareSystem.HumanResources.Job
{
    /// <summary>
    /// Interaction logic for JobListView.xaml
    /// </summary>
    public partial class JobListView : UserControl
    {
        public JobListView()
        {
            InitializeComponent();
            this.DataContext = new JobListViewModel();
        }
    }
}
