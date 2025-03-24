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
    public static class SEC_PlanItems_Endpoints
    {
        public static void MapSEC_PlanItems_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/PlanItems/PlanItems_Create/{PlanActivityId}", [AllowAnonymous] (HttpContext httpContext, int PlanActivityId, PlanItem_Create request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var check_PlanItems = context.PlanItems.FirstOrDefault(c => c.PlanActivityId == PlanActivityId);
                    if (check_PlanItems == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลกิจกรรมโครงการนี้");
                    }

                    var newPlanItem = new PlanItem
                    {
                        Name = request.Name,
                        Active = request.Active,
                        FiscalYear = request.FiscalYear,
                        PlanActivityId = PlanActivityId,
                        Unit = request.Unit ?? "",
                        BudgetTypeId = request.BudgetTypeId,
                        UndefineReserveByStaffName = request.UndefineReserveByStaffName ?? "",
                        UndefineReserveRemark = request.UndefineReserveRemark ?? "",
                        UndefineReserveForecastValue = request.UndefineReserveForecastValue ?? "",
                        Remark = request.Remark ?? "",
                        ProtectBudget = request.ProtectBudget,
                        FundCategoryEnum = request.FundCategoryEnum,
                        FundSourceEnum = request.FundSourceEnum,
                        AXCode = request.AXCode ?? "",
                        CreateDate = DateTime.Now,
                        CreateByStaffId = request.CreateByStaffId,
                    };

                    context.PlanItems.Add(newPlanItem);
                    try { if (context.SaveChanges() <= 0) return Results.BadRequest("ไม่สามารถสร้างข้อมูลได้"); }
                    catch (DbUpdateException ex) { return Results.BadRequest("ไม่สามารถสร้างข้อมูลได้: " + ex.InnerException?.Message); }
                    context.SaveChanges();

                    var user = httpContext.User;

                    PlanItemActionLog pItemAction = new PlanItemActionLog
                    {
                        ActionTypeEnum = 20,
                        Name = new PlanItemActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = newPlanItem.Active,
                        StaffId = newPlanItem.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ",
                        ClientName = "",
                        PlanItemId = newPlanItem.Id
                    };
                    context.PlanItemActionLogs.Add(pItemAction);
                    context.SaveChanges();


                    return Results.Json(new { Message = "สร้างข้อมูลรายการย่อยเสร็จสิ้น", newPlanItem },
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });
                }
            })
            .WithTags("PlanItems")
            .WithGroupName("SEC_PlanItem");

            app.MapPut("/Endpoint/SEC/Plan/PlanItems/PlanItems_Update/{id}", [AllowAnonymous] (HttpContext httpContext, int Id, PlanItem_Update request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var existingPlanItem = context.PlanItems.FirstOrDefault(c => c.Id == Id);

                    if (existingPlanItem == null)
                    {
                        return Results.NotFound("ไม่พบรายการย่อยที่ต้องการอัปเดต");
                    }

                    existingPlanItem.Name = request.Name;
                    existingPlanItem.Active = request.Active;
                    existingPlanItem.FiscalYear = request.FiscalYear;
                    existingPlanItem.PlanActivityId = request.PlanActivityId;
                    existingPlanItem.Unit = request.Unit;
                    existingPlanItem.BudgetTypeId = request.BudgetTypeId;
                    existingPlanItem.UndefineReserveByStaffName = request.UndefineReserveByStaffName;
                    existingPlanItem.UndefineReserveRemark = request.UndefineReserveRemark;
                    existingPlanItem.UndefineReserveForecastValue = request.UndefineReserveForecastValue;
                    existingPlanItem.Remark = request.Remark;
                    existingPlanItem.ProtectBudget = request.ProtectBudget;
                    existingPlanItem.FundCategoryEnum = request.FundCategoryEnum;
                    existingPlanItem.FundSourceEnum = request.FundSourceEnum;
                    existingPlanItem.AXCode = request.AXCode;

                    try { if (context.SaveChanges() <= 0) return Results.BadRequest("ไม่สามารถแก้ไขข้อมูลได้"); }
                    catch (DbUpdateException ex) { return Results.BadRequest("ไม่สามารถแก้ไขข้อมูลได้: " + ex.InnerException?.Message); }
                    context.SaveChanges();

                    var user = httpContext.User;

                    PlanItemActionLog pItemAction = new PlanItemActionLog
                    {
                        ActionTypeEnum = 20,
                        Name = new PlanItemActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = existingPlanItem.Active,
                        StaffId = existingPlanItem.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanItemId = existingPlanItem.Id
                    };
                    context.PlanItemActionLogs.Add(pItemAction);
                    context.SaveChanges();


                    return Results.Json(new { Message = "อัปเดตรายการย่อยเสร็จสิ้น", existingPlanItem },
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });
                }
            })
            .WithTags("PlanItems")
            .WithGroupName("SEC_PlanItem");

            app.MapDelete("/Endpoint/SEC/Plan/PlanItems/PlanItems_Delete/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    PlanItemService PlanItemService = new PlanItemService(context);

                    var PlanItem = context.PlanItems.FirstOrDefault(c => c.Id == id);
                    if (PlanItem == null)
                    {
                        return Results.NotFound("ไม่พบรายการย่อยที่ต้องการลบ");
                    }

                    PlanItemService.Delete(PlanItem);
                    context.SaveChanges();


                    return Results.Ok("ลบรายการย่อยเสร็จสิ้น");
                }
            })
            .WithTags("PlanItems")
            .WithGroupName("SEC_PlanItem");

            app.MapGet("/Endpoint/SEC/Plan/PlanItems/PlanItems_GetData/{PlanActivityId}", [AllowAnonymous] (HttpContext httpContext, int PlanActivityId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    // ดึงข้อมูล PlanItem ตาม PlanActivityId
                    var PlanItem = context.PlanItems.FirstOrDefault(c => c.PlanActivityId == PlanActivityId);
                    if (PlanItem == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลกิจกรรมโครงการที่ต้องการ");
                    }

                    // ดึงรายการ PlanItems ทั้งหมด
                    PlanItemService PlanItemService = new PlanItemService(context);
                    var PlanItems = PlanItemService.DbSet()
                        .Where(c => c.PlanActivityId == PlanActivityId)
                        .Select(c => new
                        {
                            Id = c.Id,
                            Name = c.Name,
                            Active = c.Active,
                            FiscalYear = c.FiscalYear,
                            PlanActivityId = c.PlanActivityId,
                            Unit = c.Unit,
                            BudgetTypeId = c.BudgetTypeId,
                            Remark = c.Remark,
                            ProtectBudget = c.ProtectBudget,
                            FundCategoryEnum = c.FundCategoryEnum,
                            FundSourceEnum = c.FundSourceEnum,
                            AXCode = c.AXCode,
                            CreateByStaffId = c.CreateByStaffId,
                        })
                        .ToList();

                    // ตรวจสอบว่ามีรายการย่อยหรือไม่
                    if (!PlanItems.Any())
                    {
                        return Results.NotFound("ไม่พบข้อมูลรายการย่อย");
                    }

                    // ดึงข้อมูลผู้ใช้งานจาก Token
                    var user = httpContext.User;

                    // บันทึกการเข้าถึงข้อมูลลงใน Log
                    PlanItemActionLog pItemAction = new PlanItemActionLog
                    {
                        ActionTypeEnum = 5, // ประเภทของการกระทำ (ดูข้อมูล)
                        Name = new PlanItemActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = PlanItem.Active, // ใช้ PlanItem ที่หาเจอ
                        StaffId = PlanItem.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanItemId = PlanItem.Id
                    };

                    context.PlanItemActionLogs.Add(pItemAction);
                    context.SaveChanges();

                    return Results.Ok(PlanItems);
                }
            })
            .WithTags("PlanItems")
            .WithGroupName("SEC_PlanItem");

        }

        record PlanItem_Create(string Name, bool Active, int FiscalYear, string? Unit, int? BudgetTypeId, string UndefineReserveByStaffName,
        string UndefineReserveRemark, string UndefineReserveForecastValue, string Remark, decimal ProtectBudget,
        int FundCategoryEnum, int FundSourceEnum, string AXCode, int CreateByStaffId);

        record PlanItem_Update(string Name, bool Active, int FiscalYear, int PlanActivityId, string? Unit, int? BudgetTypeId,
        string UndefineReserveByStaffName, string UndefineReserveRemark, string UndefineReserveForecastValue, string Remark, decimal ProtectBudget,
        int FundCategoryEnum, int FundSourceEnum, string AXCode, int CreateByStaffId);
    }
}
