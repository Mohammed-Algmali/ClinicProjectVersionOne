from typing import Dict, Any, List
from sqlalchemy.orm import Session
from sqlalchemy import select
from app.core.BaseRepository import BaseRepository
from app.modules.medical_records.models import MedicalFile


class MedicalFileRepository(BaseRepository[MedicalFile]):
    """
    مستودع بيانات الملفات الطبية الإلكترونية (tbMedicalFiles):
    المسؤول عن استعلامات وإنشاء الإضبارات والسجلات الطبية للمرضى والموظفين
    """
    def __init__(self, db: Session):
        # تمرير كلاس MedicalFile للـ BaseRepository ليرث دوال الحفظ والبحث التلقائية
        super().__init__(MedicalFile, db)

   