using Dapper;
using Driver_Report.Components;
using Driver_Report.Core.Interface;
using Driver_Report.Core.Services;

SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

var builder = WebApplication.CreateBuilder(args);

string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
                            throw new Exception("Missing connection string!");

builder.Services.AddSingleton<ISqlConnectionFactory>(sp =>
        new SqlConnectionFactory(connectionString));

builder.Services.AddScoped<IReportService, SqlReportService>();


// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
