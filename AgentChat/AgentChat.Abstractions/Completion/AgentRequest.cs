namespace AgentChat.Abstractions.Completion;

public sealed record AgentRequest(
    string Prompt,
    string? SessionId = null,
    IReadOnlyDictionary<string, object>? Context = null);