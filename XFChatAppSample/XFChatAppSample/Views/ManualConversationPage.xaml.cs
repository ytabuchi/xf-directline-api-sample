using Microsoft.Bot.Connector.DirectLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFChatAppSample.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ManualConversationPage : ContentPage
    {
        DirectLineClient client;
        string Secret;

        string conversationId = "";
        string responseId = "";

        public ManualConversationPage()
        {
            InitializeComponent();
            Secret = Secrets.DirectLineApiKey;
            client = new DirectLineClient(Secret);
        }

        private async void StartActivity_Clicked(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(conversationId))
            {
                try
                {
                    // クライアントを作成して会話を開始し、ConversationID を取得します。
                    var client = new DirectLineClient(Secret);
                    var conversation = await client.Conversations.StartConversationAsync();
                    conversationId = conversation.ConversationId;

                    ConversationIDLabel.Text = conversationId;
                }
                catch (Exception)
                {
                    throw;
                }
            }
        }

        private async void PostActivity_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(InputText.Text))
            {
                // ConversationID を指定して会話を開始します。
                var response = await client.Conversations.PostActivityAsync(
                    ConversationIDLabel.Text,
                    new Activity
                    {
                        From = new ChannelAccount("TESTNAME"),
                        Text = InputText.Text,
                        Type = ActivityTypes.Message,
                    });
                responseId = response.Id;

                ResponseIDLabel.Text = responseId;
            }
        }

        private async void GetActivity_Clicked(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(conversationId) && !string.IsNullOrEmpty(responseId))
            {
                // ConversationID を指定して、Activity（複数）を取得します。
                var activities = await client.Conversations.GetActivitiesAsync(conversationId);
                var result = activities.Activities.LastOrDefault(x => x.ReplyToId == responseId);

                ConversationLabel.Text = result.Text;

                // 取得できる Activity の確認用
                var sb = new StringBuilder();
                foreach (var item in activities.Activities)
                {
                    sb.Append($"Channel ID: {item.ChannelId}\nID: {item.Id}\nReplyToID: {item.ReplyToId}\nText: {item.Text}\n");
                }
                await DisplayAlert("Activities", sb.ToString(), "OK");
            }   
        }
    }
}