using Driver_Report.Core.Interface;
using Microsoft.AspNetCore.Components;

namespace Driver_Report.Components.Shared
{
    public partial class SafetyBadge
    {
        [Inject]
        public IReportService ReportService { get; set; } = default!;


        [Parameter]
        public int? DriverId { get; set; }

        [Parameter]
        public int? KnownReportCount { get; set; }

        private string badgeClass = "bg-secondary";
        private string badgeText = "Loading...";

        protected override void OnParametersSet()
        {
            int reportCount = 0;

            if (KnownReportCount.HasValue)
            {
                reportCount = KnownReportCount.Value;
            }

            else if (DriverId.HasValue)
            {
                var pagedData = ReportService.GetDriverReports(DriverId.Value, 1, 1);
                reportCount = pagedData.TotalCount;
            }


            if (reportCount == 0)
            {
                badgeClass = "bg-success";
                badgeText = "Clean Record";
            }
            else if (reportCount < 3)
            {
                badgeClass = "bg-warning text-dark";
                badgeText = $"Caution ({reportCount} Reports)";
            }
            else
            {
                badgeClass = "bg-danger";
                badgeText = $"High Risk ({reportCount} Reports)";
            }
        }
    }
}
