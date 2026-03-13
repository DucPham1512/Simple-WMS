using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Services;
using InternProj.Models;
#if WINDOWS
using Microsoft.UI.Xaml.Controls;
#endif

public partial class XuatKhoPrintPreviewPageModel : ObservableObject
{
    [ObservableProperty]
    private HtmlWebViewSource printSource = new();

    public void Load(PhieuXuatKhoHeader header, IEnumerable<PhieuXuatKhoData> lines)
    {
        var htmlContent = PhieuXuatKhoPrintTemplate.Build(header, lines);

        MainThread.BeginInvokeOnMainThread(() =>
        {
            PrintSource = null;

            PrintSource = new HtmlWebViewSource
            {
                Html = htmlContent
            };
        });
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
        await Shell.Current.DisplayAlertAsync("Thông báo", "Phần render đã xong. Bước tiếp theo là gọi Android print API.", "OK");
#else
        await Shell.Current.DisplayAlertAsync("Thông báo", "Chưa cấu hình in cho nền tảng này.", "OK");
#endif
    }
}