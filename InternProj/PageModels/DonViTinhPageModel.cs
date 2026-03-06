using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Text.RegularExpressions;
//using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.ObjectModel;

namespace InternProj.PageModels
{
    public partial class DonViTinhPageModel : ObservableObject
    {
        private readonly DonViTinhRepository _repository;

        [ObservableProperty]
        private ObservableCollection<DonViTinh> _danhSachDonVi = [];

        [ObservableProperty]
        private DonViTinh? _selectedItem;

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
            var donVi = new DonViTinh
            {
                Ten_Don_Vi_Tinh = Regex.Replace(TenDonViInput, @"\s+", " ").Trim(),
                Ghi_Chu = GhiChuInput
            };
            try
            {  
                bool isEdit = SelectedItem != null;

                if (isEdit) donVi.Id = SelectedItem!.Id;

                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, isEdit);

                // Reset form
                TenDonViInput = string.Empty;
                GhiChuInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
                return;
            }
            DanhSachDonVi.Add(donVi);
        }

        [RelayCommand]
        private async Task Delete(DonViTinh item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ten_Don_Vi_Tinh}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            try
            {
                await _repository.DeleteItemAsync(item);
            }
            catch (Exception ex) {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Không thể xóa '{item.Ten_Don_Vi_Tinh}'", "OK");
                return;
            }

            DanhSachDonVi.Remove(item);
        }

        [RelayCommand]
        private async Task Edit(DonViTinh item)
        {
            try
            {
                var donVi = new DonViTinh
                {
                    Id = item.Id,
                    Ten_Don_Vi_Tinh = Regex.Replace(item.Ten_Don_Vi_Tinh, @"\s+", " ").Trim(),
                    Ghi_Chu = item.Ghi_Chu
                };


                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, true);

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
                await LoadData();

        }
    }
}