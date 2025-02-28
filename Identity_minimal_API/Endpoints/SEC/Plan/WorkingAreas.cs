using Azure.Core;
using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using iLinkDomain.Service.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_WorkingAreas_Endpoints
    {
        public static void MapSEC_WorkingAreasn_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/Endpoint/SEC/Plan/WorkingAreas/WorkingAreas_Create/{PlanCoreId}", [AllowAnonymous] (int PlanCoreId, Working_Create request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var Chack_WorkingAreas = context.WorkingAreas.FirstOrDefault(c => c.PlanCoreId == PlanCoreId);
                    if (Chack_WorkingAreas == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการนี้");
                    }

                    var newWorkingArea = new WorkingArea
                    {
                        Name = request.Name,
                        Active = request.Active,
                        GeographyId = request.GeographyId,
                        ProvinceId = request.ProvinceId,
                        AmphoeId = request.AmphoeId,
                        TambonId = request.TambonId,
                        PlanCoreId = PlanCoreId,
                    };

                    context.WorkingAreas.Add(newWorkingArea);

                    context.SaveChanges();


                    return Results.Ok(new { Message = "สร้างแผนดำเนินงานเสร็จสิ้น", newWorkingArea });
                }
            })
            .WithTags("WorkingArea")
            .WithGroupName("SEC_WorkingArea");


            app.MapPut("/Endpoints/SEC/Plan/WorkingAreas/WorkingAreas_Update/{Id}", [AllowAnonymous] (int id, Working_Update request) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var existingworkingAreas = context.WorkingAreas.FirstOrDefault(c => c.Id == id);
                    if (existingworkingAreas == null)
                    {
                        return Results.NotFound("ไม่พบแผนดำเนินงานที่ต้องการ");
                    }

                    existingworkingAreas.Name = request.Name;
                    existingworkingAreas.Active = request.Active;
                    existingworkingAreas.GeographyId = request.GeographyId;
                    existingworkingAreas.ProvinceId = request.ProvinceId;
                    existingworkingAreas.AmphoeId = request.AmphoeId;
                    existingworkingAreas.TambonId = request.TambonId;
                    existingworkingAreas.PlanCoreId = request.PlanCoreId;


                    context.SaveChanges();


                    return Results.Ok(new { Message = "อัพเดทแผนดำเนินงานเสร็จสิ้น", existingworkingAreas });
                }
            })
            .WithTags("WorkingArea")
            .WithGroupName("SEC_WorkingArea");

            app.MapDelete("/Endpoints/SEC/Plan/WorkingAreas/WorkingAreas_Delete/{Id}", [AllowAnonymous] (int id) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var Chack_WorkingAreas = context.WorkingAreas.FirstOrDefault(c => c.Id == id);

                    if (Chack_WorkingAreas == null)
                    {
                        return Results.NotFound("ไม่พบแผนดำเนินงานที่ต้องการ");
                    }
                    PlanCoreService plancoreService = new PlanCoreService(context);
                    context.WorkingAreas.Remove(Chack_WorkingAreas);

                    context.SaveChanges();


                    return Results.Ok("ลบแผนดำเนินงานเสร็จสิ้น");
                }
            })
            .WithTags("WorkingArea")
            .WithGroupName("SEC_WorkingArea");

            app.MapGet("/Endpoints/SEC/Plan/WorkingAreas/WorkingAreas_GetData/{PlanCoreId}", [AllowAnonymous] (int PlanCoreId) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    var Chack_WorkingAreas = context.WorkingAreas.FirstOrDefault(c => c.PlanCoreId == PlanCoreId);
                    if (Chack_WorkingAreas == null)
                    {
                        return Results.NotFound("ไม่พบข้อมูลโครงการนี้");
                    }

                    var WorkingAreas = context.WorkingAreas
                    .Where(c => c.PlanCoreId == PlanCoreId)
                    .Select(c => new
                    {
                        Name = c.Name,
                        Active = c.Active,
                        GeographyId = c.GeographyId,
                        ProvinceId = c.ProvinceId,
                        AmphoeId = c.AmphoeId,
                        TambonId = c.TambonId,
                        PlanCoreId = c.PlanCoreId,
                    })
                    .ToList();

                    if (!WorkingAreas.Any())
                    {
                        return Results.NotFound("ไม่พบแผนดำเนินงาน");
                    }

                    return Results.Ok(WorkingAreas);
                }
            })
            .WithTags("WorkingArea")
            .WithGroupName("SEC_WorkingArea");
        }

        record Working_Create(string Name, bool Active, int? GeographyId, int? ProvinceId, int? AmphoeId, int? TambonId);

        record Working_Update(string Name, bool Active, int? GeographyId, int? ProvinceId, int? AmphoeId, int? TambonId, int? PlanCoreId);
    }
}
