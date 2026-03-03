using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
//using Microsoft.UI.Xaml.Controls.Primitives;
using System.Collections.ObjectModel;

namespace InternProj.PageModels
{
    public partial class KhoUserPageModel : ObservableObject
    {
        private readonly KhoUserRepository _repository;

        [ObservableProperty]
        private ObservableCollection<KhoUser> _danhSachKhoUser = [];

        [ObservableProperty]
        private KhoUser? _selectedItem;

        // Các trường để binding vào Entry nhập liệu

        [ObservableProperty]
        private string _maDangNhapInput;

        [ObservableProperty]
        private string _khoIdInput;

        public KhoUserPageModel(KhoUserRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachKhoUser = new ObservableCollection<KhoUser>(data);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                bool isEdit = SelectedItem != null;

                var item = new KhoUser
                {
                    MaDangNhap = MaDangNhapInput,
                    KhoId = int.TryParse(KhoIdInput,out int khoId) ? khoId : 0
                };

                if (isEdit)
                    item.Id = SelectedItem!.Id;

                await _repository.SaveItemAsync(item, isEdit);

                await LoadData();

                MaDangNhapInput = string.Empty;
                KhoIdInput = string.Empty;
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task Delete(KhoUser item)
        {
            bool answer = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn muốn xóa mã đăng nhập này không?", "Có", "Không");
            if (!answer) return;

            // Truyền string Key vào hàm xóa
            await _repository.DeleteItemAsync(item);
            DanhSachKhoUser.Remove(item);
        }
        // Hàm helper để điền dữ liệu vào ô input khi chọn một dòng để sửa
        partial void OnSelectedItemChanged(KhoUser value)
        {
            if (value != null)
            {
                MaDangNhapInput = value.MaDangNhap;
                KhoIdInput = value.KhoId.ToString();
            }
        }
    }
}