using System.Security.Claims;
using Identity_jwt.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public static class Agency_id_Hashset
{
    public static void MapAgency_id_HashsetEndpoints(this WebApplication app)
    {
        app.MapGet("/Endpoints/Hashset/Agency_id_Hashset/Agency_id_ShowdataHashset", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return Results.NotFound("User not found");

            // 🔍 ดึง Role ของ User จาก Identity
            var roles = await userManager.GetRolesAsync(user);

            // ✅ Debug Log เช็ค Role ของ User
            Console.WriteLine($"User ID: {id}, Roles: {string.Join(", ", roles)}");

            return Results.Ok(new
            {
                UserRoles = roles, // เพิ่มข้อมูล Role เพื่อดูจาก Response
                HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
                Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
                Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet
            });
        })
        .WithName("agency_id_Hashset1")
        .WithGroupName("Agency_ID")
        .WithDescription("ดึงข้อมูลIDหน่วยงานแบบตาม ID")
        .WithTags("Hashset");

        app.MapPut("/Endpoints/Hashset/Agency_id_Hashset/Agency_id_UpdateHashset", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, string type, List<int> values, UserManager<IdenUser> userManager, string userId) =>
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
                    user.HR_HRDepartmentPermissionHashSet = new HashSet<int>(values);
                    break;

                case "department":
                    user.Plan_DepartmentPermissionHashSet = new HashSet<int>(values);
                    break;

                case "plan":
                    user.Plan_DepPowerByPlanPermissionHashSet = new HashSet<int>(values);
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
        .WithName("agency_id_Hashset2")
        .WithGroupName("Agency_ID")
        .WithDescription("อัพเดทข้อมูลIDหน่วยงาน")
        .WithTags("Hashset");

        app.MapDelete("/Endpoints/Hashset/Agency_id_Hashset/Agency_id_Delete_DataHashset", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(id);
            if (user == null)
                return Results.NotFound("User not found");

            user.HR_HRDepartmentPermissionHashSet.Clear();
            user.Plan_DepartmentPermissionHashSet.Clear();
            user.Plan_DepPowerByPlanPermissionHashSet.Clear();

            var result = await userManager.UpdateAsync(user);

            return result.Succeeded ? Results.Ok("Permissions cleared") : Results.BadRequest(result.Errors);
        })
        .WithName("agency_id_Hashset3")
        .WithGroupName("Agency_ID")
        .WithDescription("ลบข้อมูลIDหน่วยงาน")
        .WithTags("Hashset");

        app.MapPost("/Endpoints/Hashset/Agency_id_Hashset/Agency_id_CreateHashset", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager, string type, List<int> values, string userId) =>
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
                    user.HR_HRDepartmentPermissionHashSet = new HashSet<int>(values);
                    break;

                case "department":
                    user.Plan_DepartmentPermissionHashSet = new HashSet<int>(values);
                    break;

                case "plan":
                    user.Plan_DepPowerByPlanPermissionHashSet = new HashSet<int>(values);
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
        .WithName("agency_id_Hashset4")
        .WithGroupName("Agency_ID")
        .WithDescription("สร้างข้อมูลIDหน่วยงาน")
        .WithTags("Hashset");

        app.MapGet("/Endpoints/Hashset/Agency_id_Hashset/Agency_id_getdataHashset", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager) =>
        {
            var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Results.Unauthorized();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return Results.NotFound("User not found");

            return Results.Ok(new
            {
                HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
                Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
                Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet
            });
        })
        .WithName("agency_id_Hashset5")
        .WithGroupName("Agency_ID")
        .WithDescription("ดึงข้อมูลIDหน่วยงาน")
        .WithTags("Hashset");
    }
}
