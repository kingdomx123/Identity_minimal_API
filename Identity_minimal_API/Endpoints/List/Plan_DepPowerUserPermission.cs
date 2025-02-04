using System.Security.Claims;
using Identity_jwt.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

public static class Plan_DepPowerUserPermission_Endpoints
{
    public static void MapPlan_DepPowerUserPermission_Endpoints(this WebApplication app)
    {
        app.MapGet("/DepPowerUserPermission_Showdata", [Authorize] async (UserManager<IdenUser> userManager, string userId) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            return Results.Ok(user.ListNumbers);
        })
        .WithName("Plan_DepPowerUserPermission1")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("List");

        app.MapPost("/DepPowerUserPermission_Create", [Authorize] async (UserManager<IdenUser> userManager, string userId, List<Tuple<int, List<int>>> newList) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            user.ListNumbers = newList;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(user.ListNumbers);
        })
        .WithName("Plan_DepPowerUserPermission2")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("List");

        app.MapPut("/DepPowerUserPermission_Update", [Authorize] async (UserManager<IdenUser> userManager, string userId, Tuple<int, List<int>> updateEntry) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            var listNumbers = user.ListNumbers;
            var existingEntryIndex = listNumbers.FindIndex(x => x.Item1 == updateEntry.Item1);
            if (existingEntryIndex >= 0)
            {
                listNumbers[existingEntryIndex] = updateEntry;
            }
            else
            {
                listNumbers.Add(updateEntry);
            }

            user.ListNumbers = listNumbers;
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(user.ListNumbers);
        })
        .WithName("Plan_DepPowerUserPermission3")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("List");

        app.MapDelete("/DepPowerUserPermission_Delete", [Authorize] async (UserManager<IdenUser> userManager, string userId, int key) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            user.ListNumbers.RemoveAll(x => x.Item1 == key);
            var result = await userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return Results.BadRequest(result.Errors);
            }

            return Results.Ok(user.ListNumbers);
        })
        .WithName("Plan_DepPowerUserPermission4")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("List");

        app.MapGet("/DepPowerUserPermission_Getdata", [Authorize] async (UserManager<IdenUser> userManager, HttpContext httpContext) =>
        {
            var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userName))
                return Results.Unauthorized();

            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
            {
                return Results.NotFound("User not found.");
            }

            return Results.Ok(user.ListNumbers);
        })
        .WithName("Plan_DepPowerUserPermission5")
        .WithGroupName("Plan_DepPowerUserPermission")
        .WithTags("List");
    }
}
