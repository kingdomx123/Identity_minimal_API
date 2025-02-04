using System.IdentityModel.Tokens.Jwt;
using Identity_jwt.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.IdentityModel.Tokens;
using System.Text;

public static class HomeMainEndpoints
{
    public static IEndpointRouteBuilder MapHomemainEndpoints(this IEndpointRouteBuilder app, WebApplicationBuilder builder)
    {
        app.MapGet("/auth", [Authorize] (HttpContext context) =>
        {
            //จุดสังเกตการเรียกใช้งานคือ ClaimTypes ดึงมาจาก GET ในส่วนของ Login ซึ่งจะเก็บสิทธ์การเข้าถึงไว้ทั้งหมด และการเรียกใช้นั้นสามารถแสดงได้ทั้ง ประเภทและ ค่าของตัวมันเองได้ โดยการ .ตามด้วยตัวแปลที่สร้างขึ้นมา
            var full = context.User.FindFirst(ClaimTypes.Name)?.Value;
            var n = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var gn = context.User.FindFirst(ClaimTypes.GivenName)?.Value;
            var sn = context.User.FindFirst(ClaimTypes.Surname)?.Value;
            var un = context.User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
            return Results.Ok($"Authenticated: {full} : {un} : {n} : {gn} : {sn}");
        }).WithGroupName("Main");

        // อ็อบเจกต์ที่เก็บข้อมูลตัวตนของผู้ใช้(Identity) และสิทธิ์ต่างๆ(Claims) หลังจากผ่านการล็อกอินสำเร็จ
        app.MapGet("/userclaim", [Authorize] (ClaimsPrincipal user) =>
        {
            // สร้างตัวแปร string เพื่อเก็บข้อมูลของผู้ใช้
            string uInfo = user.Identity?.Name ?? "...";
            // ถ้า user.Identity.Name มีค่า ให้ใช้ค่านั้น, ถ้าไม่มีค่าให้ใส่ "..."

            uInfo += ", claim : ";
            // เพิ่มคำว่า "claim : " ลงในข้อความ เพื่อเริ่มต้นแสดงข้อมูล Claims

            // วนลูปผ่าน Claims ทั้งหมด (ยกเว้น Role)
            foreach (var claim in user.Claims.Where(c => c.Type != ClaimTypes.Role))
            {
                uInfo += "t = " + claim.Type + " v = " + claim.Value + ", ";
                // t = ชนิดของ Claim (Type), v = ค่าของ Claim (Value)
            }

            return Results.Ok(uInfo);
            // ส่งค่าข้อมูลทั้งหมดกลับในรูปแบบ HTTP 200 OK พร้อมกับข้อความ uInfo
        }).WithGroupName("Main");

        app.MapGet("/userroles", [Authorize] (ClaimsPrincipal user) =>
        {
            string roleInfo = "Roles: ";
            // ดึงเฉพาะ Claims ที่เป็น Role
            var roles = user.Claims.Where(c => c.Type == ClaimTypes.Role);

            foreach (var role in roles)
            {
                roleInfo += "t = " + role.Type + " v = " + role.Value + ", ";
            }

            return Results.Ok(roleInfo);
            // ส่งข้อมูล Roles กลับในรูปแบบ HTTP 200 OK
        }).WithGroupName("Main");

        app.MapGet("/admin_only", [Authorize(Roles = "admin")] () => "คุณคือ admin").WithGroupName("Main");
        app.MapGet("/hero_or_admin", [Authorize(Roles = "admin,hero")] () => "คุณเป็นทั้ง admin และ Hero").WithGroupName("Main");

        // กำหนดเส้นทาง (route) `/reset` สำหรับการรีเซ็ตรหัสผ่าน
        app.MapGet("/reset", async (UserManager<IdenUser> userManager) =>
        {
            // ค้นหาผู้ใช้งานที่มีชื่อ "admin" ในฐานข้อมูล
            var item = await userManager.FindByNameAsync("admin");

            // หากไม่พบผู้ใช้งานชื่อ "admin" ให้ส่งสถานะ 404 Not Found พร้อมข้อความ
            if (item == null)
                return Results.NotFound("ไม่พบผู้ใช้งานชื่อนี้");

            // ลบรหัสผ่านปัจจุบันของผู้ใช้งาน "admin"
            await userManager.RemovePasswordAsync(item);

            // กำหนดรหัสผ่านใหม่ให้กับผู้ใช้งาน "admin" เป็น "Ilink111!"
            await userManager.AddPasswordAsync(item, "Ilink111!");

            // ส่งสถานะ 200 OK กลับไปเมื่อการรีเซ็ตรหัสผ่านสำเร็จ
            return Results.Ok();

        }).WithGroupName("Main");

        // หลังจากที่สร้าง admin มาแล้ว เราควรที่จะ Authorize เป็น admin
        app.MapPost("/register", [Authorize(Roles = "admin, dev")]
        async (UserManager<IdenUser> userManager, RoleManager<IdentityRole> roleManager, RegisterRequest user) =>
        {
            // ทำการเช็คว่าชื่อและรหัสผ่านเป็นค่าว่างหรือไม่ จะแสดงข้อความ Bad ออกมา
            if (user.username == null || user.password == null)
                return Results.BadRequest("กรุณากรอกข้อมูลให้ครบถ้วน");

            // หา User ใน DB 
            var u = await userManager.FindByNameAsync(user.username);
            // กรณีหาสมัครแล้วชื่อตรงกับคนที่เคยสมัครก่อนจะแสดงข้อความ Bad ออกมา
            if (u != null)
                return Results.BadRequest("มีผู้ใช้อยู่แล้ว");

            var newUser = new IdenUser
            {
                FullRealName = user.fullRealName,
                UserName = user.username,
                GivenName = user.givenName,
                Surname = user.surname,
                PositionName = user.positionName,
                HRDepartmentId = user.hrdepartmentId,
                HRDepartmentName = user.hrdepartmentName
            };
            // ใช้ User ที่อินเจ็กเข้ามา ทำการสร้าง User ขึ้นมาโดยการนำ Oject newUser ทั้งหมดมายัดใส่
            var createUserResult = await userManager.CreateAsync(newUser, user.password);
            // เช็คว่าผลลัพท์เป็นยังไง ถ้าไม่สำเร็จ จะแสดง Bad ออกมา
            // ส่วนมากที่ไม่ผ่านจะเป็นไปทาง รหัสผ่านความปลอดภัยต่ำเกินไป
            if (!createUserResult.Succeeded)
                return Results.BadRequest(createUserResult.ToString());
            // User.roles มาจากการ Post เข้ามา
            foreach (var role in user.roles)
            {
                // ทำการใช้ RoleManager ในการ CreateAsync ในการสร้าง Role ถ้า Role ไม่มี ก็จะทำการสร้างใน Db ใหม่ขึ้นมาเลย
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var createRoleResult = await roleManager.CreateAsync(new IdentityRole(role));
                }
                // ส่วนนี้จะทำการเอา User ไปประกบกับ Role คือ ตำแหน่งของผู้ใช้
                await userManager.AddToRoleAsync(newUser, role);
            }
            return Results.Ok();

        }).WithGroupName("Main");

        // API สำหรับการล็อกอิน (JWT Token)
        app.MapPost("/login", [AllowAnonymous] async (UserManager<IdenUser> userManager, RoleManager<IdentityRole> roleMgrManager, LoginRequest user) =>
        {
            //เช็คว่าชื่อกับรหัสผ่านตรงกันไหม
            var u = await userManager.FindByNameAsync(user.username);
            if (u == null)
                return Results.NotFound("ไม่พบผู้ใช้งานชื่อนี้");
            if (!await userManager.CheckPasswordAsync(u, user.password))
                return Results.BadRequest("รหัสผ่านไม่ถูกต้อง");

            // สร้าง Claims
            var claims = new List<Claim>();
            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.username)); // ชื่อผู้ใช้
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())); // Unique Token ID
            claims.Add(new Claim(ClaimTypes.Name, u.FullRealName));
            claims.Add(new Claim(ClaimTypes.GivenName, u.GivenName)); // ชื่อจริง
            claims.Add(new Claim(ClaimTypes.Surname, u.Surname)); // นามสกุล
            claims.Add(new Claim("IsFinancialDepPowerUser", u.IsFinancialDepPowerUser?.ToString() ?? "false"));
            claims.Add(new Claim("IsFinDepUser", u.IsFinDepUser?.ToString() ?? "false"));
            claims.Add(new Claim("IsPlanDepPowerUser", u.IsPlanDepPowerUser?.ToString() ?? "false"));
            claims.Add(new Claim("IsProcureDepPowerUser", u.IsProcureDepPowerUser?.ToString() ?? "false"));
            claims.Add(new Claim("IsHRDepPowerUser", u.IsHRDepPowerUser?.ToString() ?? "false"));
            claims.Add(new Claim("HR_HRDepartmentPermission", JsonSerializer.Serialize(u.HR_HRDepartmentPermission)));
            claims.Add(new Claim("Plan_DepartmentPermission", JsonSerializer.Serialize(u.Plan_DepartmentPermission)));
            claims.Add(new Claim("Plan_DepPowerByPlanPermission", JsonSerializer.Serialize(u.Plan_DepPowerByPlanPermission)));
            claims.Add(new Claim("HR_HRDepartmentPermissionHashSet", JsonSerializer.Serialize(u.HR_HRDepartmentPermissionHashSet)));
            claims.Add(new Claim("Plan_DepartmentPermissionHashSet", JsonSerializer.Serialize(u.Plan_DepartmentPermissionHashSet)));
            claims.Add(new Claim("Plan_DepPowerByPlanPermissionHashSet", JsonSerializer.Serialize(u.Plan_DepPowerByPlanPermissionHashSet)));
            claims.Add(new Claim("ListNumbers", JsonSerializer.Serialize(u.ListNumbers)));
            claims.Add(new Claim("HashSetNumbers", JsonSerializer.Serialize(u.HashSetNumbers)));
            claims.Add(new Claim("YearNumbers", JsonSerializer.Serialize(u.YearNumbers)));

            // ดึงข้อมูล ตำแหน่งมาจาก Db
            var roles = await userManager.GetRolesAsync(u);
            foreach (var role in roles)
                claims.Add(new Claim("roles", role));

            // สร้าง JWT Token
            var uniqueKey = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty);
            var secureKey = new SymmetricSecurityKey(uniqueKey);
            var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha512);
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(1), // อายุ Token
                Audience = builder.Configuration["Jwt:Audience"],
                Issuer = builder.Configuration["Jwt:Issuer"],
                SigningCredentials = credentials
            };
            var token = jwtTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtTokenHandler.WriteToken(token);
            return Results.Ok(new LoginRespose(u.Id, user.username, jwtToken));
        }).WithGroupName("Main");

        // สร้าง API Endpoint สำหรับ Refresh Token
        app.MapPost("/refresh-token", [Authorize]
        async (HttpContext httpContext, UserManager<IdenUser> userManager, IConfiguration config) =>
        {
            // ดึงข้อมูล Claims จาก Token ปัจจุบันที่ผู้ใช้ส่งมา
            var userClaims = httpContext.User;

            // ตรวจสอบว่ามี Claim "id" ใน Token หรือไม่
            var userId = userClaims.FindFirst("id")?.Value;

            // ถ้าไม่มี userId หรือ Token ไม่ถูกต้อง ให้แจ้ง BadRequest
            if (string.IsNullOrEmpty(userId))
            {
                return Results.BadRequest(new { Message = "Invalid token or user ID not found." });
            }

            // ค้นหาผู้ใช้จากฐานข้อมูลตาม userId
            var user = await userManager.FindByIdAsync(userId);

            // ถ้าหาผู้ใช้ไม่เจอ ให้แจ้งว่าไม่พบผู้ใช้งาน
            if (user == null)
            {
                return Results.NotFound(new { Message = "User not found." });
            }

            // สร้าง Claims ใหม่ (ใช้ข้อมูลจากฐานข้อมูลของผู้ใช้)
            var claims = new List<Claim>
    {
        new Claim(JwtRegisteredClaimNames.Sub, user.UserName!), // ใส่ Username ของผู้ใช้
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // สร้าง Unique Token ID
        new Claim(ClaimTypes.Name, user.FullRealName ?? string.Empty), // ใส่ชื่อจริงของผู้ใช้
        // new Claim("id", user.Id), // ID ของผู้ใช้ (ถูกคอมเมนต์ออก)
        new Claim("IsFinancialDepPowerUser", user.IsFinancialDepPowerUser?.ToString() ?? "false"), // ตรวจสอบสิทธิ์ของผู้ใช้
        new Claim("IsFinDepUser", user.IsFinDepUser?.ToString() ?? "false"),
        new Claim("IsPlanDepPowerUser", user.IsPlanDepPowerUser?.ToString() ?? "false"),
        new Claim("IsProcureDepPowerUser", user.IsProcureDepPowerUser?.ToString() ?? "false"),
        new Claim("IsHRDepPowerUser", user.IsHRDepPowerUser?.ToString() ?? "false")
    };

            // แปลงข้อมูลที่ซับซ้อนเป็น JSON ก่อนเพิ่มเข้า Claims
            if (user.HR_HRDepartmentPermission != null)
                claims.Add(new Claim("HR_HRDepartmentPermission", JsonSerializer.Serialize(user.HR_HRDepartmentPermission)));

            if (user.Plan_DepartmentPermission != null)
                claims.Add(new Claim("Plan_DepartmentPermission", JsonSerializer.Serialize(user.Plan_DepartmentPermission)));

            if (user.Plan_DepPowerByPlanPermission != null)
                claims.Add(new Claim("Plan_DepPowerByPlanPermission", JsonSerializer.Serialize(user.Plan_DepPowerByPlanPermission)));

            if (user.HR_HRDepartmentPermissionHashSet != null)
                claims.Add(new Claim("HR_HRDepartmentPermissionHashSet", JsonSerializer.Serialize(user.HR_HRDepartmentPermissionHashSet)));

            if (user.Plan_DepartmentPermissionHashSet != null)
                claims.Add(new Claim("Plan_DepartmentPermissionHashSet", JsonSerializer.Serialize(user.Plan_DepartmentPermissionHashSet)));

            if (user.Plan_DepPowerByPlanPermissionHashSet != null)
                claims.Add(new Claim("Plan_DepPowerByPlanPermissionHashSet", JsonSerializer.Serialize(user.Plan_DepPowerByPlanPermissionHashSet)));

            if (user.ListNumbers != null)
                claims.Add(new Claim("ListNumbers", JsonSerializer.Serialize(user.ListNumbers)));

            if (user.HashSetNumbers != null)
                claims.Add(new Claim("HashSetNumbers", JsonSerializer.Serialize(user.HashSetNumbers)));

            if (user.YearNumbers != null)
                claims.Add(new Claim("YearNumbers", JsonSerializer.Serialize(user.YearNumbers)));

            // ดึง Role ของผู้ใช้จากฐานข้อมูลและเพิ่มลงใน Claims
            var roles = await userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role)); // เพิ่ม Role ของผู้ใช้ลงใน Claims
            }

            // สร้าง Key และ Credentials สำหรับ Token
            var uniqueKey = Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? string.Empty); // ดึง Secret Key จาก config
            var secureKey = new SymmetricSecurityKey(uniqueKey); // สร้าง Security Key
            var credentials = new SigningCredentials(secureKey, SecurityAlgorithms.HmacSha512); // ใช้ HMAC SHA512 ในการ Sign Token

            // ตั้งค่าข้อมูลของ Token ใหม่
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // ใส่ Claims ที่สร้างไว้
                Expires = DateTime.Now.AddMinutes(15), // กำหนดอายุของ Token ใหม่เป็น 15 นาที
                Audience = config["Jwt:Audience"], // กำหนด Audience จาก appsettings.json
                Issuer = config["Jwt:Issuer"], // กำหนด Issuer จาก appsettings.json
                SigningCredentials = credentials // ใส่ Signing Credentials ที่สร้างไว้
            };

            // สร้างตัวช่วยจัดการ JWT Token
            var jwtTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtTokenHandler.CreateToken(tokenDescriptor); // สร้าง Token ใหม่
            var jwtToken = jwtTokenHandler.WriteToken(token); // แปลง Token เป็น String

            // ส่ง Token ใหม่กลับไปยัง Client
            return Results.Ok(new { Message = "Token refreshed successfully.", Token = jwtToken });
        }).WithGroupName("Main");

        app.MapGet("/Get_Data", [Authorize] async (HttpContext httpContext, UserManager<IdenUser> userManager) =>
        {
            // ดึงข้อมูล UserId จาก HttpContext.User
            var userName = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // ตรวจสอบว่ามี userId หรือไม่
            if (string.IsNullOrWhiteSpace(userName))
                return Results.BadRequest("Cannot identify the current user.");

            // ค้นหาผู้ใช้งานในฐานข้อมูล
            var user = await userManager.FindByNameAsync(userName);
            if (user == null)
                return Results.NotFound("User not found.");

            // คืนค่าข้อมูลทั้งหมดของผู้ใช้งาน
            return Results.Ok(new
            {
                Id = user.Id,
                FullRealName = user.FullRealName,
                GivenName = user.GivenName,
                UserName = user.UserName,
                Surname = user.Surname,
                PositionName = user.PositionName,
                HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
                Plan_DepartmentPermission = user.Plan_DepartmentPermission,
                Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission,
                HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
                Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
                Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet,
                IsFinancialDepPowerUser = user.IsFinancialDepPowerUser,
                IsFinDepUser = user.IsFinDepUser,
                IsPlanDepPowerUser = user.IsPlanDepPowerUser,
                IsProcureDepPowerUser = user.IsProcureDepPowerUser,
                IsHRDepPowerUser = user.IsHRDepPowerUser,
                YearNumbersJson = user.YearNumbers,
                Plan_DepPowerUserPermission = user.ListNumbers,
                Plan_DepPowerUserPermission_HashSet = user.HashSetNumbers
            });
        }).WithGroupName("Main");

        app.MapGet("/Get_DataUserAll", [Authorize(Roles = "admin, dev")] (UserManager<IdenUser> userManager) =>
        {
            // ดึงข้อมูลผู้ใช้ทั้งหมดจากฐานข้อมูล
            var users = userManager.Users.ToList();

            // ตรวจสอบว่ามีผู้ใช้หรือไม่
            if (!users.Any())
                return Results.NotFound("No users found.");

            // คืนค่าข้อมูลทั้งหมดของผู้ใช้
            var result = users.Select(user => new
            {
                Id = user.Id,
                HRDepartmentId = user.HRDepartmentId,
                FullRealName = user.FullRealName,
                GivenName = user.GivenName,
                UserName = user.UserName,
                Surname = user.Surname,
                PositionName = user.PositionName,
                HR_HRDepartmentPermission = user.HR_HRDepartmentPermission,
                Plan_DepartmentPermission = user.Plan_DepartmentPermission,
                Plan_DepPowerByPlanPermission = user.Plan_DepPowerByPlanPermission,
                HR_HRDepartmentPermissionHashSet = user.HR_HRDepartmentPermissionHashSet,
                Plan_DepartmentPermissionHashSet = user.Plan_DepartmentPermissionHashSet,
                Plan_DepPowerByPlanPermissionHashSet = user.Plan_DepPowerByPlanPermissionHashSet,
                IsFinancialDepPowerUser = user.IsFinancialDepPowerUser,
                IsFinDepUser = user.IsFinDepUser,
                IsPlanDepPowerUser = user.IsPlanDepPowerUser,
                IsProcureDepPowerUser = user.IsProcureDepPowerUser,
                IsHRDepPowerUser = user.IsHRDepPowerUser,
                YearNumbersJson = user.YearNumbers,
                Plan_DepPowerUserPermission = user.ListNumbers,
                Plan_DepPowerUserPermission_HashSet = user.HashSetNumbers
            });

            return Results.Ok(result);
        }).WithGroupName("Main");

        // เพิ่ม ตำแหน่งเพิ่มได้เฉพราะ แอดมินเท่านั้น
        app.MapPost("/AddRole", [Authorize(Roles = "admin")]
        async (RoleManager<IdentityRole> roleManager, string rolename, string userId, UserManager<IdenUser> userManager) =>
        {
            var user = await userManager.FindByIdAsync(userId);
            // ถ้าไม่พบ id ของผู้ใช้ คนนั้นจะแสดง Erorr 404
            if (user == null)
            {
                return Results.NotFound(new { message = "ไม่พบผู้ใช้" });
            }

            if (!await roleManager.RoleExistsAsync(rolename))
            {
                var createRoleResult = await roleManager.CreateAsync(new IdentityRole(rolename));
                return Results.Ok();
            }
            return Results.Conflict();
        }).WithGroupName("Main");

        // จุดสังเกต คือ วิธีการลบข้อมูลนั้นจำเป็นต้องใส่ id ของผู้ใช้ คนนั้นๆ 
        app.MapDelete("/Delete User", [Authorize(Roles = "admin")] async (string id, UserManager<IdenUser> userManager) =>
        {
            // ค้นหาผู้ใช้จากฐานข้อมูลโดยใช้ Id
            var user = await userManager.FindByIdAsync(id);
            // ถ้าไม่พบ id ของผู้ใช้ คนนั้นจะแสดง Erorr 404
            if (user == null)
            {
                return Results.NotFound(new { message = "ไม่พบผู้ใช้" });
            }

            // ลบผู้ใช้ ลบแค่ส่วนของข้อมูลผู้ใช้ แต่จะไม่ไปลบ Role
            var result = await userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return Results.BadRequest(new { message = "ไม่สามารถลบผู้ใช้ได้", errors = result.Errors });
            }

            return Results.Ok(new { message = "ลบผู้ใช้ สำรเร็จ" });
        }).WithGroupName("Main");

        return app;
    }
}
