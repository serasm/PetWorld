namespace AgentChat.Abstractions.Chat;

public sealed record ChatSessionOptions(
    string? SystemPromptOverride = null,
    IReadOnlyDictionary<string, object>? InitialContext = null);