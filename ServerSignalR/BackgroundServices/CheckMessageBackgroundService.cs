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
            var item = _storageService.GetItem();

            var message = item?.ClientMessage;

            if (message == Constants.HubConstants.HubCheckMessageTarget)
            {
                message = Constants.HubConstants.HubCheckMessageResponse;

                if (item?.ConnectionId != null)
                await _hubContext.Clients.Client(item.ConnectionId)
                    .SendAsync(Constants.HubConstants.HubRecieveMessageMethodName, message);
            }
        }
    }
}
