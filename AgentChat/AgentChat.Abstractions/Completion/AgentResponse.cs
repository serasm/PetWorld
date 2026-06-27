namespace AgentChat.Abstractions.Completion;

public sealed record AgentResponse(
    string Content,
    string SessionId,
    bool IsComplete,
    AgentUsage? Usage = null);