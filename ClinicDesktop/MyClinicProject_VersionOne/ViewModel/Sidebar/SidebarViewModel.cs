using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MyClinicProject_VersionOne.Model.Enums;
namespace MyClinicProject_VersionOne.ViewModel.Sidebar
{
    //المسؤول عن اداره القائمه الجانبيه ب الكامل في التطبيق  نموج العرض الاساسي
    public class SidebarViewModel:ViewModelBase
    {
        // متغير داخلي لحفظ حاله القائمه الجانبيه هل هي ممتده بالكامل
        private bool _isSidebarExpanded = true;

        //القائمه الذكيه التي تحتوي على جمسع عناصر القائمه الجانبيه الرئيسيه و الفرعية وتظهر في الشاشة
        public ObservableCollection<SidebarItemViewModel> MenuItems { get; }

        // خاصيه للتحكم في تمدد او تضغر القائمه الجانبيه بالكامل وتحديث الواجهه فورا
        public bool IsSidebarExpanded
        {
            get => _isSidebarExpanded;

            set
            {
                if(_isSidebarExpanded != value)
                {
                    _isSidebarExpanded = value;
                    OnPropertyChanged();
                }
            }
        }


        

        // الحدث المسؤول عن اعلام واجهه المستخدم بتغير قيم الخصائص
        public event PropertyChangedEventHandler? PropertyChanged;


        //داله مساعده لتنبيه الواجهه تلقائيا باسم الخاصيه التي تغيرت قيمتها
        // داله مسؤوله عن ارسال اشعار لواجهه المستخدم لتحديث الشاشه فورا عند تغير اي قيما
        //[CallerMemberName] "   دون الحاجه الي كتابتها يدويا" IsExpended ميزه ذكيه اجلب اسم الخاصيه التي تغيرت تلقائيا "مثل "
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            // Invoke التحقق اولا اذا كانتالواجهه مربوطه وتستمع للتغيرات ثم ارسل التنبيه
            PropertyChanged?.Invoke(
                this,//اخبر الواجهه عن الكائن الحالي الذي حدث فيه التغيير
                new PropertyChangedEventArgs(propertyName));//ارسل اسم الخاصيه التي تغيرت لتقوم الشاشه بتحديثها فورا

        }



        

        // المنشئ الذي يستقبل نوع الحساب لتحديد القائمة المناسبة له.
        // يمكن تمرير PlatformOwner أو HospitalManager أو Doctor أو Receptionist.
        public SidebarViewModel(AccountType accountType)
        {
            // إنشاء مجموعة فارغة لتخزين عناصر القائمة الجانبية.
            // تستخدم ObservableCollection حتى تتمكن الواجهة من متابعة تغييرات المجموعة.
            MenuItems = new ObservableCollection<SidebarItemViewModel>();

            // تحميل عناصر القائمة بناءً على نوع الحساب المستلم.
            LoadMenu(accountType);
        }

        // دالة مسؤولة عن تحديد القائمة التي ستظهر للمستخدم.
        // تستقبل نوع الحساب، ثم تستدعي الدالة المناسبة لبناء قائمته.
        private void LoadMenu(AccountType accountType)
        {
            // حذف العناصر الموجودة في المجموعة قبل تحميل قائمة جديدة.
            // هذا يمنع بقاء عناصر القائمة السابقة عند إعادة تحميلها.
            MenuItems.Clear();

            // فحص نوع الحساب لتحديد القائمة المناسبة.
            switch (accountType)
            {
                // إذا كان المستخدم مالك المنصة، يتم تحميل قائمة إدارة المنصة.
                case AccountType.PlatformOwner:
                    BuildPlatformMenu();
                    break;

                // إذا كان المستخدم مدير مستشفى، يتم تحميل قائمة إدارة المستشفى.
                case AccountType.HospitalManager:
                    BuildHospitalMenu();
                    break;

             
            }
        }

        // دالة مسؤولة عن إنشاء قائمة مالك المنصة.
        // تحتوي على الوظائف العامة التي تخص المنصة وجميع المستشفيات التابعة لها.
        private void BuildPlatformMenu()
        {
            // إضافة عنصر الصفحة الرئيسية إلى قائمة مالك المنصة.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "لوحه التحكم",       // النص الذي يظهر للمستخدم.
                PageName = "Dashboard"    // اسم الصفحة التي سيفتحها نظام التنقل.
            });

            // إضافة مجموعة إدارة المنصة، وتحتوي على عناصر فرعية.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "إدارة المنشاّت ",   // عنوان المجموعة الرئيسية.
                Icon = "🏥",              // الأيقونة التي تظهر بجانب العنوان.
                Children =                // العناصر الفرعية التابعة للمجموعة.
                 {
                     //   فتح صفحة إدارة المستشفيات والعيادات مع زر اضافه منشاه جديدة
                     new SidebarItemViewModel
                     {
                         Title = "جميع المنشآت",
                         PageName = "FacilityListView"
                     },

                    

                     // فتح صفحة إدارة طليات المنشاه .
                     new SidebarItemViewModel
                     {
                         Title = " طلبات التسجيل",
                         PageName = ""
                     }
                 }
            });

            // إضافة المستخدمين والصلاحيات .
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "المستخدمين والصلاحيات",
                Icon = "👥",
                Children =
                 {
                     // عنصر فرعي لفتح صفحة إدارة قائمه اداره المنشآت.
                     new SidebarItemViewModel
                     {
                         Title = "قائمه مديرو المنشآت",
                         PageName = ""
                     },

                     // فتح صفحة إدارة فريق المنصه.
                     new SidebarItemViewModel
                     {
                         Title = "فريق اداره المنصة",
                         PageName = "PlatformTeamListView"
                     },

                     // فتح صفحة إدارة الادوار والصلاحيات .
                     new SidebarItemViewModel
                     {
                         Title = "الادور والصلاحيات",
                         PageName = "RoleListView"
                     },

                     // فتح صفحة إدارة سجل من الصلاحيات .
                     new SidebarItemViewModel
                     {
                         Title = "سجل منح الصلاحيات  ",
                         PageName = ""
                     }

                 }
            });

            // إضافة مجموعة إدارة الاشتراكات والماليه.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الاشتراكات والماليه",
                Icon = "👥",
                Children =
                 {
                     // عنصر فرعي لفتح صفحة الباقاتوالاسعار.
                     new SidebarItemViewModel
                     {
                         Title = " الباقات والاسعار",
                         PageName = "PlanListView"
                     },

                     // فتح صفحة إدارة الاشتراكات الحاليه والمنتهيه .
                     new SidebarItemViewModel
                     {
                         Title = "اشتراكات المنشآت",
                         PageName = ""
                     },

                     

                     // فتح صفحة إدارة الاشتراكات الحاليه والمنتهيه .
                     new SidebarItemViewModel
                     {
                         Title = "المدفوعات والسندات",
                         PageName = ""
                     },

                     

                     // فتح صفحة إدارة سجل تغييرات الاشتراك    .
                     new SidebarItemViewModel
                     {
                         Title = "سجل تغييرات الاشتراك",
                         PageName = ""
                     }
                 }
            });

            // إضافة مجموعة إدارة الاشعارات والتعميم .
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الاشعارات والتعميم",
                Icon = "👥",
                Children =
                 {
                     // عنصر فرعي لفتح صفحة تذاكر ارسال الاشعارات.
                     new SidebarItemViewModel
                     {
                         Title = "ارسال اشعار",
                         PageName = ""
                     },

                     // فتح صفحة إدارة قوالب الاشعارات     .
                     new SidebarItemViewModel
                     {
                         Title = "قوالب الاشعارات",
                         PageName = ""
                     },

                     // فتح صفحة إدارة   سجل الاشعارات .
                     new SidebarItemViewModel
                     {
                         Title = "سجل الاشعارات",
                         PageName = ""
                     }
                 }
            });
            // إضافة مجموعة إدارة الدعم الفني.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الدعم الفني",
                Icon = "👥",
                Children =
                 {
                     // عنصر فرعي لفتح صفحة تذاكر الدعم البلاغات نفسها.
                     new SidebarItemViewModel
                     {
                         Title = "تذاكر الدعم",
                         PageName = ""
                     },

                     // فتح صفحة إدارة   التصنيفات و الاولويات .
                     new SidebarItemViewModel
                     {
                         Title = "التصنيفات و الاولويات",
                         PageName = ""
                     },

                     // فتح صفحة إدارة تقارير الدعم    .
                     new SidebarItemViewModel
                     {
                         Title = "تقارير الدعم",
                         PageName = ""
                     }
                 }
            });

            // إضافة مجموعة إدارة التقاريروالاحصائيات .
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "التقارير والاحصائيات",
                Icon = "👥",
                Children =
                 {
                     // عنصر فرعي لفتح صفحة تقارير المنشآت.
                     new SidebarItemViewModel
                     {
                         Title = "تقارير المنشآت",
                         PageName = ""
                     },

                     // فتح صفحة إدارة تقارير الاشتراكات  .
                     new SidebarItemViewModel
                     {
                         Title = "تقارير الاشتراكات",
                         PageName = ""
                     },

                     // فتح صفحة إدارة تقارير الاستخدام والاداء    .
                     new SidebarItemViewModel
                     {
                         Title = "تقارير الاستخدام والاداء",
                         PageName = ""
                     }
                 }
            });
            // إضافة مجموعة إدارة سجل العمليات والتدقيق .
            //يمكن اضافه جميع سجلات التدقيق في اي قائمه وعمل لها فلاتر
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "سجل العمليات والتدقيق",
                Icon = "👥",
                Children =
                 {
                     // سجل كامل للمستخدم والوقت والنوعا.
                     new SidebarItemViewModel
                     {
                         Title = "سجل كامل للمستخدم والوقت والنوع",
                         PageName = ""
                     }
                 }
            });


            // إضافة رابط إعدادات المنصة.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "إعدادات المنصة",
                Icon = "👥",
                Children =
                 {
                     //  إدارة الهويةالعامه.
                     new SidebarItemViewModel
                     {
                         Title = "الهويةالعامه",
                        PageName = ""
                     },
                      new SidebarItemViewModel
                     {
                         Title = "الاعدادات العامه",
                        PageName = ""
                     },

                      new SidebarItemViewModel
                     {
                         Title = "اعدادات الاشعارات",
                        PageName = ""
                     },

                      new SidebarItemViewModel
                     {
                         Title = " الامان",
                        PageName = ""
                     },

                      new SidebarItemViewModel
                     {
                         Title = "اعدادات المنشآت الجديده",
                        PageName = ""
                     }

                 }
            });
        }

        // دالة مسؤولة عن إنشاء قائمة مدير المستشفى.
        // تتضمن وظائف إدارة المرضى والمواعيد والفروع والموظفين والمالية.

        private void BuildHospitalMenu()
        {
            // 1. لوحة التحكم: ملخص أداء المنشأة والمرضى والمواعيد والمخزون.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "لوحة التحكم",
                Icon = "📊",
                PageName = "Dashboard"
            });

            // 2. عن المنشأة والفروع: بيانات المستشفى والفروع والأقسام التنظيمية.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "المنشأة والفروع",
                Icon = "🏥",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "بيانات المنشأة",
                        PageName = "Facility"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "إدارة الفروع",
                        PageName = "Branch"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الأقسام والهيكل التنظيمي",
                        PageName = "Organization"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الاشتراك وحالة المنشأة",
                        PageName = "Subscription"
                    }
                }
            });

            // 3. الموارد البشرية: ملفات الموظفين والدوام والإجازات والرواتب.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الموارد البشرية",
                Icon = "👥",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "ملفات الموظفين",
                        PageName = "Employees"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "التوظيف والوظائف",
                        PageName = "Recruitment"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الحضور والانصراف",
                        PageName = "Attendance"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الإجازات",
                        PageName = "Leaves"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الدوام والمناوبات",
                        PageName = "WorkSchedules"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "التكليفات بين الفروع",
                        PageName = "EmployeeAssignments"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "تقييم الأداء والتدريب",
                        PageName = "Performance"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الرواتب",
                        PageName = "Payroll"
                    }
                }
            });

            // 4. المستخدمون والصلاحيات: إدارة الحسابات والأدوار والوصول.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "المستخدمون والصلاحيات",
                Icon = "🔐",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "إدارة المستخدمين",
                        PageName = "UserController"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الأدوار والصلاحيات",
                        PageName = "Roles"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "ربط المستخدمين بالموظفين",
                        PageName = "UserEmployees"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "سجل الصلاحيات",
                        PageName = "PermissionAudit"
                    }
                }
            });

            // 5. الإدارة الطبية: الكادر الطبي والأقسام والخدمات العلاجية.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الإدارة الطبية",
                Icon = "🩺",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "الكادر الطبي",
                        PageName = "MedicalStaff"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الأقسام الطبية",
                        PageName = "MedicalDepartments"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الطوارئ",
                        PageName = "Emergency"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "التنويم والأسرة",
                        PageName = "Inpatient"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "المختبر",
                        PageName = "Laboratory"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الأشعة",
                        PageName = "Radiology"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الصيدلية",
                        PageName = "Pharmacy"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "العمليات والتخدير",
                        PageName = "Operations"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الوصفات وطلبات الفحوصات",
                        PageName = "MedicalOrders"
                    }
                }
            });

            // 6. المرضى والمواعيد: التسجيل والملفات الطبية وإدارة الزيارات.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "المرضى والمواعيد",
                Icon = "🗓️",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "جميع المرضى",
                        PageName = "AllPatient"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "إضافة مريض جديد",
                        PageName = "AddNewPatient"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الملفات الطبية",
                        PageName = "MedicalRecords"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "تاريخ الزيارات",
                        PageName = "PatientVisits"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "جميع المواعيد",
                        PageName = "Appointments"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "جداول العيادات",
                        PageName = "ClinicSchedules"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الحضور وقوائم الانتظار",
                        PageName = "WaitingList"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "إعادة الجدولة والإلغاء",
                        PageName = "AppointmentChanges"
                    }
                }
            });

            // 7. المشتريات والمخزون: الطلبات والموردون والمستودعات والأدوية.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "المشتريات والمخزون",
                Icon = "📦",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "طلبات الشراء",
                        PageName = "PurchaseRequests"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "اعتماد الطلبات",
                        PageName = "PurchaseApprovals"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الموردون",
                        PageName = "Suppliers"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "أوامر الشراء والاستلام",
                        PageName = "PurchaseOrders"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الأصناف والأدوية",
                        PageName = "InventoryItems"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "المستودعات",
                        PageName = "Warehouses"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الصرف والتحويل بين الفروع",
                        PageName = "StockTransfers"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الجرد وتنبيهات المخزون",
                        PageName = "InventoryAlerts"
                    }
                }
            });

            // 8. الإدارة المالية: الفواتير والتحصيل والمصروفات والحسابات.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الإدارة المالية",
                Icon = "💳",
                Children =
                {
                    new SidebarItemViewModel
                    {
                        Title = "الفواتير والتحصيل",
                        PageName = "Invoices"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "المصروفات",
                        PageName = "Expenses"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الحسابات والميزانيات",
                        PageName = "Accounts"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "المستحقات والموردون",
                        PageName = "Payables"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "التأمين والمطالبات",
                        PageName = "Insurance"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "التقارير المالية",
                        PageName = "FinancialReports"
                    }
                }
            });

            // 9. الأصول والصيانة: الأجهزة الطبية والأصول وطلبات الصيانة.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الأصول والصيانة",
                Icon = "🛠️",
                Children =
                {
                    new SidebarItemViewModel
                    {
                         Title = "سجل الأصول والأجهزة",
                         PageName = "Assets"
                    },

                    new SidebarItemViewModel
                    {
                        Title = "طلبات الصيانة",
                        PageName = "MaintenanceRequests"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "الصيانة الوقائية",
                        PageName = "PreventiveMaintenance"
                    },
                    new SidebarItemViewModel
                    {
                        Title = "توزيع الأصول على الفروع",
                        PageName = "AssetAssignments"
                    }
                }
            });

            // 10. الجودة وسلامة المرضى: مؤشرات الجودة والحوادث ومكافحة العدوى.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الجودة وسلامة المرضى",
                Icon = "🛡️",
                Children =
        {
            new SidebarItemViewModel
            {
                Title = "مؤشرات الجودة",
                PageName = "QualityIndicators"
            },
            new SidebarItemViewModel
            {
                Title = "بلاغات الحوادث",
                PageName = "IncidentReports"
            },
            new SidebarItemViewModel
            {
                Title = "مكافحة العدوى",
                PageName = "InfectionControl"
            },
            new SidebarItemViewModel
            {
                Title = "خطط التحسين",
                PageName = "ImprovementPlans"
            }
        }
            });

            // 11. التقارير والإحصائيات: متابعة الأداء ومقارنة الفروع.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "التقارير والإحصائيات",
                Icon = "📈",
                Children =
        {
            new SidebarItemViewModel
            {
                Title = "تقارير الفروع",
                PageName = "BranchReports"
            },
            new SidebarItemViewModel
            {
                Title = "تقارير المرضى والمواعيد",
                PageName = "PatientReports"
            },
            new SidebarItemViewModel
            {
                Title = "تقارير الموارد البشرية",
                PageName = "HRReports"
            },
            new SidebarItemViewModel
            {
                Title = "تقارير المشتريات والمخزون",
                PageName = "InventoryReports"
            },
            new SidebarItemViewModel
            {
                Title = "مقارنة أداء الفروع",
                PageName = "BranchPerformance"
            }
        }
            });

            // 12. التواصل والإشعارات: التنبيهات والتعميمات وسجل الإشعارات.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "التواصل والإشعارات",
                Icon = "📢",
                Children =
        {
            new SidebarItemViewModel
            {
                Title = "الإشعارات الإدارية",
                PageName = "Notifications"
            },
            new SidebarItemViewModel
            {
                Title = "التعميمات",
                PageName = "Announcements"
            },
            new SidebarItemViewModel
            {
                Title = "قوالب الإشعارات",
                PageName = "NotificationTemplates"
            },
            new SidebarItemViewModel
            {
                Title = "سجل الإشعارات",
                PageName = "NotificationHistory"
            }
        }
            });

            // 13. سجل العمليات: تتبع التعديلات والموافقات والتغييرات الإدارية.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "سجل العمليات",
                Icon = "📜",
                PageName = "AuditLog"
            });

            // 14. الإعدادات والاشتراك: إعدادات المنشأة والوحدات والباقة الحالية.
            MenuItems.Add(new SidebarItemViewModel
            {
                Title = "الإعدادات والاشتراك",
                Icon = "⚙️",
                Children =
        {
            new SidebarItemViewModel
            {
                Title = "إعدادات المنشأة",
                PageName = "FacilitySettings"
            },
            new SidebarItemViewModel
            {
                Title = "تفضيلات الإشعارات",
                PageName = "NotificationSettings"
            },
            new SidebarItemViewModel
            {
                Title = "الوحدات المفعّلة",
                PageName = "EnabledModules"
            },
            new SidebarItemViewModel
            {
                Title = "الباقة وتجديد الاشتراك",
                PageName = "Subscription"
            },
            new SidebarItemViewModel
            {
                Title = "الإعدادات الافتراضية",
                PageName = "GeneralSettings"
            }
        }
            });
        }
        // دالة مسؤولة عن إنشاء القائمة الخاصة بالطبيب.
      


    }
}
