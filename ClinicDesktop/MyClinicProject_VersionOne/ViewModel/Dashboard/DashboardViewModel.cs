
using MyClinicProject_VersionOne.Command;
using MyClinicProject_VersionOne.View.PlatformAdmin.UserAndPermissions.PlatformTeam;
using System.Windows.Input;

namespace MyClinicProject_VersionOne.ViewModel.Dashboard
{
    public class DashboardViewModel:ViewModelBase
    {
        public string CurrentUserName { get; set; }
        public string CurrentUserRole { get; set; }

        // هنا تضعها داخل المشيد:
       
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
            CurrentUserName = UserSession.FullName;

            CurrentUserRole = UserSession.RoleId switch
            {
                1 => "مدير النظام",
                2 => "مدير المنشأة",
                _ => "مستخدم"
            };

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

                        case "EmployeeListView":
                            CurrentView = new EmployeeListViewModel();
                            break;
                        
                        case "AttendanceListView":
                            CurrentView = new AttendanceListViewModel();
                            break;
                        case "AuditLogView":
                            CurrentView = new AuditLogViewModel();
                            break;

                        case "JobListView":
                            CurrentView = new JobListViewModel();
                            break;

                        case "ShiftListView":
                            CurrentView = new ShiftListViewModel();
                            break;
                    }
                }
                    

                
            });

           
        }
    }
}
