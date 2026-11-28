namespace Pcf.Common.Contracts.Notifications;

public static class NotificationTopics
{
    public const string Partners = "partner-notifications";
}

/// <summary>
/// Уведомление партнёру.
/// </summary>
public record PartnerNotification(Guid PartnerId, string Message);
