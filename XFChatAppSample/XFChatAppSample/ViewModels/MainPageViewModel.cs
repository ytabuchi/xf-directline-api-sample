using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Bot.Connector.DirectLine;
using Prism.Commands;
using Prism.Navigation;
using XFChatAppSample.Services;

namespace XFChatAppSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private IBotService BotService { get; }

        private string Watermark { get; set; }

        // Conversation ID を含むオブジェクト
        private Conversation Conversation { get; set; }

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

        public ObservableCollection<Activity> Messages { get; } = new ObservableCollection<Activity>();

        public DelegateCommand SendCommand { get; private set; }

        public MainPageViewModel(INavigationService navigationService,
                                 IBotService botService) : base(navigationService)
        {
            BotService = botService;

            Title = "Main Page";

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

            var ignore = this.StartGetMessagesLoopAsync(CancellationToken.None);

        }

        private async Task StartGetMessagesLoopAsync(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var result = await this.BotService.GetMessagesAsync(this.ConversationId, this.Watermark);
                this.Watermark = result.watermark;
                foreach (var message in result.messages)
                {
                    this.Messages.Add(message);
                }

                await Task.Delay(5000);
            }
        }
    }
}
