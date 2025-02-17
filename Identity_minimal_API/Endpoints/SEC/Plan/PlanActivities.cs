using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanActivities_Endpoints
    {
        public static void MapSEC_PlanActivities_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_Create/{PlanCoreId}", [AllowAnonymous] (int PlanCoreId, PlanActivities_Create request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var Chack_PlanActivitie_PlanCoreId = context.PlanActivities.Find(PlanCoreId);
                    if (Chack_PlanActivitie_PlanCoreId == null)
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
                    if (context.Entry(newPlanActivities).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "สร้างข้อมูลกิจกรรมโครงการเสร็จสิ้น", newPlanActivities });
                }

            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");

            app.MapPut("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_Update/{id}", [AllowAnonymous] (int Id, PlanActivities_update request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var existingPlanActivity = context.PlanActivities.Find(Id);

                    if (existingPlanActivity == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบกิจกรรมโครงการที่ต้องการอัปเดต" });
                    }

                    existingPlanActivity.PlanCoreId = request.PlanCoreId;
                    existingPlanActivity.Name = request.Name;
                    existingPlanActivity.Active = request.Active;
                    existingPlanActivity.FiscalYear = request.FiscalYear;
                    existingPlanActivity.Code = request.Code ?? "";
                    existingPlanActivity.Detail = request.Detail ?? "";
                    existingPlanActivity.IsFollowUp = request.IsFollowUp;
                    existingPlanActivity.DepartmentId = request.DepartmentId;
                    existingPlanActivity.FundCategoryEnum = request.FundCategoryEnum;
                    existingPlanActivity.Weight = request.Weight;

                    if (context.Entry(existingPlanActivity).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "อัปเดตกิจกรรมโครงการเสร็จสิ้น", existingPlanActivity });
                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");

            app.MapDelete("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_Delete/{id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var PlanActivitie = context.PlanActivities.Find(id);
                    if (PlanActivitie == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบกิจกรรมโครงการที่ต้องการลบ" });
                    }

                    context.PlanActivities.Remove(PlanActivitie);
                    if (context.Entry(PlanActivitie).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "ลบกิจกรรมโครงการเสร็จสิ้น" });
                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");

            app.MapGet("/Endpoint/SEC/Plan/PlanActivities/PlanActivities_GetData/{PlanCoreId}", [AllowAnonymous] (int PlanCoreId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var PlanActivitie = context.PlanActivities.Find(PlanCoreId);
                    if (PlanActivitie == null)
                    {
                        return Results.NotFound(new { Message = "ไม่พบข้อมูลโครงการที่ต้องการ" });
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
                            // แสดงข้อมูลจากอีกตาราง โดยเช็คจากไอดีที่ได้รับ
                            //Department = c.Department, 
                            FundCategoryEnum = c.FundCategoryEnum,
                            Weight = c.Weight,
                        })
                        .ToList();

                    if(PlanActivitie == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลกิจกรรมโครงการ");
                    }

                    return Results.Ok(Planactv);

                }
            })
            .WithTags("PlanActivities")
            .WithGroupName("SEC_PlanActivitie");
        }

        record PlanActivities_Create(string Name, bool Active, int FiscalYear, string Code, string Detail, bool IsFollowUp,
        int DepartmentId, int FundCategoryEnum, decimal Weight, string OtherFundSourceName, string OperationPeriod);

        record PlanActivities_update(int PlanCoreId, string Name, bool Active, int FiscalYear, string Code, string Detail, bool IsFollowUp,
        int DepartmentId, int FundCategoryEnum, decimal Weight, string OtherFundSourceName, string OperationPeriod);
    }
}
