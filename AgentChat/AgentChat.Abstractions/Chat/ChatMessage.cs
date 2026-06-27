namespace AgentChat.Abstractions.Chat;

public sealed record ChatMessage(
    string SessionId,
    string Content,
    ChatRole Role,
    DateTimeOffset? Timestamp,
    ChatUsage? Usage = null);