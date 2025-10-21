namespace Domain.SeedWork.Notification;

public class Notification : INotification
{
    public List<NotificationModel> Notifications { get; } = [];
    public bool HasNotification => Notifications.Count != 0;
    public void AddNotification(string message) 
        => Notifications.Add(new NotificationModel(message));
}