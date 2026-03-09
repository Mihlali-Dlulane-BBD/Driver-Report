using Driver_Report.Core.Interface;
using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;

namespace Driver_Report.Components.Pages
{
    public partial class AddReport
    {
        [Inject]
        public NavigationManager NavManager { get; set; } = default!;

        [Inject]
        public IReportService ReportService { get; set; } = default!;

        private ReportFormModel reportModel = new ReportFormModel();

        private void HandleSubmit()
        {
           
            ReportService.SubmitNewIncident(
                reportModel.LicensePlate,
                reportModel.FirstName,
                reportModel.LastName,
                reportModel.Platform,
                reportModel.Reason
            );

           
            reportModel = new ReportFormModel();
            NavManager.NavigateTo("/");
        }

        private void GoBack()
        {
            NavManager.NavigateTo("/");
        }

        public class ReportFormModel
        {
            [Required(ErrorMessage = "A license plate is absolutely required.")]
            [StringLength(8, MinimumLength = 2, ErrorMessage = "Plate must be between 2 and 8 characters.")]
            public string LicensePlate { get; set; } = "";

            [StringLength(50, ErrorMessage = "First name is too long.")]
            public string FirstName { get; set; } = "";

            [StringLength(50, ErrorMessage = "Last name is too long.")]
            public string LastName { get; set; } = "";

            [StringLength(50, ErrorMessage = "Platform name is too long.")]
            public string Platform { get; set; } = "";

            [Required(ErrorMessage = "You must describe what the driver did.")]
            [StringLength(1000, MinimumLength = 10, ErrorMessage = "Please provide at least 10 characters of detail.")]
            public string Reason { get; set; } = "";
        }
    }
}
