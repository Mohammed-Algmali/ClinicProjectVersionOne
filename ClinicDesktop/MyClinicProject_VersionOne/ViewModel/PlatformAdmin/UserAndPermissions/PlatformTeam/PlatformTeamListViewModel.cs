

using System;
using System.Collections.ObjectModel;

namespace MyClinicProject_VersionOne.ViewModel.PlatformAdmin.UserAndPermissions.PlatformTeam
{
    public class PlatformTeamListViewModel
    {
        //القائمه الديناميكيه التي سيقرا منها ال DataGrid
        public ObservableCollection<PlatformUser> PlatformUsersList { get; set; }

        //هذا المشيد 
        public PlatformTeamListViewModel()
        {
            PlatformUsersList = new ObservableCollection<PlatformUser>
            {
                new PlatformUser
                {
                    UserId =1,
                    UserName = "Mohammed Ameen",
                    FullName = "Mohammed Ameed Algmali",
                    UserEmail ="Mohammed@mohammed.com",
                    UserAddress= "Taiz",
                    UserPhone = "774047066",
                    UserStatus = Model.Enums.UserStatus.Active,
                    UserCreatedAt = DateTime.Now,
                    RoleId = 1
                },

                new PlatformUser
                {
                    UserId =1,
                    UserName = "hamza mojeeb",
                    FullName = "Mohammed Ameed Algmali",
                    UserEmail ="Mohammed@mohammed.com",
                    UserAddress= "Taiz",
                    UserPhone = "774047066",
                    UserStatus = Model.Enums.UserStatus.Active,
                    UserCreatedAt = DateTime.Now,
                    RoleId = 1
                },
                new PlatformUser
                {
                    UserId =1,
                    UserName = "mohamed",
                    FullName = "عبده مسعد",
                    UserEmail ="Mohammed@mohammed.com",
                    UserAddress= "Taiz",
                    UserPhone = "774047066",
                    UserStatus = Model.Enums.UserStatus.Active,
                    UserCreatedAt = DateTime.Now,
                    RoleId = 1
                }
            };
        }
    }
}
