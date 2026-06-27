namespace AgentChat.Abstractions.Chat;

public sealed record ChatChunk(
    string Delta,
    string SessionId,
    bool IsComplete);