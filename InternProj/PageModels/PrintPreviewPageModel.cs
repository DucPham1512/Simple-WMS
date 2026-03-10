using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Services;
using InternProj.Models;
using Microsoft.UI.Xaml.Controls;

public partial class PrintPreviewPageModel : ObservableObject
{
    [ObservableProperty]
    private HtmlWebViewSource printSource = new();

    public void Load(PhieuNhapKhoHeader header, IEnumerable<PhieuNhapKhoData> lines)
    {
        PrintSource = new HtmlWebViewSource
        {
            Html = PhieuNhapKhoPrintTemplate.Build(header, lines)
        };
    }

    [RelayCommand]
    private async Task Print(WebView webView) // Accept the control as a parameter
    {
        if (webView == null) return;

#if WINDOWS
        var nativeWebView = webView.Handler?.PlatformView as Microsoft.UI.Xaml.Controls.WebView2;

        if (nativeWebView != null)
        {
            await nativeWebView.EnsureCoreWebView2Async();

            var printSettings = nativeWebView.CoreWebView2.Environment.CreatePrintSettings();
            printSettings.ShouldPrintBackgrounds = true;

            var printStatus = await nativeWebView.CoreWebView2.PrintAsync(printSettings);
        }
#elif ANDROID
    // Android printing logic
#endif
    }
}