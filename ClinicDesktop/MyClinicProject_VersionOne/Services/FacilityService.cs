
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
namespace MyClinicProject_VersionOne.Services
{
    public class FacilityService:BaseService
    {
      

        public async Task<(bool Success, FacilityListModel?Data, string ErrorMessage)> CreateFacilityAsync(FacilityRegistrationRequestModel newFacility) 
        {
            return await SendPostRequestAsync<FacilityListModel, FacilityRegistrationRequestModel>("/facility", newFacility);
        }

    }
}
