namespace Identity_minimal_API.Model
{
    public class UpdateAccessRequest
    {
        public bool? IsFinancialDepPowerUser { get; set; }
        public bool? IsFinDepUser { get; set; }
        public bool? IsPlanDepPowerUser { get; set; }
        public bool? IsProcureDepPowerUser { get; set; }
        public bool? IsHRDepPowerUser { get; set; }
    }

}
