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
    public partial class NhaCungCapPageModel : BasePageModel
    {
        private readonly NhaCungCapRepository _repository;

        [ObservableProperty]
        private ObservableCollection<NhaCungCap> _danhSachNcc = [];

        [ObservableProperty]
        private NhaCungCap? _selectedItem;

        // Các trường để binding vào Entry nhập liệu
        [ObservableProperty]
        private string _maNccInput;

        [ObservableProperty]
        private string _tenNccInput;

        [ObservableProperty]
        private string _ghiChuInput;

        public NhaCungCapPageModel(NhaCungCapRepository repository,
                                    DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
        }

        [RelayCommand]
        public override async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachNcc = new ObservableCollection<NhaCungCap>(data);
        }

        [RelayCommand]
        private async Task Save()
        {
            if (string.IsNullOrEmpty(MaNccInput))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Mã nhà cung cấp không được để trống.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(TenNccInput))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Tên nhà cung cấp không được để trống.", "OK");
                return;
            }

            try
            {
                var donVi = new NhaCungCap
                {
                    Ma_Ncc = Regex.Replace(MaNccInput, @"\s+", " ").Trim(),
                    Ten_Ncc = Regex.Replace(TenNccInput, @"\s+", " ").Trim(),
                    Ghi_Chu = GhiChuInput
                };
                // Xác định xem đang Thêm hay Sửa
                bool isEdit = SelectedItem != null;

                if (isEdit) donVi.Id = SelectedItem!.Id;

                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, isEdit);


                // Reset form
                MaNccInput = string.Empty;
                TenNccInput = string.Empty;
                GhiChuInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Nhà cung cấp đã tồn tại.", "OK");
                await LoadData();
            }
        }

        [RelayCommand]
        private async Task Delete(NhaCungCap item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ten_Ncc}'?", "Có", "Không");
            if (!answer) return;

            try
            {
                await _repository.DeleteItemAsync(item);
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Không thể xóa '{item.Ten_Ncc}'", "OK");
                return;
            }
        }

        [RelayCommand]
        private async Task Edit(NhaCungCap item)
        {
            if (string.IsNullOrEmpty(item.Ma_Ncc))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Mã nhà cung cấp không được để trống.", "OK");
                return;
            }

            if (string.IsNullOrEmpty(item.Ten_Ncc))
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Tên nhà cung cấp không được để trống.", "OK");
                return;
            }

            try
            {
                var donVi = new NhaCungCap
                {
                    Id = item.Id,
                    Ma_Ncc = Regex.Replace(item.Ma_Ncc, @"\s+", " ").Trim(),
                    Ten_Ncc = Regex.Replace(item.Ten_Ncc, @"\s+", " ").Trim(),
                    Ghi_Chu = item.Ghi_Chu
                };

                await _repository.SaveItemAsync(donVi, true);

            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Nhà cung cấp đã tồn tại", "OK");
                await LoadData();
            }
        }
    }
}