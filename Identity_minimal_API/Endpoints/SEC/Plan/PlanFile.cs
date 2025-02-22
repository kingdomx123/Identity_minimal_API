using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanFile_Endpoints
    {
        public static void MapSEC_PlanFile_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/upload", async (IFormFile file, int uploadUserId, int month) =>
            {
                using (PlanDbContext context = new PlanDbContext(connectionString))
                {
                    if (file == null || file.Length == 0)
                    {
                        return Results.BadRequest("กรุณาเลือกไฟล์");
                    }

                    var uploadsFolder = @"F:\Project_Phayao\PlanFile";

                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, file.FileName);

                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    // ✅ สร้างข้อมูล `PlanFile` และบันทึกลงฐานข้อมูล
                    var planFile = new PlanFile
                    {
                        Name = file.FileName,
                        Size = (int)file.Length,
                        Path = filePath,
                        UploadDate = DateTime.UtcNow,
                        UploadUserId = uploadUserId,
                        Type = file.ContentType,
                        Active = true,
                        Month = month
                    };

                    context.PlanFiles.Add(planFile);
                    if (context.Entry(planFile).State == EntityState.Added)
                    {
                        context.SaveChanges();
                    }

                    return Results.Ok(new { Message = "อัปโหลดสำเร็จ", FileId = planFile });
                }
            })
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200, typeof(object));

            app.MapPost("/uploadNotPlanFile", async (IFormFile file) =>
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest("กรุณาเลือกไฟล์");
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                Directory.CreateDirectory(uploadsFolder);

                var filePath = Path.Combine(uploadsFolder, file.FileName);
                using var stream = new FileStream(filePath, FileMode.Create);
                await file.CopyToAsync(stream);

                return Results.Ok(new { FileName = file.FileName, FilePath = $"/uploads/{file.FileName}" });
            })
            .RequireAuthorization()             // ต้องมี JWT เพื่อเข้าถึง
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200, typeof(object));

        }
    }
}
