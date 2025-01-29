using Identity_minimal_API.Domain.Enum;

namespace Identity_minimal_API.Model // ปรับ namespace ให้ตรงกับโปรเจกต์ของคุณ
{
    public class Plan_DepPowerUserPermissionDictionary
    {
        public Guid Id { get; set; } // Primary key

        public int DepartmentId { get; set; } // Department ID (Key)

        public Guid PlanDepPermissionHashsetId { get; set; } // Foreign Key

        // Navigation Property เพื่อเชื่อมกับ Plan_DepPowerUserPermission_Hashset
        public Plan_DepPowerUserPermission_Hashset? PlanDepPermissionHashset { get; set; }
    }
}
