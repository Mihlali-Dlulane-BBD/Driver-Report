using Microsoft.JSInterop;

namespace Driver_Report.Components.Shared
{
    public partial class ThemeToggle
    {
        private bool isDarkMode = false;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var savedTheme = await JS.InvokeAsync<string>("themeSwitcher.getTheme");
                isDarkMode = savedTheme == "dark";
                StateHasChanged();
            }
        }

        private async Task ToggleTheme()
        {
            var currentRealTheme = await JS.InvokeAsync<string>("themeSwitcher.toggleTheme");
            isDarkMode = currentRealTheme == "dark";
        }
    }
}
