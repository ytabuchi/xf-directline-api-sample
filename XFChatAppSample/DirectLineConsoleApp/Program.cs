using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace DirectLineConsoleApp
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string endpoint = "https://<YOUR_BOTSERVICE>.azurewebsites.net/.bot/";
            string secret = "<YOUR_SECRET>";

            var tokenClient = new DirectLineClient(
                new Uri(endpoint),
                new DirectLineClientCredentials(secret));
            var conversation = await tokenClient.Tokens.GenerateTokenForNewConversationAsync();

            // ここで conversation が取れない…
            if (conversation == null)
                return;

            var client = new DirectLineClient(
                new Uri(endpoint),
                new DirectLineClientCredentials(conversation.Token));

            await client.StreamingConversations.ConnectAsync(
                conversation.ConversationId,
                ReceiveActivities);

            var startConversation = await client.StreamingConversations.StartConversationAsync();
            var from = new ChannelAccount() { Id = "123", Name = "ytabuchi" };
            var message = Console.ReadLine();

            while (message != "end")
            {
                try
                {
                    var response = await client.StreamingConversations.PostActivityAsync(
                        startConversation.ConversationId,
                        new Activity()
                        {
                            Type = "message",
                            Text = message,
                            From = from
                        });
                }
                catch (OperationException ex)
                {
                    Console.WriteLine(
                        $"OperationException when calling PostActivityAsync: ({ex.StatusCode})");
                }
                message = Console.ReadLine();
            }
        }

        public static void ReceiveActivities(ActivitySet activitySet)
        {
            if (activitySet != null)
            {
                foreach (var a in activitySet.Activities)
                {
                    if (a.Type == ActivityTypes.Message && a.From.Id.Contains("bot"))
                    {
                        Console.WriteLine($"<Bot>: {a.Text}");
                    }
                }
            }
        }
    }
}
