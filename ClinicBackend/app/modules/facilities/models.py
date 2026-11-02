# facilities/models.py
from datetime import datetime
from typing import Optional, List
from sqlalchemy import BigInteger, Integer, String, Text, DateTime, ForeignKey, func
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


class Facility(Base):
    """
    جدول المنشآت الطبية (المستشفيات، المراكز الطبية، المجمعات العيادية).
    - يمثل الكيان الرئيسي الذي يسجله مالك المنشأة (User).
    - ترتبط به الاشتراكات (Subscriptions) والفروع التابعة له (Branches).
    """
    __tablename__ = "Facilities"

    FacilityId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    UserId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Users.UserId"), nullable=False, index=True)
    FacilityName: Mapped[str] = mapped_column(String(100), nullable=False)
    FacilityType: Mapped[str] = mapped_column(String(50), nullable=False)  # مثل: مستشفى، مركز، مجمع
    FacilityLicenseNumber: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    LicenseIssueDate: Mapped[Optional[datetime]] = mapped_column(DateTime, nullable=True)
    LicenseExpiryDate: Mapped[Optional[datetime]] = mapped_column(DateTime, nullable=True)
    FacilityStatus: Mapped[str] = mapped_column(String(50), default="Active")
    FacilityCode: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    FacilityCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    FacilityCountry: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    FacilityCity: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    FacilityAddress: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    FacilityPhone: Mapped[Optional[str]] = mapped_column(String(20), nullable=True)
    FacilityEmail: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    FacilityWebSite: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)

    # العلاقات: المنشأة الواحدة تمتلك فرعاً واحداً أو أكثر
    branches: Mapped[List["Branch"]] = relationship("Branch", back_populates="facility", cascade="all, delete-orphan")


class Branch(Base):
    """
    جدول الفروع التابعة للمنشأة الطبية.
    - يمثل الموقع الجغرافي أو المقر الفرعي للمستشفى/المركز.
    - يرتبط بمنشأة رئيسية (Facility) ويحتوي على عدة أقسام (Departments).
    """
    __tablename__ = "tbBranches"

    BranchId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    FacilityId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Facilities.FacilityId"), nullable=False, index=True)
    BranchName: Mapped[str] = mapped_column(String(255), nullable=False)
    BranchPhone: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    BranchCity: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)
    BranchAddress: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    ManagerId: Mapped[Optional[int]] = mapped_column(BigInteger, nullable=True)  # معرف مدير الفرع
    WorkingHours: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    BranchStatus: Mapped[str] = mapped_column(String(50), default="Active")
    BranchCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # العلاقات: الفرع يتبع منشأة واحدة، ويمتلك عدة أقسام
    facility: Mapped["Facility"] = relationship("Facility", back_populates="branches")
    departments: Mapped[List["Department"]] = relationship("Department", back_populates="branch", cascade="all, delete-orphan")


class Department(Base):
    """
    جدول الأقسام الطبية والإدارية داخل الفرع.
    - مثل: قسم الباطنية، قسم الطوارئ، قسم العظام، قسم الإدارة.
    - يتبع فرعاً معيناً (Branch) ويعمل فيه الكادر الطبي والموظفون (Employees).
    """
    __tablename__ = "tbDepartments"

    DepartmentId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    BranchId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbBranches.BranchId"), nullable=False, index=True)
    DepartmentName: Mapped[str] = mapped_column(String(255), nullable=False)
    DepartmentType: Mapped[Optional[str]] = mapped_column(String(100), nullable=True)  # طبي، إداري، مختبر
    DepartmentCode: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    DepartmentDescription: Mapped[Optional[str]] = mapped_column(Text, nullable=True)
    PhoneExtension: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)
    DepartmentStatus: Mapped[str] = mapped_column(String(50), default="Active")
    DepartmentCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # العلاقات: القسم يتبع فرعاً واحداً
    branch: Mapped["Branch"] = relationship("Branch", back_populates="departments")