# medical_records/models.py
from datetime import datetime
from typing import Optional, List
from sqlalchemy import (
    BigInteger,
    Integer,
    String,
    Text,
    DateTime,
    ForeignKey,
    func,
    Float
)
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


# ==============================================================================
# 1. جدول المريض (patient)
# ==============================================================================
class Patient(Base):
    """
    جدول البيانات الشخصية والديموغرافية للمريض
    """
    __tablename__ = "patient"

    patient_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للمريض في النظام

    user_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, unique=True, index=True)
    # ربط المريض بحسابه في جدول Users (الذي يحوي الاسم الكامل، الهاتف، والعنوان)

    gender: Mapped[str] = mapped_column(String(50), nullable=False)
    # الجنس (ذكر / أنثى)

    birth_date: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # تاريخ ميلاد المريض لحساب العمر بدقة

    emergency_content_phone: Mapped[Optional[int]] = mapped_column(BigInteger, nullable=True)
    # رقم هاتف جهة الاتصال في حالات الطوارئ (الأب، الأم، أو القريب)

    status: Mapped[str] = mapped_column(String(50), default="Active")
    # حالة ملف المريض (Active / Inactive)

    create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت فتح ملف المريض في النظام

    # العلاقات
    medical_file: Mapped[Optional["MedicalFile"]] = relationship("MedicalFile", back_populates="patient", uselist=False)


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

    patient_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("patient.patient_id"), nullable=False, unique=True, index=True)
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

    # العلاقات
    patient: Mapped["Patient"] = relationship("Patient", back_populates="medical_file")
    chronic_diseases: Mapped[List["PatientChronicDisease"]] = relationship("PatientChronicDisease", back_populates="medical_file", cascade="all, delete-orphan")
    allergies: Mapped[List["Allergy"]] = relationship("Allergy", back_populates="medical_file", cascade="all, delete-orphan")
    surgeries: Mapped[List["Surgery"]] = relationship("Surgery", back_populates="medical_file", cascade="all, delete-orphan")


# ==============================================================================
# 3. جدول الأمراض المزمنة (tbPatientChronicDiseases)
# ==============================================================================
class PatientChronicDisease(Base):
    """
    جدول التاريخ المرضي والأمراض المزمنة للمريض
    """
    __tablename__ = "tbPatientChronicDiseases"

    patient_chronic_diseases_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسجل المرض المزمن

    medical_file_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbMedicalFiles.medical_file_id"), nullable=False, index=True)
    # معرف الملف الطبي للمريض

    chronic_diseases_name: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم المرض المزمن (مثل: السكري، ضغط الدم، الربو، الفشل الكلوي)

    chronic_diseases_code: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # الرمز الطبي الدولي للمرض (ICD-10 Code مثل: E11 للسكري)

    severity: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # درجة خطورة الحالة (خفيف Mild، متوسط Moderate، حاد Severe)

    notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات العلاج والمتابعة المستمرة للمرض

    create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ تسجيل المرض المزمن في ملف المريض

    # العلاقات
    medical_file: Mapped["MedicalFile"] = relationship("MedicalFile", back_populates="chronic_diseases")


# ==============================================================================
# 4. جدول الحساسيات (tbAllergies)
# ==============================================================================
class Allergy(Base):
    """
    جدول سجل الحساسيات لدى المريض (أدوية، أطعمة، بيئية)
    """
    __tablename__ = "tbAllergies"

    allergies_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي لسجل الحساسية

    medical_file_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbMedicalFiles.medical_file_id"), nullable=False, index=True)
    # معرف الملف الطبي التابع له المريض

    allergien_name: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم المادة المسببة للحساسية (مثل: البنسلين، الأسبرين، الفول السوداني)

    allergien_code: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # الكود الطبي لمسبب الحساسية

    action_description: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # رد الفعل التحسسي والأعراض التي تظهر على المريض (مثل: ضيق تنفس، طفح جلدي، صدمة)

    severity: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # مستوى الخطورة (خفيف، متوسط، يهدد الحياة Life-Threatening)

    status: Mapped[str] = mapped_column(String(50), default="Active")
    # حالة الحساسية: نشطة ومستمرة (Active) أو خاملة/زالت (Inactive)

    create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ رصد وتوثيق الحساسية في ملف المريض

    # العلاقات
    medical_file: Mapped["MedicalFile"] = relationship("MedicalFile", back_populates="allergies")


# ==============================================================================
# 5. جدول العمليات الجراحية السابقة (tbSurgeries)
# ==============================================================================
class Surgery(Base):
    """
    جدول التاريخ الجراحي والعمليات السابقة للمريض
    """
    __tablename__ = "tbSurgeries"

    Surgery_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للعملية الجراحية

    Medical_file_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbMedicalFiles.medical_file_id"), nullable=False, index=True)
    # معرف الملف الطبي للمريض

    Surgery_name: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم العملية الجراحية (مثل: استئصال الزائدة الدودية، إصلاح فتق إربي، استبدال مفصل)

    Surgery_code: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # الكود الطبي الإجرائي للعملية الجراحية (CPT / ICD Procedure Code)

    Surgery_date: Mapped[Optional[datetime]] = mapped_column(DateTime, nullable=True)
    # تاريخ إجراء العملية الجراحية

    Indication: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # الداعي الطبي والسبب الرئيسي لإجراء العملية الجراحية

    Bodysite: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # موضع الجراحة في الجسم (مثل: الجانب الأيمن من البطن، الركبة اليسرى)

    Onesthesia: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # نوع التخدير المستخدم (تخدير عام General، موضعي Local، نصفي Spinal)

    Hospital_name: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # اسم المستشفى أو المركز الذي أُجريت فيه العملية

    Surgerian_name: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    # اسم الطبيب الجراح الذي أجرى العملية

    Complications: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # المضاعفات الجانبية إن حدثت بعد العملية (مثل: نزيف، التهاب، أو لا توجد)

    Notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات وتوصيات إضافية بخصوص الجراحة وفترة التعافي

    Create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ تسجيل الجراحة في ملف المريض

    # العلاقات
    medical_file: Mapped["MedicalFile"] = relationship("MedicalFile", back_populates="surgeries")


# ==============================================================================
# 6. جدول التشخيصات الطبية (tbDiagnoses)
# ==============================================================================
class Diagnosis(Base):
    """
    جدول التشخيصات الطبية المسجلة للحالة
    """
    __tablename__ = "tbDiagnoses"

    Diagosis_id: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    # المعرف الأساسي للتشخيص الطبي

    Visit_id: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbVisits.visit_id"), nullable=False, index=True)
    # معرف الزيارة الطبية التي قام الطبيب فيها بتسجيل هذا التشخيص

    Diagosis_name: Mapped[str] = mapped_column(String(100), nullable=False)
    # اسم التشخيص الطبي (مثل: التهاب الشعب الهوائية الحاد، قرحة معدية)

    Diagosis_code: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # رمز التشخيص الدولي المعتمد (ICD-10 Code مثل: J20.9)

    Diagosis_type: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    # تصنيف نوع التشخيص (تشخيص مبدئي Provisional، نهائي Final، أو تفريقي Differential)

    Description: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # تفاصيل وشرح سريري إضافي لحالة المريض وأعراضه

    Notes: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    # ملاحظات وتوجيهات الطبيب المعالج للتشخيص

    Create_at: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    # تاريخ ووقت توثيق التشخيص في السجل


    # يضاف داخل medical_records/models.py


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

    doctor_id: Mapped[int] = mapped_column(BigInteger, nullable=False, index=True)
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