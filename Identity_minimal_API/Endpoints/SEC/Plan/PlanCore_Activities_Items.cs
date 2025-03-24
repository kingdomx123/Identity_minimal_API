using System.Text.Json.Serialization;
using System.Text.Json;
using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanCore_Activities_Items_Endpoints
    {
        public static void MapSEC_PlanCore_Activities_Items_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Unified/Plan/Create", [AllowAnonymous] (HttpContext httpContext, UnifiedRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var user = httpContext.User;

                    // ✅ เช็คว่า User ได้เข้าสู่ระบบหรือไม่
                    if (!user.Identity?.IsAuthenticated ?? false)
                    {
                        return Results.Unauthorized();
                    }

                    // ✅ ดึงค่า Claims ที่กำหนดไว้ใน JWT
                    var isPlanPowerUser = user.Claims.FirstOrDefault(c => c.Type == "IsPlanDepPowerUser")?.Value == "true";
                    //var isHRPowerUser = user.Claims.FirstOrDefault(c => c.Type == "IsHRDepPowerUser")?.Value == "true";
                    //var isAdmin = user.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "true";

                    // ✅ กำหนดเงื่อนไขการเข้าถึง
                    if (!isPlanPowerUser)
                    {
                        return Results.Forbid();
                    }

                    var newPlanCore = new PlanCore
                    {
                        Name = request.PlanCore.Name ?? "",
                        FiscalYear = request.PlanCore.FiscalYear,
                        Code = request.PlanCore.Code ?? "",
                        Active = request.PlanCore.Active,
                        DepartmentId = request.PlanCore.DepartmentId,
                        PlanTypeId = request.PlanCore.PlanTypeId,
                        Detail = request.PlanCore.Detail ?? "",
                        Objective = request.PlanCore.Objective ?? "",
                        Benefit = request.PlanCore.Benefit ?? "",
                        PlanCategoryEnum = request.PlanCore.PlanCategoryEnum,
                        CreateByStaffId = request.PlanCore.CreateByStaffId,
                        IsApproved = request.PlanCore.IsApproved,
                        CodeNumber = request.PlanCore.CodeNumber,
                        ProjectDuration = request.PlanCore.ProjectDuration ?? null,
                        MonthStart = request.PlanCore.MonthStart ?? null,
                        MonthEnd = request.PlanCore.MonthEnd ?? null,
                        OtherTarget = request.PlanCore.OtherTarget ?? "",
                        OpFormDocNumber = request.PlanCore.OpFormDocNumber ?? "",
                        OpFormDepName = request.PlanCore.OpFormDepName ?? "",
                        OpFormDepTel = request.PlanCore.OpFormDepTel ?? "",
                        OpFormLocation = request.PlanCore.OpFormLocation ?? "",
                        OpFormRequester = request.PlanCore.OpFormRequester ?? "",
                        OpFormRequesterPosition = request.PlanCore.OpFormRequesterPosition ?? "",
                        OpFormWriteDate = request.PlanCore.OpFormWriteDate ?? null,
                        Output = request.PlanCore.Output ?? "",
                        OpFormInform = request.PlanCore.OpFormInform ?? "",
                        IsControlByPlanCoreCode = request.PlanCore.IsControlByPlanCoreCode,
                        IsSent = request.PlanCore.IsSent,
                        DateStart = request.PlanCore.DateStart ?? null,
                        DateEnd = request.PlanCore.DateEnd ?? null,
                        OpFormRequesterStaffId = request.PlanCore.OpFormRequesterStaffId ?? null,
                        OpFormRequestDepartmentId = request.PlanCore.OpFormRequestDepartmentId ?? null,
                        DepStrategy = request.PlanCore.DepStrategy ?? "",
                        DepStrategyIndicator = request.PlanCore.DepStrategyIndicator ?? "",
                        DepPerformanceIndicator = request.PlanCore.DepPerformanceIndicator ?? "",
                        TotalYearlyBudget = request.PlanCore.TotalYearlyBudget ?? null,
                        FundTypeId = request.PlanCore.FundTypeId ?? null,
                        TargetIdListValue = request.PlanCore.TargetIdListValue ?? "",
                        CreateDate = DateTime.UtcNow
                    };

                    context.PlanCores.Add(newPlanCore);
                    context.SaveChanges();

                    PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog
                    {
                        ActionTypeEnum = 20, // ประเภทของการกระทำ (พิมพ์/ดูเอกสาร)
                        Name = new PlanCoreActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = newPlanCore.Active,
                        StaffId = newPlanCore.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanCoreId = newPlanCore.Id
                    };

                    // บันทึกข้อมูลลงฐานข้อมูล
                    context.PlanCoreActionLogs.Add(pCoreActionLog);
                    context.SaveChanges();


                    foreach (var activity in request.PlanActivitiesRe)
                    {
                        var newPlanActivity = new PlanActivity
                        {
                            PlanCoreId = newPlanCore.Id,  // Now this has a valid Id
                            Name = activity.Name,
                            Active = activity.Active,
                            FiscalYear = activity.FiscalYear,
                            Code = activity.Code ?? "",
                            Detail = activity.Detail ?? "",
                            IsFollowUp = activity.IsFollowUp,
                            DepartmentId = activity.DepartmentId,
                            FundCategoryEnum = activity.FundCategoryEnum,
                            Weight = activity.Weight,
                            OtherFundSourceName = activity.OtherFundSourceName ?? "",
                            OperationPeriod = activity.OperationPeriod ?? "",
                            CreateDate = DateTime.UtcNow
                        };


                        context.PlanActivities.Add(newPlanActivity);
                        context.SaveChanges();

                        PlanActivityActionLog pActivityActionLog = new PlanActivityActionLog
                        {
                            ActionTypeEnum = 20,
                            Name = new PlanActivityActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                            Active = newPlanActivity.Active,
                            StaffId = newPlanActivity.CreateByStaffId,
                            ActionDate = DateTime.UtcNow,
                            Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                            HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                            StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                            ClientName = "",
                            PlanActivityId = newPlanActivity.Id
                        };

                        context.PlanActivityActionLogs.Add(pActivityActionLog);
                        context.SaveChanges();

                        foreach (var item in activity.PlanItems)
                        {
                            var newPlanItem = new PlanItem
                            {
                                PlanActivityId = newPlanActivity.Id,
                                Name = item.Name ?? "",
                                Active = item.Active,
                                FiscalYear = item.FiscalYear,
                                Unit = item.Unit ?? "",
                                BudgetTypeId = item.BudgetTypeId,
                                UndefineReserveByStaffName = item.UndefineReserveByStaffName ?? "",
                                UndefineReserveRemark = item.UndefineReserveRemark ?? "",
                                UndefineReserveForecastValue = item.UndefineReserveForecastValue ?? "",
                                Remark = item.Remark ?? "",
                                ProtectBudget = item.ProtectBudget,
                                FundCategoryEnum = item.FundCategoryEnum,
                                FundSourceEnum = item.FundSourceEnum,
                                AXCode = item.AXCode ?? "",
                                CreateDate = DateTime.UtcNow
                            };
                            context.PlanItems.Add(newPlanItem);
                            context.SaveChanges();

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
                        }

                    }

                    context.SaveChanges();

                    // ดึงข้อมูลที่เพิ่งสร้างขึ้นมา
                    var planCore = context.PlanCores
                        .AsNoTracking()
                        .Include(p => p.PlanActivities)
                            .ThenInclude(a => a.PlanItems)
                        .FirstOrDefault(p => p.Id == newPlanCore.Id);

                    if (planCore == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการ" });
                    }

                    // ใช้ JsonSerializerOptions เพื่อหลีกเลี่ยง Object Cycle
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true
                    };

                    return Results.Json(new
                    {
                        Message = "สร้างข้อมูลโครงการสำเร็จ",
                        PlanCore = planCore
                    }, options);
                }
            })
            .WithTags("SEC")
            .WithGroupName("SEC");

            app.MapPut("/Endpoint/SEC/Unified/Plan/Update/{id}", [AllowAnonymous] (HttpContext httpContext, int id, UnifiedRequest_update request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var user = httpContext.User;

                    // ✅ เช็คว่า User ได้เข้าสู่ระบบหรือไม่
                    if (!user.Identity?.IsAuthenticated ?? false)
                    {
                        return Results.Unauthorized();
                    }

                    // ✅ ดึงค่า Claims ที่กำหนดไว้ใน JWT
                    var isPlanPowerUser = user.Claims.FirstOrDefault(c => c.Type == "IsPlanDepPowerUser")?.Value == "true";
                    //var isHRPowerUser = user.Claims.FirstOrDefault(c => c.Type == "IsHRDepPowerUser")?.Value == "true";
                    //var isAdmin = user.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "true";

                    // ✅ กำหนดเงื่อนไขการเข้าถึง
                    if (!isPlanPowerUser)
                    {
                        return Results.Forbid();
                    }

                    var existingPlanCore = context.PlanCores
                        .Include(p => p.PlanActivities)
                            .ThenInclude(a => a.PlanItems)
                        .FirstOrDefault(p => p.Id == id);

                    if (existingPlanCore == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการ" });
                    }

                    // อัปเดตข้อมูล PlanCore
                    existingPlanCore.Name = request.PlanCore.Name ?? "";
                    existingPlanCore.FiscalYear = request.PlanCore.FiscalYear;
                    existingPlanCore.Code = request.PlanCore.Code ?? "";
                    existingPlanCore.Active = request.PlanCore.Active;
                    existingPlanCore.DepartmentId = request.PlanCore.DepartmentId;
                    existingPlanCore.PlanTypeId = request.PlanCore.PlanTypeId;
                    existingPlanCore.Detail = request.PlanCore.Detail ?? "";
                    existingPlanCore.Objective = request.PlanCore.Objective ?? "";
                    existingPlanCore.Benefit = request.PlanCore.Benefit ?? "";
                    existingPlanCore.PlanCategoryEnum = request.PlanCore.PlanCategoryEnum;
                    existingPlanCore.CreateByStaffId = request.PlanCore.CreateByStaffId;
                    existingPlanCore.IsApproved = request.PlanCore.IsApproved;
                    existingPlanCore.CodeNumber = request.PlanCore.CodeNumber;
                    existingPlanCore.ProjectDuration = request.PlanCore.ProjectDuration;
                    existingPlanCore.MonthStart = request.PlanCore.MonthStart;
                    existingPlanCore.MonthEnd = request.PlanCore.MonthEnd;
                    existingPlanCore.OtherTarget = request.PlanCore.OtherTarget ?? "";
                    existingPlanCore.OpFormDocNumber = request.PlanCore.OpFormDocNumber ?? "";
                    existingPlanCore.OpFormDepName = request.PlanCore.OpFormDepName ?? "";
                    existingPlanCore.OpFormDepTel = request.PlanCore.OpFormDepTel ?? "";
                    existingPlanCore.OpFormLocation = request.PlanCore.OpFormLocation ?? "";
                    existingPlanCore.OpFormRequester = request.PlanCore.OpFormRequester ?? "";
                    existingPlanCore.OpFormRequesterPosition = request.PlanCore.OpFormRequesterPosition ?? "";
                    existingPlanCore.OpFormWriteDate = request.PlanCore.OpFormWriteDate;
                    existingPlanCore.Output = request.PlanCore.Output ?? "";
                    existingPlanCore.OpFormInform = request.PlanCore.OpFormInform ?? "";
                    existingPlanCore.IsControlByPlanCoreCode = request.PlanCore.IsControlByPlanCoreCode;
                    existingPlanCore.IsSent = request.PlanCore.IsSent;
                    existingPlanCore.DateStart = request.PlanCore.DateStart;
                    existingPlanCore.DateEnd = request.PlanCore.DateEnd;
                    existingPlanCore.OpFormRequesterStaffId = request.PlanCore.OpFormRequesterStaffId;
                    existingPlanCore.OpFormRequestDepartmentId = request.PlanCore.OpFormRequestDepartmentId;
                    existingPlanCore.DepStrategy = request.PlanCore.DepStrategy ?? "";
                    existingPlanCore.DepStrategyIndicator = request.PlanCore.DepStrategyIndicator ?? "";
                    existingPlanCore.DepPerformanceIndicator = request.PlanCore.DepPerformanceIndicator ?? "";
                    existingPlanCore.TotalYearlyBudget = request.PlanCore.TotalYearlyBudget;
                    existingPlanCore.FundTypeId = request.PlanCore.FundTypeId;
                    existingPlanCore.TargetIdListValue = request.PlanCore.TargetIdListValue ?? "";

                    PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog()
                    {
                        ActionTypeEnum = 20,
                        Name = new PlanActivityActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = existingPlanCore.Active,
                        StaffId = existingPlanCore.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanCoreId = existingPlanCore.Id
                    };

                    context.PlanCoreActionLogs.Add(pCoreActionLog);
                    context.SaveChanges();

                    var existingPlanActivities = context.PlanActivities
                    .Where(a => a.PlanCoreId == existingPlanCore.Id)
                    .ToList();

                    foreach (var activity in request.PlanActivitiesRe)
                    {
                        var existingPlanActivity = context.PlanActivities
                        .FirstOrDefault(a => a.Id == activity.Id);


                        if (existingPlanActivity != null)
                        {
                            // อัปเดตข้อมูล
                            existingPlanActivity.Name = activity.Name;
                            existingPlanActivity.Active = activity.Active;
                            existingPlanActivity.FiscalYear = activity.FiscalYear;
                            existingPlanActivity.Code = activity.Code ?? "";
                            existingPlanActivity.Detail = activity.Detail ?? "";
                            existingPlanActivity.IsFollowUp = activity.IsFollowUp;
                            existingPlanActivity.DepartmentId = activity.DepartmentId;
                            existingPlanActivity.FundCategoryEnum = activity.FundCategoryEnum;
                            existingPlanActivity.Weight = activity.Weight;
                            existingPlanActivity.OtherFundSourceName = activity.OtherFundSourceName ?? "";
                            existingPlanActivity.OperationPeriod = activity.OperationPeriod ?? "";
                        }
                        else
                        {
                            // ถ้าไม่มี ให้สร้างใหม่
                            existingPlanActivity = new PlanActivity
                            {
                                PlanCoreId = existingPlanCore.Id,
                                Name = activity.Name,
                                Active = activity.Active,
                                FiscalYear = activity.FiscalYear,
                                Code = activity.Code ?? "",
                                Detail = activity.Detail ?? "",
                                IsFollowUp = activity.IsFollowUp,
                                DepartmentId = activity.DepartmentId,
                                FundCategoryEnum = activity.FundCategoryEnum,
                                Weight = activity.Weight,
                                OtherFundSourceName = activity.OtherFundSourceName ?? "",
                                OperationPeriod = activity.OperationPeriod ?? "",
                                CreateDate = DateTime.UtcNow
                            };
                            context.PlanActivities.Add(existingPlanActivity);
                        }
                        context.SaveChanges();

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

                        // 🔄 อัปเดต PlanItems ที่อยู่ใน Activity นี้
                        var existingPlanItems = context.PlanItems
                            .Where(i => i.PlanActivityId == existingPlanActivity.Id)
                            .ToList();

                        foreach (var item in activity.PlanItems)
                        {
                            var existingPlanItem = existingPlanItems
                                .FirstOrDefault(i => i.Id == item.Id); // ค้นหาตัวที่ต้องการอัปเดต

                            if (existingPlanItem != null)
                            {
                                // อัปเดตข้อมูล
                                existingPlanItem.Name = item.Name ?? "";
                                existingPlanItem.Active = item.Active;
                                existingPlanItem.FiscalYear = item.FiscalYear;
                                existingPlanItem.Unit = item.Unit ?? "";
                                existingPlanItem.BudgetTypeId = item.BudgetTypeId;
                                existingPlanItem.UndefineReserveByStaffName = item.UndefineReserveByStaffName ?? "";
                                existingPlanItem.UndefineReserveRemark = item.UndefineReserveRemark ?? "";
                                existingPlanItem.UndefineReserveForecastValue = item.UndefineReserveForecastValue ?? "";
                                existingPlanItem.Remark = item.Remark ?? "";
                                existingPlanItem.ProtectBudget = item.ProtectBudget;
                                existingPlanItem.FundCategoryEnum = item.FundCategoryEnum;
                                existingPlanItem.FundSourceEnum = item.FundSourceEnum;
                                existingPlanItem.AXCode = item.AXCode ?? "";
                            }
                            else
                            {
                                // ถ้าไม่มี ให้สร้างใหม่
                                existingPlanItem = new PlanItem
                                {
                                    PlanActivityId = existingPlanActivity.Id,
                                    Name = item.Name ?? "",
                                    Active = item.Active,
                                    FiscalYear = item.FiscalYear,
                                    Unit = item.Unit ?? "",
                                    BudgetTypeId = item.BudgetTypeId,
                                    UndefineReserveByStaffName = item.UndefineReserveByStaffName ?? "",
                                    UndefineReserveRemark = item.UndefineReserveRemark ?? "",
                                    UndefineReserveForecastValue = item.UndefineReserveForecastValue ?? "",
                                    Remark = item.Remark ?? "",
                                    ProtectBudget = item.ProtectBudget,
                                    FundCategoryEnum = item.FundCategoryEnum,
                                    FundSourceEnum = item.FundSourceEnum,
                                    AXCode = item.AXCode ?? "",
                                    CreateDate = DateTime.UtcNow
                                };
                                context.PlanItems.Add(existingPlanItem);
                            }
                            context.SaveChanges();

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
                        }
                    }
                    context.SaveChanges();

                    // ดึงข้อมูลที่อัปเดตแล้ว
                    var updatedPlanCore = context.PlanCores
                        .AsNoTracking()
                        .Include(p => p.PlanActivities)
                            .ThenInclude(a => a.PlanItems)
                        .FirstOrDefault(p => p.Id == id);

                    // ใช้ JsonSerializerOptions เพื่อหลีกเลี่ยง Object Cycle
                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true
                    };

                    return Results.Json(new
                    {
                        Message = "อัปเดตข้อมูลโครงการสำเร็จ",
                        PlanCore = updatedPlanCore
                    }, options);
                }
            })
            .WithTags("SEC")
            .WithGroupName("SEC");

            app.MapGet("/Endpoint/SEC/Unified/Plan/Get_DTO/{fiscalYear}/{departmentId}", [AllowAnonymous] async (HttpContext httpContext, int fiscalYear, int departmentId) =>
            {
                try
                {
                    using (PlanDbContext context = new PlanDbContext(connectionString))
                    {
                        var user = httpContext.User;
                        var listNumbersClaim = user.FindFirst("ListNumbers")?.Value;

                        if (string.IsNullOrEmpty(listNumbersClaim))
                        {
                            return Results.Forbid(); // ไม่มีสิทธิ์เข้าถึง
                        }

                        // Deserialize JSON เป็น List<Tuple<int, List<int>>>
                        var allowedFiscalYearsAndDepartments = JsonSerializer.Deserialize<List<Tuple<int, List<int>>>>(listNumbersClaim);

                        // ตรวจสอบว่าผู้ใช้สามารถเข้าถึงปีงบประมาณและหน่วยงานที่ร้องขอได้หรือไม่
                        bool isAuthorized = allowedFiscalYearsAndDepartments!.Any(tuple =>
                            tuple.Item1 == fiscalYear && tuple.Item2.Contains(departmentId));

                        if (!isAuthorized)
                        {
                            return Results.Forbid(); // ไม่มีสิทธิ์เข้าถึง
                        }

                        if (context.PlanCores == null)
                        {
                            return Results.Problem("ไม่สามารถเข้าถึงตารางฐานข้อมูลโครงการได้");
                        }

                        var plancores = await context.PlanCores
                            .AsNoTracking()
                            .Where(c => c.FiscalYear == fiscalYear && c.DepartmentId == departmentId)
                            .Include(p => p.PlanActivities)
                                .ThenInclude(a => a.PlanItems)
                                .ThenInclude(i => i.SummaryStatementCaches)
                            .ToListAsync();

                        if (!plancores.Any())
                        {
                            return Results.NotFound(new { Message = "ไม่พบปีงบประมาณและหน่วยงานที่ต้องการ" });
                        }

                        var result = plancores.Select(p => new
                        {
                            p.IsDeletable,
                            p.TotalBudgetCache,
                            p.NetBudgetCache,
                            p.UsedBudgetCache,
                            p.Id,
                            p.FiscalYear,
                            p.DepartmentId,
                            p.Name,
                            p.Code,
                            p.Detail,
                            p.Objective,
                            p.Benefit,
                            p.PlanTypeId,
                            p.PlanCategoryEnum,
                            p.IsApproved,
                            p.ProjectDuration,
                            p.MonthStart,
                            p.MonthEnd,
                            p.OtherTarget,
                            p.OpFormDocNumber,
                            p.OpFormDepName,
                            p.OpFormDepTel,
                            p.OpFormLocation,
                            p.OpFormRequester,
                            p.OpFormRequesterPosition,
                            p.OpFormWriteDate,
                            p.Output,
                            p.OpFormInform,
                            p.IsControlByPlanCoreCode,
                            p.IsSent,
                            p.DateStart,
                            p.DateEnd,
                            p.OpFormRequesterStaffId,
                            p.OpFormRequestDepartmentId,
                            p.DepStrategy,
                            p.DepStrategyIndicator,
                            p.DepPerformanceIndicator,
                            p.TotalYearlyBudget,
                            p.FundTypeId,
                            p.TargetIdListValue,
                            p.CreateByStaffId,

                            PlanActivities = p.PlanActivities.Select(a => new
                            {
                                a.Id,
                                a.Name,
                                a.Code,
                                a.Active,
                                a.Detail,
                                a.IsFollowUp,
                                a.DepartmentId,
                                a.FundCategoryEnum,
                                a.Weight,
                                a.OtherFundSourceName,
                                a.OperationPeriod,
                                a.CreateByStaffId,

                                PlanItems = a.PlanItems.Select(i => new
                                {
                                    i.Id,
                                    i.Name,
                                    i.Active,
                                    i.FiscalYear,
                                    i.Unit,
                                    i.BudgetTypeId,
                                    i.UndefineReserveByStaffName,
                                    i.UndefineReserveRemark,
                                    i.UndefineReserveForecastValue,
                                    i.Remark,
                                    i.ProtectBudget,
                                    i.FundCategoryEnum,
                                    i.FundSourceEnum,
                                    i.AXCode,
                                    i.CreateByStaffId
                                }).ToList()
                            }).ToList()
                        }).ToList();

                        var firstPlanCore = plancores.FirstOrDefault();
                        var firstPlanActivity = firstPlanCore?.PlanActivities.FirstOrDefault(); // ดึงกิจกรรมตัวแรก
                        var firstPlanItems = firstPlanActivity?.PlanItems.FirstOrDefault(); // ดึงกิจกรรมตัวแรก

                        PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog()
                        {
                            ActionTypeEnum = 5,
                            Name = new PlanActivityActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                            Active = firstPlanCore?.Active ?? false,
                            StaffId = firstPlanCore?.CreateByStaffId ?? 0,
                            ActionDate = DateTime.UtcNow,
                            Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                            HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                            StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                            ClientName = "",
                            PlanCoreId = firstPlanCore?.Id ?? 0
                        };

                        context.PlanCoreActionLogs.Add(pCoreActionLog);
                        context.SaveChanges();

                        PlanActivityActionLog pActivityActionLog = new PlanActivityActionLog
                        {
                            ActionTypeEnum = 5, // ประเภทของการกระทำ (ดูข้อมูล)
                            Name = new PlanActivityActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                            Active = firstPlanActivity?.Active ?? false,
                            StaffId = firstPlanActivity?.CreateByStaffId ?? 0,
                            ActionDate = DateTime.UtcNow,
                            Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                            HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                            StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                            ClientName = "",
                            PlanActivityId = firstPlanActivity?.Id ?? 0
                        };

                        context.PlanActivityActionLogs.Add(pActivityActionLog);
                        context.SaveChanges();

                        PlanItemActionLog pItemAction = new PlanItemActionLog
                        {
                            ActionTypeEnum = 5, // ประเภทของการกระทำ (ดูข้อมูล)
                            Name = new PlanItemActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                            Active = firstPlanItems?.Active ?? false, // ใช้ PlanItem ที่หาเจอ
                            StaffId = firstPlanItems?.CreateByStaffId ?? 0,
                            ActionDate = DateTime.UtcNow,
                            Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                            HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                            StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                            ClientName = "",
                            PlanItemId = firstPlanItems?.Id ?? 0
                        };

                        context.PlanItemActionLogs.Add(pItemAction);
                        context.SaveChanges();

                        return Results.Json(new
                        {
                            Message = "ดึงข้อมูลโครงการสำเร็จ",
                            Plans = result,
                            ActionLog1 = pCoreActionLog,
                            ActionLog2 = pActivityActionLog,
                            ActionLog3 = pItemAction,
                        });
                    }
                }
                catch (Exception ex)
                {
                    return Results.Problem(
                        title: "เกิดข้อผิดพลาดในการดึงข้อมูลโครงการ",
                        detail: ex.Message,
                        statusCode: StatusCodes.Status500InternalServerError
                    );
                }
            })
            .WithTags("SEC")
            .WithGroupName("SEC");

            //ยังแก้ไม่ได้
            //app.MapGet("/Endpoint/SEC/Unified/Plan/Get/{fiscalYear}/{departmentId}", [AllowAnonymous] (int fiscalYear, int departmentId) =>
            //{
            //    using (PlanDbContext context = new PlanDbContext(connectionString))
            //    {
            //        if (context.PlanCores == null)
            //        {
            //            return Results.Problem("ไม่สามารถเข้าถึงตารางฐานข้อมูลโครงการได้");
            //        }

            //        PlanCoreService plancoreService = new PlanCoreService(context);
            //        var plancores = plancoreService.DbSet()
            //        .AsNoTracking()
            //        .Include(p => p.PlanActivities)
            //            .ThenInclude(a => a.PlanItems)
            //        //.ThenInclude(i => i.SummaryStatementCaches)
            //        .Where(c => c.FiscalYear == fiscalYear && c.DepartmentId == departmentId)
            //        .ToList();


            //        if (!plancores.Any())
            //        {
            //            return Results.NotFound(new { Message = "ไม่พบปีงบประมาณและหน่วยงานที่ต้องการ" });
            //        }

            //        return Results.Json(new
            //        {
            //            Message = "ดึงข้อมูลโครงการสำเร็จ",
            //            plancores
            //        }, new JsonSerializerOptions
            //        {
            //            ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            //            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            //            WriteIndented = true
            //        });
            //    }
            //})
            //.WithTags("SEC")
            //.WithGroupName("SEC");

            app.MapDelete("/Unified/Plan/Delete/{id}", [AllowAnonymous] (HttpContext httpContext, int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var user = httpContext.User;

                    // ✅ เช็คว่า User ได้เข้าสู่ระบบหรือไม่
                    if (!user.Identity?.IsAuthenticated ?? false)
                    {
                        return Results.Unauthorized();
                    }

                    // ✅ ดึงค่า Claims ที่กำหนดไว้ใน JWT
                    var isPlanPowerUser = user.Claims.FirstOrDefault(c => c.Type == "IsPlanDepPowerUser")?.Value == "true";
                    //var isHRPowerUser = user.Claims.FirstOrDefault(c => c.Type == "IsHRDepPowerUser")?.Value == "true";
                    //var isAdmin = user.Claims.FirstOrDefault(c => c.Type == "IsAdmin")?.Value == "true";

                    // ✅ กำหนดเงื่อนไขการเข้าถึง
                    if (!isPlanPowerUser)
                    {
                        return Results.Forbid();
                    }

                    var PlanItemService = new PlanItemService(context);
                    var PlanActivityService = new PlanActivityService(context);
                    var PlanCoreService = new PlanCoreService(context);

                    var PlanCore = context.PlanCores.FirstOrDefault(p => p.Id == id);
                    if (PlanCore == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการที่ต้องการลบ");
                    }

                    // ดึงรายการ PlanActivity ที่เกี่ยวข้องกับ PlanCore
                    var PlanActivities = context.PlanActivities.Where(a => a.PlanCoreId == id).ToList();

                    foreach (var activity in PlanActivities)
                    {
                        // ดึงรายการ PlanItem ที่เกี่ยวข้องกับ PlanActivity
                        var PlanItems = context.PlanItems.Where(i => i.PlanActivityId == activity.Id).ToList();

                        // ลบ PlanItem ทั้งหมดที่เกี่ยวข้องโดยใช้ Service
                        foreach (var item in PlanItems)
                        {
                            PlanItemService.Delete(item);
                        }

                        // ลบ PlanActivity โดยใช้ Service
                        PlanActivityService.Delete(activity);
                    }

                    // ลบ PlanCore โดยใช้ Service
                    PlanCoreService.Delete(PlanCore);

                    // บันทึกการเปลี่ยนแปลง
                    context.SaveChanges();

                    return Results.Ok("ลบข้อมูลสำเร็จ");
                }
            })
            .WithTags("SEC")
            .WithGroupName("SEC");

        }

        record UnifiedRequest(PlanCoreRequest PlanCore, List<PlanActivityRequest> PlanActivitiesRe);

        record UnifiedRequest_update(PlanCoreRequest PlanCore, List<PlanActivityRequest_update> PlanActivitiesRe);

        record PlanCoreRequest(string Name, int FiscalYear, string Code, bool Active, int DepartmentId, int PlanTypeId, string Detail, string Objective, string Benefit,
        int PlanCategoryEnum, int CreateByStaffId, bool IsApproved, int CodeNumber, int? ProjectDuration, int? MonthStart, int? MonthEnd, string OtherTarget,
        string OpFormDocNumber, string OpFormDepName, string OpFormDepTel, string OpFormLocation, string OpFormRequester, string OpFormRequesterPosition, DateTime? OpFormWriteDate,
        string Output, string OpFormInform, bool IsControlByPlanCoreCode, bool IsSent, DateTime? DateStart, DateTime? DateEnd, int? OpFormRequesterStaffId, int? OpFormRequestDepartmentId,
        string DepStrategy, string DepStrategyIndicator, string DepPerformanceIndicator, decimal? TotalYearlyBudget, int? FundTypeId, string TargetIdListValue);

        record PlanActivityRequest(string Name, bool Active, int FiscalYear, string Code, string Detail, bool IsFollowUp,
        int DepartmentId, int FundCategoryEnum, decimal Weight, string OtherFundSourceName, string OperationPeriod, int CreateByStaffId, List<PlanItemRequest> PlanItems);

        record PlanItemRequest(string Name, bool Active, int FiscalYear, string? Unit, int? BudgetTypeId, string UndefineReserveByStaffName,
        string UndefineReserveRemark, string UndefineReserveForecastValue, string Remark, decimal ProtectBudget,
        int FundCategoryEnum, int FundSourceEnum, string AXCode, int CreateByStaffId);

        record PlanActivityRequest_update(int Id, string Name, bool Active, int FiscalYear, string Code, string Detail, bool IsFollowUp,
        int DepartmentId, int FundCategoryEnum, decimal Weight, string OtherFundSourceName, string OperationPeriod, int CreateByStaffId, List<PlanItemRequest_update> PlanItems);

        record PlanItemRequest_update(int Id, string Name, bool Active, int FiscalYear, string? Unit, int? BudgetTypeId, string UndefineReserveByStaffName,
        string UndefineReserveRemark, string UndefineReserveForecastValue, string Remark, decimal ProtectBudget,
        int FundCategoryEnum, int FundSourceEnum, string AXCode, int CreateByStaffId);
    }
}
