using Identity_jwt.Domain;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Identity_jwt.Data
{
    public class IdenDbcontext : IdentityDbContext<IdenUser, IdentityRole, string>
    {
        public IdenDbcontext(DbContextOptions<IdenDbcontext> options) : base(options) { }

        public DbSet<IdenUser> IdenUsers => Set<IdenUser>();
        // ตั้งให้ set ให้ส่งข้อมูลไปยัง ตารางนี้
        protected override void OnModelCreating(ModelBuilder builder)
        {
            // เรียก base.OnModelCreating(builder) เพื่อให้ EF Core ตั้งค่าพื้นฐานก่อน
            base.OnModelCreating(builder);

            // กำหนด ValueComparer สำหรับ HashSet<int>
            // ValueComparer เป็นตัวช่วยให้ EF Core เข้าใจวิธีเปรียบเทียบค่าของ HashSet<int>
            var intHashSetComparer = new ValueComparer<HashSet<int>>(
                // ฟังก์ชันสำหรับเปรียบเทียบ HashSet<int> ว่าสองชุดเท่ากันหรือไม่
                (c1, c2) => c1.SetEquals(c2),
                // ฟังก์ชันสำหรับสร้าง HashCode จาก HashSet<int> (ช่วยเพิ่มประสิทธิภาพ)
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                // ฟังก์ชันสำหรับสร้างสำเนาใหม่ของ HashSet<int>
                c => new HashSet<int>(c)
            );

            // กำหนดค่าคอนฟิกสำหรับ property HR_HRDepartmentPermissionHashSet
            builder.Entity<IdenUser>()
                .Property(e => e.HR_HRDepartmentPermissionHashSet)
                .HasConversion(
                    // ฟังก์ชันแปลง HashSet<int> -> String เพื่อเก็บในฐานข้อมูล
                    v => "[" + string.Join(',', v) + "]", // แปลง HashSet<int> เป็น string โดยเพิ่ม [] รอบค่าที่แปลง
                                                          // ฟังก์ชันแปลง String -> HashSet<int> เมื่อดึงข้อมูลออกมา
                    v => string.IsNullOrWhiteSpace(v) || v == "[]" // กรณีค่าเป็นค่าว่างหรือ "[]"
                        ? new HashSet<int>() // สร้าง HashSet<int> ว่าง
                        : new HashSet<int>(
                            v.Trim('[', ']') // ตัด [] ออก
                             .Split(',', StringSplitOptions.RemoveEmptyEntries) // แยกค่าตาม ',' และลบค่าที่ว่าง
                             .Select(int.Parse) // แปลงค่า string เป็น int
                        )
                )
                // ตั้งค่า ValueComparer ที่กำหนดไว้ก่อนหน้าเพื่อให้ EF Core รู้วิธีเปรียบเทียบค่า
                .Metadata.SetValueComparer(intHashSetComparer);

            // กำหนดค่าคอนฟิกสำหรับ property Plan_DepartmentPermissionHashSet
            builder.Entity<IdenUser>()
                .Property(e => e.Plan_DepartmentPermissionHashSet)
                .HasConversion(
                    v => "[" + string.Join(',', v) + "]", // แปลง HashSet<int> เป็น string
                    v => string.IsNullOrWhiteSpace(v) || v == "[]" // กรณีค่าว่าง
                        ? new HashSet<int>() // คืน HashSet<int> ว่าง
                        : new HashSet<int>(
                            v.Trim('[', ']') // ตัด []
                             .Split(',', StringSplitOptions.RemoveEmptyEntries) // แยกค่าด้วย ','
                             .Select(int.Parse) // แปลง string เป็น int
                        )
                )
                .Metadata.SetValueComparer(intHashSetComparer);

            // กำหนดค่าคอนฟิกสำหรับ property Plan_DepPowerByPlanPermissionHashSet
            builder.Entity<IdenUser>()
                .Property(e => e.Plan_DepPowerByPlanPermissionHashSet)
                .HasConversion(
                    v => "[" + string.Join(',', v) + "]", // แปลง HashSet<int> เป็น string
                    v => string.IsNullOrWhiteSpace(v) || v == "[]" // กรณีค่าว่าง
                        ? new HashSet<int>() // คืน HashSet<int> ว่าง
                        : new HashSet<int>(
                            v.Trim('[', ']') // ตัด []
                             .Split(',', StringSplitOptions.RemoveEmptyEntries) // แยกค่าด้วย ','
                             .Select(int.Parse) // แปลง string เป็น int
                        )
                )
                .Metadata.SetValueComparer(intHashSetComparer);
        }
    }
}
