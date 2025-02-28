using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_StrategicIndicator_Endpoints
    {
        public static void MapSEC_StrategicIndicator_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/StrategicIndicator/StrategicIndicator_Create", [AllowAnonymous] (StrategicIndicatorRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var newStrategicIndicator = new StrategicIndicator
                    {
                        Name = request.Name,
                        FiscalYear = request.FiscalYear,
                        Active = request.Active,
                        ParentStrategicIndicatorId = request.ParentStrategicIndicatorId,
                        Unit = request.Unit,
                        Amount = request.Amount,
                        Weight = request.Weight,
                        IsLowerBetter = request.IsLowerBetter,
                        IsPercentageValue = request.IsPercentageValue,
                        IsSuperKpi = request.IsSuperKpi,
                        BscperspectiveTypeEnum = request.BscperspectiveTypeEnum,
                    };

                    context.StrategicIndicators.Add(newStrategicIndicator);

                    context.SaveChanges();


                    return Results.Ok(new { Message = "สร้างข้อมูลตัวชี้วัดตามประเด็นยุทธศาสตร์เสร็จสิ้น", newStrategicIndicator });
                }
            })
            .WithTags("StrategicIndicators")
            .WithGroupName("SEC_StrategicIndicator");

            app.MapPut("/Endpoint/SEC/Plan/StrategicIndicator/StrategicIndicator_Update/{id}", [AllowAnonymous] (int id, StrategicIndicatorRequest request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var updateStrategicIndicator = context.StrategicIndicators.FirstOrDefault(c => c.Id == id);

                    if (updateStrategicIndicator == null)
                    {
                        return Results.NotFound("ไม่พบตัวชี้วัดตามประเด็นยุทธศาสตร์ที่ต้องการอัปเดต");
                    }

                    updateStrategicIndicator.Name = request.Name;
                    updateStrategicIndicator.FiscalYear = request.FiscalYear;
                    updateStrategicIndicator.Active = request.Active;
                    updateStrategicIndicator.ParentStrategicIndicatorId = request.ParentStrategicIndicatorId;
                    updateStrategicIndicator.Unit = request.Unit;
                    updateStrategicIndicator.Amount = request.Amount;
                    updateStrategicIndicator.Weight = request.Weight;
                    updateStrategicIndicator.IsLowerBetter = request.IsLowerBetter;
                    updateStrategicIndicator.IsPercentageValue = request.IsPercentageValue;
                    updateStrategicIndicator.IsSuperKpi = request.IsSuperKpi;
                    updateStrategicIndicator.BscperspectiveTypeEnum = request.BscperspectiveTypeEnum;


                    context.SaveChanges();


                    return Results.Ok(new { Message = "อัปเดตตัวชี้วัดตามประเด็นยุทธศาสตร์เสร็จสิ้น", updateStrategicIndicator });
                }
            })
            .WithTags("StrategicIndicators")
            .WithGroupName("SEC_StrategicIndicator");

            app.MapDelete("/Endpoint/SEC/Plan/StrategicIndicator/StrategicIndicator_Delete/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    StrategicIndicatorService strategicIndicatorService = new StrategicIndicatorService(context);

                    var StrategicIndicators = context.StrategicIndicators.FirstOrDefault(c => c.Id == id);
                    if (StrategicIndicators == null)
                    {
                        return Results.NotFound("ไม่พบกิจกรรมโครงการที่ต้องการลบ");
                    }

                    strategicIndicatorService.Delete(StrategicIndicators);

                    context.SaveChanges();


                    return Results.Ok("ลบตัวชี้วัดตามประเด็นยุทธศาสตร์เสร็จสิ้น");
                }

            })
            .WithTags("StrategicIndicators")
            .WithGroupName("SEC_StrategicIndicator");

            app.MapGet("/Endpoint/SEC/Plan/StrategicIndicator/StrategicIndicator_GetData/{FiscalYear}", [AllowAnonymous] (int FiscalYear) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var PlanStrategicIndicators = context.StrategicIndicators.FirstOrDefault(c => c.FiscalYear == FiscalYear);
                    if (PlanStrategicIndicators == null)
                    {
                        return Results.NotFound("ไม่พบตัวชี้วัดตามประเด็นยุทธศาสตร์ที่ต้องการ");
                    }

                    StrategicIndicatorService strategicIndicatorService = new StrategicIndicatorService(context);

                    // ดึงข้อมูลทุกฟิลด์ของ StrategicIndicator
                    var PlanStrs = strategicIndicatorService.DbSet()
                        .AsNoTracking() // ป้องกัน EF Core ติดตามข้อมูลที่เชื่อมไปยังตารางอื่น
                        .Where(c => c.FiscalYear == FiscalYear)
                        .ToList();

                    if (!PlanStrs.Any())
                    {
                        return Results.NotFound("ไม่พบตัวชี้วัดตามประเด็นยุทธศาสตร์");
                    }

                    return Results.Ok(PlanStrs);
                }
            })
            .WithTags("StrategicIndicators")
            .WithGroupName("SEC_StrategicIndicator");

        }
        record StrategicIndicatorRequest(string Name, int FiscalYear, bool Active, int? ParentStrategicIndicatorId, string? Unit, decimal Amount, decimal Weight,
        bool IsLowerBetter, bool IsPercentageValue, bool IsSuperKpi, int BscperspectiveTypeEnum);
    }
}
