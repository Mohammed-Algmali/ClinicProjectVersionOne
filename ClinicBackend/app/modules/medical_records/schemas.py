from datetime import datetime
from typing import Optional
from pydantic import BaseModel
from app.modules.schemas import PatientCreate








class PatientRegisterRequest(BaseModel):
    Patient:PatientCreate