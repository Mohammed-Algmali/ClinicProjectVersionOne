# # seed.py
# from app.core.database import SessionLocal
# from app.modules.users.models  import Role, User
# from app.modules.auth.utils import get_password_hash


# def create_initial_admin():
#   db = SessionLocal()
#   try:
#     # 1. التأكد من وجود دور "مدير النظام" (Admin Role)
#     admin_role = db.query(Role).filter(Role.RoleName == "Admin").first()
#     if not admin_role:
#       admin_role = Role(
#           RoleName="Admin",
#           RoleDescription="مدير النظام بصلاحيات كاملة",
#           RoleStatus="Active",
#           RoleLevel=1,
#       )
#       db.add(admin_role)
#       db.commit()
#       db.refresh(admin_role)
#       print(" تم إنشاء دور مدير النظام (Admin Role) بنجاح.")

#     # 2. التأكد من عدم وجود المستخدم مسبقاً
#     existing_user = db.query(User).filter(User.Username == "admin").first()
#     # if existing_user:
#     #   print("⚠️ المستخدم (admin) موجود مسبقاً في قاعدة البيانات.")
#     #   return

#     # 3. تشفير كلمة المرور وإنشاء المستخدم الأول
#     hashed_pwd = get_password_hash("admin1223")  # كلمة المرور هنا: admin123

#     new_user = User(
#         RoleId=admin_role.RoleId,
#         UserFullName="مدير النظام الرئيسي",
#         Username="admin",  # اسم المستخدم
#         UserEmail="admin@clinic.com",
#         UserPhone="777000000",
#         Password=hashed_pwd,  # يتم حفظها مشفرة في القاعدة
#         UserStatus="Active",
#         UserAddress="حضرموت",
#     )

#     db.add(new_user)
#     db.commit()

#     print("\n" + "=" * 50)
#     print(" تم إنشاء حساب المدير بنجاح:")
#     print("   اسم المستخدم: admin")
#     print("   كلمة المرور:   admin123")
#     print("=" * 50 + "\n")

#   finally:
#     db.close()


# if __name__ == "__main__":
#   create_initial_admin()