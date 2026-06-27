namespace AgentChat.Abstractions.Chat;

public sealed record ChatUsage(long? InputTokens, long? OutputTokens, long? TotalTokenCount);