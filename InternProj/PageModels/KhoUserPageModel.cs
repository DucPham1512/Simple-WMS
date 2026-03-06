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
        private readonly KhoUserRepository _kuRepository;

        private readonly KhoRepository _khoRepository;
        [ObservableProperty]
        private ObservableCollection<KhoUser> _danhSachKhoUser = [];

        [ObservableProperty]
        private ObservableCollection<Kho> _danhSachKho = [];

        [ObservableProperty]
        private Kho? _selectedKho;

        [ObservableProperty]
        private KhoUser? _selectedItem;

        // Các trường để binding vào Entry nhập liệu

        [ObservableProperty]
        private string _maDangNhapInput;

        [ObservableProperty]
        private string _khoIdInput;

        public KhoUserPageModel(KhoUserRepository kuRepository, KhoRepository khoRepository)
        {
            _kuRepository = kuRepository;
            _khoRepository = khoRepository;
        }

        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _kuRepository.ListAsync();
            DanhSachKhoUser = new ObservableCollection<KhoUser>(data);
            var listKho = await _khoRepository.ListAsync();
            DanhSachKho = new ObservableCollection<Kho>(listKho);
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
                    KhoId = SelectedKho.Id
                };

                if (isEdit)
                    item.Id = SelectedItem!.Id;

                await _kuRepository.SaveItemAsync(item, isEdit);

                await LoadData();

                MaDangNhapInput = string.Empty;
                KhoIdInput = SelectedKho.Id.ToString();
                SelectedItem = null;

                await Shell.Current.DisplayAlertAsync("Thông báo", "Đã lưu thành công", "OK");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task Edit(KhoUser item)
        {
            try
            {

                await _kuRepository.SaveItemAsync(item, true);

                await LoadData();

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
            await _kuRepository.DeleteItemAsync(item);
            DanhSachKhoUser.Remove(item);
        }
        // Hàm helper để điền dữ liệu vào ô input khi chọn một dòng để sửa
        partial void OnSelectedItemChanged(KhoUser? value)
        {
            if (value != null)
            {
                MaDangNhapInput = value.MaDangNhap;
                KhoIdInput = value.KhoId.ToString();
            }
        }

        public void SyncTenKhoForRow(KhoUser row)
        {
            var kho = DanhSachKho.FirstOrDefault(x => x.Id == row.KhoId);
            row.Ten_Kho = kho?.Ten_Kho ?? string.Empty;
        }
    }
}