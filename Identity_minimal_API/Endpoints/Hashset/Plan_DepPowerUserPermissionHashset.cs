using System.Security.Claims;
using Identity_jwt.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

public static class HashSetEndpoints
{
    public static void MapHashSetEndpoints(this WebApplication app)
    {
        app.MapGet("/DepPowerUserPermissionHashset_Showdata", [Authorize] async (UserManager<IdenUser> userManager, string userId) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            return Results.Ok(user.HashSetNumbers);
        })
        .WithName("Plan_DepPowerUserPermissionHashset1")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("Hashset");

        app.MapPost("/DepPowerUserPermissionHashset_Create", [Authorize] async (UserManager<IdenUser> userManager, string userId, HashSet<Tuple<int, HashSet<int>>> newSet) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            user.HashSetNumbers = newSet;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(user.HashSetNumbers);
        })
        .WithName("Plan_DepPowerUserPermissionHashset2")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("Hashset");

        app.MapPut("/DepPowerUserPermissionHashset_Update", [Authorize] async (UserManager<IdenUser> userManager, string userId, Tuple<int, HashSet<int>> updateEntry) =>
        {
            // ดึงข้อมูลผู้ใช้จากฐานข้อมูล
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            // ดึง HashSetNumbers จากผู้ใช้ในฐานข้อมูล
            var hashSetNumbers = user.HashSetNumbers;

            // ตรวจสอบว่า Key มีอยู่ใน HashSet หรือไม่
            var existingEntry = hashSetNumbers.FirstOrDefault(x => x.Item1 == updateEntry.Item1);
            if (existingEntry != null)
            {
                hashSetNumbers.Remove(existingEntry); // ลบรายการเก่าออก
            }

            // เพิ่มหรืออัปเดตรายการใน HashSet
            hashSetNumbers.Add(updateEntry);

            // บันทึกข้อมูลที่อัปเดตกลับไปยัง DB
            user.HashSetNumbers = hashSetNumbers;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            // ส่งข้อมูลที่อัปเดตกลับไปใน Response
            return Results.Ok(user.HashSetNumbers);
        })
        .WithName("Plan_DepPowerUserPermissionHashset3")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("Hashset");

        app.MapDelete("/DepPowerUserPermissionHashset_Delete", [Authorize] async (UserManager<IdenUser> userManager, string userId, int key) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            var hashSetNumbers = user.HashSetNumbers;

            // ลบ Key ที่ตรงกันออกจาก HashSet
            var existingEntry = hashSetNumbers.FirstOrDefault(x => x.Item1 == key);
            if (existingEntry != null)
            {
                hashSetNumbers.Remove(existingEntry);
            }

            user.HashSetNumbers = hashSetNumbers;

            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(user.HashSetNumbers);
        })
        .WithName("Plan_DepPowerUserPermissionHashset4")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("Hashset");

        app.MapGet("/DepPowerUserPermissionHashset_getdata", [Authorize] async (UserManager<IdenUser> userManager, HttpContext httpContext) =>
        {
            var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Results.Unauthorized();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            return Results.Ok(user.HashSetNumbers);
        })
        .WithName("Plan_DepPowerUserPermissionHashset5")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("Hashset");
    }
}