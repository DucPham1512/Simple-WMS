using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Services;
using InternProj.Models;

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
    private async Task Print()
    {
#if WINDOWS
        await Shell.Current.DisplayAlertAsync("Thông báo", "Phần render đã xong. Bước tiếp theo là gọi Windows print API.", "OK");
#elif ANDROID
        await Shell.Current.DisplayAlertAsync("Thông báo", "Phần render đã xong. Bước tiếp theo là gọi Android print API.", "OK");
#else
        await Shell.Current.DisplayAlertAsync("Thông báo", "Chưa cấu hình in cho nền tảng này.", "OK");
#endif
    }
}