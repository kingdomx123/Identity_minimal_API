using Identity_jwt.Domain;

namespace Identity_minimal_API.Domain.Enum
{
    public class Plan_DepPowerUserPermission_Hashset
    {
        public Guid Id { get; set; } // Primary key
        public int DepartmentIdHashset { get; set; } // Represents the department ID
        public List<int>? PermissionIdHashset { get; set; } // Represents permissions (as a list)
        public string IdenUserId { get; set; } = string.Empty;
    }
}
