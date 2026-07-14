using System.Text;
using AgentChat.Abstractions.Chat;
using AgentChat.Abstractions.Store;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using ChatMessage = AgentChat.Abstractions.Chat.ChatMessage;
using ChatRole = AgentChat.Abstractions.Chat.ChatRole;

namespace AgentChat.Maf;

public class MafChatSession : IChatSession
{
    private readonly AIAgent _agent;
    private AgentSession _mafSession;
    private readonly ISessionStore _store;
    private readonly ILogger _logger;
    private readonly List<ChatMessage> _history = new();
    private bool _disposed;
    
    public string SessionId { get; }
    public IReadOnlyList<ChatMessage> History => _history.AsReadOnly();

    public MafChatSession(
        AIAgent agent, 
        AgentSession mafSession,
        string sessionId, 
        ISessionStore store,
        ILogger logger)
    {
        _agent = agent;
        _mafSession = mafSession;
        SessionId = sessionId;
        _store = store;
        _logger = logger;
    }
    
    public async Task<ChatMessage> SendAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        
        _history.Add(new ChatMessage(SessionId, userMessage, ChatRole.User, DateTimeOffset.UtcNow));
        _logger.LogDebug("Chat send | SessionId={SessionId} | Length={Len}", SessionId, userMessage.Length);

        var result = await _agent.RunAsync(
            userMessage,
            await _agent.CreateSessionAsync(cancellationToken),
            null,
            cancellationToken);

        var text = string.Concat(
            result.Messages
                .Where(m => m.Role == Microsoft.Extensions.AI.ChatRole.Assistant)
                .SelectMany(m => m.Contents)
                .OfType<TextContent>()
                .Select(c => c.Text)
        );

        var response = new ChatMessage(
            SessionId,
            Content: text,
            Role: Microsoft.Extensions.AI.ChatRole.Assistant.Map(),
            Timestamp: result.CreatedAt,
            Usage: result.Usage?.Map());
        
        _history.Add(response);
        return response;
    }

    public async IAsyncEnumerable<ChatChunk> StreamAsync(string userMessage, CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        
        _history.Add(new ChatMessage(SessionId, userMessage, ChatRole.User, DateTimeOffset.UtcNow));

        var fullContent = new StringBuilder();

        await foreach (var update in _agent.RunStreamingAsync(userMessage,
                           await _agent.CreateSessionAsync(cancellationToken), null, cancellationToken))
        {
            fullContent.Append(update.Text);

            yield return new ChatChunk(
                Delta: update.Text ?? string.Empty,
                SessionId: SessionId,
                IsComplete: false);
        }
        
        yield return new ChatChunk(string.Empty, SessionId, IsComplete: true);
        
        _history.Add(new ChatMessage(SessionId, fullContent.ToString(), ChatRole.User, DateTimeOffset.UtcNow));
    }

    private async Task PersistSessionAsync(CancellationToken cancellationToken = default)
    {
        var serialized = await _agent.SerializeSessionAsync(_mafSession, null, cancellationToken);
        var json = serialized.GetRawText();
        await _store.SaveAsync(SessionId, json, cancellationToken);
        _logger.LogDebug("Session persisted | SessionId={SessionId} | Length={Len}", SessionId, json.Length);
    }

    private void ThrowIfDisposed()
    {
        ObjectDisposedException.ThrowIf(_disposed, nameof(MafChatSession));
    }
}