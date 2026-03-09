using Driver_Report.Core.Models;

namespace Driver_Report.Core.Interface
{
    public interface IReportService
    {
        void AddReport(Report report);
        int AddDriver(Driver driver);
        Report? GetReport(int reportId);
        Driver? GetDriver(int driverId);

        Driver? SearchDriver(string searchTerm);
        List<Report> GetDriverReports(int driverId);
        void SubmitNewIncident(string licensePlate, string? firstName, string? lastName, string? platformName, string reason);
        PagedResult<Report> GetDriverReports(int driverId, int pageNumber, int pageSize);
    }
}
