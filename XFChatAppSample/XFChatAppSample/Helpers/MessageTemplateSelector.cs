using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Bot.Connector.DirectLine;
using Xamarin.Forms;
using XFChatAppSample.ViewModels;

namespace XFChatAppSample.Helpers
{
    public class MessageTemplateSelector : DataTemplateSelector
    {
        public DataTemplate CardTemplate { get; set; }
        public DataTemplate InputTemplate { get; set; }
        public DataTemplate OutputTemplate { get; set; }

        // itemにはセルのデータ、containerにはセルの親(ListViewやTableView)が渡されます
        protected override DataTemplate OnSelectTemplate(object item, BindableObject container)
        {
            if (((CardActivity)item).Activity.AttachmentLayout == "list")
                return CardTemplate;
            return ((CardActivity)item).Activity.From.Id == "ytabuchichatbot" ? OutputTemplate : InputTemplate;
        }
    }
}
