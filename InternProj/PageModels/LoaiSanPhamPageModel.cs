using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
//using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
//using static Android.Renderscripts.ScriptGroup;

namespace InternProj.PageModels
{
    public partial class LoaiSanPhamPageModel : BasePageModel
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

        public LoaiSanPhamPageModel (LoaiSanPhamRepository repository, 
                                    DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
        }

        [RelayCommand]
        public override async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachLSP = new ObservableCollection<LoaiSanPham>(data);
        }

        [RelayCommand]
        private async Task Save()
        {

            if (string.IsNullOrWhiteSpace(MaLSPInput))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Mã loại sản phẩm không được để trống.", "OK");
                await LoadData();
                return;
            }

            if (string.IsNullOrWhiteSpace(TenLSPInput))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Tên loại sản phẩm không được để trống.", "OK");
                await LoadData();
                return;
            }

            try
            {
                bool isEdit = SelectedItem != null;

                var item = new LoaiSanPham
                {
                    Ma_LSP = Regex.Replace(MaLSPInput, @"\s+", " ").Trim(),
                    Ten_LSP = Regex.Replace(TenLSPInput, @"\s+", " ").Trim(),
                    Ghi_Chu = GhiChuInput
                };

                if (isEdit)
                    item.Id = SelectedItem!.Id;

                await _repository.SaveItemAsync(item, isEdit);

                MaLSPInput = string.Empty;
                TenLSPInput = string.Empty;
                GhiChuInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
                await LoadData();
            }
        }

        [RelayCommand]
        private async Task Edit(LoaiSanPham item)
        {
            if (string.IsNullOrEmpty(item.Ma_LSP))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Mã loại sản phẩm không được để trống.", "OK");
                await LoadData();
                return;
            }

            if (string.IsNullOrEmpty(item.Ten_LSP))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Tên loại sản phẩm không được để trống.", "OK");
                await LoadData();
                return;
            }
            try
            {
                var donVi = new LoaiSanPham
                {
                    Id = item.Id,
                    Ma_LSP = Regex.Replace(item.Ma_LSP, @"\s+", " ").Trim(),
                    Ten_LSP = Regex.Replace(item.Ten_LSP, @"\s+", " ").Trim(),
                    Ghi_Chu = item.Ghi_Chu
                };


                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, true);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
                await LoadData();
            }
        }


        [RelayCommand]
        private async Task Delete(LoaiSanPham item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ma_LSP}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            try
            {
                await _repository.DeleteItemAsync(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Không thể xóa '{item.Ten_LSP}'", "OK");
                return;
            }
        }

    }
}