using Identity_jwt.Domain;

namespace Identity_minimal_API.Domain.Enum
{
    public class Plan_DepPowerUserPermission
    {
        public Guid Id { get; set; } = Guid.NewGuid(); // สร้าง ID อัตโนมัติ
        public int DepartmentId { get; set; }

        // เก็บ List<int> เป็น JSON ในฐานข้อมูล
        public List<int>? PermissionId { get; set; }
        public string IdenUserId { get; set; } = string.Empty;
    }
}
