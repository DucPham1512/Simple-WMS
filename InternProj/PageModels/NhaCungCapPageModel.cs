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
    public partial class NhaCungCapPageModel : ObservableObject
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

        public NhaCungCapPageModel(NhaCungCapRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachNcc = new ObservableCollection<NhaCungCap>(data);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                var donVi = new NhaCungCap
                {
                    Ma_Ncc = MaNccInput,
                    Ten_Ncc = TenNccInput,
                    Ghi_Chu = GhiChuInput
                };
                // Xác định xem đang Thêm hay Sửa
                bool isEdit = SelectedItem != null;

                if (isEdit) donVi.Id = SelectedItem!.Id;

                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, isEdit);

                await LoadData();

                // Reset form
                MaNccInput = string.Empty;
                TenNccInput = string.Empty;
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
        private async Task Delete(NhaCungCap item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa '{item.Ten_Ncc}'?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            await _repository.DeleteItemAsync(item);
            
            await LoadData();
        }

        [RelayCommand]
        private async Task Edit(NhaCungCap item)
        {
            try
            {
                var donVi = new NhaCungCap
                {
                    Id = item.Id,
                    Ma_Ncc = item.Ma_Ncc,
                    Ten_Ncc = item.Ten_Ncc,
                    Ghi_Chu = item.Ghi_Chu
                };

                // Gọi hàm Save mới
                await _repository.SaveItemAsync(donVi, true);

                await LoadData();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
        // Hàm helper để điền dữ liệu vào ô input khi chọn một dòng để sửa
        partial void OnSelectedItemChanged(NhaCungCap? value)
        {
            if (value != null)
            {
                MaNccInput = value.Ma_Ncc;
                TenNccInput = value.Ten_Ncc;
                GhiChuInput = value.Ghi_Chu;
            }
        }

    }
}