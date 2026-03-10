using __XamlGeneratedCode__;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj;
using InternProj.Models;
using System.Text.RegularExpressions;

//using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.ObjectModel;
using InternProj.Data;

namespace InternProj.PageModels
{
    public partial class SanPhamPageModel : ObservableObject
    {
        private readonly SanPhamRepository _spRepository;
        private readonly LoaiSanPhamRepository _lspRepository;
        private readonly DonViTinhRepository _dvtRepository;

        [ObservableProperty]
        private ObservableCollection<SanPham> _danhSachSP = [];

        [ObservableProperty]
        private ObservableCollection<LoaiSanPham> _danhSachLSP = [];

        [ObservableProperty]
        private ObservableCollection<DonViTinh> _danhSachDVT = [];

        [ObservableProperty]
        private SanPham? _selectedItem;

        // Các trường để binding vào Entry nhập liệu

        [ObservableProperty]
        private string _maSPInput;

        [ObservableProperty]
        private string _tenSPInput;

        [ObservableProperty]
        private string _idLSPInput;

        [ObservableProperty]
        private string _idDVTInput;

        [ObservableProperty]
        private LoaiSanPham? _selectedLSP;

        [ObservableProperty]
        private DonViTinh? _selectedDVT;

        [ObservableProperty]
        private string _ghiChuInput;

        public SanPhamPageModel(SanPhamRepository spRepository, LoaiSanPhamRepository lspRepository, DonViTinhRepository dvtRepository)
        {
            _spRepository = spRepository;
            _lspRepository = lspRepository;
            _dvtRepository = dvtRepository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _spRepository.ListAsync();
            DanhSachSP = new ObservableCollection<SanPham>(data);
            var lspList = await _lspRepository.ListAsync();
            DanhSachLSP = new ObservableCollection<LoaiSanPham>(lspList);
            var dvtList = await _dvtRepository.ListAsync();
            DanhSachDVT = new ObservableCollection<DonViTinh>(dvtList);
        }

        [RelayCommand]
        private async Task Save()
        {

            if (SelectedLSP is null)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Vui lòng chọn loại sản phẩm", "OK");
                return;
            }

            if (SelectedDVT is null)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Vui lòng chọn đơn vị tính", "OK");
                return;
            }

            try
            {
                bool isEdit = SelectedItem != null;

                var item = new SanPham
                {
                    Ma_SP = Regex.Replace(MaSPInput, @"\s+", " ").Trim(),
                    Ten_SP = Regex.Replace(TenSPInput, @"\s+", " ").Trim(),
                    Id_LSP = SelectedLSP.Id,
                    Id_DVT = SelectedDVT.Id,
                    Ghi_Chu = GhiChuInput
                };

                if (isEdit)
                    item.Id = SelectedItem!.Id;

                await _spRepository.SaveItemAsync(item, isEdit);


                MaSPInput = string.Empty;
                TenSPInput = string.Empty;
                SelectedLSP = null;
                SelectedDVT = null;
                GhiChuInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
                await LoadData();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task Delete(SanPham item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ma_SP}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            try
            {
                await _spRepository.DeleteItemAsync(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Không thể xóa '{item.Ten_SP}'", "OK");
                return;
            }
            DanhSachSP.Remove(item);
        }

        [RelayCommand]
        private async Task Edit(SanPham item)
        {
            var sp = new SanPham
            {
                Id = item.Id,
                Ma_SP = Regex.Replace(item.Ma_SP, @"\s+", " ").Trim(),
                Ten_SP = Regex.Replace(item.Ten_SP, @"\s+", " ").Trim(),
                Id_LSP = item.Id_LSP,
                Id_DVT = item.Id_DVT,
                Ghi_Chu = item.Ghi_Chu
            };

            try
            {
                await _spRepository.SaveItemAsync(sp, true);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
            await LoadData();
        }
        public void SyncTenDVTForRow(SanPham row)
        {
            var dvt = DanhSachDVT.FirstOrDefault(x => x.Id == row.Id_DVT);
            row.Ten_DVT = dvt?.Ten_Don_Vi_Tinh ?? string.Empty;
        }
        public void SyncTenLSPForRow(SanPham row)
        {
            var lsp = DanhSachLSP.FirstOrDefault(x => x.Id == row.Id_LSP);
            row.Ten_LSP = lsp?.Ten_LSP ?? string.Empty;
        }
    }
}