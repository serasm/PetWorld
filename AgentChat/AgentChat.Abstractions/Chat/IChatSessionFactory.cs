namespace AgentChat.Abstractions.Chat;

public interface IChatSessionFactory
{
    Task<IChatSession> CreateAsync(string? sessionId, ChatSessionOptions? options = null, CancellationToken cancellationToken = default);
    Task<IChatSession> ResumeAsync(string sessionId, CancellationToken cancellationToken = default);
}