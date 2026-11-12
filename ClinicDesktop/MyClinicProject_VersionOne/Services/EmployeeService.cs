using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyClinicProject_VersionOne.Services
{
    public class EmployeeService:BaseService
    {

        public async Task<(bool Success, EmployeeResponseModel? Data, string ErrorMessage)> CreateEmployeeAsync(EmployeeRegistrationRequestModel newEmployee)
        {
            return await SendPostRequestAsync<EmployeeResponseModel, EmployeeRegistrationRequestModel>("/employees/", newEmployee);
        }

        public async Task<(bool Success, List<EmployeeListModel>? Data, string ErrorMessage)> GetEmployeesAsync()
        {
            // استدعاء دالة SendGetRequestAsync الموروثة من BaseService
            return await SendGetRequestAsync<List<EmployeeListModel>>("/employees/");
        }

    }
}
