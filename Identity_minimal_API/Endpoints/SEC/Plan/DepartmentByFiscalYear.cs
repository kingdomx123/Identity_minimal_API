using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;

public static class SEC_Plan_Department_Endpoints
{
    public static void MapSEC_Plan_Department_Endpoints(this WebApplication app, string connectionString)
    {
        // กำหนดเส้นทาง API สำหรับดึงข้อมูลแผนกตามปีงบประมาณ
        app.MapPost("/Endpoints/SEC/plan/DepartmentByFiscalYear", [AllowAnonymous]
        (string fiscalYear) =>
        {
            // แปลงค่า fiscalYear จาก string เป็น int และตรวจสอบว่าการแปลงสำเร็จหรือไม่
            if (!int.TryParse(fiscalYear, out int fYear)) return Results.BadRequest();

            // ใช้ PlanDbContext ในการเชื่อมต่อฐานข้อมูล (ผ่าน connection string)
            using (PlanDbContext context = new PlanDbContext(connectionString))
            {
                // สร้าง instance ของ DepartmentService เพื่อเรียกใช้งานข้อมูลแผนก
                DepartmentService departmentService = new DepartmentService(context);

                // ดึงข้อมูลแผนกที่ยัง Active และอยู่ในปีงบประมาณที่กำหนด
                var departments = departmentService.DbSet()
                    .Where(c => c.Active && c.FiscalYear == fYear)
                    .Select(c => new { Name = c.Name, Id = c.Id, ParentId = c.ParentDepartmentId }) // เลือกเฉพาะฟิลด์ที่ต้องการ
                    .ToList();

                // ส่งผลลัพธ์กลับเป็น JSON
                return Results.Ok(departments);
            }
        })
        .WithTags("Department")
        .WithGroupName("SEC");

        // กำหนดเส้นทาง API สำหรับดึงข้อมูลแผนกระดับบนสุดตามปีงบประมาณ
        app.MapPost("/Endpoints/SEC/plan/TopLevelDepartmentByFiscalYear", [AllowAnonymous]
        (string fiscalYear) =>
        {
            // แปลงค่า fiscalYear จาก string เป็น int และตรวจสอบว่าการแปลงสำเร็จหรือไม่
            if (!int.TryParse(fiscalYear, out int fYear)) return Results.BadRequest();

            // ใช้ PlanDbContext ในการเชื่อมต่อฐานข้อมูล
            using (PlanDbContext context = new PlanDbContext(connectionString))
            {
                // สร้าง instance ของ DepartmentService เพื่อดึงข้อมูลแผนกระดับบนสุด
                DepartmentService departmentService = new DepartmentService(context);

                // ดึงข้อมูลแผนกระดับบนสุดของปีงบประมาณที่กำหนด
                var topLevelDepartments = departmentService.GetTopHierarchyByFiscalYear(fYear)
            .Select(c => new
            {
                Name = c.Name,
                Id = c.Id,
                SubCount = c.SubEntities.Count(d => d.Active) // นับจำนวนหน่วยย่อยที่ยัง Active
            })
            .ToList();

                // ส่งผลลัพธ์กลับเป็น JSON
                return Results.Ok(topLevelDepartments);
            }
        })
        .WithTags("Department")
        .WithGroupName("SEC");
    }
}