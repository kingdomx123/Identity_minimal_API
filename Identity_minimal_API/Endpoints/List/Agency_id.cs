using Identity_jwt.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

public static class AgencyPermissionsEndpoints
{
    public static void MapAgencyPermissionsEndpoints(this WebApplication app)
    {
        // Endpoint สำหรับดึงข้อมูลสิทธิ์ของผู้ใช้
        app.MapGet("/Endpoints/List/Agency_id/Agency_id_Showdata", [AllowAnonymous] async (string id, UserManager<IdenUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return Results.NotFound("User not found");

            return Results.Ok(new
            {
                HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
                Plan_DepartmentPermission = user.Plan_DepartmentPermission,
                Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission
            });
        })
        .WithName("agency_id_1")
        .WithGroupName("Agency_ID")
        .WithDescription("ดึงข้อมูลIDหน่วยงานแบบตาม ID")
        .WithTags("List");

        // Endpoint สำหรับอัปเดตสิทธิ์ของผู้ใช้
        app.MapPut("/Endpoints/List/Agency_id/Agency_id_Update", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, string type, List<int> values, UserManager<IdenUser> userManager, string userId) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            if (values == null || !values.Any())
                return Results.BadRequest("Values list cannot be null or empty");

            switch (type.Trim().ToLower())
            {
                case "hr":
                    user.HR_HRDepartmentPermission = values;
                    break;
                case "department":
                    user.Plan_DepartmentPermission = values;
                    break;
                case "plan":
                    user.Plan_DepPowerByPlanPermission = values;
                    break;
                default:
                    return Results.BadRequest("Invalid permission type. Valid types are: 'hr', 'department', 'plan'.");
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"Failed to update user: {errorMessages}");
            }

            return Results.Ok($"Permissions updated successfully for type: {type}");
        })
        .WithName("agency_id_2")
        .WithGroupName("Agency_ID")
        .WithDescription("อัพเดทข้อมูลIDหน่วยงาน")
        .WithTags("List");

        // Endpoint สำหรับลบสิทธิ์ของผู้ใช้
        app.MapDelete("/Endpoints/List/Agency_id/Agency_id_Delete_Data", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return Results.NotFound("User not found");

            user.HR_HRDepartmentPermission.Clear();
            user.Plan_DepartmentPermission.Clear();
            user.Plan_DepPowerByPlanPermission.Clear();

            var result = await userManager.UpdateAsync(user);

            return result.Succeeded ? Results.Ok("Permissions cleared") : Results.BadRequest(result.Errors);
        })
        .WithName("agency_id_3")
        .WithGroupName("Agency_ID")
        .WithDescription("ลบข้อมูลIDหน่วยงาน")
        .WithTags("List");

        // Endpoint สำหรับเพิ่มสิทธิ์ของผู้ใช้
        app.MapPost("/Endpoints/List/Agency_id/Agency_id_Create", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager, string type, List<int> values, string userId) =>
        {
            if (string.IsNullOrWhiteSpace(userId))
                return Results.Unauthorized();

            if (values == null || !values.Any())
                return Results.BadRequest("Values list cannot be null or empty");

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            switch (type.Trim().ToLower())
            {
                case "hr":
                    user.HR_HRDepartmentPermission = values;
                    break;
                case "department":
                    user.Plan_DepartmentPermission = values;
                    break;
                case "plan":
                    user.Plan_DepPowerByPlanPermission = values;
                    break;
                default:
                    return Results.BadRequest("Invalid permission type. Valid types are: 'hr', 'department', 'plan'.");
            }

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return Results.BadRequest($"Failed to create user: {errorMessages}");
            }

            return Results.Ok($"User permissions added successfully for type: {type}");
        })
        .WithName("agency_id_4")
        .WithGroupName("Agency_ID")
        .WithDescription("สร้างข้อมูลIDหน่วยงาน")
        .WithTags("List");

        app.MapGet("/Endpoints/List/Agency_id/Agency_id_Getdata", [Authorize] async (HttpContext httpContext, UserManager<IdenUser> userManager) =>
        {
            var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Results.Unauthorized();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return Results.NotFound("User not found");

            return Results.Ok(new
            {
                HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
                Plan_DepartmentPermission = user.Plan_DepartmentPermission,
                Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission
            });
        })
        .WithName("agency_id_5")
        .WithGroupName("Agency_ID")
        .WithDescription("ดึงข้อมูลIDหน่วยงาน")
        .WithTags("List");
    }
}
