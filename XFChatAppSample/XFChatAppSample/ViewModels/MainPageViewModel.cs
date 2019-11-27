using System;
using Prism.Navigation;

namespace XFChatAppSample.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private string _message;
        public string Message
        {
            get => _message;
            set => SetProperty(ref _message, value);
        }

        public MainPageViewModel(INavigationService navigationService) : base(navigationService)
        {
            Title = "Main Page";
            Message = "Hello prism!";
        }
    }
}
