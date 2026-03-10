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
            }
            catch
            {

            }
            finally
            {
                isAnalyzing = false;
            }
        }

        private string SimulateDataExtraction(string url)
        {

            var plateRegex = new Regex(@"[A-Z]{2}\d{2}[A-Z]{2}GP", RegexOptions.IgnoreCase);

            var match = plateRegex.Match(url);

            if (match.Success)
            {
                //fake link like: https://uber.com/share?plate=AA11AAGP
                return match.Value.ToUpper();
            }

            // Return hard coded plate
            return "BB22BBGP"; // Jane Smith
        }
    }
}
