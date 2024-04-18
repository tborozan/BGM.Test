using BGM.Test.Web.Components;
using BGM.Test.Web.DAL;
using BGM.Test.Web.Extensions;
using BGM.Test.Web.Helpers;
using BGM.Test.Web.Services;
using BGM.Test.Web.Services.Interfaces;
using BGM.Test.Web.Workers;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

WebApplicationBuilder builder = WebApplication.CreateSlimBuilder(args);

builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();
builder.Services.AddMudServices();

builder.ConfigureOptions();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<BgmDbContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddTransient<ISftpService, SftpService>();

builder.Services.AddHostedService<ImportWorker>();

WebApplication app = builder.Build();

app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.MigrateDatabase<BgmDbContext>();

app.Run();