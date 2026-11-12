

using System.Threading.Tasks;

namespace MyClinicProject_VersionOne.Services
{
    public class ShiftService:BaseService
    {
        /// <summary>
        /// دالة إنشاء وردية جديدة وإرسالها إلى FastAPI بنفس أسلوب إنشاء المنشأة
        /// </summary>
        public async Task<(bool Success, ShiftResponseModel? Data, string ErrorMessage)> CreateShiftAsync(ShiftCreateRequestModel newShift)
        {
            return await SendPostRequestAsync<ShiftResponseModel, ShiftCreateRequestModel>("/shifts/", newShift);
        }
    }
}
