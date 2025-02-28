using iLinkDomain.DataAccess.SEC.Plan;
using iLinkDomain.Model.SEC.Plan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.EntityFrameworkCore;

namespace Identity_minimal_API.Endpoints.SEC.Plan
{
    public static class SEC_PlanFile_Endpoints
    {
        public static void MapSEC_PlanFile_Endpoints(this WebApplication app, string connectionString)
        {
            app.MapPost("/upload", [AllowAnonymous] async (IFormFile file, int uploadUserId, int month) =>
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
            .WithTags("PlanFile")
            .WithGroupName("SEC_PlanFile")
            .RequireAuthorization()             // ต้องมี JWT เพื่อเข้าถึง
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200, typeof(object));

            app.MapGet("/downloadFile/{fileName}", (string fileName) =>
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
                var filePath = Path.Combine(uploadsFolder, fileName);

                if (!System.IO.File.Exists(filePath))
                {
                    return Results.NotFound("ไม่พบไฟล์ที่ต้องการดาวน์โหลด");
                }

                var fileStream = System.IO.File.OpenRead(filePath);
                var contentType = GetContentType(filePath);

                // ✅ เพิ่ม Content-Disposition เพื่อบังคับดาวน์โหลด
                return Results.File(fileStream, contentType, fileName, enableRangeProcessing: true);
            })
            .WithTags("PlanFile")
            .WithGroupName("SEC_PlanFile");


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
            .WithTags("PlanFile")
            .WithGroupName("SEC_PlanFile")
            .RequireAuthorization()              // เปิดใช้งาน JWT Authentication
            .DisableAntiforgery()                // ปิด CSRF แต่ยังใช้ JWT           // ต้องมี JWT เพื่อเข้าถึง
            .Accepts<IFormFile>("multipart/form-data")
            .Produces(200, typeof(object));

            //app.MapPost("/uploadNotPlanFile", [AllowAnonymous] async (IFormFile file) =>
            //{
            //    if (file == null || file.Length == 0)
            //    {
            //        return Results.BadRequest("กรุณาเลือกไฟล์");
            //    }

            //    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
            //    Directory.CreateDirectory(uploadsFolder);

            //    var filePath = Path.Combine(uploadsFolder, file.FileName);
            //    using var stream = new FileStream(filePath, FileMode.Create);
            //    await file.CopyToAsync(stream);

            //    return Results.Ok(new { FileName = file.FileName, FilePath = $"/uploads/{file.FileName}" });
            //})
            //.WithTags("PlanFile")
            //.WithGroupName("SEC_PlanFile")
            //.DisableAntiforgery()              // ปิดการตรวจสอบ CSRF
            //.Accepts<IFormFile>("multipart/form-data")
            //.Produces(200, typeof(object));


            static string GetContentType(string path)
            {
                var provider = new FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(path, out var contentType))
                {
                    contentType = "application/octet-stream"; // ค่า default สำหรับไฟล์ที่ไม่รู้ประเภท
                }
                return contentType;
            }
        }

    }
}
