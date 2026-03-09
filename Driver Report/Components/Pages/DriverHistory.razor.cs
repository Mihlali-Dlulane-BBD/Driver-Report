using Driver_Report.Core.Interface;
using Driver_Report.Core.Models;
using Microsoft.AspNetCore.Components;

namespace Driver_Report.Components.Pages
{
    public partial class DriverHistory
    {

        [Parameter]
        public int DriverId { get; set; }

        [Inject]
        public NavigationManager NavManager { get; set; } = default!;

        [Inject]
        public IReportService ReportService { get; set; } = default!;


        private Driver? driver;

        private PagedResult<Report> pagedData = new PagedResult<Report>();
        private int currentPage = 1;
        private int pageSize = 5;
        private bool isLoading = true;

        protected override void OnInitialized()
        {
            driver = ReportService.GetDriver(DriverId);
            if (driver != null)
            {
                LoadPage(1);
            }
            else
            {
                isLoading = false;
            }
        }


        private void LoadPage(int pageNumber)
        {
            if (pageNumber < 1 || (pagedData.TotalPages > 0 && pageNumber > pagedData.TotalPages)) return;
            currentPage = pageNumber;
            pagedData = ReportService.GetDriverReports(DriverId, currentPage, pageSize);
            isLoading = false;
        }

        private void GoBack()
        {
            NavManager.NavigateTo("/");
        }
    }
}
