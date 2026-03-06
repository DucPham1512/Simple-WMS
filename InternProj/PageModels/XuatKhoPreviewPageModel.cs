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
        PrintSource = new HtmlWebViewSource
        {
            Html = PhieuXuatKhoPrintTemplate.Build(header, lines)
        };
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