using System.Net.Http;
using System.Security.Claims;
using System.Text.Json;
using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanActivities_Endpoints
    {
        public static void MapSEC_PlanActivities_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_Create/{PlanCoreId}", [AllowAnonymous] (HttpContext httpContext, int PlanCoreId, PlanActivities_Create request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var Chack_PlanCoreId = context.PlanActivities.FirstOrDefault(c => c.PlanCoreId == PlanCoreId);
                    if (Chack_PlanCoreId == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการนี้" });
                    }

                    var newPlanActivities = new PlanActivity
                    {
                        PlanCoreId = PlanCoreId,
                        Name = request.Name,
                        Active = request.Active,
                        FiscalYear = request.FiscalYear,
                        Code = request.Code ?? "",
                        Detail = request.Detail ?? "",
                        IsFollowUp = request.IsFollowUp,
                        DepartmentId = request.DepartmentId,
                        FundCategoryEnum = request.FundCategoryEnum,
                        Weight = request.Weight,
                        OtherFundSourceName = request.OtherFundSourceName ?? "",
                        OperationPeriod = request.OperationPeriod ?? "",
                        CreateDate = DateTime.Now,
                    };

                    context.PlanActivities.Add(newPlanActivities);
                    try { if (context.SaveChanges() <= 0) return Results.BadRequest("ไม่สามารถสร้างข้อมูลได้"); }
                    catch (DbUpdateException ex) { return Results.BadRequest("ไม่สามารถสร้างข้อมูลได้: " + ex.InnerException?.Message); }
                    context.SaveChanges();

                    var user = httpContext.User;

                    PlanActivityActionLog pActivityActionLog = new PlanActivityActionLog
                    {
                        ActionTypeEnum = 20,
                        Name = new PlanActivityActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = newPlanActivities.Active,
                        StaffId = newPlanActivities.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanActivityId = newPlanActivities.Id
                    };

                    context.PlanActivityActionLogs.Add(pActivityActionLog);
                    context.SaveChanges();


                    return Results.Json(new { Message = "สร้างข้อมูลกิจกรรมโครงการเสร็จสิ้น", newPlanActivities },
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });
                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");

            app.MapPut("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_Update/{id}", [AllowAnonymous] (HttpContext httpContext, int Id, PlanActivities_update request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var existingPlanActivity = context.PlanActivities.FirstOrDefault(c => c.Id == Id);

                    if (existingPlanActivity == null)
                    {
                        return Results.NotFound("ไม่พบกิจกรรมโครงการที่ต้องการอัปเดต");
                    }

                    existingPlanActivity.PlanCoreId = request.PlanCoreId;
                    existingPlanActivity.Name = request.planActivities_Create.Name;
                    existingPlanActivity.Active = request.planActivities_Create.Active;
                    existingPlanActivity.FiscalYear = request.planActivities_Create.FiscalYear;
                    existingPlanActivity.Code = request.planActivities_Create.Code ?? "";
                    existingPlanActivity.Detail = request.planActivities_Create.Detail ?? "";
                    existingPlanActivity.IsFollowUp = request.planActivities_Create.IsFollowUp;
                    existingPlanActivity.DepartmentId = request.planActivities_Create.DepartmentId;
                    existingPlanActivity.FundCategoryEnum = request.planActivities_Create.FundCategoryEnum;
                    existingPlanActivity.Weight = request.planActivities_Create.Weight;

                    try { if (context.SaveChanges() <= 0) return Results.BadRequest("ไม่สามารถแก้ไขข้อมูลได้"); }
                    catch (DbUpdateException ex) { return Results.BadRequest("ไม่สามารถแก้ไขข้อมูลได้: " + ex.InnerException?.Message); }
                    context.SaveChanges();

                    var user = httpContext.User;

                    PlanActivityActionLog pActivityActionLog = new PlanActivityActionLog
                    {
                        ActionTypeEnum = 20,
                        Name = new PlanActivityActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = existingPlanActivity.Active,
                        StaffId = existingPlanActivity.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanActivityId = existingPlanActivity.Id
                    };

                    context.PlanActivityActionLogs.Add(pActivityActionLog);
                    context.SaveChanges();

                    return Results.Json(new { Message = "อัปเดตกิจกรรมโครงการเสร็จสิ้น", existingPlanActivity },
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });
                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");

            app.MapDelete("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_Delete/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    PlanActivityService planActivityService = new PlanActivityService(context);

                    var PlanActivitie = context.PlanActivities.FirstOrDefault(c => c.Id == id);
                    if (PlanActivitie == null)
                    {
                        return Results.NotFound("ไม่พบกิจกรรมโครงการที่ต้องการลบ");
                    }

                    planActivityService.Delete(PlanActivitie);

                    context.SaveChanges();

                    return Results.Ok("ลบกิจกรรมโครงการเสร็จสิ้น");
                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");

            app.MapGet("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_GetData/{PlanCoreId}", [AllowAnonymous] (HttpContext httpContext, int PlanCoreId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var PlanActivitie = context.PlanActivities.FirstOrDefault(c => c.PlanCoreId == PlanCoreId);
                    if (PlanActivitie == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการที่ต้องการ");
                    }

                    PlanActivityService planActivityService = new PlanActivityService(context);

                    var Planactv = planActivityService.DbSet()
                        .Where(c => c.PlanCoreId == PlanCoreId)
                        .Select(c => new
                        {
                            Id = c.Id,
                            PlanCoreId = c.PlanCoreId,
                            Name = c.Name,
                            Active = c.Active,
                            FiscalYear = c.FiscalYear,
                            Code = c.Code,
                            Detail = c.Detail,
                            IsFollowUp = c.IsFollowUp,
                            DepartmentId = c.DepartmentId,
                            FundCategoryEnum = c.FundCategoryEnum,
                            Weight = c.Weight,
                        })
                        .ToList();

                    if (!Planactv.Any())
                    {
                        return Results.NotFound("ไม่พบข้อมูลกิจกรรในโครงการ");
                    }

                    // ดึงข้อมูลผู้ใช้งานจาก Token
                    var user = httpContext.User;
                    string staffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ";

                    // บันทึกการเข้าถึงข้อมูลลงใน Log
                    PlanActivityActionLog pActivityActionLog = new PlanActivityActionLog
                    {
                        ActionTypeEnum = 5, // ประเภทของการกระทำ (ดูข้อมูล)
                        Name = new PlanActivityActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = PlanActivitie.Active, // ใช้ PlanItem ที่หาเจอ
                        StaffId = PlanActivitie.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = staffName, // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanActivityId = PlanActivitie.Id
                    };

                    context.PlanActivityActionLogs.Add(pActivityActionLog);
                    context.SaveChanges();

                    return Results.Ok(Planactv);

                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");
        }

        record PlanActivities_Create(string Name, bool Active, int FiscalYear, string Code, string Detail, bool IsFollowUp,
        int DepartmentId, int FundCategoryEnum, decimal Weight, string OtherFundSourceName, string OperationPeriod);

        record PlanActivities_update(int PlanCoreId, PlanActivities_Create planActivities_Create);
    }
}
