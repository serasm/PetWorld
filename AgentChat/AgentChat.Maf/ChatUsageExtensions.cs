using AgentChat.Abstractions.Chat;

namespace AgentChat.Maf;

public static class ChatUsageExtensions
{
    public static ChatUsage Map(this Microsoft.Extensions.AI.UsageDetails usage)
    {
        return new ChatUsage(usage.InputTokenCount, usage.OutputTokenCount, usage.TotalTokenCount);
    }
}