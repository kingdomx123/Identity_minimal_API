//using Identity_jwt.Data;
//using Identity_jwt.Domain;
//using Identity_minimal_API.Model;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.IdentityModel.Tokens;
//using Microsoft.OpenApi.Models;
//using System.IdentityModel.Tokens.Jwt;
//using System.Security.Claims;
//using System.Text;
//using System.Text.Json;

//var builder = WebApplication.CreateBuilder(args);

//// 🔗 เพิ่มการเชื่อมต่อฐานข้อมูล
//builder.Services.AddSqlServer<IdenDbcontext>(builder.Configuration.GetConnectionString("DefaultConnection"));
//builder.Services.AddIdentity<IdenUser, IdentityRole>()
//              .AddEntityFrameworkStores<IdenDbcontext>()
//              .AddDefaultTokenProviders();

//// 🔧 เพิ่มบริการ Swagger
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen(option =>
//{
//    //คำสั่งเพิ่มไปยังอีกหน้า และกำหนดหัวข้อ
//    option.SwaggerDoc("Page1", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Page1" });
//    option.SwaggerDoc("Page2", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Page2" });
//    option.SwaggerDoc("Page3", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Page3" });
//    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
//    {
//        In = ParameterLocation.Header,
//        Description = "กรุณาระบุค่า Token",
//        Name = "Authorization",
//        Type = SecuritySchemeType.Http,
//        BearerFormat = "JWT",
//        Scheme = "Bearer"
//    });
//    option.AddSecurityRequirement(new OpenApiSecurityRequirement
//    {
//          {
//              new OpenApiSecurityScheme
//              {
//                  Reference = new OpenApiReference
//                  {
//                      Type = ReferenceType.SecurityScheme,
//                      Id = "Bearer"
//                  }
//              },
//              new string[]{}
//          }
//    });
//});

//// เพิ่มบริการการกำหนดค่า JWT
//builder.Services.AddAuthentication(options =>
//{
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//}).AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        ValidateAudience = true,
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
//        ValidateLifetime = false,
//        ValidateIssuerSigningKey = true
//    };
//});

//// 🛠️ เพิ่มบริการ Authorization
//builder.Services.AddAuthorization();

//var app = builder.Build();

//// 🛠️ ตั้งค่า Swagger สำหรับโหมด Development
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI(options =>
//    {
//        //คำสั่งเพิ่มหน้าสลับ
//        options.SwaggerEndpoint("/swagger/Page1/swagger.json", "API v1");
//        options.SwaggerEndpoint("/swagger/Page2/swagger.json", "API v2");
//        options.SwaggerEndpoint("/swagger/Page3/swagger.json", "API v3");
//    });
//}

//// 🔒 Middleware สำหรับ HTTPS
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapGet("/DepPowerUserPermissionHashset_Showdata", [Authorize] async (UserManager<IdenUser> userManager, string userId) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    return Results.Ok(user.HashSetNumbers);
//})
//.WithName("Plan_DepPowerUserPermissionHashset1")
//.WithTags("Hashset");

//app.MapPost("/DepPowerUserPermissionHashset_Create", [Authorize] async (UserManager<IdenUser> userManager, string userId, HashSet<Tuple<int, HashSet<int>>> newSet) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    user.HashSetNumbers = newSet;
//    var result = await userManager.UpdateAsync(user);

//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(result.Errors);
//    }

//    return Results.Ok(user.HashSetNumbers);
//})
//.WithName("Plan_DepPowerUserPermissionHashset2")
//.WithTags("Hashset");

//app.MapPut("/DepPowerUserPermissionHashset_Update", [Authorize] async (UserManager<IdenUser> userManager, string userId, Tuple<int, HashSet<int>> updateEntry) =>
//{
//    // ดึงข้อมูลผู้ใช้จากฐานข้อมูล
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    // ดึง HashSetNumbers จากผู้ใช้ในฐานข้อมูล
//    var hashSetNumbers = user.HashSetNumbers;

//    // ตรวจสอบว่า Key มีอยู่ใน HashSet หรือไม่
//    var existingEntry = hashSetNumbers.FirstOrDefault(x => x.Item1 == updateEntry.Item1);
//    if (existingEntry != null)
//    {
//        hashSetNumbers.Remove(existingEntry); // ลบรายการเก่าออก
//    }

//    // เพิ่มหรืออัปเดตรายการใน HashSet
//    hashSetNumbers.Add(updateEntry);

//    // บันทึกข้อมูลที่อัปเดตกลับไปยัง DB
//    user.HashSetNumbers = hashSetNumbers;
//    var result = await userManager.UpdateAsync(user);

//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(result.Errors);
//    }

//    // ส่งข้อมูลที่อัปเดตกลับไปใน Response
//    return Results.Ok(user.HashSetNumbers);
//})
//.WithName("Plan_DepPowerUserPermissionHashset3")
//.WithTags("Hashset");

//app.MapDelete("/DepPowerUserPermissionHashset_Delete", [Authorize] async (UserManager<IdenUser> userManager, string userId, int key) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    var hashSetNumbers = user.HashSetNumbers;

//    // ลบ Key ที่ตรงกันออกจาก HashSet
//    var existingEntry = hashSetNumbers.FirstOrDefault(x => x.Item1 == key);
//    if (existingEntry != null)
//    {
//        hashSetNumbers.Remove(existingEntry);
//    }

//    user.HashSetNumbers = hashSetNumbers;

//    var result = await userManager.UpdateAsync(user);

//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(result.Errors);
//    }

//    return Results.Ok(user.HashSetNumbers);
//})
//.WithName("Plan_DepPowerUserPermissionHashset4")
//.WithTags("Hashset");

//app.MapGet("/DepPowerUserPermission_Showdata", [Authorize] async (UserManager<IdenUser> userManager, string userId) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    return Results.Ok(user.ListNumbers);
//})
//.WithName("Plan_DepPowerUserPermission1")
//.WithTags("List");

//app.MapPost("/DepPowerUserPermission_Create", [Authorize] async (UserManager<IdenUser> userManager, string userId, List<Tuple<int, List<int>>> newList) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    user.ListNumbers = newList;
//    var result = await userManager.UpdateAsync(user);

//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(result.Errors);
//    }

//    return Results.Ok(user.ListNumbers);
//})
//.WithName("Plan_DepPowerUserPermission2")
//.WithTags("List");

//app.MapPut("/DepPowerUserPermission_Update", [Authorize] async (UserManager<IdenUser> userManager, string userId, Tuple<int, List<int>> updateEntry) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    var listNumbers = user.ListNumbers;

//    // หาอินเด็กซ์ของรายการที่ต้องการอัปเดต
//    var existingEntryIndex = listNumbers.FindIndex(x => x.Item1 == updateEntry.Item1 && x.Item2.SequenceEqual(updateEntry.Item2));
//    if (existingEntryIndex >= 0)
//    {
//        listNumbers[existingEntryIndex] = updateEntry; // อัปเดตรายการเดิม
//    }
//    else
//    {
//        listNumbers.Add(updateEntry); // เพิ่มรายการใหม่
//    }

//    user.ListNumbers = listNumbers;
//    var result = await userManager.UpdateAsync(user);

//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(result.Errors);
//    }

//    return Results.Ok(user.ListNumbers);
//})
//.WithName("Plan_DepPowerUserPermission3")
//.WithTags("List");

//app.MapDelete("/DepPowerUserPermission_Delete", [Authorize] async (UserManager<IdenUser> userManager, string userId, int key) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("User not found.");
//    }

//    var listNumbers = user.ListNumbers;

//    // ลบรายการที่มีคีย์ตรงกับ key ที่ระบุ
//    listNumbers.RemoveAll(x => x.Item1 == key);
//    user.ListNumbers = listNumbers;

//    var result = await userManager.UpdateAsync(user);

//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(result.Errors);
//    }

//    return Results.Ok(user.ListNumbers);
//})
//.WithName("Plan_DepPowerUserPermission4")
//.WithTags("List");

//app.MapPost("/Create_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, Dictionary<int, List<int>> yearNumbers, string userId) =>
//{
//    if (string.IsNullOrEmpty(userId))
//        return Results.Unauthorized();

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    user.YearNumbers = yearNumbers;

//    var result = await userManager.UpdateAsync(user);
//    return result.Succeeded
//        ? Results.Ok(user.YearNumbers)
//        : Results.BadRequest(result.Errors.Select(e => e.Description).ToList());
//})
//.WithName("Create_Plan_DepPowerUserPermission")
//.WithTags("Dictionary");

//app.MapGet("/Read_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, string userId) =>
//{
//    if (string.IsNullOrEmpty(userId))
//        return Results.Unauthorized();

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    return Results.Ok(user.YearNumbers);
//})
//.WithName("Read_Plan_DepPowerUserPermission")
//.WithTags("Dictionary");

//app.MapPut("/Update_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, Dictionary<int, List<int>> updatedYearNumbers, string userId) =>
//{
//    if (string.IsNullOrEmpty(userId))
//        return Results.Unauthorized();

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    user.YearNumbers = updatedYearNumbers;

//    var result = await userManager.UpdateAsync(user);
//    return result.Succeeded
//        ? Results.Ok(user.YearNumbers)
//        : Results.BadRequest(result.Errors.Select(e => e.Description).ToList());
//})
//.WithName("Update_Plan_DepPowerUserPermission")
//.WithTags("Dictionary");

//app.MapDelete("/Delete_Plan_DepPowerUserPermission", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, string userId) =>
//{
//    if (string.IsNullOrEmpty(userId))
//        return Results.Unauthorized();

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    user.YearNumbers = null!; // Clear the dictionary

//    var result = await userManager.UpdateAsync(user);
//    return result.Succeeded
//        ? Results.Ok("YearNumbers deleted successfully")
//        : Results.BadRequest(result.Errors.Select(e => e.Description).ToList());
//})
//.WithName("Delete_Plan_DepPowerUserPermission")
//.WithTags("Dictionary");

//app.MapGet("/Agency_id_ShowdataHashset", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
//{
//    var user = await userManager.FindByIdAsync(id);
//    if (user == null)
//        return Results.NotFound("User not found");

//    return Results.Ok(new
//    {
//        HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
//        Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
//        Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet
//    });
//})
//.WithName("agency_id_Hashset1")
//.WithTags("Hashset");

//app.MapPut("/Agency_id_UpdateHashset", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, string type, List<int> values, UserManager<IdenUser> userManager, string userId) =>
//{
//    if (string.IsNullOrWhiteSpace(userId))
//        return Results.Unauthorized();

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    if (values == null || !values.Any())
//        return Results.BadRequest("Values list cannot be null or empty");

//    switch (type.Trim().ToLower())
//    {
//        case "hr":
//            user.HR_HRDepartmentPermissionHashSet = new HashSet<int>(values);
//            break;

//        case "department":
//            user.Plan_DepartmentPermissionHashSet = new HashSet<int>(values);
//            break;

//        case "plan":
//            user.Plan_DepPowerByPlanPermissionHashSet = new HashSet<int>(values);
//            break;

//        default:
//            return Results.BadRequest("Invalid permission type. Valid types are: 'hr', 'department', 'plan'.");
//    }

//    var result = await userManager.UpdateAsync(user);
//    if (!result.Succeeded)
//    {
//        var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
//        return Results.BadRequest($"Failed to update user: {errorMessages}");
//    }

//    return Results.Ok($"Permissions updated successfully for type: {type}");
//})
//.WithName("agency_id_Hashset2")
//.WithTags("Hashset");

//app.MapDelete("/Agency_id_Delete_DataHashset", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
//{
//    var user = await userManager.FindByIdAsync(id);
//    if (user == null)
//        return Results.NotFound("User not found");

//    user.HR_HRDepartmentPermissionHashSet.Clear();
//    user.Plan_DepartmentPermissionHashSet.Clear();
//    user.Plan_DepPowerByPlanPermissionHashSet.Clear();

//    var result = await userManager.UpdateAsync(user);

//    return result.Succeeded ? Results.Ok("Permissions cleared") : Results.BadRequest(result.Errors);
//})
//.WithName("agency_id_Hashset3")
//.WithTags("Hashset");

//app.MapPost("/Agency_id_CreateHashset", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager, string type, List<int> values, string userId) =>
//{
//    if (string.IsNullOrWhiteSpace(userId))
//        return Results.Unauthorized();

//    if (values == null || !values.Any())
//        return Results.BadRequest("Values list cannot be null or empty");

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    switch (type.Trim().ToLower())
//    {
//        case "hr":
//            user.HR_HRDepartmentPermissionHashSet = new HashSet<int>(values);
//            break;

//        case "department":
//            user.Plan_DepartmentPermissionHashSet = new HashSet<int>(values);
//            break;

//        case "plan":
//            user.Plan_DepPowerByPlanPermissionHashSet = new HashSet<int>(values);
//            break;

//        default:
//            return Results.BadRequest("Invalid permission type. Valid types are: 'hr', 'department', 'plan'.");
//    }

//    var result = await userManager.UpdateAsync(user);
//    if (!result.Succeeded)
//    {
//        var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
//        return Results.BadRequest($"Failed to create user: {errorMessages}");
//    }

//    return Results.Ok($"User permissions added successfully for type: {type}");
//})
//.WithName("agency_id_Hashset4")
//.WithTags("Hashset");

//app.MapGet("/Agency_id_Showdata", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
//{
//    var user = await userManager.FindByIdAsync(id);
//    if (user == null)
//        return Results.NotFound("User not found");

//    return Results.Ok(new
//    {
//        HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
//        Plan_DepartmentPermission = user.Plan_DepartmentPermission,
//        Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission
//    });
//})
//.WithName("agency_id_1")
//.WithTags("List");

//app.MapPut("/Agency_id_Update", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, string type, List<int> values, UserManager<IdenUser> userManager, string userId) =>
//{
//    if (string.IsNullOrWhiteSpace(userId))
//        return Results.Unauthorized();

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    if (values == null || !values.Any())
//        return Results.BadRequest("Values list cannot be null or empty");

//    switch (type.Trim().ToLower())
//    {
//        case "hr":
//            user.HR_HRDepartmentPermission = values;
//            break;

//        case "department":
//            user.Plan_DepartmentPermission = values;
//            break;

//        case "plan":
//            user.Plan_DepPowerByPlanPermission = values;
//            break;

//        default:
//            return Results.BadRequest("Invalid permission type. Valid types are: 'hr', 'department', 'plan'.");
//    }

//    var result = await userManager.UpdateAsync(user);
//    if (!result.Succeeded)
//    {
//        var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
//        return Results.BadRequest($"Failed to update user: {errorMessages}");
//    }

//    return Results.Ok($"Permissions updated successfully for type: {type}");
//})
//.WithName("agency_id_2")
//.WithTags("List");

//app.MapDelete("/Agency_id_Delete_Data", [Authorize(Roles = "admin, dev")] async (string id, UserManager<IdenUser> userManager) =>
//{
//    var user = await userManager.FindByIdAsync(id);
//    if (user == null)
//        return Results.NotFound("User not found");

//    user.HR_HRDepartmentPermission.Clear();
//    user.Plan_DepartmentPermission.Clear();
//    user.Plan_DepPowerByPlanPermission.Clear();

//    var result = await userManager.UpdateAsync(user);

//    return result.Succeeded ? Results.Ok("Permissions cleared") : Results.BadRequest(result.Errors);
//})
//.WithName("agency_id_3")
//.WithTags("List");

//app.MapPost("/Agency_id_Create", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager, string type, List<int> values, string userId) =>
//{
//    if (string.IsNullOrWhiteSpace(userId))
//        return Results.Unauthorized();

//    if (values == null || !values.Any())
//        return Results.BadRequest("Values list cannot be null or empty");

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found");

//    switch (type.Trim().ToLower())
//    {
//        case "hr":
//            user.HR_HRDepartmentPermission = values;
//            break;

//        case "department":
//            user.Plan_DepartmentPermission = values;
//            break;

//        case "plan":
//            user.Plan_DepPowerByPlanPermission = values;
//            break;

//        default:
//            return Results.BadRequest("Invalid permission type. Valid types are: 'hr', 'department', 'plan'.");
//    }

//    var result = await userManager.UpdateAsync(user);
//    if (!result.Succeeded)
//    {
//        var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
//        return Results.BadRequest($"Failed to create user: {errorMessages}");
//    }

//    return Results.Ok($"User permissions added successfully for type: {type}");
//})
//.WithName("agency_id_4")
//.WithTags("List");

//app.MapPut("Faculty_Admin_update", [Authorize(Roles = "admin, dev")] async (string id, HttpContext httpContext, UserManager<IdenUser> userManager, UpdateAccessRequest request) =>
//{

//    // ตรวจสอบว่ามี id ของผู้ใช้เป้าหมายในคำขอหรือไม่
//    if (string.IsNullOrEmpty(id))
//    {
//        return Results.BadRequest("กรุณาระบุ UserId");
//    }

//    // ค้นหาผู้ใช้ในฐานข้อมูล
//    var targetUser = await userManager.FindByIdAsync(id);
//    if (targetUser == null)
//    {
//        return Results.NotFound("ไม่พบผู้ใช้งานในระบบ");
//    }

//    // อัปเดตสิทธิ์การเข้าถึง
//    targetUser.IsFinancialDepPowerUser = request.IsFinancialDepPowerUser;
//    targetUser.IsFinDepUser = request.IsFinDepUser;
//    targetUser.IsPlanDepPowerUser = request.IsPlanDepPowerUser;
//    targetUser.IsProcureDepPowerUser = request.IsProcureDepPowerUser;
//    targetUser.IsHRDepPowerUser = request.IsHRDepPowerUser;

//    // บันทึกการเปลี่ยนแปลงในฐานข้อมูล
//    var result = await userManager.UpdateAsync(targetUser);
//    if (!result.Succeeded)
//    {
//        return Results.BadRequest("ไม่สามารถอัปเดตข้อมูลได้");
//    }

//    return Results.Ok($"อัปเดตสิทธิ์การเข้าถึงสำหรับผู้ใช้ {targetUser.UserName} สำเร็จ");
//})
//.WithName("Faculty_Admin1")
//.WithTags("bool");

//app.MapGet("/Faculty_Admin_select", [Authorize(Roles = "admin, dev")] async (UserManager<IdenUser> userManager, string userId) =>
//{
//    // ตรวจสอบว่า userId ที่ระบุมีในระบบหรือไม่
//    var user = await userManager.FindByIdAsync(userId);

//    if (user == null)
//    {
//        return Results.NotFound("ไม่พบข้อมูลผู้ใช้");
//    }

//    // คืนค่าข้อมูลทั้งหมดของผู้ใช้งาน
//    return Results.Ok(new
//    {
//        FullRealName = user.FullRealName,
//        IsFinancialDepPowerUser = user.IsFinancialDepPowerUser,
//        IsFinDepUser = user.IsFinDepUser,
//        IsPlanDepPowerUser = user.IsPlanDepPowerUser,
//        IsProcureDepPowerUser = user.IsProcureDepPowerUser,
//        IsHRDepPowerUser = user.IsHRDepPowerUser,
//    });
//})
//.WithName("Faculty_Admin2")
//.WithTags("bool");

//app.MapGet("/Faculty_Admin_getuser", [Authorize(Roles = "admin, dev")] (HttpContext httpContext) =>
//{
//    // ดึงค่า Claims ที่เกี่ยวข้องกับสิทธิ์
//    var isFinancialDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsFinancialDepPowerUser")?.Value;
//    var isFinDepUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsFinDepUser")?.Value;
//    var isPlanDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsPlanDepPowerUser")?.Value;
//    var isProcureDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsProcureDepPowerUser")?.Value;
//    var isHRDepPowerUser = httpContext.User.Claims.FirstOrDefault(c => c.Type == "IsHRDepPowerUser")?.Value;

//    // รวบรวมสิทธิ์ที่มี
//    var permissions = new List<string>();
//    if (isFinancialDepPowerUser == "True") permissions.Add("Admin การเงิน");
//    if (isFinDepUser == "True") permissions.Add("Admin กองคลัง");
//    if (isPlanDepPowerUser == "True") permissions.Add("Admin งานแผน");
//    if (isProcureDepPowerUser == "True") permissions.Add("Admin งานพัสดุ");
//    if (isHRDepPowerUser == "True") permissions.Add("Admin งานบุคคล");

//    // หากไม่มีสิทธิ์เข้าถึง
//    if (!permissions.Any())
//    {
//        return Results.Ok("ไม่มีสิทธิ์ในการเข้าถึง");
//    }

//    return Results.Ok(new { Permissions = permissions });
//})
//.WithName("Faculty_Admin3")
//.WithTags("bool");

//app.MapDelete("/Faculty_Admin_Clear", [Authorize(Roles = "admin, dev")] async (HttpContext httpContext, UserManager<IdenUser> userManager, string userId) =>
//{
//    if (string.IsNullOrEmpty(userId))
//    {
//        return Results.Unauthorized();
//    }

//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//    {
//        return Results.NotFound("ไม่พบผู้ใช้งานในระบบ");
//    }

//    // ลบสิทธิ์ทั้งหมดของผู้ใช้
//    user.IsFinancialDepPowerUser = false;
//    user.IsFinDepUser = false;
//    user.IsPlanDepPowerUser = false;
//    user.IsProcureDepPowerUser = false;
//    user.IsHRDepPowerUser = false;

//    var result = await userManager.UpdateAsync(user);
//    if (!result.Succeeded)
//    {
//        return Results.BadRequest("ไม่สามารถลบสิทธิ์ได้");
//    }

//    return Results.Ok($"ลบสิทธิ์ทั้งหมดของผู้ใช้ {user.UserName} สำเร็จ");
//})
//.WithName("Faculty_Admin4")
//.WithTags("bool");

//app.MapPost("/Faculty_Admin_Create", [Authorize(Roles = "admin, dev")] async (string userId, UserManager<IdenUser> userManager, UpdateAccessRequest request) =>
//{
//    var user = await userManager.FindByIdAsync(userId); // คุณต้องสร้างฟังก์ชันนี้เพื่อดึงข้อมูล Claims

//    if (user == null)
//    {
//        return Results.Ok("ไม่พบข้อมูลผู้ใช้");
//    }
//    var userClaims = await userManager.GetClaimsAsync(user);

//    // ค้นหาผู้ใช้ในระบบ
//    var targetUser = await userManager.FindByIdAsync(userId);
//    if (targetUser == null)
//    {
//        return Results.NotFound("ไม่พบผู้ใช้งานในระบบ");
//    }

//    // ตรวจสอบสิทธิ์ของแอดมิน หากต้องการให้แอดมินอัปเดตสิทธิ์ของผู้ใช้อื่น
//    var isAdmin = userClaims.Any(c => c.Type == ClaimTypes.Role && c.Value == "admin");
//    if (userId != targetUser.Id && !isAdmin)
//    {
//        return Results.Forbid();
//    }

//    // อัปเดตสิทธิ์ตามคำขอ
//    targetUser.IsFinancialDepPowerUser = request.IsFinancialDepPowerUser ?? targetUser.IsFinancialDepPowerUser;
//    targetUser.IsFinDepUser = request.IsFinDepUser ?? targetUser.IsFinDepUser;
//    targetUser.IsPlanDepPowerUser = request.IsPlanDepPowerUser ?? targetUser.IsPlanDepPowerUser;
//    targetUser.IsProcureDepPowerUser = request.IsProcureDepPowerUser ?? targetUser.IsProcureDepPowerUser;
//    targetUser.IsHRDepPowerUser = request.IsHRDepPowerUser ?? targetUser.IsHRDepPowerUser;

//    // บันทึกการเปลี่ยนแปลง
//    var result = await userManager.UpdateAsync(targetUser);
//    if (!result.Succeeded)
//    {
//        return Results.BadRequest("ไม่สามารถอัปเดตสิทธิ์ได้");
//    }

//    return Results.Ok($"ปรับปรุงสิทธิ์ของผู้ใช้ {targetUser.UserName} สำเร็จ");
//})
//.WithName("Faculty_Admin5")
//.WithTags("bool");

//app.MapGet("/auth", [Authorize] (HttpContext context) =>
//{
//    //จุดสังเกตการเรียกใช้งานคือ ClaimTypes ดึงมาจาก GET ในส่วนของ Login ซึ่งจะเก็บสิทธ์การเข้าถึงไว้ทั้งหมด และการเรียกใช้นั้นสามารถแสดงได้ทั้ง ประเภทและ ค่าของตัวมันเองได้ โดยการ .ตามด้วยตัวแปลที่สร้างขึ้นมา
//    var full = context.User.FindFirst(ClaimTypes.Name)?.Value;
//    var n = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
//    var gn = context.User.FindFirst(ClaimTypes.GivenName)?.Value;
//    var sn = context.User.FindFirst(ClaimTypes.Surname)?.Value;
//    var un = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
//    return Results.Ok($"Authenticated: {full} : {un} : {n} : {gn} : {sn}");
//});

//// อ็อบเจกต์ที่เก็บข้อมูลตัวตนของผู้ใช้(Identity) และสิทธิ์ต่างๆ(Claims) หลังจากผ่านการล็อกอินสำเร็จ
//app.MapGet("/userclaim", [Authorize] (ClaimsPrincipal user) =>
//{
//    // สร้างตัวแปร string เพื่อเก็บข้อมูลของผู้ใช้
//    string uInfo = user.Identity?.Name ?? "...";
//    // ถ้า user.Identity.Name มีค่า ให้ใช้ค่านั้น, ถ้าไม่มีค่าให้ใส่ "..."

//    uInfo += ", claim : ";
//    // เพิ่มคำว่า "claim : " ลงในข้อความ เพื่อเริ่มต้นแสดงข้อมูล Claims

//    // วนลูปผ่าน Claims ทั้งหมด (ยกเว้น Role)
//    foreach (var claim in user.Claims.Where(c => c.Type != ClaimTypes.Role))
//    {
//        uInfo += "t = " + claim.Type + " v = " + claim.Value + ", ";
//        // t = ชนิดของ Claim (Type), v = ค่าของ Claim (Value)
//    }

//    return Results.Ok(uInfo);
//    // ส่งค่าข้อมูลทั้งหมดกลับในรูปแบบ HTTP 200 OK พร้อมกับข้อความ uInfo
//});

//app.MapGet("/userroles", [Authorize] (ClaimsPrincipal user) =>
//{
//    string roleInfo = "Roles: ";
//    // ดึงเฉพาะ Claims ที่เป็น Role
//    var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role);

//    foreach (var role in roles)
//    {
//        roleInfo += "t = " + role.Type + " v = " + role.Value + ", ";
//    }

//    return Results.Ok(roleInfo);
//    // ส่งข้อมูล Roles กลับในรูปแบบ HTTP 200 OK
//});

//app.MapGet("/admin_only", [Authorize(Roles = "admin")] () => "คุณคือ admin");
//app.MapGet("/hero_or_admin", [Authorize(Roles = "admin,hero")] () => "คุณเป็นทั้ง admin และ Hero");

//// กำหนดเส้นทาง (route) `/reset` สำหรับการรีเซ็ตรหัสผ่าน
//app.MapGet("/reset", async (UserManager<IdenUser> userManager) =>
//{
//    // ค้นหาผู้ใช้งานที่มีชื่อ "admin" ในฐานข้อมูล
//    var item = await userManager.FindByNameAsync("admin");

//    // หากไม่พบผู้ใช้งานชื่อ "admin" ให้ส่งสถานะ 404 Not Found พร้อมข้อความ
//    if (item == null)
//        return Results.NotFound("ไม่พบผู้ใช้งานชื่อนี้");

//    // ลบรหัสผ่านปัจจุบันของผู้ใช้งาน "admin"
//    await userManager.RemovePasswordAsync(item);

//    // กำหนดรหัสผ่านใหม่ให้กับผู้ใช้งาน "admin" เป็น "Ilink111!"
//    await userManager.AddPasswordAsync(item, "Ilink111!");

//    // ส่งสถานะ 200 OK กลับไปเมื่อการรีเซ็ตรหัสผ่านสำเร็จ
//    return Results.Ok();

//});

//// หลังจากที่สร้าง admin มาแล้ว เราควรที่จะ Authorize เป็น admin
//app.MapPost("/register", [Authorize(Roles = "admin, dev")]
//async (UserManager<IdenUser> userManager, RoleManager<IdentityRole> roleManager, RegisterRequest user) =>
//{
//    // ทำการเช็คว่าชื่อและรหัสผ่านเป็นค่าว่างหรือไม่ จะแสดงข้อความ Bad ออกมา
//    if (user.username == null || user.password == null)
//        return Results.BadRequest("กรุณากรอกข้อมูลให้ครบถ้วน");

//    // หา User ใน DB 
//    var u = await userManager.FindByNameAsync(user.username);
//    // กรณีหาสมัครแล้วชื่อตรงกับคนที่เคยสมัครก่อนจะแสดงข้อความ Bad ออกมา
//    if (u != null)
//        return Results.BadRequest("มีผู้ใช้อยู่แล้ว");

//    var newUser = new IdenUser
//    {
//        FullRealName = user.fullRealName,
//        UserName = user.username,
//        GivenName = user.givenName,
//        Surname = user.surname,
//        PositionName = user.positionName,
//        HRDepartmentId = user.hrdepartmentId,
//        HRDepartmentName = user.hrdepartmentName
//    };
//    // ใช้ User ที่อินเจ็กเข้ามา ทำการสร้าง User ขึ้นมาโดยการนำ Oject newUser ทั้งหมดมายัดใส่
//    var createUserResult = await userManager.CreateAsync(newUser, user.password);
//    // เช็คว่าผลลัพท์เป็นยังไง ถ้าไม่สำเร็จ จะแสดง Bad ออกมา
//    // ส่วนมากที่ไม่ผ่านจะเป็นไปทาง รหัสผ่านความปลอดภัยต่ำเกินไป
//    if (!createUserResult.Succeeded)
//        return Results.BadRequest(createUserResult.ToString());
//    // User.roles มาจากการ Post เข้ามา
//    foreach (var role in user.roles)
//    {
//        // ทำการใช้ RoleManager ในการ CreateAsync ในการสร้าง Role ถ้า Role ไม่มี ก็จะทำการสร้างใน Db ใหม่ขึ้นมาเลย
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            var createRoleResult = await roleManager.CreateAsync(new IdentityRole(role));
//        }
//        // ส่วนนี้จะทำการเอา User ไปประกบกับ Role คือ ตำแหน่งของผู้ใช้
//        await userManager.AddToRoleAsync(newUser, role);
//    }
//    return Results.Ok();

//});

//// API สำหรับการล็อกอิน (JWT Token)
//app.MapPost("/login", [AllowAnonymous]
//async (UserManager<IdenUser> userManager, RoleManager<IdentityRole> roleMgrManager, LoginRequest user) =>
//{
//    //เช็คว่าชื่อกับรหัสผ่านตรงกันไหม
//    var u = await userManager.FindByNameAsync(user.username);
//    if (u == null)
//        return Results.NotFound("ไม่พบผู้ใช้งานชื่อนี้");
//    if (!await userManager.CheckPasswordAsync(u, user.password))
//        return Results.BadRequest("รหัสผ่านไม่ถูกต้อง");

//    // สร้าง Claims
//    var claims = new List<Claim>();
//    claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.username)); // ชื่อผู้ใช้
//    claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // Unique Token ID
//    claims.Add(new Claim(ClaimTypes.Name, u.FullRealName));
//    claims.Add(new Claim(ClaimTypes.GivenName, u.GivenName)); // ชื่อจริง
//    claims.Add(new Claim(ClaimTypes.Surname, u.Surname)); // นามสกุล
//    //claims.Add(new Claim("id", u.Id));
//    claims.Add(new Claim("IsFinancialDepPowerUser", u.IsFinancialDepPowerUser?.ToString() ?? "false"));
//    claims.Add(new Claim("IsFinDepUser", u.IsFinDepUser?.ToString() ?? "false"));
//    claims.Add(new Claim("IsPlanDepPowerUser", u.IsPlanDepPowerUser?.ToString() ?? "false"));
//    claims.Add(new Claim("IsProcureDepPowerUser", u.IsProcureDepPowerUser?.ToString() ?? "false"));
//    claims.Add(new Claim("IsHRDepPowerUser", u.IsHRDepPowerUser?.ToString() ?? "false"));
//    // เนื่องจากข้อมูลบางส่วนมีโครงสร้างซับซ้อน เช่น List<Tuple<int, List<int>>> และ HashSet<Tuple<int, HashSet<int>>> เราจึงต้องแปลงเป็น JSON เพื่อให้สามารถเก็บใน Claims ได้.
//    // เพิ่ม Claims สำหรับ Permission Lists
//    claims.Add(new Claim("HR_HRDepartmentPermission", JsonSerializer.Serialize(u.HR_HRDepartmentPermission)));
//    claims.Add(new Claim("Plan_DepartmentPermission", JsonSerializer.Serialize(u.Plan_DepartmentPermission)));
//    claims.Add(new Claim("Plan_DepPowerByPlanPermission", JsonSerializer.Serialize(u.Plan_DepPowerByPlanPermission)));
//    // เพิ่ม Claims สำหรับ Permission HashSets
//    claims.Add(new Claim("HR_HRDepartmentPermissionHashSet", JsonSerializer.Serialize(u.HR_HRDepartmentPermissionHashSet)));
//    claims.Add(new Claim("Plan_DepartmentPermissionHashSet", JsonSerializer.Serialize(u.Plan_DepartmentPermissionHashSet)));
//    claims.Add(new Claim("Plan_DepPowerByPlanPermissionHashSet", JsonSerializer.Serialize(u.Plan_DepPowerByPlanPermissionHashSet)));
//    // เพิ่ม Claims สำหรับ ListNumbers และ HashSetNumbers
//    claims.Add(new Claim("ListNumbers", JsonSerializer.Serialize(u.ListNumbers)));
//    claims.Add(new Claim("HashSetNumbers", JsonSerializer.Serialize(u.HashSetNumbers)));
//    // เพิ่ม Claims สำหรับ YearNumbers
//    claims.Add(new Claim("YearNumbers", JsonSerializer.Serialize(u.YearNumbers)));

//    // ดึงข้อมูล ตำแหน่งมาจาก Db
//    var roles = await userManager.GetRolesAsync(u);
//    foreach (var role in roles)
//        claims.Add(new Claim("roles", role));

//    // สร้าง JWT Token
//    var uniqueKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty);
//    var secureKey = new SymmetricSecurityKey(uniqueKey);
//    var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha512);
//    var jwtTokenHandler = new JwtSecurityTokenHandler();
//    var tokenDescriptor = new SecurityTokenDescriptor
//    {
//        Subject = new ClaimsIdentity(claims),
//        Expires = DateTime.Now.AddMinutes(15), // อายุ Token
//        Audience = builder.Configuration["Jwt:Audience"],
//        Issuer = builder.Configuration["Jwt:Issuer"],
//        SigningCredentials = credentials
//    };
//    var token = jwtTokenHandler.CreateToken(tokenDescriptor);
//    var jwtToken = jwtTokenHandler.WriteToken(token);
//    return Results.Ok(new LoginRespose(u.Id, user.username, jwtToken));
//});

//// สร้าง API Endpoint สำหรับ Refresh Token
//app.MapPost("/refresh-token", [Authorize]
//async (HttpContext httpContext, UserManager<IdenUser> userManager, IConfiguration config) =>
//{
//    // ดึงข้อมูล Claims จาก Token ปัจจุบันที่ผู้ใช้ส่งมา
//    var userClaims = httpContext.User;

//    // ตรวจสอบว่ามี Claim "id" ใน Token หรือไม่
//    var userId = userClaims.FindFirst("id")?.Value;

//    // ถ้าไม่มี userId หรือ Token ไม่ถูกต้อง ให้แจ้ง BadRequest
//    if (string.IsNullOrEmpty(userId))
//    {
//        return Results.BadRequest(new { Message = "Invalid token or user ID not found." });
//    }

//    // ค้นหาผู้ใช้จากฐานข้อมูลตาม userId
//    var user = await userManager.FindByIdAsync(userId);

//    // ถ้าหาผู้ใช้ไม่เจอ ให้แจ้งว่าไม่พบผู้ใช้งาน
//    if (user == null)
//    {
//        return Results.NotFound(new { Message = "User not found." });
//    }

//    // สร้าง Claims ใหม่ (ใช้ข้อมูลจากฐานข้อมูลของผู้ใช้)
//    var claims = new List<Claim>
//    {
//        new Claim(JwtRegisteredClaimNames.Sub, user.UserName!), // ใส่ Username ของผู้ใช้
//        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // สร้าง Unique Token ID
//        new Claim(ClaimTypes.Name, user.FullRealName ?? string.Empty), // ใส่ชื่อจริงของผู้ใช้
//        // new Claim("id", user.Id), // ID ของผู้ใช้ (ถูกคอมเมนต์ออก)
//        new Claim("IsFinancialDepPowerUser", user.IsFinancialDepPowerUser?.ToString() ?? "false"), // ตรวจสอบสิทธิ์ของผู้ใช้
//        new Claim("IsFinDepUser", user.IsFinDepUser?.ToString() ?? "false"),
//        new Claim("IsPlanDepPowerUser", user.IsPlanDepPowerUser?.ToString() ?? "false"),
//        new Claim("IsProcureDepPowerUser", user.IsProcureDepPowerUser?.ToString() ?? "false"),
//        new Claim("IsHRDepPowerUser", user.IsHRDepPowerUser?.ToString() ?? "false")
//    };

//    // แปลงข้อมูลที่ซับซ้อนเป็น JSON ก่อนเพิ่มเข้า Claims
//    if (user.HR_HRDepartmentPermission != null)
//        claims.Add(new Claim("HR_HRDepartmentPermission", JsonSerializer.Serialize(user.HR_HRDepartmentPermission)));

//    if (user.Plan_DepartmentPermission != null)
//        claims.Add(new Claim("Plan_DepartmentPermission", JsonSerializer.Serialize(user.Plan_DepartmentPermission)));

//    if (user.Plan_DepPowerByPlanPermission != null)
//        claims.Add(new Claim("Plan_DepPowerByPlanPermission", JsonSerializer.Serialize(user.Plan_DepPowerByPlanPermission)));

//    if (user.HR_HRDepartmentPermissionHashSet != null)
//        claims.Add(new Claim("HR_HRDepartmentPermissionHashSet", JsonSerializer.Serialize(user.HR_HRDepartmentPermissionHashSet)));

//    if (user.Plan_DepartmentPermissionHashSet != null)
//        claims.Add(new Claim("Plan_DepartmentPermissionHashSet", JsonSerializer.Serialize(user.Plan_DepartmentPermissionHashSet)));

//    if (user.Plan_DepPowerByPlanPermissionHashSet != null)
//        claims.Add(new Claim("Plan_DepPowerByPlanPermissionHashSet", JsonSerializer.Serialize(user.Plan_DepPowerByPlanPermissionHashSet)));

//    if (user.ListNumbers != null)
//        claims.Add(new Claim("ListNumbers", JsonSerializer.Serialize(user.ListNumbers)));

//    if (user.HashSetNumbers != null)
//        claims.Add(new Claim("HashSetNumbers", JsonSerializer.Serialize(user.HashSetNumbers)));

//    if (user.YearNumbers != null)
//        claims.Add(new Claim("YearNumbers", JsonSerializer.Serialize(user.YearNumbers)));

//    // ดึง Role ของผู้ใช้จากฐานข้อมูลและเพิ่มลงใน Claims
//    var roles = await userManager.GetRolesAsync(user);
//    foreach (var role in roles)
//    {
//        claims.Add(new Claim(ClaimTypes.Role, role)); // เพิ่ม Role ของผู้ใช้ลงใน Claims
//    }

//    // สร้าง Key และ Credentials สำหรับ Token
//    var uniqueKey = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty); // ดึง Secret Key จาก config
//    var secureKey = new SymmetricSecurityKey(uniqueKey); // สร้าง Security Key
//    var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha512); // ใช้ HMAC SHA512 ในการ Sign Token

//    // ตั้งค่าข้อมูลของ Token ใหม่
//    var tokenDescriptor = new SecurityTokenDescriptor
//    {
//        Subject = new ClaimsIdentity(claims), // ใส่ Claims ที่สร้างไว้
//        Expires = DateTime.Now.AddMinutes(15), // กำหนดอายุของ Token ใหม่เป็น 15 นาที
//        Audience = config["Jwt:Audience"], // กำหนด Audience จาก appsettings.json
//        Issuer = config["Jwt:Issuer"], // กำหนด Issuer จาก appsettings.json
//        SigningCredentials = credentials // ใส่ Signing Credentials ที่สร้างไว้
//    };

//    // สร้างตัวช่วยจัดการ JWT Token
//    var jwtTokenHandler = new JwtSecurityTokenHandler();
//    var token = jwtTokenHandler.CreateToken(tokenDescriptor); // สร้าง Token ใหม่
//    var jwtToken = jwtTokenHandler.WriteToken(token); // แปลง Token เป็น String

//    // ส่ง Token ใหม่กลับไปยัง Client
//    return Results.Ok(new { Message = "Token refreshed successfully.", Token = jwtToken });
//});

//app.MapGet("/Get_Data", [Authorize] async (HttpContext httpContext, UserManager<IdenUser> userManager) =>
//{
//    // ดึงข้อมูล UserId จาก HttpContext.User
//    var userId = httpContext.User.Claims.FirstOrDefault(c => c.Type == "id")?.Value;

//    // ตรวจสอบว่ามี userId หรือไม่
//    if (string.IsNullOrWhiteSpace(userId))
//        return Results.BadRequest("Cannot identify the current user.");

//    // ค้นหาผู้ใช้งานในฐานข้อมูล
//    var user = await userManager.FindByIdAsync(userId);
//    if (user == null)
//        return Results.NotFound("User not found.");

//    // คืนค่าข้อมูลทั้งหมดของผู้ใช้งาน
//    return Results.Ok(new
//    {
//        Id = user.Id,
//        FullRealName = user.FullRealName,
//        GivenName = user.GivenName,
//        UserName = user.UserName,
//        Surname = user.Surname,
//        PositionName = user.PositionName,
//        HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
//        Plan_DepartmentPermission = user.Plan_DepartmentPermission,
//        Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission,
//        HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
//        Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
//        Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet,
//        IsFinancialDepPowerUser = user.IsFinancialDepPowerUser,
//        IsFinDepUser = user.IsFinDepUser,
//        IsPlanDepPowerUser = user.IsPlanDepPowerUser,
//        IsProcureDepPowerUser = user.IsProcureDepPowerUser,
//        IsHRDepPowerUser = user.IsHRDepPowerUser,
//        YearNumbersJson = user.YearNumbers,
//        Plan_DepPowerUserPermission = user.ListNumbers,
//        Plan_DepPowerUserPermission_HashSet = user.HashSetNumbers
//    });
//});

//app.MapGet("/Get_DataUserAll", [Authorize(Roles = "admin, dev")] (UserManager<IdenUser> userManager) =>
//{
//    // ดึงข้อมูลผู้ใช้ทั้งหมดจากฐานข้อมูล
//    var users = userManager.Users.ToList();

//    // ตรวจสอบว่ามีผู้ใช้หรือไม่
//    if (!users.Any())
//        return Results.NotFound("No users found.");

//    // คืนค่าข้อมูลทั้งหมดของผู้ใช้
//    var result = users.Select(user => new
//    {
//        Id = user.Id,
//        HRDepartmentId = user.HRDepartmentId,
//        FullRealName = user.FullRealName,
//        GivenName = user.GivenName,
//        UserName = user.UserName,
//        Surname = user.Surname,
//        PositionName = user.PositionName,
//        HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
//        Plan_DepartmentPermission = user.Plan_DepartmentPermission,
//        Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission,
//        HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
//        Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
//        Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet,
//        IsFinancialDepPowerUser = user.IsFinancialDepPowerUser,
//        IsFinDepUser = user.IsFinDepUser,
//        IsPlanDepPowerUser = user.IsPlanDepPowerUser,
//        IsProcureDepPowerUser = user.IsProcureDepPowerUser,
//        IsHRDepPowerUser = user.IsHRDepPowerUser,
//        YearNumbersJson = user.YearNumbers,
//        Plan_DepPowerUserPermission = user.ListNumbers,
//        Plan_DepPowerUserPermission_HashSet = user.HashSetNumbers
//    });

//    return Results.Ok(result);
//});

//// เพิ่ม ตำแหน่งเพิ่มได้เฉพราะ แอดมินเท่านั้น
//app.MapPost("/AddRole", [Authorize(Roles = "admin")]
//async (RoleManager<IdentityRole> roleManager, string rolename, string userId, UserManager<IdenUser> userManager) =>
//{
//    var user = await userManager.FindByIdAsync(userId);
//    // ถ้าไม่พบ id ของผู้ใช้ คนนั้นจะแสดง Erorr 404
//    if (user == null)
//    {
//        return Results.NotFound(new { message = "ไม่พบผู้ใช้" });
//    }

//    if (!await roleManager.RoleExistsAsync(rolename))
//    {
//        var createRoleResult = await roleManager.CreateAsync(new IdentityRole(rolename));
//        return Results.Ok();
//    }
//    return Results.Conflict();
//});

//// จุดสังเกต คือ วิธีการลบข้อมูลนั้นจำเป็นต้องใส่ id ของผู้ใช้ คนนั้นๆ 
//app.MapDelete("/Delete User", [Authorize(Roles = "admin")] async (string id, UserManager<IdenUser> userManager) =>
//{
//    // ค้นหาผู้ใช้จากฐานข้อมูลโดยใช้ Id
//    var user = await userManager.FindByIdAsync(id);
//    // ถ้าไม่พบ id ของผู้ใช้ คนนั้นจะแสดง Erorr 404
//    if (user == null)
//    {
//        return Results.NotFound(new { message = "ไม่พบผู้ใช้" });
//    }

//    // ลบผู้ใช้ ลบแค่ส่วนของข้อมูลผู้ใช้ แต่จะไม่ไปลบ Role
//    var result = await userManager.DeleteAsync(user);
//    if (!result.Succeeded)
//    {
//        return Results.BadRequest(new { message = "ไม่สามารถลบผู้ใช้ได้", errors = result.Errors });
//    }

//    return Results.Ok(new { message = "ลบผู้ใช้ สำรเร็จ" });
//});


//// 🚀 เริ่มต้นแอปพลิเคชัน
//app.Run();

//record LoginRequest(string username, string password);
//record LoginRespose(string userId, string username, string token);
//record RegisterRequest(string username, string password, string givenName, string surname, string[] roles, string fullRealName, string positionName, string hrdepartmentName, int hrdepartmentId);