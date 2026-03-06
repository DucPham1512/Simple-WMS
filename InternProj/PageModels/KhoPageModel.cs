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
    public partial class KhoPageModel : ObservableObject
    {
        private readonly KhoRepository _repository;

        [ObservableProperty]
        private ObservableCollection<Kho> _danhSachKho = [];

        [ObservableProperty]
        private Kho? _selectedItem;

        // Các trường để binding vào Entry nhập liệu
        [ObservableProperty]
        private string _tenKhoInput;

        [ObservableProperty]
        private string _ghiChuInput;

        public KhoPageModel(KhoRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachKho = new ObservableCollection<Kho>(data);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                var Kho = new Kho
                {
                    Ten_Kho = Regex.Replace(TenKhoInput, @"\s+", " ").Trim(),
                    Ghi_Chu = GhiChuInput
                };
                // Xác định xem đang Thêm hay Sửa
                bool isEdit = SelectedItem != null;

                if (isEdit) Kho.Id = SelectedItem!.Id;

                // Gọi hàm Save mới
                await _repository.SaveItemAsync(Kho, isEdit);


                // Reset form
                TenKhoInput = string.Empty;
                GhiChuInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
            await LoadData();
        }

        [RelayCommand]
        private async Task Delete(Kho item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ten_Kho}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            try
            {
                await _repository.DeleteItemAsync(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Không thể xóa '{item.Ten_Kho}'", "OK");
                return;
            }
            DanhSachKho.Remove(item);
        }

        [RelayCommand]
        private async Task Edit(Kho item)
        {
            try
            {
                var donVi = new Kho
                {
                    Id = item.Id,
                    Ten_Kho = Regex.Replace(item.Ten_Kho, @"\s+", " ").Trim(),
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