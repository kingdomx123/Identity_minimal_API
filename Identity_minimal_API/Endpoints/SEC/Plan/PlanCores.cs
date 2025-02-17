using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanCores_Endpoints
    {
        public static void MapSEC_PlanCores_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/PlanCore/CreatePlanCore", [AllowAnonymous] (PlanCoreRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
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
                    if (context.Entry(newPlancore).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "สร้างข้อมูลโครงสร้างเสร็จสิ้น", newPlancore });
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapPut("/Endpoint/SEC/Plan/PlanCore/UpdatePlanCore/{id}", [AllowAnonymous] (int id, PlanCoreRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var existingPlan = context.PlanCores.Find(id);
                    if (existingPlan == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการที่ต้องการแก้ไข" });
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

                    if (context.Entry(existingPlan).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "อัปเดตข้อมูลโครงการสำเร็จ", existingPlan});
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapDelete("/Endpoint/SEC/Plan/PlanCore/DeletePlanCore/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var plancores = context.PlanCores.Find(id);
                    if (plancores == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการที่ต้องการลบ" });
                    }

                    context.PlanCores.Remove(plancores);
                    if (context.Entry(plancores).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "ลบข้อมูลโครงการเสร็จสิ้น"});
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");

            app.MapGet("/Endpoint/SEC/Plan/PlanCore/ShowPlanCore", [AllowAnonymous] () =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    PlanCoreService plancoreService = new PlanCoreService(context);
                    var plancores = plancoreService.DbSet()
                        .Select(c => new
                        {
                            Id = c.Id,
                            Name = c.Name,
                            DepartmentId = c.DepartmentId,
                            OpFormRequestDepartmentId = c.OpFormRequestDepartmentId,
                            PlanTypeId = c.PlanTypeId,
                            DateStart = c.DateStart,
                            DateEnd = c.DateEnd, 
                            CreateDate = c.CreateDate,
                            TotalYearlyBudget = c.TotalYearlyBudget,
                            FundTypeId = c.FundTypeId,
                        })
                        .Take(100)
                        .ToList();  // ดึงรายการแรกสุด (Id มากที่สุด)

                    if (plancores == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการ");
                    }

                    return Results.Ok(plancores);
                }
            })
            .WithTags("PlanCore")
            .WithGroupName("SEC_PlanCore");
        }
    }

    record PlanCoreRequest(string Name, int FiscalYear, string Code, bool Active, int DepartmentId, int PlanTypeId, string Detail, string Objective, string Benefit,
    int PlanCategoryEnum, int CreateByStaffId, bool IsApproved, int CodeNumber, int? ProjectDuration, int? MonthStart, int? MonthEnd, string OtherTarget,
    string OpFormDocNumber, string OpFormDepName, string OpFormDepTel, string OpFormLocation, string OpFormRequester, string OpFormRequesterPosition, DateTime? OpFormWriteDate,
    string Output, string OpFormInform, bool IsControlByPlanCoreCode, bool IsSent, DateTime? DateStart, DateTime? DateEnd, int? OpFormRequesterStaffId, int? OpFormRequestDepartmentId,
    string DepStrategy, string DepStrategyIndicator, string DepPerformanceIndicator, decimal? TotalYearlyBudget, int? FundTypeId, string TargetIdListValue);

}
