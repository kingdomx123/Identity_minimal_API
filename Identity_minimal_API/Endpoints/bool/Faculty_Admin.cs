using Identity_jwt.Domain;
using Identity_minimal_API.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

public static class FacultyAdminEndpoints
{
    public static void MapFacultyAdminEndpoints(this WebApplication app)
    {
        app.MapPut("/Endpoints/bool/Faculty_Admin/Faculty_Admin_update", [Authorize(Roles = "admin, dev")] async (string id, HttpContext httpContext, UserManager<IdenUser> userManager, UpdateAccessRequest request) =>
        {

            // ตรวจสอบว่ามี id ของผู้ใช้เป้าหมายในคำขอหรือไม่
            if (string.IsNullOrEmpty(id))
            {
                return Results.BadRequest("กรุณาระบุ UserId");
            }

            // ค้นหาผู้ใช้ในฐานข้อมูล
            var targetUser = await userManager.FindByIdAsync(id);
            if (targetUser == null)
            {
                return Results.NotFound("ไม่พบผู้ใช้งานในระบบ");
            }

            // อัปเดตสิทธิ์การเข้าถึง
            targetUser.IsFinancialDepPowerUser = request.IsFinancialDepPowerUser;
            targetUser.IsFinDepUser = request.IsFinDepUser;
            targetUser.IsPlanDepPowerUser = request.IsPlanDepPowerUser;
            targetUser.IsProcureDepPowerUser = request.IsProcureDepPowerUser;
            targetUser.IsHRDepPowerUser = request.IsHRDepPowerUser;

            // บันทึกการเปลี่ยนแปลงในฐานข้อมูล
            var result = await userManager.UpdateAsync(targetUser);
            if (!result.Succeeded)
            {
                return Results.BadRequest("ไม่สามารถอัปเดตข้อมูลได้");
            }

            return Results.Ok($"อัปเดตสิทธิ์การเข้าถึงสำหรับผู้ใช้ {targetUser.UserName} สำเร็จ");
        })
        .WithName("Faculty_Admin1")
        .WithGroupName("Faculty_Admin")
        .WithDescription("อัพเดทข้อมูลadminคณะ")
        .WithTags("bool");

        app.MapGet("/Endpoints/bool/Faculty_Admin/Faculty_Admin_select", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, string userId) =>
        {
            // ตรวจสอบว่า userId ที่ระบุมีในระบบหรือไม่
            var user = await userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Results.NotFound("ไม่พบข้อมูลผู้ใช้");
            }

            // คืนค่าข้อมูลทั้งหมดของผู้ใช้งาน
            return Results.Ok(new
            {
                FullRealName = user.FullRealName,
                IsFinancialDepPowerUser = user.IsFinancialDepPowerUser,
                IsFinDepUser = user.IsFinDepUser,
                IsPlanDepPowerUser = user.IsPlanDepPowerUser,
                IsProcureDepPowerUser = user.IsProcureDepPowerUser,
                IsHRDepPowerUser = user.IsHRDepPowerUser,
            });
        })
        .WithName("Faculty_Admin2")
        .WithGroupName("Faculty_Admin")
        .WithDescription("ดึงข้อมูลadminคณะแบบตาม ID")
        .WithTags("bool");

        app.MapGet("/Endpoints/bool/Faculty_Admin/Faculty_Admin_getuser", [Authorize(Roles = "admin, dev")] (HttpContext httpContext) =>
        {
            // ดึงค่า Claims ที่เกี่ยวข้องกับสิทธิ์
            var isFinancialDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsFinancialDepPowerUser")?.Value;
            var isFinDepUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsFinDepUser")?.Value;
            var isPlanDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsPlanDepPowerUser")?.Value;
            var isProcureDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsProcureDepPowerUser")?.Value;
            var isHRDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsHRDepPowerUser")?.Value;

            // รวบรวมสิทธิ์ที่มี
            var permissions = new List<string>();
            if (isFinancialDepPowerUser == "true") permissions.Add("Admin การเงิน");
            if (isFinDepUser == "true") permissions.Add("Admin กองคลัง");
            if (isPlanDepPowerUser == "true") permissions.Add("Admin งานแผน");
            if (isProcureDepPowerUser == "true") permissions.Add("Admin งานพัสดุ");
            if (isHRDepPowerUser == "true") permissions.Add("Admin งานบุคคล");

            // หากไม่มีสิทธิ์เข้าถึง
            if (!permissions.Any())
            {
                return Results.Ok("ไม่มีสิทธิ์ในการเข้าถึง");
            }

            return Results.Ok(new { Permissions = permissions });
        })
        .WithName("Faculty_Admin3")
        .WithGroupName("Faculty_Admin")
        .WithDescription("ดึงข้อมูลadminคณะ")
        .WithTags("bool");

        app.MapDelete("/Endpoints/bool/Faculty_Admin/Faculty_Admin_Clear", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager, string userId) =>
        {
            if (string.IsNullOrEmpty(userId))
            {
                return Results.Unauthorized();
            }

            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("ไม่พบผู้ใช้งานในระบบ");
            }

            // ลบสิทธิ์ทั้งหมดของผู้ใช้
            user.IsFinancialDepPowerUser = false;
            user.IsFinDepUser = false;
            user.IsPlanDepPowerUser = false;
            user.IsProcureDepPowerUser = false;
            user.IsHRDepPowerUser = false;

            var result = await userManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return Results.BadRequest("ไม่สามารถลบสิทธิ์ได้");
            }

            return Results.Ok($"ลบสิทธิ์ทั้งหมดของผู้ใช้ {user.UserName} สำเร็จ");
        })
        .WithName("Faculty_Admin4")
        .WithGroupName("Faculty_Admin")
        .WithDescription("ลบข้อมูลadminคณะทั้งหมด")
        .WithTags("bool");

        app.MapPost("/Endpoints/bool/Faculty_Admin/Faculty_Admin_Create", [Authorize(Roles = "admin, dev")] async (string userId, UserManager<IdenUser> userManager, UpdateAccessRequest request) =>
        {
            var user = await userManager.FindByIdAsync(userId); // คุณต้องสร้างฟังก์ชันนี้เพื่อดึงข้อมูล Claims

            if (user == null)
            {
                return Results.Ok("ไม่พบข้อมูลผู้ใช้");
            }
            var userClaims = await userManager.GetClaimsAsync(user);

            // ค้นหาผู้ใช้ในระบบ
            var targetUser = await userManager.FindByIdAsync(userId);
            if (targetUser == null)
            {
                return Results.NotFound("ไม่พบผู้ใช้งานในระบบ");
            }

            // ตรวจสอบสิทธิ์ของแอดมิน หากต้องการให้แอดมินอัปเดตสิทธิ์ของผู้ใช้อื่น
            var isAdmin = userClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == "admin");
            if (userId != targetUser.Id && !isAdmin)
            {
                return Results.Forbid();
            }

            // อัปเดตสิทธิ์ตามคำขอ
            targetUser.IsFinancialDepPowerUser = request.IsFinancialDepPowerUser ?? targetUser.IsFinancialDepPowerUser;
            targetUser.IsFinDepUser = request.IsFinDepUser ?? targetUser.IsFinDepUser;
            targetUser.IsPlanDepPowerUser = request.IsPlanDepPowerUser ?? targetUser.IsPlanDepPowerUser;
            targetUser.IsProcureDepPowerUser = request.IsProcureDepPowerUser ?? targetUser.IsProcureDepPowerUser;
            targetUser.IsHRDepPowerUser = request.IsHRDepPowerUser ?? targetUser.IsHRDepPowerUser;

            // บันทึกการเปลี่ยนแปลง
            var result = await userManager.UpdateAsync(targetUser);
            if (!result.Succeeded)
            {
                return Results.BadRequest("ไม่สามารถอัปเดตสิทธิ์ได้");
            }

            return Results.Ok($"ปรับปรุงสิทธิ์ของผู้ใช้ {targetUser.UserName} สำเร็จ");
        })
        .WithName("Faculty_Admin5")
        .WithGroupName("Faculty_Admin")
        .WithDescription("สร้างข้อมูลadminคณะ")
        .WithTags("bool");
    }
}
