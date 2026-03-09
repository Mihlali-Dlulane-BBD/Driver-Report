namespace Driver_Report.Core.Models
{
    public class Driver
    {   
        public int DriverId { get; set; }
        public required string DriverLicensePlate { get;set; }
        public string? DriverFirstName { get;set; }
        public string? DriverLastName { get; set; }
    }
}
