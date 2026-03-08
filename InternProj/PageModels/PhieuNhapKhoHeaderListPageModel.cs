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
    public partial class PhieuNhapKhoHeaderListPageModel : ObservableObject
    {
        private readonly PhieuNhapKhoRepository _pnkRepository;

        private readonly KhoRepository _khoRepository;

        private readonly NhaCungCapRepository _nccRepository;

        [ObservableProperty]
        private ObservableCollection<PhieuNhapKhoHeader> _danhSachPhieu = new();

        [ObservableProperty]
        private ObservableCollection<Kho> _danhSachKho = new();

        [ObservableProperty]
        private ObservableCollection<NhaCungCap> _danhSachNCC = new();

        [ObservableProperty]
        private PhieuNhapKhoHeader? _selectedItem;

        public PhieuNhapKhoHeaderListPageModel(PhieuNhapKhoRepository pnkRepository, KhoRepository khoRepository, NhaCungCapRepository nccRepository)
        {
            _pnkRepository = pnkRepository;
            _khoRepository = khoRepository;
            _nccRepository = nccRepository;
        }

        // This is called automatically when SelectedItem changes
        partial void OnSelectedItemChanged(PhieuNhapKhoHeader? value)
        {
            if (value == null) return;

            // can't make this partial method async, so fire-and-forget safely:
            _ = OpenEditAsync(value);
        }


        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _pnkRepository.ListAsync();
            DanhSachPhieu = new ObservableCollection<PhieuNhapKhoHeader>(data);
            var khoList = await _khoRepository.ListAsync();
            DanhSachKho = new ObservableCollection<Kho>(khoList);
            var nccList = await _nccRepository.ListAsync();
            DanhSachNCC = new ObservableCollection<NhaCungCap>(nccList);
        }

        [RelayCommand]
        private async Task New()
        {
            await Shell.Current.GoToAsync(nameof(TaoPhieuNhapKhoPage));
        }

        [RelayCommand]
        private async Task Save(PhieuNhapKhoHeader item)
        {
            var pnkHeader = new PhieuNhapKhoHeader
            {
                Id = item.Id,
                So_Phieu_Nhap_Kho = Regex.Replace(item.So_Phieu_Nhap_Kho, @"\s+", " ").Trim(),
                Kho_ID = item.Kho_ID,
                NCC_ID = item.NCC_ID,
                Ten_Kho = item.Ten_Kho,
                Ten_NCC = item.Ten_NCC
            };
            try
            {
                await _pnkRepository.EditHeaderAsync(pnkHeader);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Không thể lưu phiếu nhập kho này", "OK");
            }
            await LoadData();
        }

        [RelayCommand]
        private async Task Delete(PhieuNhapKhoHeader item)
        {
            await _pnkRepository.DeleteItemAsync(item);
            await LoadData();
        }


        public async Task OpenEditAsync(PhieuNhapKhoHeader item)
        {
            var services = Application.Current?.MainPage?.Handler?.MauiContext?.Services;
            if (services != null && InternProj.Pages.MainTabbedPage.Current != null)
            {
                var page = services.GetService(typeof(EditPhieuNhapKhoPage)) as EditPhieuNhapKhoPage;
                if (page?.BindingContext is EditPhieuNhapKhoPageModel vm)
                {
                    vm.Header = item;
                }
                InternProj.Pages.MainTabbedPage.Current.LoadPageIntoActiveTab(page, "Sửa Phiếu Nhập");
            }
            else
            {
                await Shell.Current.GoToAsync(
                    nameof(EditPhieuNhapKhoPage),
                    new Dictionary<string, object>
                    {
                        ["Header"] = item
                    });
            }
            SelectedItem = null;
        }

        [RelayCommand]
        private async Task OpenPrintPreview(PhieuNhapKhoHeader? item)
        {
            var lines = await _pnkRepository.GetAsync(item.Id);

            var page = App.Current?.Handler?.MauiContext?.Services.GetService<PrintPreviewPage>();
            if (page?.BindingContext is PrintPreviewPageModel vm)
            {
                vm.Load(item, lines);
                await Shell.Current.Navigation.PushAsync(page);
            }
        }

        public void SyncTenKhoForRow(PhieuNhapKhoHeader row)
        {
            var kho = DanhSachKho.FirstOrDefault(x => x.Id == row.Kho_ID);
            row.Ten_Kho = kho?.Ten_Kho ?? string.Empty;
        }

        public void SyncTenNCCForRow(PhieuNhapKhoHeader row)
        {
            var ncc = DanhSachNCC.FirstOrDefault(x => x.Id == row.NCC_ID);
            row.Ten_NCC = ncc?.Ten_Ncc ?? string.Empty;
        }
        public IReadOnlyList<string> ActionOptions { get; } =
            new[] { "Lưu", "Sửa", "Xóa", "In" };
    }
}