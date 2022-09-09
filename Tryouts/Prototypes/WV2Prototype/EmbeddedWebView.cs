using Avalonia.Controls;
using Avalonia.Platform;
using Microsoft.Web.WebView2.WinForms;
using System;
using System.Threading.Tasks;

namespace MorganStanley.ComposeUI.Prototypes.WV2Prototype
{
    public class EmbeddedWebView : NativeControlHost
    {
        WebView2 _webView;
        public EmbeddedWebView()
        {
            _webView = new WebView2();

            _ = Init();
        }

        public async Task Init()
        {
            await _webView.EnsureCoreWebView2Async(null);
            _webView.CoreWebView2.WebMessageReceived += CoreWebView2_WebMessageReceived;
        }

        private void CoreWebView2_WebMessageReceived
        (
            object? sender,
            Microsoft.Web.WebView2.Core.CoreWebView2WebMessageReceivedEventArgs e)
        {

        }

        public Uri? Source
        {
            get => _webView.Source;
            set => _webView.Source = value;
        }


        protected override IPlatformHandle CreateNativeControlCore(IPlatformHandle parent)
        {
            IPlatformHandle handle =
                new PlatformHandle(_webView.Handle, "HWND");

            return handle;
        }

        protected override void DestroyNativeControlCore(IPlatformHandle control)
        {
            _webView.Dispose();
        }
    }
}
