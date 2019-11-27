using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;

namespace XFChatAppSample.Services
{
    public class BotService : IBotService
    {
        private static string BotUserName { get; } = "XFChatAppSample";

        private IDirectLineClient _directLineClient { get; }

        public BotService(IDirectLineClient directLineClient)
        {
            _directLineClient = directLineClient;
        }

        public async Task<(IEnumerable<Activity> messages, string watermark)> GetMessagesAsync(string conversationId, string watermark)
        {
            var r = await _directLineClient.Conversations.GetActivitiesAsync(
                conversationId,
                watermark);
            return (r.Activities, r.Watermark);
        }

        public async Task SendMessageAsync(string conversationId, string message)
        {
            await _directLineClient.Conversations.PostActivityAsync(
                conversationId,
                new Activity
                {
                    From = new ChannelAccount(BotUserName),
                    Text = message,
                    Type = ActivityTypes.Message,
                });
        }

        public async Task<string> StartConversationAsync()
        {
            var conversation = await _directLineClient.Conversations.StartConversationAsync();
            return conversation.ConversationId;
        }
    }
}
