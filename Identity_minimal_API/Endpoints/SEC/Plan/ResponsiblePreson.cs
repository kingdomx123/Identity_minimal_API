using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_ResponsiblePreson_Endpoints
    {
        public static void MapSEC_ResponsiblePreson_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/ResponsiblePreson/ResponsiblePreson_Create/{PlanCoreId}", [AllowAnonymous] (int PlanCoreId, ResponsiblePresonRequest_Create request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var Chack_ResponsiblePerson_PlanCoreId = context.ResponsiblePersons.FirstOrDefault(c => c.PlanCoreId == PlanCoreId);
                    if (Chack_ResponsiblePerson_PlanCoreId == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการนี้");
                    }

                    var newResponsiblePreson = new ResponsiblePerson
                    {
                        Name = request.Name,
                        Active = request.Active,
                        FiscalYear = request.FiscalYear,
                        StaffId = request.StaffId,
                        PlanPersonResponsibilityEnum = request.PlanPersonResponsibilityEnum,
                        PlanCoreId = PlanCoreId,
                        HRDepartmentId = request.HRDepartmentId,
                        HRDepartmentName = request.HRDepartmentName ?? "",
                        PhoneNumber = request.PhoneNumber ?? "",
                    };

                    context.ResponsiblePersons.Add(newResponsiblePreson);

                    context.SaveChanges();


                    return Results.Ok(new { Message = "สร้างข้อมูลบุคคลากรเสร็จสิ้น", newResponsiblePreson });
                }
            })
            .WithTags("ResponsiblePreson")
            .WithGroupName("SEC_ResponsiblePreson");

            app.MapPut("/Endpoint/SEC/Plan/ResponsiblePreson/ResponsiblePreson_Update/{id}", [AllowAnonymous] (int id, ResponsiblePresonRequest_Update request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var existingResponsiblePerson = context.ResponsiblePersons.FirstOrDefault(c => c.Id == id);
                    if (existingResponsiblePerson == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลบุคคลากรที่ต้องการแก้ไข");
                    }

                    existingResponsiblePerson.Name = request.Name;
                    existingResponsiblePerson.Active = request.Active;
                    existingResponsiblePerson.FiscalYear = request.FiscalYear;
                    existingResponsiblePerson.StaffId = request.StaffId;
                    existingResponsiblePerson.PlanPersonResponsibilityEnum = request.PlanPersonResponsibilityEnum;
                    existingResponsiblePerson.PlanCoreId = request.PlanCoreId;
                    existingResponsiblePerson.HRDepartmentId = request.HRDepartmentId;
                    existingResponsiblePerson.HRDepartmentName = request.HRDepartmentName ?? "";
                    existingResponsiblePerson.PhoneNumber = request.PhoneNumber ?? "";


                    context.SaveChanges();


                    return Results.Ok(new { Message = "อัพเดทข้อมูลบุคคลากรสำเร็จ", existingResponsiblePerson });
                }
            })
            .WithTags("ResponsiblePreson")
            .WithGroupName("SEC_ResponsiblePreson");


            app.MapDelete("/Endpoint/SEC/Plan/ResponsiblePreson/ResponsiblePreson_Delete/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var ResponsiblePreson = context.ResponsiblePersons.FirstOrDefault(c => c.Id == id); ;
                    if (ResponsiblePreson == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลบุคคลากรที่ต้องการลบ");
                    }

                    context.ResponsiblePersons.Remove(ResponsiblePreson);
                    context.SaveChanges();

                    return Results.Ok(new { Message = "ลบบุคคลากรเสร็จสิ้น" });
                }
            })
            .WithTags("ResponsiblePreson")
            .WithGroupName("SEC_ResponsiblePreson");

            app.MapGet("/Endpoint/SEC/Plan/ResponsiblePreson/ResponsiblePreson_GetData/{PlanCoreId}", [AllowAnonymous] (int PlanCoreId, int PlanActivityId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var ResponsiblePreson = context.ResponsiblePersons.FirstOrDefault(c => c.PlanCoreId == PlanCoreId && c.PlanActivityId == PlanActivityId);
                    if (ResponsiblePreson == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการและกิจกรรมโครงการที่ต้องการ");
                    }

                    var Repersons = context.ResponsiblePersons
                    // เช็คว่าดึงข้อมูลจากตรงไหนบ้าง
                        .Where(c => c.PlanActivityId == PlanActivityId && c.PlanCoreId == PlanCoreId)
                        .Select(c => new
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Active = c.Active,
                            FiscalYear = c.FiscalYear,
                            StaffId = c.StaffId,
                            PlanPersonResponsibilityEnum = c.PlanPersonResponsibilityEnum,
                            PlanCoreId = c.PlanCoreId,
                            PlanActivityId = c.PlanActivityId,
                            HRDepartmentId = c.HRDepartmentId,
                            HRDepartmentName = c.HRDepartmentName,
                            PhoneNumber = c.PhoneNumber
                        })
                        .ToList();

                    if (!Repersons.Any())
                    {
                        return Results.NotFound("ไม่พบข้อมูลบุคคลากร");
                    }

                    return Results.Ok(Repersons);
                }
            })
            .WithTags("ResponsiblePreson")
            .WithGroupName("SEC_ResponsiblePreson");
        }
    }

    record ResponsiblePresonRequest_Update(string Name, bool Active, int FiscalYear, int StaffId, int PlanPersonResponsibilityEnum, int? PlanCoreId,
    int? HRDepartmentId, string HRDepartmentName, string PhoneNumber);

    record ResponsiblePresonRequest_Create(string Name, bool Active, int FiscalYear, int StaffId, int PlanPersonResponsibilityEnum,
    int? HRDepartmentId, string HRDepartmentName, string PhoneNumber);
}
