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
    public partial class DonViTinhPageModel : ObservableObject
    {
        private readonly DonViTinhRepository _repository;

        [ObservableProperty]
        private ObservableCollection<DonViTinh> _danhSachDonVi = [];

        [ObservableProperty]
        private DonViTinh _selectedItem;

        // Các trường để binding vào Entry nhập liệu
        [ObservableProperty]
        private string _tenDonViInput;

        [ObservableProperty]
        private string _ghiChuInput;

        public DonViTinhPageModel(DonViTinhRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachDonVi = new ObservableCollection<DonViTinh>(data);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                var donVi = new DonViTinh
                {
                    Ten_Don_Vi_Tinh = TenDonViInput,
                    Ghi_Chu = GhiChuInput
                };
                // Xác định xem đang Thêm hay Sửa
                bool isEdit = SelectedItem != null;

                if (isEdit) donVi.Id = SelectedItem!.Id;

                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, isEdit);

                await LoadData();

                // Reset form
                TenDonViInput = string.Empty;
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
        private async Task Delete(DonViTinh item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ten_Don_Vi_Tinh}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            await _repository.DeleteItemAsync(item);
            DanhSachDonVi.Remove(item);
        }
        // Hàm helper để điền dữ liệu vào ô input khi chọn một dòng để sửa
        partial void OnSelectedItemChanged(DonViTinh value)
        {
            if (value != null)
            {
                TenDonViInput = value.Ten_Don_Vi_Tinh;
                GhiChuInput = value.Ghi_Chu;
            }
        }

    }
}