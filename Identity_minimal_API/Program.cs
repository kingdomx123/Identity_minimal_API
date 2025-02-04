using Identity_jwt.Data;
using Identity_jwt.Domain;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// 🔗 เพิ่มการเชื่อมต่อฐานข้อมูล
builder.Services.AddSqlServer<IdenDbcontext>(builder.Configuration.GetConnectionString("DefaultConnection"));
builder.Services.AddIdentity<IdenUser, IdentityRole>()
              .AddEntityFrameworkStores<IdenDbcontext>()
              .AddDefaultTokenProviders();

// 🔧 เพิ่มบริการ Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{
    //คำสั่งเพิ่มไปยังอีกหน้า และกำหนดหัวข้อ
    option.SwaggerDoc("Main", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Main" });
    option.SwaggerDoc("Faculty_Admin", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Faculty_Admin" });
    option.SwaggerDoc("Plan_DepPowerUserPermission", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Plan_DepPowerUserPermission" });
    option.SwaggerDoc("Agency_ID", new OpenApiInfo { Title = "Minimal API JWT และ Identity", Version = "Agency_ID" });
    option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "กรุณาระบุค่า Token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    option.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
          {
              new OpenApiSecurityScheme
              {
                  Reference = new OpenApiReference
                  {
                      Type = ReferenceType.SecurityScheme,
                      Id = "Bearer"
                  }
              },
              new string[]{}
          }
    });
});

// เพิ่มบริการการกำหนดค่า JWT
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateAudience = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? string.Empty)),
        ValidateIssuerSigningKey = true,
        ValidateLifetime = true, // ✅ เปิดการตรวจสอบหมดอายุของ Token
        ClockSkew = TimeSpan.Zero // ⏳ ปิดการยืดเวลาอายุของ Token (Default คือ 5 นาที)
    };
});

// 🛠️ เพิ่มบริการ Authorization
builder.Services.AddAuthorization();

var app = builder.Build();

// 🛠️ ตั้งค่า Swagger สำหรับโหมด Development
if (app.Environment.IsDevelopment()) // เช็คว่าแอปกำลังรันในโหมด Development หรือไม่
{
    app.UseSwagger(); // เปิดใช้งาน Swagger Middleware
    app.UseSwaggerUI(options =>
    {
        // 📌 เพิ่ม API Documents หลายตัวให้สามารถเลือกดูได้ใน Swagger UI
        options.SwaggerEndpoint("/swagger/Main/swagger.json", "Main"); // เอกสาร API สำหรับ "Main"
        options.SwaggerEndpoint("/swagger/Faculty_Admin/swagger.json", "Faculty_Admin"); // เอกสาร API สำหรับ "Faculty_Admin"
        options.SwaggerEndpoint("/swagger/Plan_DepPowerUserPermission/swagger.json", "Plan_DepPowerUserPermission"); // เอกสาร API สำหรับ "Plan_DepPowerUserPermission"
        options.SwaggerEndpoint("/swagger/Agency_ID/swagger.json", "Agency_ID"); // เอกสาร API สำหรับ "Agency_ID"

        // 🔒 ทำให้ Authorization (Token) คงอยู่เมื่อลองเปลี่ยนหน้า Swagger UI
        options.EnablePersistAuthorization();
    });
}

// 🔒 Middleware สำหรับ HTTPS
app.UseAuthentication();
app.UseAuthorization();

// เรียกใช้ HashSet API ที่แยกออกไป
app.MapHashSetEndpoints();
app.MapAgency_id_HashsetEndpoints();
// เรียกใช้ YearNumbers API
app.MapYearNumbersEndpoints();
app.MapFacultyAdminEndpoints();
app.MapAgencyPermissionsEndpoints();
app.MapPlan_DepPowerUserPermission_Endpoints();
app.MapHomemainEndpoints(builder);

// 🚀 เริ่มต้นแอปพลิเคชัน
app.Run();

record LoginRequest(string username, string password);
record LoginRespose(string userId, string username, string token);
record RegisterRequest(string username, string password, string givenName, string surname, string[] roles, string fullRealName, string positionName, string hrdepartmentName, int hrdepartmentId);