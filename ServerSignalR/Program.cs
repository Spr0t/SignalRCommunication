using Serilog;
using ServerSignalR.BackgroundServices;
using ServerSignalR.Hubs;
using ServerSignalR.Services;

var builder = WebApplication.CreateBuilder(args);
//builder.Host.UseDefaultServiceProvider(x => x.ValidateScopes = true);

try
{
    var configuration = new LoggerConfiguration()
        .Enrich.FromLogContext()
        .WriteTo.Debug()
        .WriteTo.Console()
        .WriteTo.File(
            path: Path.Combine("logs", "SignalRServer-logs.txt"),
            fileSizeLimitBytes: 10485760,
            rollOnFileSizeLimit: true,
            retainedFileCountLimit: 10,
            rollingInterval: RollingInterval.Day);

    Log.Logger = configuration.CreateLogger();

    builder.Logging.AddSerilog();

    Log.Information("Logger created");

    builder.Services.AddSignalR();

    builder.Services.AddSingleton<IStorageService, StorageService>();
    builder.Services.AddHostedService<CheckMessageBackgroundService>();

    var app = builder.Build();

    app.UseStaticFiles();

    app.UseRouting();

    app.MapHub<MessageHub>("/message");

    app.Run();
}
catch (Exception ex)
{
    Log.Logger.Error(ex, ex.Message);
    throw;
}
