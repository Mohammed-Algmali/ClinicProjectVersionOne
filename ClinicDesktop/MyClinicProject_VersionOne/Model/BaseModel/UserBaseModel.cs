

using System.Text.Json.Serialization;

namespace MyClinicProject_VersionOne.Model.BaseModel
{
    public class UserBaseModel
    {
        // تم تعديلها إلى "UserFullName" لتطابق بايثون بالضبط
        [JsonPropertyName("UserFullName")]
        public string UserFullName { get; set; } = string.Empty;

        // تم تعديلها إلى "Username"
       

        // تم تعديلها إلى "UserPhone"
        [JsonPropertyName("UserPhone")]
        public string UserPhone { get; set; } = string.Empty;

        // تم تعديلها إلى "UserEmail"
        [JsonPropertyName("UserEmail")]
        public string UserEmail { get; set; } = string.Empty;

        [JsonPropertyName("Username")]
        public string UserName { get; set; } = string.Empty;

        // مطابقة لـ Password في كلاس ManagerCreate في بايثون
        [JsonPropertyName("Password")]
        public string? UserPassword { get; set; }

        // مطابقة لـ RoleId في بايثون
        [JsonPropertyName("RoleId")]
        public int RoleId { get; set; }


        /// <summary>
        /// //////////Gender
        /// </summary>
        [JsonPropertyName("gender")]
        public string Gender { get; set; } = string.Empty;



        /// <summary>
        /// //////////BirthDate
        /// </summary>
        [JsonPropertyName("birth_date")]
        public string BirthDate { get; set; } = string.Empty;

        /// <summary>
        /// /////////UserAdress
        /// </summary>
        [JsonPropertyName("UserAddress")]
        public string UserAdress { get; set; } = string.Empty;




    }
}
