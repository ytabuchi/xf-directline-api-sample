// XAMLで埋め込みリソース画像を表示するには？
// http://www.atmarkit.co.jp/ait/articles/1610/05/news020.html
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace XFChatAppSample.Helpers
{
    [ContentProperty("Source")]
    public class ImageResourceExtension : IMarkupExtension
    {
        // XAML コードで設定する属性（今回はクラスにContentProperty属性も指定する）
        public string Source { get; set; }

        // IMarkupExtension インタフェースの実装
        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (Source == null) return null;
            return ImageSource.FromResource(Source);
        }
    }
}
