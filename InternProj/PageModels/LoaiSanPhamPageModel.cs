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
    public partial class LoaiSanPhamPageModel : ObservableObject
    {
        private readonly LoaiSanPhamRepository _repository;

        [ObservableProperty]
        private ObservableCollection<LoaiSanPham> _danhSachLSP = [];

        [ObservableProperty]
        private LoaiSanPham? _selectedItem;

        // Các trường để binding vào Entry nhập liệu

        [ObservableProperty]
        private string _maLSPInput;

        [ObservableProperty]
        private string _tenLSPInput;

        [ObservableProperty]
        private string _ghiChuInput;

        public LoaiSanPhamPageModel(LoaiSanPhamRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachLSP = new ObservableCollection<LoaiSanPham>(data);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                bool isEdit = SelectedItem != null;

                var item = new LoaiSanPham
                {
                    Ma_LSP = MaLSPInput,
                    Ten_LSP = TenLSPInput,
                    Ghi_Chu = GhiChuInput
                };

                if (isEdit)
                    item.Id = SelectedItem!.Id;

                await _repository.SaveItemAsync(item, isEdit);

                await LoadData();

                MaLSPInput = string.Empty;
                TenLSPInput = string.Empty;
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
        private async Task Delete(LoaiSanPham item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ma_LSP}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            await _repository.DeleteItemAsync(item);
            DanhSachLSP.Remove(item);
        }
        // Hàm helper để điền dữ liệu vào ô input khi chọn một dòng để sửa
        partial void OnSelectedItemChanged(LoaiSanPham value)
        {
            if (value != null)
            {
                MaLSPInput = value.Ma_LSP;
                TenLSPInput = value.Ten_LSP;
                GhiChuInput = value.Ghi_Chu;
            }
        }
    }
}