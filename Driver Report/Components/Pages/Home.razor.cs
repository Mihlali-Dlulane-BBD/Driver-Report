using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Driver_Report.Core.Interface;
using Driver_Report.Core.Models;


namespace Driver_Report.Components.Pages
{
    public partial class Home
    {
        [Inject]
        public NavigationManager NavManager { get; set; } = default!;

        [Inject]
        public IReportService ReportService { get; set; } = default!;

       
        private string searchQuery = "";
        private bool searchPerformed = false;
        private Driver? foundDriver;

       
        private void Search()
        {
            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                foundDriver = ReportService.SearchDriver(searchQuery);
                searchPerformed = true;
            }
            else
            {
                searchPerformed = false;
                foundDriver = null;
            }
        }

        private void HandleEnterKey(KeyboardEventArgs e)
        {
            if (e.Key == "Enter") Search();
        }

        private void GoToAddReport()
        {
            NavManager.NavigateTo("/add-report");
        }

        private void ViewHistory(int driverId)
        {
            NavManager.NavigateTo($"/driver-history/{driverId}");
        }
    }
}
