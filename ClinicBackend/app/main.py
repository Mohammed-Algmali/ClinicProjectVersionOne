
# from app.modules.facilities import models
# from app.modules.human_resources import models
# from app.modules.subscriptions import models

# from app.modules.system_core import models
# from app.modules.users import models
# from fastapi import FastAPI
# from app.core.database import engine, Base
import app.modules.biometrics.models


# from app.modules.appointments import models
# from app.modules.billing import models

# from app.modules.laboratories import models
# from app.modules.medical_records import models
# from app.modules.prescriptions import models
# from app.modules.biometrics import models



# # # أمر إنشاء جميع الجداول تلقائياً في قاعدة البيانات
#Base.metadata.create_all(bind=engine)
#print("All tables created successfully !")
#Base.metadata.drop_all(bind=engine)
#print("All tables drop successfully !")





from fastapi import FastAPI
from app.core.database import engine, Base
from app.modules.auth import router as auth_router
from app.modules.users.router import roles_router , user_router 
from app.modules.facilities.router import router as facility_router
from app.modules.human_resources.router import  employees_router,shift_router

# # إضافة الراوتر مع بقية الراوترات

# أضف هذا الجزء في main.py
from app.core.exceptions import AppException
from fastapi import Request
from fastapi.responses import JSONResponse





app = FastAPI(title="نظام إدارة المنشآت الطبية", version="1.0.0")

# ربط موجه المصادقة بالتطبيق#
app.include_router(auth_router.router)
app.include_router(roles_router)
app.include_router(user_router)
app.include_router(facility_router)
app.include_router(employees_router)
app.include_router(shift_router)




@app.exception_handler(AppException)
async def custom_app_exception_handler(request: Request, exc: AppException):
  """يلتقط أي خطأ يخرج من طبقة الـ Service ويصيغه للفرونت إند بشكل موحد وأنيق"""
  return JSONResponse(
      status_code=exc.status_code,
      content={
          "success": False,
          "error_code": exc.error_code,
          "message": exc.message,
      },
  )


@app.get("/")
def root():
  return {"message": "النظام الطبي يعمل بنجاح"}