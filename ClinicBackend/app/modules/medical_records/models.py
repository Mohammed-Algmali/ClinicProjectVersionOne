# medical_records/models.py
from datetime import datetime,date
from typing import Optional, List
from sqlalchemy import (
    BigInteger,
    Integer,
    String,
    Text,
    DateTime,
    ForeignKey,
    func,
    Date,
    Boolean,
    Float
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base



# ==============================================================================
# 2. جدول الملف الطبي الدائم (tbMedicalFiles)
# ==============================================================================
class MedicalFile(Base):
    """
    جدول الإضبارة والملف الطبي الموحد للمريض (EMR Master Record)
    """
    __tablename__ = "tbMedicalFiles"

    medical_file_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للملف الطبي
    user_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, unique=True, index=True)
    # معرف المريض المالك لهذا الملف الطبي
    record_num: Mapped[str] = mapped_column(String(50), unique=True, nullable=False, index=True)
    # رقم الملف الطبي الموحد للمريض في المستشفى (مثل: MRN-2026-001)
    status: Mapped[str] = mapped_column(String(50), default="Active")
    # حالة الملف الطبي (Active نشط / Archived مؤرشف)
    smoking_status: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # حالة التدخين (مدخن، غير مدخن، مدخن سابق)

    groub_blood: Mapped[Optional[str]] = mapped_column(String(10), nullable=True)
    # فصيلة الدم وعامل الريزوس (مثل: O+, A-, B+, AB+)

    notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات طبية عامة وتنبيهات هامة تخص المريض
    emergency_content_phone: Mapped[Optional[int]] = mapped_column(BigInteger, nullable=True)
    # رقم هاتف جهة الاتصال في حالات الطوارئ (الأب، الأم، أو القريب)
  

    # 1. سجل العمليات الجراحية للمريض
    surgeries_history: Mapped[list["PatientSurgeriesHistory"]] = relationship(
        "PatientSurgeriesHistory", 
        back_populates="medical_file",
        cascade="all, delete-orphan"
    )

    # 2. سجل الأمراض المزمنة
    chronic_diseases: Mapped[list["PatientChronicDisease"]] = relationship(
        "PatientChronicDisease", 
        back_populates="medical_file",
        cascade="all, delete-orphan"
    )
    
    # 3. سجل الحساسيات
    allergies: Mapped[list["PatientAllergy"]] = relationship(
        "PatientAllergy", 
        back_populates="medical_file",
        cascade="all, delete-orphan"
    )

    # 4. سجل الزيارات الطبية للمريض
    visits: Mapped[list["Visit"]] = relationship(
        "Visit", 
        back_populates="medical_file",
        cascade="all, delete-orphan"
    )




#///////////////////////////////////////
# 1. كلاس كتالوج مسببات الحساسية (Allergy Catalog)
# المرجع الثابت لجميع المواد المسببة للحساسية في النظام لمنع تكرار النصوص والأخطاء الإملائية
# ==============================================================================
class AllergyCatalog(Base):
    # تحديد اسم الجدول المقابل في قاعدة بيانات PostgreSQL
    __tablename__ = "allergies_catalog"

    # المعرف الأساسي للكتالوج (Primary Key) مع ترقيم تلقائي بنظام IDENTITY
    allergy_catalog_id: Mapped[int] = mapped_column(
        Integer,autoincrement=True, primary_key=True
    )

    # الكود المعياري للحساسية (مثل: ALG-DRUG-01) - فريد وإجباري ومفهرس لسرعة البحث
    allergy_code: Mapped[str] = mapped_column(
        String(30), unique=True, nullable=False, index=True
    )

    # اسم المادة المسببة (مثل: بنسلين، فول سوداني) - مفهرس لتسريع الـ Auto-complete في الـ WPF
    allergen_name: Mapped[str] = mapped_column(String(200), nullable=False, index=True)

    # نوع وتصنيف الحساسية (أدوية: Drug، أطعمة: Food، بيئية: Environmental)
    category: Mapped[str] = mapped_column(String(50), nullable=False)

    # وصف إضافي أو تفاصيل عن طبيعة المادة المسببة (يقبل NULL)
    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # حالة التفعيل: True متاح للاختيار، False معطل ومؤرشف دون كسر السجلات التاريخية للمرضى
    is_active: Mapped[bool] = mapped_column(Boolean, default=True, nullable=False)

    # علاقة عكسية لسجلات المرضى الذين يعانون من هذه الحساسية
    # علاقة عكسية لسجلات المرضى الذين يعانون من هذه الحساسية
    patient_allergies: Mapped[list["PatientAllergy"]] = relationship(
        "PatientAllergy", 
        back_populates="allergy"
    )


# ==============================================================================
# 2. كلاس حساسيات المريض (Patient Allergies)
# يربط بين الملف الطبي للمريض والمادة المسجلة في الكتالوج مع حفظ التفاصيل السريرية
# ==============================================================================
class PatientAllergy(Base):
    # تحديد اسم الجدول الفعلي في قاعدة البيانات
    __tablename__ = "patient_allergies"

    # المعرف الفريد لسجل الحساسية للمريض (Primary Key)
    id: Mapped[int] = mapped_column(Integer, autoincrement=True, primary_key=True)

    # المفتاح الأجنبي لربط السجل بالملف الطبي للمريض (CASCADE: يُحذف تلقائياً عند حذف ملف المريض)
    medical_file_id: Mapped[int] = mapped_column(BigInteger,ForeignKey("tbMedicalFiles.medical_file_id", ondelete="CASCADE"),nullable=False,index=True,)

    # المفتاح الأجنبي للربط بالكتالوج (RESTRICT: يمنع حذف المادة من الكتالوج إذا كانت مرتبطة بأي مريض)
    allergy_catalog_id: Mapped[int] = mapped_column(Integer,ForeignKey("allergies_catalog.allergy_catalog_id", ondelete="RESTRICT"),nullable=False,index=True,)

    # درجة خطورة الحساسية (خفيفة / متوسطة / شديدة) لتحديد مستوى التنبيه في الواجهة
    severity: Mapped[str] = mapped_column(String(50), nullable=False)

    # رد الفعل التحسسي والأعراض السريرية الظاهرة (مثل: طفح جلدي، ضيق تنفس، تورم)
    reaction: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # تاريخ أول ظهور أو تشخيص للحساسية عند المريض
    diagnosed_date: Mapped[Optional[date]] = mapped_column(Date, nullable=True)

    # ملاحظات طبية وتدابير وقائية أو علاجية إضافية (مثل: يحمل قلم إبينفرين)
    notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # التاريخ والوقت التلقائي لإدخال هذا السجل في النظام
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True),server_default=func.now(),nullable=False,)

    # --------------------------------------------------------------------------
    # علاقات الـ ORM (للتعامل مع الجداول ككائنات وتسهيل استخراج البيانات):
    # --------------------------------------------------------------------------
    
    # ربط السجل بكائن الملف الطبي الرئيسي (يتكامل مع back_populates داخل MedicalFile)
    medical_file: Mapped["MedicalFile"] = relationship(
        "MedicalFile", 
        back_populates="allergies"
    )

    # 2. ربط السجل بالكتالوج
    allergy: Mapped["AllergyCatalog"] = relationship(
        "AllergyCatalog", 
        back_populates="patient_allergies",
        lazy="joined"
    )





# كلاس جدول كتالوج العمليات الجراحية (المرجع الثابت لجميع العمليات في النظام)
class SurgeryCatalog(Base):
    # تحديد الاسم الفعلي للجدول داخل قاعدة بيانات PostgreSQL
    __tablename__ = "surgeries_catalog"

    # معرف العملية (المفتاح الأساسي Primary Key):
    # - Identity(always=True): يولد الترقيم التلقائي عبر ميزة IDENTITY القياسية في PostgreSQL بدلاً من SERIAL
    # - primary_key=True: يحدد الحقل كمعرف فريد لا يتكرر لربطه بالجداول الأخرى
    surgery_catalog_id: Mapped[int] = mapped_column(Integer,autoincrement=True, primary_key=True)

    # كود العملية الطبي (مثل: SURG-01 أو كود ICD-9):
    # - String(20): نصي محدد بحد أقصى 20 حرفاً
    # - unique=True: يمنع تكرار الكود نهائياً
    # - nullable=False: حقل إجباري لا يمكن تركه فارغاً
    # - index=True: ينشئ فهرساً لتسريع عمليات البحث بالكود
    surgery_code: Mapped[str] = mapped_column(String(20), unique=True, nullable=False, index=True)

    # اسم العملية الجراحية (بالعربية أو الإنجليزية):
    # - String(250): نص بحد أقصى 250 حرفاً
    # - nullable=False: إجباري
    # - index=True: ينشئ فهرساً لجعل البحث الفوري (AutoComplete) في واجهة المستخدم فائق السرعة
    surgery_name: Mapped[str] = mapped_column(String(250), nullable=False, index=True)

    # التخصص أو القسم الطبي (مثل: جراحة عامة، عيون، عظام):
    # - Optional[str]: حقل اختياري يقبل أن يكون فارغاً (NULL) في قاعدة البيانات
    # - nullable=True: مسموح تركه بدون قيمة
    department: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)

    # وصف إضافي أو تفاصيل عن طبيعة العملية ومستلزماتها:
    # - Text: يقبل نصوصاً طويلة ومفتوحة بدون حد للأحرف (البديل القياسي لـ NVARCHAR(MAX))
    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # حالة العملية (هل هي مفعلة ومتاحة للاختيار أم معطلة):
    # - Boolean: يخزن قيمة صحيحة/خاطئة (True/False)
    # - default=True: مفعلة افتراضياً عند الإضافة
    # - nullable=False: لا يقبل قيمة فارغة
    # - الفائدة: إذا تم إيقاف عملية معينة، نغيرها إلى False دون حذف السجل لمنع كسر العلاقات في السجلات القديمة
    is_active: Mapped[bool] = mapped_column(Boolean, default=True, nullable=False)



class PatientSurgeriesHistory(Base):

    __tablename__ = "patient_surgeries_history"

    # معرف السجل الرئيسي (IDENTITY)
    id: Mapped[int] = mapped_column(
        Integer,autoincrement=True, primary_key=True
    )

    # الربط بالملف الطبي (tbMedicalFiles) - BigInteger
    medical_file_id: Mapped[int] = mapped_column(
        BigInteger,
        ForeignKey("tbMedicalFiles.medical_file_id", ondelete="CASCADE"),
        nullable=False,
        index=True,
    )

    # الربط بكتالوج العمليات (surgeries_catalog)
    # لاحظ أن اسم العمود في الجدول هو surgerycatalogid
    surgery_catalog_id: Mapped[int] = mapped_column(
        Integer,
        ForeignKey("surgeries_catalog.surgery_catalog_id", ondelete="RESTRICT"),
        nullable=False,
        index=True,
    )

    # تاريخ إجراء العملية الجراحية
    surgery_date: Mapped[date] = mapped_column(Date, nullable=False)

    # بيانات اختيارية عن العملية
    hospital_name: Mapped[Optional[str]] = mapped_column(
        String(200), nullable=True
    )
    surgeon_name: Mapped[Optional[str]] = mapped_column(
        String(200), nullable=True
    )
    indication: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    complications: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # وقت إنشاء السجل في قاعدة البيانات
    created_at: Mapped[datetime] = mapped_column(
        DateTime(timezone=True),
        server_default=func.now(),
        nullable=False,
    )

    # علاقات SQLAlchemy (تسمح بالوصول المباشر للبيانات المرتبطة ككائنات)
    # 1. العلاقة مع الملف الطبي (مرتبطة بالطرفين عبر back_populates)
    medical_file: Mapped["MedicalFile"] = relationship(back_populates="surgeries_history")

    # 2. العلاقة مع الكتالوج (مفعلة مع lazy='joined' لجلب اسم وتفاصيل العملية تلقائياً)
    surgery: Mapped["SurgeryCatalog"] = relationship(lazy="joined")


# ==============================================================================
# 1. كلاس كتالوج التشخيصات الطبية (Diagnoses Catalog)
# المرجع الثابت للأمراض والأكواد القياسية (ICD-10) لتسهيل الـ Auto-Complete
# ==============================================================================
class DiagnosisCatalog(Base):
    __tablename__ = "diagnoses_catalog"

    # المعرف الأساسي للكتالوج (Primary Key) مع ترقيم تلقائي
    diagnosis_catalog_id: Mapped[int] = mapped_column(Integer, autoincrement=True, primary_key=True)
    # كود المرض القياسي (مثل: E11.9) - فريد وإجباري ومفهرس
    diagnosis_code: Mapped[str] = mapped_column(String(30), unique=True, nullable=False, index=True)

    # الاسم الطبي الكامل للمرض - مفهرس للبحث السريع في واجهة الـ WPF
    diagnosis_name: Mapped[str] = mapped_column(String(250), nullable=False, index=True)

    # التخصص أو القسم الطبي (باطنية، غدد صماء، عظام...)
    category: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)

    # تفاصيل أو معايير سريرية للتشخيص
    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # حالة التفعيل: لإيقاف اختيار مرض معين مستقبلاً دون المساس بالسجلات القديمة
    is_active: Mapped[bool] = mapped_column(Boolean, default=True, nullable=False)

    # علاقة عكسية مع سجلات تشخيصات المرضى
    patient_diagnoses: Mapped[List["PatientDiagnosis"]] = relationship("PatientDiagnosis", back_populates="diagnosis")


# ==============================================================================
# 2. كلاس سجل تشخيصات المريض (Patient Diagnoses)
# يربط بين الملف الطبي للمريض والمرض المسجل في الكتالوج
# ==============================================================================
class PatientDiagnosis(Base):
    __tablename__ = "patient_diagnoses"

    # المعرف الفريد لسجل التشخيص للمريض
    id: Mapped[int] = mapped_column(Integer, autoincrement=True, primary_key=True)

    # الربط بالزيارة بدلاً من الملف الطبي
    visit_id: Mapped[int] = mapped_column(BigInteger,ForeignKey("tbVisits.visit_id", ondelete="CASCADE"),nullable=False,index=True)

    # الربط بكتالوج التشخيصات (يمنع حذف المرض من الكتالوج طالما مسجل لمريض)
    diagnosis_catalog_id: Mapped[int] = mapped_column(Integer,ForeignKey("diagnoses_catalog.diagnosis_catalog_id", ondelete="RESTRICT"),nullable=False,index=True)

    # نوع التشخيص: أولي (Preliminary)، نهائي (Final)، تفريقي (Differential)
    diagnosis_type: Mapped[str] = mapped_column(String(50), default="Preliminary", nullable=False)

    # حالة التشخيص الحالية: نشط (Active)، مستقر (Stable)، تم الشفاء (Resolved)
    status: Mapped[str] = mapped_column(String(50), default="Active", nullable=False)

    # تاريخ تسجيل هذا التشخيص
    diagnosis_date: Mapped[date] = mapped_column(Date, nullable=False)

    # اسم الطبيب الذي شخص الحالة
    doctor_name: Mapped[Optional[str]] = mapped_column(String(200), nullable=True)

    # ملاحظات إكلينيكية، الخطة العلاجية، أو توصيات المتابعة
    notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # وقت وتاريخ إنشاء السجل التلقائي في قاعدة البيانات
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True),server_default=func.now(),nullable=False,)

    # --------------------------------------------------------------------------
    # علاقات الـ ORM
    # --------------------------------------------------------------------------
    # 1. الربط مع كائن الزيارة
    visit: Mapped["Visit"] = relationship("Visit", back_populates="diagnoses")

    # 2. الربط السريع مع الكتالوج لجلب اسم المرض وكوده مباشرة للـ DataGrid
    diagnosis: Mapped["DiagnosisCatalog"] = relationship("DiagnosisCatalog", back_populates="patient_diagnoses")


# ==============================================================================
# 1. كلاس كتالوج الأمراض المزمنة (Chronic Diseases Catalog)
# المرجع الثابت للأمراض المزمنة لتوحيد المصطلحات وتسريع البحث في شاشة الإدخال
# ==============================================================================
class ChronicDiseaseCatalog(Base):
    __tablename__ = "chronic_diseases_catalog"

    # المعرف الأساسي للكتالوج (Primary Key) مع ترقيم تلقائي
    disease_catalog_id: Mapped[int] = mapped_column(Integer, autoincrement=True, primary_key=True)

    # كود المرض المعتمد (مثل: CD-HYP-01) - فريد وإجباري ومفهرس
    disease_code: Mapped[str] = mapped_column(String(30), unique=True, nullable=False, index=True)

    # اسم المرض المزمن (مثل: ارتفاع ضغط الدم الأساسي) - مفهرس للـ Auto-Complete
    disease_name: Mapped[str] = mapped_column(String(250), nullable=False, index=True)

    # تصنيف الجهاز الحيوي (قلب وأوعية، جهاز تنفسي، مناعة ذاتية...)
    system_category: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)

    # وصف إضافي أو إرشادات سريرية
    description: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # حالة التفعيل في النظام (مفعل True / معطل False)
    is_active: Mapped[bool] = mapped_column(Boolean, default=True, nullable=False)

    # علاقة عكسية لسجلات المرضى المصابين بهذا المرض
    patient_diseases: Mapped[List["PatientChronicDisease"]] = relationship("PatientChronicDisease", back_populates="disease")


# ==============================================================================
# 2. كلاس سجل الأمراض المزمنة للمريض (Patient Chronic Disease)
# يربط بين الملف الطبي للمريض والمرض المسجل في الكتالوج
# ==============================================================================
class PatientChronicDisease(Base):
    __tablename__ = "patient_chronic_diseases"

    # المعرف الفريد لسجل المرض المزمن للمريض
    id: Mapped[int] = mapped_column(Integer, autoincrement=True, primary_key=True)

    # الربط بالملف الطبي (يُحذف تلقائياً عند حذف ملف المريض CASCADE)
    medical_file_id: Mapped[int] = mapped_column(BigInteger,ForeignKey("tbMedicalFiles.medical_file_id", ondelete="CASCADE"),nullable=False,
        index=True)

    # الربط بكتالوج الأمراض المزمنة (RESTRICT يمنع حذف المرض من الكتالوج طالما مسجل لمريض)
    disease_catalog_id: Mapped[int] = mapped_column(Integer,ForeignKey("chronic_diseases_catalog.disease_catalog_id", ondelete="RESTRICT"),nullable=False,
        index=True,)

    # تاريخ أول تشخيص للمرض عند المريض
    diagnosed_date: Mapped[Optional[date]] = mapped_column(Date, nullable=True)

    # حالة السيطرة على المرض: Controlled (مستقر)، Uncontrolled (غير منضبط)، Active
    status: Mapped[str] = mapped_column(String(50), default="Active", nullable=False)

    # الخطة العلاجية الدائمة أو الأدوية المستمرة التي يتناولها المريض
    current_treatment: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # ملاحظات الطبيب أو مواعيد الفحوصات الدورية المطلوبة
    notes: Mapped[Optional[str]] = mapped_column(Text, nullable=True)

    # وقت وتاريخ تسجيل الحالة في قاعدة البيانات تلقائياً
    created_at: Mapped[datetime] = mapped_column(DateTime(timezone=True),server_default=func.now(),nullable=False)

    # --------------------------------------------------------------------------
    # علاقات الـ ORM
    # --------------------------------------------------------------------------
    # الربط بكائن الملف الطبي (يتكامل مع السطر المعرف مسبقاً في كلاس MedicalFile)
    medical_file: Mapped["MedicalFile"] = relationship("MedicalFile", back_populates="chronic_diseases")

    # الربط المباشر بالكتالوج لجلب تفاصيل وكود واسم المرض تلقائياً للواجهة
    disease: Mapped["ChronicDiseaseCatalog"] = relationship("ChronicDiseaseCatalog", back_populates="patient_diseases", lazy="joined")


#//////////////////////////////////////////////////////////////////













class Visit(Base):
    """
    جدول الزيارات واللقاءات السريرية الفعلية للمريض
    """
    __tablename__ = "tbVisits"

    visit_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للزيارة السريرية

    appointment_id: Mapped[Optional[int]] = mapped_column(BigInteger, ForeignKey("tbAppointments.appointment_id"), nullable=True, index=True)
    # معرف الموعد المسبق إن كانت الزيارة ناتجة عن حجز مسبق، أو Null إن كانت زيارة طارئة/مباشرة

    medical_file_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbMedicalFiles.medical_file_id"), nullable=False, index=True)
    # معرف الملف الطبي للمريض

    Employee_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbEmployees.EmployeeId"), nullable=False, index=True)
    # معرف الطبيب المعالج الذي باشر الكشف على المريض في هذه الزيارة

    visit_checkintime: Mapped[datetime] = mapped_column(DateTime, nullable=False)
    # وقت دخول المريض إلى العيادة أو الاستقبال وبدء الزيارة

    visit_checkouttime: Mapped[Optional[datetime]] = mapped_column(DateTime, nullable=True)
    # وقت انتهاء الزيارة ومغادرة المريض للعيادة

    visit_type: Mapped[str] = mapped_column(String(50), default="Consultation")
    # نوع الزيارة: كشف جديد (New Visit)، مراجعة/استشارة مجانية (Follow-up)، أو طوارئ (Emergency)

    visit_status: Mapped[str] = mapped_column(String(50), default="Completed")
    # حالة الزيارة الحالية: قيد الانتظار (Waiting)، عند الطبيب (In-Consultation)، أو منتهية (Completed)

    created_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # تاريخ ووقت فتح سجل الزيارة

    # داخل كلاس Visit:
    medical_file: Mapped["MedicalFile"] = relationship(
        "MedicalFile", 
        back_populates="visits"
    )

    # ربط الزيارة بتشخيصات الطبيب خلالها
    diagnoses: Mapped[list["PatientDiagnosis"]] = relationship(
        "PatientDiagnosis", 
        back_populates="visit",
        cascade="all, delete-orphan"
    )
class VitalSign(Base):
    """
    جدول العلامات الحيوية المقاسة للمريض أثناء الزيارة
    """
    __tablename__ = "tbVitalSigns"

    vital_sign_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لقياس العلامات الحيوية

    visit_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbVisits.visit_id"), nullable=False, index=True)
    # معرف الزيارة التي تم فيها أخذ هذه القياسات الحيوية للمريض

    blood_pressure: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # ضغط الدم (الانقباضي/الانبساطي مثل: 120/80)

    heart_rate: Mapped[Optional[int]] = mapped_column(Integer, nullable=True)
    # معدل نبضات القلب في الدقيقة (Pulse Rate مثل: 75 bpm)

    temperature_c: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # درجة حرارة الجسم بالدرجة المئوية (مثل: 37.2 °C)

    oxygen_stauration: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # نسبة تشبع الأكسجين في الدم (SpO2 مثل: 98%)

    weight: Mapped[Optional[float]] = mapped_column(Float, nullable=True)
    # وزن المريض بالكيلوغرام (مثل: 70.5 kg)

    height: Mapped[Optional[float]] = mapped_column(Float, nullable=True)
    # طول المريض بالسنتيمتر (مثل: 172 cm) لحساب مؤشر كتلة الجسم BMI

    breath: Mapped[Optional[int]] = mapped_column(Integer, nullable=True)
    # معدل التنفس في الدقيقة (Respiratory Rate)

    notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات الممرض/الطبيب عند أخذ القياسات (مثل: المريض كان مجهداً أثناء قياس الضغط)