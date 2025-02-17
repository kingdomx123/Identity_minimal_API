using iLinkDomain.DataAccess.SEC.HR;
using iLinkDomain.Service.SEC.HR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public static class SEC_HREndpoints
{
    public static void MapSEC_HREndpoints(this WebApplication app, string connectionString)
    {
        // กำหนดเส้นทาง API สำหรับดึงข้อมูลพนักงานทั้งหมด (สูงสุด 50 รายการ)
        app.MapGet("/Endpoint/SEC/hr/Staff", () =>
        {
            // ใช้ HRDbContext ในการเชื่อมต่อฐานข้อมูล
            using (HRDbContext hRDbContext = new HRDbContext(connectionString))
            {
                // สร้าง instance ของ StaffService เพื่อดึงข้อมูลพนักงาน
                StaffService staffService = new StaffService(hRDbContext);

                // ดึงข้อมูลพนักงานที่ Active และรวมข้อมูล Title (คำนำหน้าชื่อ)
                var staffList = staffService.DbSet()
                    .Include(c => c.NameTitle) // โหลดข้อมูลคำนำหน้าชื่อ
                    .Where(c => c.Active) // กรองเฉพาะพนักงานที่ Active
                    .Select(c => new
                    {
                        FullName = c.FullNameWithTitle, // ชื่อเต็มรวมคำนำหน้า
                        Name = c.Name,
                        Surname = c.Surname,
                        Id = c.Id,
                        hrDepId = c.HRDepartmentId, // รหัสแผนก
                        HRDep = c.HRDepartment.Name, // ชื่อแผนก (โหลดผ่าน Navigation Property)
                        HRDepName = c.DepartmentName // ชื่อแผนกเพิ่มเติม (อาจมาจากฟิลด์ที่เก็บค่าโดยตรง)
                    })
                    .Take(50) // จำกัดผลลัพธ์ที่ 50 รายการ
                    .ToList();

                // ส่งผลลัพธ์กลับเป็น JSON
                return Results.Ok(staffList);
            }
        })
        .WithTags("HR")
        .WithGroupName("SEC");

        // กำหนดเส้นทาง API สำหรับดึงข้อมูลแผนกและจำนวนพนักงานในแต่ละแผนก
        app.MapGet("/Endpoint/SEC/hr/HRDepartment", () =>
        {
            // ใช้ HRDbContext ในการเชื่อมต่อฐานข้อมูล
            using (HRDbContext hRDbContext = new HRDbContext(connectionString))
            {
                // สร้าง instance ของ HRDepartmentService เพื่อดึงข้อมูลแผนก HR
                HRDepartmentService hrDepServ = new HRDepartmentService(hRDbContext);

                // ดึงข้อมูลแผนกที่ Active และรวมข้อมูลพนักงานในแผนกนั้น
                var departmentList = hrDepServ.DbSet()
                    .Include(c => c.Staffs) // โหลดข้อมูลพนักงานในแต่ละแผนก
                    .Where(c => c.Active) // กรองเฉพาะแผนกที่ Active
                    .Select(c => new
                    {
                        Name = c.Name, // ชื่อแผนก
                        StaffCount = c.Staffs.Count(d => d.Active), // นับจำนวนพนักงานที่ Active ในแผนก
                        FirstStaffName = c.Staffs.First(d => d.Active).FullName // ดึงชื่อพนักงานคนแรกที่ Active
                    })
                    .ToList();

                // ส่งผลลัพธ์กลับเป็น JSON
                return Results.Ok(departmentList);
            }
        })
        .WithTags("HR")
        .WithGroupName("SEC");

        // กำหนดเส้นทาง API สำหรับดึงข้อมูลพนักงานตามรหัสแผนก HR
        app.MapPost("/Endpoint/SEC/hr/StaffByHRDepartment", [AllowAnonymous]
        (string hrDepId) =>
        {
            // แปลงค่า hrDepId จาก string เป็น int และตรวจสอบว่าการแปลงสำเร็จหรือไม่
            if (!int.TryParse(hrDepId, out int depId)) return Results.BadRequest();

            // ใช้ HRDbContext ในการเชื่อมต่อฐานข้อมูล
            using (HRDbContext context = new HRDbContext(connectionString))
            {
                // สร้าง instance ของ StaffService เพื่อดึงข้อมูลพนักงานในแผนกที่ระบุ
                StaffService staffServ = new StaffService(context);

                // ดึงข้อมูลพนักงานที่อยู่ในแผนกที่ระบุ และยัง Active
                var staffList = staffServ.DbSet()
                    .Where(c => c.Active && c.HRDepartmentId == depId)
                    .Select(c => new
                    {
                        FullName = c.FullNameWithTitle, // ชื่อเต็มรวมคำนำหน้า
                        Name = c.Name,
                        Surname = c.Surname,
                        Id = c.Id,
                        hrDepId = c.HRDepartmentId, // รหัสแผนก
                        HRDep = c.HRDepartment.Name, // ชื่อแผนก (โหลดผ่าน Navigation Property)
                        HRDepName = c.DepartmentName // ชื่อแผนกเพิ่มเติม (อาจมาจากฟิลด์ที่เก็บค่าโดยตรง)
                    })
                    .ToList();

                // ส่งผลลัพธ์กลับเป็น JSON
                return Results.Ok(staffList);
            }
        })
        .WithTags("HR")
        .WithGroupName("SEC");
    }
}