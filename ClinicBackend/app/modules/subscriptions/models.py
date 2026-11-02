# subscriptions/models.py
from datetime import datetime
from typing import Optional, List
from decimal import Decimal
from sqlalchemy import BigInteger, Integer, String, DateTime, Numeric, ForeignKey, func
from sqlalchemy.orm import Mapped, mapped_column, relationship
from app.core.database import Base


class Plan(Base):
    """
    جدول باقات النظام الأساسية (SaaS Plans).
    - مثل: الباقة الأساسية Basic، الباقة المتقدمة Pro، باقة المستشفيات Enterprise.
    - تحدد المميزات الرئيسية مثل عدد الفروع المسموح بها للمنشأة الطبية.
    """
    __tablename__ = "tbPlans"

    PlanId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    PlanName: Mapped[str] = mapped_column(String(100), nullable=False)
    PlanDescription: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)
    AllowedBranches: Mapped[int] = mapped_column(Integer, default=1, nullable=False)  # عدد الفروع المسموح فتحها
    PlanStatus: Mapped[str] = mapped_column(String(50), default="Active")
    PlanCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # العلاقات: الخطة الواحدة يمكن أن يكون لها عدة أسعار (شهري، سنوي)
    prices: Mapped[List["PlanPrice"]] = relationship("PlanPrice", back_populates="plan", cascade="all, delete-orphan")


class PlanPrice(Base):
    """
    جدول الأسعار ودورات الفوترة الخاصة بكل باقة (Billing Cycles & Pricing).
    - يسمح بتحديد أكثر من سعر لنفس الباقة (مثلاً: اشتراك شهري، نصف سنوي، أو سنوي) مع تحديد العملة.
    """
    __tablename__ = "tbPlanPrices"

    PlanPriceId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    PlanId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbPlans.PlanId"), nullable=False, index=True)
    BillingCycle: Mapped[str] = mapped_column(String(50), nullable=False)  # مثل: Monthly, Quarterly, Annually
    PriceAmount: Mapped[Decimal] = mapped_column(Numeric(10, 2), nullable=False)
    CurrencyCode: Mapped[str] = mapped_column(String(10), default="USD")  # رمز العملة (USD, YER, SAR)
    PriceStatus: Mapped[str] = mapped_column(String(50), default="Active")

    # العلاقات
    plan: Mapped["Plan"] = relationship("Plan", back_populates="prices")
    subscriptions: Mapped[List["Subscription"]] = relationship("Subscription", back_populates="plan_price")


class Subscription(Base):
    """
    جدول اشتراكات المنشآت الطبية (Facility Subscriptions).
    - يمثل الاشتراك الفعلي الجاري للمنشأة في باقة وسعر محددين.
    - يتحكم في صلاحية وصول المنشأة للنظام وتواريخ البدء والانتهاء والتجديد.
    """
    __tablename__ = "tbSubscriptions"

    SubscriptionId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    FacilityId: Mapped[int] = mapped_column(BigInteger, ForeignKey("Facilities.FacilityId"), nullable=False, index=True)
    PlanPriceId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbPlanPrices.PlanPriceId"), nullable=False, index=True)
    SubscriptionStartDate: Mapped[datetime] = mapped_column(DateTime, nullable=False)
    SubscriptionEndDate: Mapped[datetime] = mapped_column(DateTime, nullable=False)
    SubscriptionCreatedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())
    SubscriptionCancelledAt: Mapped[Optional[datetime]] = mapped_column(DateTime, nullable=True)
    SubscriptionStatus: Mapped[str] = mapped_column(String(50), default="Active")  # Active, Expired, Suspended, Cancelled

    # العلاقات
    plan_price: Mapped["PlanPrice"] = relationship("PlanPrice", back_populates="subscriptions")
    history: Mapped[List["SubscriptionHistory"]] = relationship("SubscriptionHistory", back_populates="subscription", cascade="all, delete-orphan")
    # payments: Mapped[List["Payment"]] = relationship("Payment", back_populates="subscription")  # في مجلد billing


class SubscriptionHistory(Base):
    """
    جدول تتبع سجل وتغيرات الاشتراك (Subscription Audit & History).
    - يسجل أي تعديل يطرأ على حالة اشتراك المنشأة (مثل: تجديد، إيقاف مؤقت، ترقية، أو إلغاء) مع ذكر السبب والمسؤول.
    """
    __tablename__ = "tbSubscriptionHistory"

    HistoryId: Mapped[int] = mapped_column(BigInteger, primary_key=True, autoincrement=True)
    SubscriptionId: Mapped[int] = mapped_column(BigInteger, ForeignKey("tbSubscriptions.SubscriptionId"), nullable=False, index=True)
    SubHOldStatus: Mapped[Optional[str]] = mapped_column(String(50), nullable=True)  # الحالة السابقة
    SubHNewStatus: Mapped[str] = mapped_column(String(50), nullable=False)          # الحالة الجديدة
    ChangedBy: Mapped[Optional[int]] = mapped_column(BigInteger, nullable=True)     # معرف المستخدم الذي أجرى التعديل
    SubHChangeReason: Mapped[Optional[str]] = mapped_column(String(255), nullable=True)  # سبب التغيير
    SubHChangedAt: Mapped[datetime] = mapped_column(DateTime, server_default=func.now())

    # العلاقات
    subscription: Mapped["Subscription"] = relationship("Subscription", back_populates="history")