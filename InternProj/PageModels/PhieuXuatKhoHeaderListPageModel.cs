using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using InternProj.Pages;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
//using static Android.Preferences.PreferenceActivity;

namespace InternProj.PageModels
{
    public partial class PhieuXuatKhoHeaderListPageModel : ObservableObject
    {
        private readonly PhieuXuatKhoRepository _pnkRepository;

        private readonly KhoRepository _khoRepository;

        [ObservableProperty]
        private ObservableCollection<PhieuXuatKhoHeader> _danhSachPhieu = new();

        [ObservableProperty]
        private ObservableCollection<Kho> _danhSachKho = new();

        [ObservableProperty]
        private PhieuXuatKhoHeader? _selectedItem;

        public PhieuXuatKhoHeaderListPageModel(PhieuXuatKhoRepository pnkRepository, KhoRepository khoRepository)
        {
            _pnkRepository = pnkRepository;
            _khoRepository = khoRepository;
        }

        // This is called automatically when SelectedItem changes
        partial void OnSelectedItemChanged(PhieuXuatKhoHeader? value)
        {
            if (value == null) return;

            // can't make this partial method async, so fire-and-forget safely:
            _ = OpenEditAsync(value);
        }


        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _pnkRepository.ListAsync();
            DanhSachPhieu = new ObservableCollection<PhieuXuatKhoHeader>(data);
            var khoList = await _khoRepository.ListAsync();
            DanhSachKho = new ObservableCollection<Kho>(khoList);
        }

        [RelayCommand]
        private async Task New()
        {
            await Shell.Current.GoToAsync(nameof(TaoPhieuXuatKhoPage));
        }

        [RelayCommand]
        private async Task Save(PhieuXuatKhoHeader item)
        {
            try
            {
                var pxkHeader = new PhieuXuatKhoHeader
                {
                    Id = item.Id,
                    So_Phieu_Xuat_Kho = Regex.Replace(item.So_Phieu_Xuat_Kho, @"\s+", " ").Trim(),
                    Kho_ID = item.Kho_ID,
                    Ten_Kho = item.Ten_Kho,
                };

                await _pnkRepository.EditHeaderAsync(pxkHeader);
            } catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Error", "Không thể lưu phiếu xuất kho", "OK");
            }

            await LoadData();
        }

        [RelayCommand]
        private async Task Delete(PhieuXuatKhoHeader item)
        {
            await _pnkRepository.DeleteItemAsync(item);
            await LoadData();
        }


        public async Task OpenEditAsync(PhieuXuatKhoHeader item)
        {
            var services = Application.Current?.MainPage?.Handler?.MauiContext?.Services;
            if (services != null && InternProj.Pages.MainTabbedPage.Current != null)
            {
                var page = services.GetService(typeof(EditPhieuXuatKhoPage)) as EditPhieuXuatKhoPage;
                if (page?.BindingContext is EditPhieuXuatKhoPageModel vm)
                {
                    vm.Header = item;
                    _ = vm.InitializeAsync(item);
                }
                InternProj.Pages.MainTabbedPage.Current.LoadPageIntoActiveTab(page, "Sửa Phiếu Xuất");
            }
            else
            {
                await Shell.Current.GoToAsync(
                    nameof(EditPhieuXuatKhoPage),
                    new Dictionary<string, object>
                    {
                        ["Header"] = item
                    });
            }
            SelectedItem = null;
        }

        [RelayCommand]
        private async Task OpenPrintPreview(PhieuXuatKhoHeader? item)
        {
            var lines = await _pnkRepository.GetAsync(item.Id);

            var page = App.Current?.Handler?.MauiContext?.Services.GetService<XuatKhoPrintPreviewPage>();
            if ( page == null || page?.BindingContext is not XuatKhoPrintPreviewPageModel)
            {
                var typeName = page?.BindingContext?.GetType().FullName ?? "NULL";

                await Shell.Current.DisplayAlertAsync(
                    "Error",
                    $"BindingContext type: {typeName}",
                    "OK");
            }
            if (page?.BindingContext is XuatKhoPrintPreviewPageModel vm)
            {
                vm.Load(item, lines);
                await Shell.Current.Navigation.PushAsync(page);
            }
        }

        public void SyncTenKhoForRow(PhieuXuatKhoHeader row)
        {
            var kho = DanhSachKho.FirstOrDefault(x => x.Id == row.Kho_ID);
            row.Ten_Kho = kho?.Ten_Kho ?? string.Empty;
        }

        public IReadOnlyList<string> ActionOptions { get; } =
            new[] {"Lưu","Sửa", "Xóa","In" };


    }
}