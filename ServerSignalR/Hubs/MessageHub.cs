using Microsoft.AspNetCore.SignalR;
using ServerSignalR.Services;

namespace ServerSignalR.Hubs;

public class MessageHub : Hub
{
    private readonly IStorageService _storageService;
    private readonly ILogger<MessageHub> _logger;

    public MessageHub(IStorageService storageService, 
        ILogger<MessageHub> logger)
    {
        _storageService = storageService;
        _logger = logger;
    }

    public void RecieveMessage(string message)
    {
        _logger.LogInformation(message);

        //Imitation server processing info delay
        Thread.Sleep(5000);

        _storageService.AddMessage(message);

        Clients.Caller.SendAsync(Constants.HubConstants.HubRecieveMessageMethodName, 
            Constants.HubConstants.HubRecieveStatusMessage);
    }
}
