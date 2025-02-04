using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace Identity_jwt.Domain
{
    public class IdenUser : IdentityUser
    {
        public int? HRDepartmentId { get; set; }
        public string HRDepartmentName { get; set; } = "";
        public string FullRealName { get; set; } = "";
        public string PositionName { get; set; } = "";
        public string GivenName { get; set; } = "";
        public string Surname { get; set; } = "";
        public List<int> HR_HRDepartmentPermission { get; set; } = new List<int>();
        public List<int> Plan_DepartmentPermission { get; set; } = new List<int>();
        public List<int> Plan_DepPowerByPlanPermission { get; set; } = new List<int>();
        public HashSet<int> HR_HRDepartmentPermissionHashSet { get; set; } = new HashSet<int>();
        public HashSet<int> Plan_DepartmentPermissionHashSet { get; set; } = new HashSet<int>();
        public HashSet<int> Plan_DepPowerByPlanPermissionHashSet { get; set; } = new HashSet<int>();

        public bool? IsFinancialDepPowerUser { get; set; } = null;
        public bool? IsFinDepUser { get; set; } = null;
        public bool? IsPlanDepPowerUser { get; set; } = null;
        public bool? IsProcureDepPowerUser { get; set; } = null;
        public bool? IsHRDepPowerUser { get; set; } = null;
        public string YearNumbersJson { get; set; } = "[]"; // เก็บ JSON String

        // ฟิลด์สำหรับเก็บ JSON String ที่ใช้กับ List<int, List<int>>
        public string Plan_DepPowerUserPermission { get; set; } = "[]";

        // ฟิลด์สำหรับเก็บ JSON String ที่ใช้กับ HashSet<int, List<int>>
        public string Plan_DepPowerUserPermission_HashSet { get; set; } = "[]";

        [NotMapped] // ระบุว่าไม่ต้องแมป Property นี้ในฐานข้อมูล
        public List<Tuple<int, List<int>>> ListNumbers
        {
            get => string.IsNullOrEmpty(Plan_DepPowerUserPermission) || Plan_DepPowerUserPermission == "[]"
                // ถ้า JSON ว่างหรือไม่มีข้อมูล ให้คืนค่า List เปล่า
                ? new List<Tuple<int, List<int>>>()
                // ถ้ามีข้อมูลใน JSON ให้ Deserialize เป็น List
                : DeserializeListFormat(Plan_DepPowerUserPermission);

            set => Plan_DepPowerUserPermission = SerializeListFormat(value); // Serialize เมื่อมีการตั้งค่าใหม่
        }

        // ฟังก์ชันสำหรับ Serialize ข้อมูลจาก List<int, List<int>> เป็น JSON String
        private string SerializeListFormat(List<Tuple<int, List<int>>> data)
        {
            if (data == null || data.Count == 0)
                return "[]"; // ถ้าไม่มีข้อมูล ให้คืนค่า "[]"

            // แปลง List เป็น JSON String โดยใช้รูปแบบ [ปี, (เลข1, เลข2, ...)]
            return string.Join(", ", data.Select(tuple =>
                $"[{tuple.Item1}, ({string.Join(", ", tuple.Item2)})]"
            ));
        }

        // ฟังก์ชันสำหรับ Deserialize JSON String กลับเป็น List<int, List<int>>
        private List<Tuple<int, List<int>>> DeserializeListFormat(string data)
        {
            var result = new List<Tuple<int, List<int>>>(); // สร้าง List ว่างเปล่า

            if (string.IsNullOrWhiteSpace(data) || data == "[]")
                return result; // ถ้าไม่มีข้อมูล ให้คืนค่า List เปล่า

            // แยก JSON String เป็นรายการข้อมูล โดยตัด "], " ออก
            var entries = data.Split("], ", StringSplitOptions.TrimEntries).Select(e => e.Trim('[', ']'));

            foreach (var entry in entries)
            {
                var parts = entry.Split(", (", 2); // แยกข้อมูลเป็น [Key, Value]
                if (parts.Length == 2)
                {
                    var key = int.Parse(parts[0]); // แปลงส่วนแรกเป็นปี (Key)
                                                   // แปลงส่วนที่สองเป็น List<int> (Value)
                    var values = parts[1].Trim(')').Split(", ").Select(int.Parse).ToList();
                    result.Add(new Tuple<int, List<int>>(key, values)); // เพิ่มใน List
                }
            }

            return result;
        }

        // Property สำหรับจัดการ HashSet<int, HashSet<int>>
        [NotMapped] // ระบุว่าไม่ต้องแมป Property นี้ในฐานข้อมูล
        public HashSet<Tuple<int, HashSet<int>>> HashSetNumbers
        {
            get => string.IsNullOrEmpty(Plan_DepPowerUserPermission_HashSet) || Plan_DepPowerUserPermission_HashSet == "[]"
                // ถ้า JSON ว่างหรือไม่มีข้อมูล ให้คืนค่า HashSet เปล่า
                ? new HashSet<Tuple<int, HashSet<int>>>()
                // ถ้ามีข้อมูลใน JSON ให้ Deserialize เป็น HashSet
                : DeserializeHashSetFormat(Plan_DepPowerUserPermission_HashSet);

            set => Plan_DepPowerUserPermission_HashSet = SerializeHashSetFormat(value); // Serialize เมื่อมีการตั้งค่าใหม่
        }

        // ฟังก์ชันสำหรับ Serialize ข้อมูลจาก HashSet<Tuple<int, HashSet<int>>> เป็น JSON String
        private string SerializeHashSetFormat(HashSet<Tuple<int, HashSet<int>>> data)
        {
            if (data == null || data.Count == 0)
                return "[]"; // ถ้าไม่มีข้อมูล ให้คืนค่า "[]"

            // แปลง HashSet เป็น JSON String โดยใช้รูปแบบ [ปี, (เลข1, เลข2, ...)]
            return string.Join(", ", data.Select(tuple =>
                $"[{tuple.Item1}, ({string.Join(", ", tuple.Item2)})]"
            ));
        }

        // ฟังก์ชันสำหรับ Deserialize JSON String กลับเป็น HashSet<Tuple<int, HashSet<int>>>
        private HashSet<Tuple<int, HashSet<int>>> DeserializeHashSetFormat(string data)
        {
            var result = new HashSet<Tuple<int, HashSet<int>>>(); // สร้าง HashSet ว่างเปล่า

            if (string.IsNullOrWhiteSpace(data) || data == "[]")
                return result; // ถ้าไม่มีข้อมูล ให้คืนค่า HashSet เปล่า

            // แยก JSON String เป็นรายการข้อมูล โดยตัด "], " ออก
            var entries = data.Split("], ", StringSplitOptions.TrimEntries).Select(e => e.Trim('[', ']'));

            foreach (var entry in entries)
            {
                var parts = entry.Split(", (", 2); // แยกข้อมูลเป็น [Key, Value]
                if (parts.Length == 2)
                {
                    var key = int.Parse(parts[0]); // แปลงส่วนแรกเป็นปี (Key)
                                                   // แปลงส่วนที่สองเป็น HashSet<int> (Value)
                    var values = parts[1].Trim(')').Split(", ").Select(int.Parse).ToHashSet();
                    result.Add(new Tuple<int, HashSet<int>>(key, values)); // เพิ่มใน HashSet
                }
            }

            return result;
        }

        [NotMapped]
        public Dictionary<int, List<int>> YearNumbers
        {
            get => string.IsNullOrEmpty(YearNumbersJson) || YearNumbersJson == "[]"
                ? new Dictionary<int, List<int>>()
                : DeserializeCustomFormat(YearNumbersJson);

            set => YearNumbersJson = SerializeToCustomFormat(value);
        }

        // ฟังก์ชันสำหรับแปลงข้อมูลให้เป็นรูปแบบ [ปี, (เลข1, เลข2, ...)]
        private string SerializeToCustomFormat(Dictionary<int, List<int>> data)
        {
            if (data == null || data.Count == 0)
                return "[]"; // กรณีไม่มีข้อมูล ให้ใช้ []

            return string.Join(", ", data.Select(kvp =>
                $"[{kvp.Key}, ({string.Join(", ", kvp.Value)})]"
            ));
        }

        // ฟังก์ชันสำหรับแปลง JSON กลับเป็น Dictionary<int, List<int>>
        private Dictionary<int, List<int>> DeserializeCustomFormat(string data)
        {
            var result = new Dictionary<int, List<int>>();

            if (string.IsNullOrWhiteSpace(data) || data == "[]")
                return result;

            // แยกข้อมูลตาม ", " สำหรับข้อมูลแบบ [ปี, (เลข1, เลข2, ...)]
            var entries = data.Split("], ", StringSplitOptions.TrimEntries).Select(e => e.Trim('[', ']'));

            foreach (var entry in entries)
            {
                var parts = entry.Split(", (", 2); // แยกปีและรายการ
                if (parts.Length == 2)
                {
                    var year = int.Parse(parts[0]);
                    var numbers = parts[1].Trim(')').Split(", ").Select(int.Parse).ToList();
                    result[year] = numbers;
                }
            }

            return result;
        }
    }
}
