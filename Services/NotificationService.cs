using FirebaseAdmin.Messaging;

namespace VecoBackend.Services;

public class NotificationService
{
    public static async Task Notify(List<string> tokens, bool isAccepted, string title)
    {
        try
        {
            string ans;
            ans = isAccepted ? $"{title} принято." : $"{title} отклонено.";

            var message = new MulticastMessage
            {
                Tokens = tokens,
                Notification = new Notification
                {
                    Title = "Судьи вынесли вердикт!",
                    Body = ans,
                }
            };

            await FirebaseMessaging.DefaultInstance.SendMulticastAsync(message);
        }

        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}