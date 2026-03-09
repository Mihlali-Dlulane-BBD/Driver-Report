namespace Driver_Report.Core.Models
{
    public class Report
    {
        public required string ReportReason { get; set; }
        public required DateOnly ReportDate {  get; set; }
        public required int DriverId { get; set; }
    }
}
