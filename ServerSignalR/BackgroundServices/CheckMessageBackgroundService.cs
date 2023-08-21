using ServerSignalR.Services;
using Microsoft.AspNetCore.SignalR;
using ServerSignalR.Hubs;

namespace ServerSignalR.BackgroundServices;

public class CheckMessageBackgroundService : BackgroundService
{
    private readonly IHubContext<MessageHub> _hubContext;
    private readonly IStorageService _storageService;
    private readonly ILogger<CheckMessageBackgroundService> _logger;

    public CheckMessageBackgroundService(IHubContext<MessageHub> hubContext,
        IStorageService storageService,
        ILogger<CheckMessageBackgroundService> logger)
    {
        _hubContext = hubContext;
        _storageService = storageService;
        _logger = logger;
    }

    //safety, will trigger stoppingToken, when app will be closed
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // to "release" job to continue main flow, could configure in more graceful way
        await Task.Yield();

        while (!stoppingToken.IsCancellationRequested)
        {
            var message = _storageService.GetMessage();

            if (message == Constants.HubConstants.HubCheckMessageTarget)
            {
                _logger.LogInformation(message);

                message = Constants.HubConstants.HubCheckMessageResponse;

                await _hubContext.Clients.All
                    .SendAsync(Constants.HubConstants.HubRecieveMessageMethodName, message);
            }
        }
    }
}
