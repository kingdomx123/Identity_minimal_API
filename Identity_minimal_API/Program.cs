using Identity_jwt.Data;
using Identity_jwt.Domain;
using Identity_minimal_API.Endpoints.SEC.Plan;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString =
    builder.Configuration.GetConnectionString("iLinkDBConnect")
        ?? throw new InvalidOperationException("Connection string 'iLinkDBConnect' not found.");

builder.Services.AddSqlServer<IdenDbcontext>(builder.Configuration.GetConnectionString("InfolinkDbConnect"));
builder.Services.AddIdentity<IdenUser, IdentityRole>()
              .AddEntityFrameworkStores<IdenDbcontext>()
              .AddDefaultTokenProviders();


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(option =>
{

    option.SwaggerDoc("Main", new OpenApiInfo { Title = "หน้าหลัก", Version = "Main" });
    option.SwaggerDoc("Faculty_Admin", new OpenApiInfo { Title = "admin คณะ", Version = "Faculty_Admin" });
    option.SwaggerDoc("Plan_DepPowerUserPermission", new OpenApiInfo { Title = "ปีงบประมาณ ,หน่วยงาน", Version = "Plan_DepPowerUserPermission" });
    option.SwaggerDoc("Agency_ID", new OpenApiInfo { Title = "id หน่วยงาน", Version = "Agency_ID" });
    option.SwaggerDoc("SEC", new OpenApiInfo { Title = "SEC", Version = "SEC" });
    option.SwaggerDoc("SEC_PlanCore", new OpenApiInfo { Title = "SEC_PlanCore", Version = "SEC_PlanCore" });
    option.SwaggerDoc("SEC_ResponsiblePreson", new OpenApiInfo { Title = "SEC_ResponsiblePreson", Version = "SEC_ResponsiblePreson" });
    option.SwaggerDoc("SEC_PlanActivitie", new OpenApiInfo { Title = "SEC_PlanActivitie", Version = "SEC_PlanActivitie" });
    option.SwaggerDoc("SEC_PlanItem", new OpenApiInfo { Title = "SEC_PlanItem", Version = "SEC_PlanItem" });
    option.SwaggerDoc("SEC_WorkingArea", new OpenApiInfo { Title = "SEC_WorkingArea", Version = "SEC_WorkingArea" });
    option.SwaggerDoc("SEC_PlanFile", new OpenApiInfo { Title = "SEC_PlanFile", Version = "SEC_PlanFile" }); 
    option.SwaggerDoc("SEC_StrategicIndicator", new OpenApiInfo { Title = "SEC_StrategicIndicator", Version = "SEC_StrategicIndicator" });


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
        ValidateLifetime = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("PlanCoreAccess", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "IsFinancialDepPowerUser" && c.Value == "true") ||
                (c.Type == "IsFinDepUser" && c.Value == "true")
            )
        )
    );
});

builder.Services.Configure<JsonOptions>(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {

        options.SwaggerEndpoint("/swagger/Main/swagger.json", "Main");
        options.SwaggerEndpoint("/swagger/Faculty_Admin/swagger.json", "Faculty_Admin");
        options.SwaggerEndpoint("/swagger/Agency_ID/swagger.json", "Agency_ID");
        options.SwaggerEndpoint("/swagger/SEC/swagger.json", "SEC");
        options.SwaggerEndpoint("/swagger/SEC_PlanCore/swagger.json", "SEC_PlanCore");
        options.SwaggerEndpoint("/swagger/SEC_PlanActivitie/swagger.json", "SEC_PlanActivitie");
        options.SwaggerEndpoint("/swagger/SEC_PlanItem/swagger.json", "SEC_PlanItem");
        options.SwaggerEndpoint("/swagger/SEC_ResponsiblePreson/swagger.json", "SEC_ResponsiblePreson");
        options.SwaggerEndpoint("/swagger/Plan_DepPowerUserPermission/swagger.json", "Plan_DepPowerUserPermission");
        options.SwaggerEndpoint("/swagger/SEC_WorkingArea/swagger.json", "SEC_WorkingArea");
        options.SwaggerEndpoint("/swagger/SEC_PlanFile/swagger.json", "SEC_PlanFile");
        options.SwaggerEndpoint("/swagger/SEC_StrategicIndicator/swagger.json", "SEC_StrategicIndicator");

        // ทำให้ Authorization ไม่ออกจากระบบเองแม้จะเปลี่ยนหน้าต่างหรือยกเลิกการรันระบบ
        options.EnablePersistAuthorization();
    });
}

app.UseAuthentication();
app.UseAuthorization();

app.MapHashSetEndpoints();
app.MapAgency_id_HashsetEndpoints();
app.MapYearNumbersEndpoints();
app.MapFacultyAdminEndpoints();
app.MapAgencyPermissionsEndpoints();
app.MapPlan_DepPowerUserPermission_Endpoints();
app.MapHomemainEndpoints(builder);
app.MapSEC_Plan_Department_Endpoints(connectionString);
app.MapSEC_HREndpoints(connectionString);
app.MapSEC_PlanCores_Endpoints(connectionString);
app.MapSEC_ResponsiblePreson_Endpoints(connectionString);
app.MapSEC_PlanActivities_Endpoints(connectionString);
app.MapSEC_PlanItems_Endpoints(connectionString);
app.MapSEC_WorkingAreasn_Endpoints(connectionString);
app.MapSEC_PlanFile_Endpoints(connectionString);
app.MapSEC_PlanCore_Activities_Items_Endpoints(connectionString);
app.MapSEC_StrategicIndicator_Endpoints(connectionString);


app.Run();