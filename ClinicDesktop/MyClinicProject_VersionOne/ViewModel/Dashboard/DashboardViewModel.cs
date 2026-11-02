
using MyClinicProject_VersionOne.Command;
using MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.PlatformTeam;
using System.Windows.Input;

namespace MyClinicProject_VersionOne.ViewModel.Dashboard
{
    public class DashboardViewModel:ViewModelBase
    {
       private object _currentView;

        public object CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ICommand NavigateToHomeCommand { get; }


        public DashboardViewModel()
        {

            CurrentView = new FacilityListViewModel();
            NavigateToHomeCommand = new RelayCommand(parameter =>
            {
                if( parameter is string pageName)
                {
                    switch (pageName)
                    {
                        case "FacilityListView":
                            CurrentView = new FacilityListViewModel();
                            break;
                        case "PlatformTeamListView":
                            CurrentView = new PlatformTeamListViewModel();
                            break;

                        case "PlanListView":
                            CurrentView = new PlanListViewModel();
                            break;

                        case "RoleListView":
                            CurrentView = new RoleListViewModel();
                            break;
                    }
                    

                }
            });

           
        }
    }
}
