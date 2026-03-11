using Driver_Report.Core.Interface;
using Driver_Report.Core.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Text.RegularExpressions;


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

        private bool isLinkMode = false;
        private string rideLinkUrl = "";
        private bool isAnalyzing = false;
        private string? linkErrorMessage;

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

        private async Task AnalyzeLink()
        {
            if (string.IsNullOrWhiteSpace(rideLinkUrl)) return;

            isAnalyzing = true;
            searchPerformed = false;
            foundDriver = null;
            linkErrorMessage = null;

            await Task.Delay(1500);

            try
            {
                string extractedPlate = SimulateDataExtraction(rideLinkUrl);

                if (!string.IsNullOrEmpty(extractedPlate))
                {

                    searchQuery = extractedPlate;
                    foundDriver = ReportService.SearchDriver(extractedPlate);
                    searchPerformed = true;
                }
                else
                {
                    linkErrorMessage = "We couldn't extract a valid license plate from that link. Please ensure it is a valid ride-share tracking link, or switch to Manual Search.";
                }
            }
            catch
            {
                linkErrorMessage = "An unexpected error occurred while analyzing the link. Please try again.";
            }
            finally
            {
                isAnalyzing = false;
            }
        }

        private string SimulateDataExtraction(string url)
        {

            var plateRegex = new Regex(@"[A-Z]{2}\d{2}[A-Z]{2}", RegexOptions.IgnoreCase);

            var match = plateRegex.Match(url);

            if (match.Success)
            {
                //fake link like: https://uber.com/share?plate=AA11AAGP
                return match.Value.ToUpper();
            }


            return string.Empty;
        }
    }
}
