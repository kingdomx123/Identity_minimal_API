using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Identity_jwt.Domain; // ใช้งาน Entity ที่เกี่ยวข้องกับ Identity
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

public static class YearNumbersEndpoints
{
    // ฟังก์ชันขยาย WebApplication เพื่อเพิ่ม API สำหรับจัดการ YearNumbers
    public static void MapYearNumbersEndpoints(this WebApplication app)
    {
        app.MapPost("/Endpoints/Dictionary/Plan_DepPowerUserPermission_Dictionary/Create_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, Dictionary<int, List<int>> yearNumbers, string userId) =>
        {
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            user.YearNumbers = yearNumbers;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded
                ? Results.Ok(user.YearNumbers)
                : Results.BadRequest(result.Errors.Select(e => e.Description).ToList());
        })
        .WithName("Create_Plan_DepPowerUserPermission")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithDescription("สร้างปีงบประมาณและหน่วยงาน")
        .WithTags("Dictionary");

        app.MapGet("/Endpoints/Dictionary/Plan_DepPowerUserPermission_Dictionary/Read_Plan_select_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, string userId) =>
        {
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            return Results.Ok(user.YearNumbers);
        })
        .WithName("Read_Plan_select_DepPowerUserPermission")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithDescription("ดึงข้อมูลปีงบประมาณและหน่วยงานแบบตาม ID")
        .WithTags("Dictionary");

        app.MapGet("/Endpoints/Dictionary/Plan_DepPowerUserPermission_Dictionary/Read_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, HttpContext httpContext) =>
        {
            var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Results.Unauthorized();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return Results.NotFound("User not found");

            return Results.Ok(user.YearNumbers);
        })
        .WithName("Read_Plan_DepPowerUserPermission")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithDescription("ดึงข้อมูลปีงบประมาณและหน่วยงาน")
        .WithTags("Dictionary");


        app.MapPut("/Endpoints/Dictionary/Plan_DepPowerUserPermission_Dictionary/Update_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, Dictionary<int, List<int>> updatedYearNumbers, string userId) =>
        {
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            user.YearNumbers = updatedYearNumbers;

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded
                ? Results.Ok(user.YearNumbers)
                : Results.BadRequest(result.Errors.Select(e => e.Description).ToList());
        })
        .WithName("Update_Plan_DepPowerUserPermission")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithDescription("อัพเดทข้อมูลปีงบประมาณและหน่วยงาน")
        .WithTags("Dictionary");

        app.MapDelete("/Endpoints/Dictionary/Plan_DepPowerUserPermission_Dictionary/Delete_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, string userId) =>
        {
            if (string.IsNullOrEmpty(userId))
                return Results.Unauthorized();

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
                return Results.NotFound("User not found");

            user.YearNumbers = null!; // Clear the dictionary

            var result = await userManager.UpdateAsync(user);
            return result.Succeeded
                ? Results.Ok("YearNumbers deleted successfully")
                : Results.BadRequest(result.Errors.Select(e => e.Description).ToList());
        })
        .WithName("Delete_Plan_DepPowerUserPermission")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithDescription("ลบข้อมูลปีงบประมาณและหน่วยงาน")
        .WithTags("Dictionary");
    }
}
