using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Navigation;
using XFChatAppSample.Services;

namespace XFChatAppSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IBotService BotService { get; }

        private string Watermark { get; set; }

        private string _inputMessage;
        public string InputMessage
        {
            get => _inputMessage;
            set => SetProperty(ref _inputMessage, value);
        }

        private string _conversationId;
        public string ConversationId
        {
            get => _conversationId;
            set => SetProperty(ref _conversationId, value);
        }

        public ObservableCollection<CardActivity> Messages { get; } = new ObservableCollection<CardActivity>();

        public DelegateCommand SendCommand { get; private set; }

        public MainPageViewModel(INavigationService navigationService,
                                 IBotService botService) : base(navigationService)
        {
            BotService = botService;

            Title = "Bot Service Sample";

            SendCommand = new DelegateCommand(SendMessageAsync);
        }

        private async void SendMessageAsync()
        {
            await BotService.SendMessageAsync(ConversationId, InputMessage);
            InputMessage = "";
        }

        public override async void OnNavigatedTo(INavigationParameters parameters)
        {
            base.OnNavigatedTo(parameters);

            ConversationId = await BotService.StartConversationAsync();

            _ = this.StartGetMessagesLoopAsync(CancellationToken.None);

        }

        private async Task StartGetMessagesLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = await this.BotService.GetMessagesAsync(this.ConversationId, this.Watermark);

                this.Watermark = result.watermark;
                foreach (var message in result.messages)
                {
                    if (message.AttachmentLayout == "list")
                    {
                        var cardActivity = new CardActivity(message);

                        var json = message.Attachments.FirstOrDefault()?.Content.ToString();
                        var cardInfo = JsonConvert.DeserializeObject<CardInfo>(json);

                        cardActivity.CardTitle = cardInfo.CardTitle;
                        cardActivity.CardText = cardInfo.CardText;
                        cardActivity.CardImage = cardInfo.CardImages.FirstOrDefault()?.Url;

                        Messages.Add(cardActivity);
                    }
                    else
                    {
                        Messages.Add(new CardActivity(message));
                    }
                }

                await Task.Delay(5000);
            }
        }
    }


    public class CardInfo
    {
        [JsonProperty(PropertyName = "title")]
        public string CardTitle { get; set; }

        [JsonProperty(PropertyName = "text")]
        public string CardText { get; set; }

        [JsonProperty(PropertyName = "images")]
        public List<ImageUrl> CardImages { get; set; }

        public class ImageUrl
        {
            [JsonProperty(PropertyName = "url")]
            public string Url { get; set; }
        }
    }


    public class CardActivity
    {
        public string CardTitle { get; set; }
        public string CardText { get; set; }
        public string CardImage { get; set; }
        public Activity Activity { get; }

        public CardActivity(Activity activity)
        {
            Activity = activity;
        }
    }
}
