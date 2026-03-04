using __XamlGeneratedCode__;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj;
using InternProj.Models;


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
        private string _idDVTInput;

        [ObservableProperty]
        private string _idLSPInput;

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
            try
            {
                bool isEdit = SelectedItem != null;

                var item = new SanPham
                {
                    Ma_SP = MaSPInput,
                    Ten_SP = TenSPInput,
                    Id_LSP = int.TryParse(IdLSPInput, out int idLSP) ? idLSP : 0,
                    Id_DVT = int.TryParse(IdDVTInput, out int idDVT) ? idDVT : 0,
                    Ghi_Chu = GhiChuInput
                };

                if (isEdit)
                    item.Id = SelectedItem!.Id;

                await _spRepository.SaveItemAsync(item, isEdit);

                await LoadData();

                MaSPInput = string.Empty;
                TenSPInput = string.Empty;
                IdLSPInput = string.Empty;
                IdDVTInput = string.Empty;
                GhiChuInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
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
            await _spRepository.DeleteItemAsync(item);
            DanhSachSP.Remove(item);
        }

        [RelayCommand]
        private async Task Edit(SanPham item)
        {
            try
            { 

                await _spRepository.SaveItemAsync(item, true);

                await LoadData();

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
        // Hàm helper để điền dữ liệu vào ô input khi chọn một dòng để sửa
        partial void OnSelectedItemChanged(SanPham? value)
        {
            if (value != null)
            {
                MaSPInput = value.Ma_SP;
                TenSPInput = value.Ten_SP;
                IdLSPInput = value.Id_LSP.ToString();
                IdDVTInput = value.Id_DVT.ToString();
                GhiChuInput = value.Ghi_Chu;
            }
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