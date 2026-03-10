using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Services;
using InternProj.Models;
using Microsoft.UI.Xaml.Controls;

public partial class XuatKhoPrintPreviewPageModel : ObservableObject
{
    [ObservableProperty]
    private HtmlWebViewSource printSource = new();

    public void Load(PhieuXuatKhoHeader header, IEnumerable<PhieuXuatKhoData> lines)
    {
        // 1. Generate the HTML string
        var htmlContent = PhieuXuatKhoPrintTemplate.Build(header, lines);

        // 2. Force the update onto the Main UI Thread
        MainThread.BeginInvokeOnMainThread(() =>
        {
            // 3. Null it out first to force the UI to detach (optional but very safe)
            PrintSource = null;

            // 4. Assign a brand new object. The [ObservableProperty] will alert the UI automatically.
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