using System.Text.Json;
using System.Security.Claims;
using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanCores_Endpoints
    {
        public static void MapSEC_PlanCores_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/PlanCore/CreatePlanCore", (HttpContext httpContext, PlanCoreRequestMain request) =>
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

                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    if (string.IsNullOrWhiteSpace(request.Name) || request.FiscalYear <= 0)
                        return Results.BadRequest("กรุณากรอกข้อมูลให้ครบถ้วน");

                    var newPlancore = new PlanCore
                    {
                        Name = request.Name,
                        DateStart = request.DateStart ?? null,
                        DateEnd = request.DateEnd ?? null,
                        FiscalYear = request.FiscalYear,
                        Active = request.Active,
                        DepartmentId = request.DepartmentId,
                        OpFormRequestDepartmentId = request.OpFormRequestDepartmentId,
                        PlanTypeId = request.PlanTypeId,
                        Code = request.Code ?? "",
                        Detail = request.Detail ?? "",
                        Objective = request.Objective ?? "",
                        Benefit = request.Benefit ?? "",
                        OtherTarget = request.OtherTarget ?? "",
                        OpFormDocNumber = request.OpFormDocNumber ?? "",
                        OpFormDepName = request.OpFormDepName ?? "",
                        OpFormDepTel = request.OpFormDepTel ?? "",
                        OpFormLocation = request.OpFormLocation ?? "",
                        OpFormRequester = request.OpFormRequester ?? "",
                        OpFormRequesterPosition = request.OpFormRequesterPosition ?? "",
                        Output = request.Output ?? "",
                        OpFormInform = request.OpFormInform ?? "",
                        DepStrategy = request.DepStrategy ?? "",
                        DepStrategyIndicator = request.DepStrategyIndicator ?? "",
                        DepPerformanceIndicator = request.DepPerformanceIndicator ?? "",
                        PlanCategoryEnum = request.PlanCategoryEnum,
                        CreateByStaffId = request.CreateByStaffId,
                        IsApproved = request.IsApproved,
                        CodeNumber = request.CodeNumber,
                        IsControlByPlanCoreCode = request.IsControlByPlanCoreCode,
                        IsSent = request.IsSent,
                        ProjectDuration = request.ProjectDuration,
                        MonthStart = request.MonthStart,
                        MonthEnd = request.MonthEnd,
                        OpFormWriteDate = request.OpFormWriteDate ?? null,
                        OpFormRequesterStaffId = request.OpFormRequesterStaffId,
                        TotalYearlyBudget = request.TotalYearlyBudget,
                        FundTypeId = request.FundTypeId,
                        TargetIdListValue = request.TargetIdListValue ?? "",
                        CreateDate = DateTime.UtcNow,
                    };

                    context.PlanCores.Add(newPlancore);
                    try { if (context.SaveChanges() <= 0) return Results.BadRequest("ไม่สามารถสร้างข้อมูลได้"); }
                    catch (DbUpdateException ex) { return Results.BadRequest("ไม่สามารถสร้างข้อมูลได้: " + ex.InnerException?.Message); }
                    context.SaveChanges();

                    // สร้างและกำหนดค่าให้ PlanCoreActionLog ทีละบรรทัด
                    PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog
                    {
                        ActionTypeEnum = 20, // ประเภทของการกระทำ (พิมพ์/ดูเอกสาร)
                        Name = new PlanCoreActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = newPlancore.Active,
                        StaffId = newPlancore.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanCoreId = newPlancore.Id
                    };

                    // บันทึกข้อมูลลงฐานข้อมูล
                    context.PlanCoreActionLogs.Add(pCoreActionLog);
                    context.SaveChanges();

                    return Results.Json(new { Message = "สร้างข้อมูลโครงสร้างเสร็จสิ้น", newPlancore },
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });
                    }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapPut("/Endpoint/SEC/Plan/PlanCore/UpdatePlanCore/{id}", [AllowAnonymous] (HttpContext httpContext, int id, PlanCoreRequestMain request) =>
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

                    var existingPlan = context.PlanCores.FirstOrDefault(c => c.Id == id);
                    if (existingPlan == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการที่ต้องการแก้ไข");
                    }

                    existingPlan.Name = request.Name;
                    existingPlan.DateStart = request.DateStart;
                    existingPlan.DateEnd = request.DateEnd;
                    existingPlan.FiscalYear = request.FiscalYear;
                    existingPlan.Active = request.Active;
                    existingPlan.DepartmentId = request.DepartmentId;
                    existingPlan.OpFormRequestDepartmentId = request.OpFormRequestDepartmentId;
                    existingPlan.PlanTypeId = request.PlanTypeId;
                    existingPlan.Code = request.Code ?? "";
                    existingPlan.Detail = request.Detail ?? "";
                    existingPlan.Objective = request.Objective ?? "";
                    existingPlan.Benefit = request.Benefit ?? "";
                    existingPlan.OtherTarget = request.OtherTarget ?? "";
                    existingPlan.OpFormDocNumber = request.OpFormDocNumber ?? "";
                    existingPlan.OpFormDepName = request.OpFormDepName ?? "";
                    existingPlan.OpFormDepTel = request.OpFormDepTel ?? "";
                    existingPlan.OpFormLocation = request.OpFormLocation ?? "";
                    existingPlan.OpFormRequester = request.OpFormRequester ?? "";
                    existingPlan.OpFormRequesterPosition = request.OpFormRequesterPosition ?? "";
                    existingPlan.Output = request.Output ?? "";
                    existingPlan.OpFormInform = request.OpFormInform ?? "";
                    existingPlan.DepStrategy = request.DepStrategy ?? "";
                    existingPlan.DepStrategyIndicator = request.DepStrategyIndicator ?? "";
                    existingPlan.DepPerformanceIndicator = request.DepPerformanceIndicator ?? "";
                    existingPlan.PlanCategoryEnum = request.PlanCategoryEnum;
                    existingPlan.CreateByStaffId = request.CreateByStaffId;
                    existingPlan.IsApproved = request.IsApproved;
                    existingPlan.CodeNumber = request.CodeNumber;
                    existingPlan.IsControlByPlanCoreCode = request.IsControlByPlanCoreCode;
                    existingPlan.IsSent = request.IsSent;
                    existingPlan.ProjectDuration = request.ProjectDuration;
                    existingPlan.MonthStart = request.MonthStart;
                    existingPlan.MonthEnd = request.MonthEnd;
                    existingPlan.OpFormWriteDate = request.OpFormWriteDate;
                    existingPlan.OpFormRequesterStaffId = request.OpFormRequesterStaffId;
                    existingPlan.TotalYearlyBudget = request.TotalYearlyBudget;
                    existingPlan.FundTypeId = request.FundTypeId;

                    try { if (context.SaveChanges() <= 0) return Results.BadRequest("ไม่สามารถแก้ไขข้อมูลได้"); }
                    catch (DbUpdateException ex) { return Results.BadRequest("ไม่สามารถแก้ไขข้อมูลได้: " + ex.InnerException?.Message); }
                    context.SaveChanges();

                    PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog()
                    {
                        ActionTypeEnum = 20,
                        Name = new PlanActivityActionLog { ActionTypeEnum = 20 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = existingPlan.Active,
                        StaffId = existingPlan.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanCoreId = existingPlan.Id
                    };

                    context.PlanCoreActionLogs.Add(pCoreActionLog);
                    context.SaveChanges();


                    return Results.Ok(new { Message = "อัปเดตข้อมูลโครงการสำเร็จ", existingPlan });
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapDelete("/Endpoint/SEC/Plan/PlanCore/DeletePlanCore/{id}", [AllowAnonymous] (HttpContext httpContext, int id) =>
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

                    PlanCoreService plancoreService = new PlanCoreService(context);

                    var plancores = context.PlanCores.FirstOrDefault(c => c.Id == id);
                    if (plancores == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการที่ต้องการลบ");
                    }

                    plancoreService.Delete(plancores);

                    context.SaveChanges();


                    return Results.Json(new { Message = "ลบข้อมูลโครงการเสร็จสิ้น" },
                    new JsonSerializerOptions
                    {
                        ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
                        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
                        WriteIndented = true
                    });
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapGet("/Endpoint/SEC/Plan/PlanCore/ShowPlanCoresd/{FiscalYear}/{DepartmentId}", (HttpContext httpContext, int FiscalYear, int DepartmentId) =>
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
                        tuple.Item1 == FiscalYear && tuple.Item2.Contains(DepartmentId));

                    if (!isAuthorized)
                    {
                        return Results.Forbid(); // ไม่มีสิทธิ์เข้าถึง
                    }

                    var plancore = context.PlanCores.FirstOrDefault(c => c.FiscalYear == FiscalYear && c.DepartmentId == DepartmentId);
                    if (plancore == null)
                    {
                        return Results.NotFound("ไม่พบปีงบประมาณและหน่วยงานที่ต้องการ");
                    }

                    PlanCoreService plancoreService = new PlanCoreService(context);
                    var plancores = plancoreService.DbSet()
                        .Where(c => c.FiscalYear == FiscalYear && c.DepartmentId == DepartmentId)
                        .Select(c => new
                        {
                            Id = c.Id,
                            Name = c.Name,
                            FiscalYear = c.FiscalYear,
                            Code = c.Code,
                            Active = c.Active,
                            DepartmentId = c.DepartmentId,
                            PlanTypeId = c.PlanTypeId,
                            Detail = c.Detail,
                            Objective = c.Objective,
                            Benefit = c.Benefit,
                            PlanCategoryEnum = c.PlanCategoryEnum,
                            CreateDate = c.CreateDate,
                            CreateByStaffId = c.CreateByStaffId,
                            IsApproved = c.IsApproved,
                            CodeNumber = c.CodeNumber,
                            ProjectDuration = c.ProjectDuration,
                            MonthStart = c.MonthStart,
                            MonthEnd = c.MonthEnd,
                            OtherTarget = c.OtherTarget,
                            OpFormDocNumber = c.OpFormDocNumber,
                            OpFormDepName = c.OpFormDepName,
                            OpFormDepTel = c.OpFormDepTel,
                            OpFormLocation = c.OpFormLocation,
                            OpFormRequester = c.OpFormRequester,
                            OpFormRequesterPosition = c.OpFormRequesterPosition,
                            OpFormWriteDate = c.OpFormWriteDate,
                            Output = c.Output,
                            OpFormInform = c.OpFormInform,
                            IsControlByPlanCoreCode = c.IsControlByPlanCoreCode,
                            IsSent = c.IsSent,
                            DateStart = c.DateStart,
                            DateEnd = c.DateEnd,
                            OpFormRequesterStaffId = c.OpFormRequesterStaffId,
                            OpFormRequestDepartmentId = c.OpFormRequestDepartmentId,
                            DepStrategy = c.DepStrategy,
                            DepStrategyIndicator = c.DepStrategyIndicator,
                            DepPerformanceIndicator = c.DepPerformanceIndicator,
                        })
                        .Take(100)
                        .ToList();

                    if (!plancores.Any())
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการ");
                    }

                    //var user = httpContext.User;

                    PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog()
                    {
                        ActionTypeEnum = 5,
                        Name = new PlanActivityActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = plancore.Active,
                        StaffId = plancore.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanCoreId = plancore.Id
                    };

                    context.PlanCoreActionLogs.Add(pCoreActionLog);
                    context.SaveChanges();

                    return Results.Ok(plancores);
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapGet("/Endpoint/SEC/Plan/PlanCore/ShowPlanCore/{FiscalYear}/{DepartmentId}", [Authorize(Policy = "PlanCoreAccess")] (HttpContext httpContext, int FiscalYear, int DepartmentId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var plancore = context.PlanCores.FirstOrDefault(c => c.FiscalYear == FiscalYear && c.DepartmentId == DepartmentId);
                    if (plancore == null)
                    {
                        return Results.NotFound("ไม่พบปีงบประมาณและหน่วยงานที่ต้องการ");
                    }

                    PlanCoreService plancoreService = new PlanCoreService(context);
                    var plancores = plancoreService.DbSet()
                        .Where(c => c.FiscalYear == FiscalYear && c.DepartmentId == DepartmentId)
                        .Select(c => new
                        {
                            Id = c.Id,
                            Name = c.Name,
                            FiscalYear = c.FiscalYear,
                            Code = c.Code,
                            Active = c.Active,
                            DepartmentId = c.DepartmentId,
                            PlanTypeId = c.PlanTypeId,
                            Detail = c.Detail,
                            Objective = c.Objective,
                            Benefit = c.Benefit,
                            PlanCategoryEnum = c.PlanCategoryEnum,
                            CreateDate = c.CreateDate,
                            CreateByStaffId = c.CreateByStaffId,
                            IsApproved = c.IsApproved,
                            CodeNumber = c.CodeNumber,
                            ProjectDuration = c.ProjectDuration,
                            MonthStart = c.MonthStart,
                            MonthEnd = c.MonthEnd,
                            OtherTarget = c.OtherTarget,
                            OpFormDocNumber = c.OpFormDocNumber,
                            OpFormDepName = c.OpFormDepName,
                            OpFormDepTel = c.OpFormDepTel,
                            OpFormLocation = c.OpFormLocation,
                            OpFormRequester = c.OpFormRequester,
                            OpFormRequesterPosition = c.OpFormRequesterPosition,
                            OpFormWriteDate = c.OpFormWriteDate,
                            Output = c.Output,
                            OpFormInform = c.OpFormInform,
                            IsControlByPlanCoreCode = c.IsControlByPlanCoreCode,
                            IsSent = c.IsSent,
                            DateStart = c.DateStart,
                            DateEnd = c.DateEnd,
                            OpFormRequesterStaffId = c.OpFormRequesterStaffId,
                            OpFormRequestDepartmentId = c.OpFormRequestDepartmentId,
                            DepStrategy = c.DepStrategy,
                            DepStrategyIndicator = c.DepStrategyIndicator,
                            DepPerformanceIndicator = c.DepPerformanceIndicator,
                        })
                        .Take(100)
                        .ToList();

                    if (!plancores.Any())
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการ");
                    }

                    var user = httpContext.User;

                    PlanCoreActionLog pCoreActionLog = new PlanCoreActionLog()
                    {
                        ActionTypeEnum = 5,
                        Name = new PlanActivityActionLog { ActionTypeEnum = 5 }.ActionTypeName, // ดึงค่า ActionTypeName
                        Active = plancore.Active,
                        StaffId = plancore.CreateByStaffId,
                        ActionDate = DateTime.UtcNow,
                        Ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "ไม่พบ",
                        HostName = httpContext.Request.Headers["Host"].ToString() ?? "ไม่พบ",
                        StaffName = user.FindFirst(ClaimTypes.Name)?.Value ?? "ไม่พบ", // ดึงชื่อจาก JWT Token
                        ClientName = "",
                        PlanCoreId = plancore.Id
                    };

                    context.PlanCoreActionLogs.Add(pCoreActionLog);
                    context.SaveChanges();

                    return Results.Ok(plancores);
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

        }
    }

    record PlanCoreRequestMain(string Name, int FiscalYear, string Code, bool Active, int DepartmentId, int PlanTypeId, string Detail, string Objective, string Benefit,
    int PlanCategoryEnum, int CreateByStaffId, bool IsApproved, int CodeNumber, int? ProjectDuration, int? MonthStart, int? MonthEnd, string OtherTarget,
    string OpFormDocNumber, string OpFormDepName, string OpFormDepTel, string OpFormLocation, string OpFormRequester, string OpFormRequesterPosition, DateTime? OpFormWriteDate,
    string Output, string OpFormInform, bool IsControlByPlanCoreCode, bool IsSent, DateTime? DateStart, DateTime? DateEnd, int? OpFormRequesterStaffId, int? OpFormRequestDepartmentId,
    string DepStrategy, string DepStrategyIndicator, string DepPerformanceIndicator, decimal? TotalYearlyBudget, int? FundTypeId, string TargetIdListValue);

}
