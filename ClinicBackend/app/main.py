


# # استيراد كافة ملفات الـ models ليتعرف عليها محرك القاعدة
# from app.modules.appointments import models
# from app.modules.billing import models
# from app.modules.facilities import models
# from app.modules.human_resources import models
# from app.modules.laboratories import models
# from app.modules.medical_records import models
# from app.modules.prescriptions import models
# from app.modules.subscriptions import models
# from app.modules.system_core import models
# from app.modules.users import models

# # أمر إنشاء جميع الجداول تلقائياً في قاعدة البيانات
# Base.metadata.create_all(bind=engine)
# print("All tables created successfully !")

from fastapi import FastAPI
from app.core.database import engine, Base
from app.modules.auth import router as auth_router
from app.modules.users import router as role_router




app = FastAPI(title="نظام إدارة المنشآت الطبية", version="1.0.0")

# ربط موجه المصادقة بالتطبيق
app.include_router(auth_router.router)
app.include_router(role_router.router)


@app.get("/")
def root():
  return {"message": "النظام الطبي يعمل بنجاح"}