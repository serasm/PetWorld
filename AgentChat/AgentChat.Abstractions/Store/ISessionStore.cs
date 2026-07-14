namespace AgentChat.Abstractions.Store;

public interface ISessionStore
{
    public Task SaveAsync(string sessionId, string json, CancellationToken cancellationToken);
    public Task<string?> LoadAsync(string sessionId, CancellationToken cancellationToken);
    public Task DeleteAsync(string sessionId, CancellationToken cancellationToken);
}