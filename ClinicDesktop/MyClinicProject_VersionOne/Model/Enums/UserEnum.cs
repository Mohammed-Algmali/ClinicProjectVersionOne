using System;



namespace MyClinicProject_VersionOne.Model.Enums
{
    public enum Gender
    {
        male,
        female
    }



    // تعريف الأدوار كـ Enum
    public enum PlatformRole
    {
        SuperAdmin,          // مدير النظام العام

        PlatformAccountant,  // المدير المالي للمنصة

        SupportAdmin,        // مسؤول الدعم الفني

        OperationsManager,   // مسؤول العمليات والاعتماد

        CommunicationsAdmin, // مسؤول التواصل والإعلام

        FacilityAdmin        // مدير المنشأة
    }





    public enum UserStatus
    {
        Active,
        Inactive,

        Suspended,

        Locked

    }

    public enum AccountType
    {
        PlatformOwner,   // مالك المنصة
        HospitalManager, // مدير المستشفى
        Doctor,          // الطبيب
        Receptionist     // موظف الاستقبال
    }


}
