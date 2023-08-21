using System.Collections.Concurrent;

namespace ServerSignalR.Services;

public interface IStorageService
{
    void AddMessage(string mess);
    string? GetMessage();
}

public class StorageService : IStorageService
{
    private readonly ILogger<StorageService> _logger;
    private ConcurrentQueue<string> QueueMessageStorage { get; set; } = new();

    public StorageService(ILogger<StorageService> logger)
    {
        _logger = logger;
    }

    public void AddMessage(string mess)
    {
        _logger.LogInformation(mess);

        QueueMessageStorage.Enqueue(mess);
    }
    public string? GetMessage()
    {
        QueueMessageStorage.TryDequeue(out var result);

        if (result != null)
            _logger.LogInformation(result);

        return result;
    }
}
