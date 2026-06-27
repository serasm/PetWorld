namespace AgentChat.Abstractions.Chat;

public interface IChatSession
{
    Task<ChatMessage> SendAsync(string userMessage, CancellationToken cancellationToken = default);
    IAsyncEnumerable<ChatChunk> StreamAsync(string userMessage, CancellationToken cancellationToken = default);
    IReadOnlyList<ChatMessage> History { get; }
}