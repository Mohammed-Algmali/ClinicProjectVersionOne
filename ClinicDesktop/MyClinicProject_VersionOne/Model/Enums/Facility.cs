using System;

namespace MyClinicProject_VersionOne.Model.Enums
{
    public enum FacilityStatuses
    {
        Active,     //نشط 
        Inactive,   //غير نشط
        Supended,   //متوقف موقتا
        Blocked     //محظور

    }

    public enum FacilityTypes
    {
        Hospital,   //مستشفى
        Clinic,    //عياده
        Center,    //مركز طبي
        Complex    //مجمع طبي
    }
}
