using AgentChat.Abstractions.Chat;

namespace AgentChat.Maf;

public static class ChatRoleExtensions
{
    public static AgentChat.Abstractions.Chat.ChatRole Map(this Microsoft.Extensions.AI.ChatRole role)
    {
        return role.ToString().ToLowerInvariant() switch
        {
            "user" => ChatRole.User,
            "assistant" => ChatRole.Assistant,
            "system" => ChatRole.System,
            "tool" => ChatRole.Tool,
            _ => throw new ArgumentOutOfRangeException(nameof(role), role, null)
        };
    }
}