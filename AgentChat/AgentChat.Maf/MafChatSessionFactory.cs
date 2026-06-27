using System.Text.Json;
using AgentChat.Abstractions.Chat;
using AgentChat.Abstractions.Store;
using Microsoft.Agents.AI;
using Microsoft.Extensions.Logging;

namespace AgentChat.Maf;

public class MafChatSessionFactory : IChatSessionFactory
{
    private readonly AIAgent _agent;
    private readonly ISessionStore _store;
    private readonly ILogger<MafChatSessionFactory> _logger;

    public MafChatSessionFactory(
        AIAgent agent,
        ISessionStore store,
        ILogger<MafChatSessionFactory> logger)
    {
        _agent = agent;
        _store = store;
        _logger = logger;
    }
    
    public async Task<IChatSession> CreateAsync(string? sessionId = null, ChatSessionOptions? options = null, CancellationToken cancellationToken = default)
    {
        var mafSession = await _agent.CreateSessionAsync(cancellationToken).ConfigureAwait(false);
        if(string.IsNullOrWhiteSpace(sessionId))
            sessionId = Guid.NewGuid().ToString("N");
        
        _logger.LogInformation("Created session {sessionId}", sessionId);
        return new MafChatSession(_agent, mafSession, sessionId, _store, _logger);
    }

    public async Task<IChatSession> ResumeAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        var json = await _store.LoadAsync(sessionId, cancellationToken);

        if (json is null)
            throw new InvalidOperationException($"Session '{sessionId}' not found in store. Cannot resume.");

        var serialized = JsonDocument.Parse(json).RootElement;
        var mafSession = await _agent.DeserializeSessionAsync(serialized, null, cancellationToken);
        
        _logger.LogInformation("Chat session resumed | SessionId={SessionId}", sessionId);
        return new MafChatSession(_agent, mafSession, sessionId, _store, _logger);
    }
}