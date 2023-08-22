using Microsoft.Extensions.Logging;
using ServerSignalR.Entities;
using System.Collections.Concurrent;

namespace ServerSignalR.Services;

public interface IStorageService
{
    void AddItem(string id, string mess);
    Client? GetItem();
}

public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;
    private ConcurrentQueue<Client> QueueMessageStorage { get; set; } = new();

    public StorageService(ILogger<StorageService> logger)
    {
        _logger = logger;
    }

    public void AddItem(string id, string mess)
    {
        _logger.LogInformation(mess, id);

        var item = new Client()
        {
            ConnectionId = id,
            ClientMessage = mess
        };

        QueueMessageStorage.Enqueue(item);
    }

    public Client? GetItem()
    {
        QueueMessageStorage.TryDequeue(out var result);;

        return result;
    }
}
