﻿using System;
using Prism;
using Prism.Ioc;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using XFChatAppSample.Views;
using XFChatAppSample.ViewModels;
using Microsoft.Bot.Connector.DirectLine;
using XFChatAppSample.Services;

namespace XFChatAppSample
{
    public partial class App
    {
        public App() : this(null) { }

        public App(IPlatformInitializer initializer) : base(initializer) { }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(Views.MainPage)}");
            // 手動で Azure Bot Service とやり取りするサンプルは以下を利用してください。
            //await NavigationService.NavigateAsync($"{nameof(NavigationPage)}/{nameof(ManualConversationPage)}");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterInstance<IDirectLineClient>(new DirectLineClient(Secrets.DirectLineApiKey));
            containerRegistry.RegisterSingleton<IBotService, BotService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<ManualConversationPage>();
        }
    }
}
