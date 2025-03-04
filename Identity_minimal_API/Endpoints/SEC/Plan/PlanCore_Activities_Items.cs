using System.Text.Json.Serialization;
using System.Text.Json;
using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanCore_Activities_Items_Endpoints
    {
        public static void MapSEC_PlanCore_Activities_Items_Endpoints(this WebApplication app, string connectionString)
        {
            //app.MapPost("/Unified/Plan/Create_ALL", [AllowAnonymous] (PlanCore request) =>
            //{
            //    using (var context = new PlanDbContext(connectionString))
            //    {
            //        // บันทึก PlanCore
            //        request.CreateDate = DateTime.UtcNow;
            //        context.PlanCores.Add(request);
            //        context.SaveChanges();

            //        // ดึงข้อมูลกลับมาเพื่อแสดงผล (พร้อม PlanActivities & PlanItems)
            //        var planCore = context.PlanCores
            //            .AsNoTracking()
            //            .Include(p => p.PlanActivities)
            //                .ThenInclude(a => a.PlanItems)
            //            .FirstOrDefault(p => p.Id == request.Id);

            //        if (planCore == null)
            //        {
            //            return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการ" });
            //        }

            //        // ป้องกัน Object Cycle
            //        var options = new JsonSerializerOptions
            //        {
            //            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            //            WriteIndented = true
            //        };

            //        return Results.Json(new
            //        {
            //            Message = "สร้างข้อมูลโครงการสำเร็จ",
            //            PlanCore = planCore
            //        }, options);
            //    }
            //})
            //.WithTags("SEC")
            //.WithGroupName("SEC");

            app.MapPost("/Endpoint/SEC/Unified/Plan/Create", [AllowAnonymous] (UnifiedRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
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

                    // Insert PlanCore FIRST
                    context.PlanCores.Add(newPlanCore);
                    context.SaveChanges();  // Now newPlanCore.Id is available


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

            app.MapPut("/Endpoint/SEC/Unified/Plan/Update/{id}", [AllowAnonymous] (int id, UnifiedRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
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

                    // ลบกิจกรรมเดิมและเพิ่มใหม่
                    //context.PlanActivities.RemoveRange(existingPlanCore.PlanActivities);

                    foreach (var activity in request.PlanActivitiesRe)
                    {
                        var newPlanActivity = new PlanActivity
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

                        context.PlanActivities.Add(newPlanActivity);
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

            app.MapGet("/Endpoint/SEC/Unified/Plan/Get_DTO/{fiscalYear}/{departmentId}", [AllowAnonymous] async (int fiscalYear, int departmentId) =>
            {
                try
                {
                    using (PlanDbContext context = new PlanDbContext(connectionString))
                    {
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
                            p.RemainBudgetCache,
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
                                    i.AXCode
                                }).ToList()
                            }).ToList()
                        }).ToList();

                        return Results.Json(new
                        {
                            Message = "ดึงข้อมูลโครงการสำเร็จ",
                            Plans = result
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

            app.MapGet("/Endpoint/SEC/Unified/Plan/Get/{fiscalYear}/{departmentId}", [AllowAnonymous] (int fiscalYear, int departmentId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    if (context.PlanCores == null)
                    {
                        return Results.Problem("ไม่สามารถเข้าถึงตารางฐานข้อมูลโครงการได้");
                    }

                    PlanCoreService plancoreService = new PlanCoreService(context);
                    var plancores = plancoreService.DbSet()
                    .AsNoTracking()
                    .Include(p => p.PlanActivities)
                        .ThenInclude(a => a.PlanItems)
                    //.ThenInclude(i => i.SummaryStatementCaches)
                    .Where(c => c.FiscalYear == fiscalYear && c.DepartmentId == departmentId)
                    .ToList();

                    //var plancores = context.PlanCores
                    //.AsNoTracking()
                    //.Where(c => c.FiscalYear == fiscalYear && c.DepartmentId == departmentId)
                    //.Include(p => p.PlanActivities)
                    //    .ThenInclude(a => a.PlanItems) // ดึง PlanItems
                    //    .ThenInclude(i => i.BudgetType) // ดึง BudgetType ใน PlanItems
                    //.Include(p => p.PlanActivities)
                    //    .ThenInclude(a => a.PlanItems) // ดึง PlanItems
                    //    .ThenInclude(i => i.BudgetType) // ดึง BudgetType ใน PlanItems
                    //.ThenInclude(i => i.SummaryStatementCaches)
                    //.ToList();  // โหลดข้อมูลทั้งหมด



                    if (!plancores.Any())
                    {
                        return Results.NotFound(new { Message = "ไม่พบปีงบประมาณและหน่วยงานที่ต้องการ" });
                    }

                    var options = new JsonSerializerOptions
                    {
                        ReferenceHandler = ReferenceHandler.IgnoreCycles,
                        WriteIndented = true
                    };

                    return Results.Json(new
                    {
                        Message = "ดึงข้อมูลโครงการสำเร็จ",
                        Plans = plancores
                    }, options);
                }
            })
            .WithTags("SEC")
            .WithGroupName("SEC");

            app.MapDelete("/Unified/Plan/Delete/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
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

        record PlanCoreRequest(string Name, int FiscalYear, string Code, bool Active, int DepartmentId, int PlanTypeId, string Detail, string Objective, string Benefit,
        int PlanCategoryEnum, int CreateByStaffId, bool IsApproved, int CodeNumber, int? ProjectDuration, int? MonthStart, int? MonthEnd, string OtherTarget,
        string OpFormDocNumber, string OpFormDepName, string OpFormDepTel, string OpFormLocation, string OpFormRequester, string OpFormRequesterPosition, DateTime? OpFormWriteDate,
        string Output, string OpFormInform, bool IsControlByPlanCoreCode, bool IsSent, DateTime? DateStart, DateTime? DateEnd, int? OpFormRequesterStaffId, int? OpFormRequestDepartmentId,
        string DepStrategy, string DepStrategyIndicator, string DepPerformanceIndicator, decimal? TotalYearlyBudget, int? FundTypeId, string TargetIdListValue);

        record PlanActivityRequest(string Name, bool Active, int FiscalYear, string Code, string Detail, bool IsFollowUp,
        int DepartmentId, int FundCategoryEnum, decimal Weight, string OtherFundSourceName, string OperationPeriod, List<PlanItemRequest> PlanItems);

        record PlanItemRequest(string Name, bool Active, int FiscalYear, string? Unit, int? BudgetTypeId, string UndefineReserveByStaffName,
        string UndefineReserveRemark, string UndefineReserveForecastValue, string Remark, decimal ProtectBudget,
        int FundCategoryEnum, int FundSourceEnum, string AXCode);
    }
}
