using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    public partial class TaoPhieuXuatKhoPageModel : ObservableObject
    {
        private readonly PhieuXuatKhoRepository _repository;

        [ObservableProperty] 
        private string _soPhieuXuatKhoInput;
        [ObservableProperty] 
        private DateTime _ngayXuatKhoInput = DateTime.Today;
        [ObservableProperty] 
        private string _khoIdInput;
        [ObservableProperty] 
        private string _ghiChuInput;

        // Current line inputs
        [ObservableProperty] 
        private string _sanPhamIdInput;
        [ObservableProperty] 
        private string _soLuongInput;
        [ObservableProperty] 
        private string _donGiaInput;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuXuatKhoRawData> danhSachDong = new();

        public TaoPhieuXuatKhoPageModel(PhieuXuatKhoRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task AddLine()
        {
            try
            {
                if (!int.TryParse(SanPhamIdInput, out var sanPhamId) || sanPhamId <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Sản phẩm không hợp lệ.", "OK");
                    return;
                }

                if (!int.TryParse(SoLuongInput, out var soLuong) || soLuong <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Số lượng phải lớn hơn 0.", "OK");
                    return;
                }

                if (!float.TryParse(DonGiaInput, out var donGia) || donGia < 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Đơn giá không hợp lệ.", "OK");
                    return;
                }

                DanhSachDong.Add(new PhieuXuatKhoRawData
                {
                    SanPhamId = sanPhamId,
                    SoLuong = soLuong,
                    DonGia = donGia
                });

                // Clear current line inputs after adding
                SanPhamIdInput = string.Empty;
                SoLuongInput = string.Empty;
                DonGiaInput = string.Empty;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private void RemoveLine(PhieuXuatKhoRawData? item)
        {
            if (item == null) return;
            DanhSachDong.Remove(item);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SoPhieuXuatKhoInput))
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Số phiếu nhập không được rỗng.", "OK");
                    return;
                }

                if (!int.TryParse(KhoIdInput, out var khoId) || khoId <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Kho không hợp lệ.", "OK");
                    return;
                }

                if (DanhSachDong.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Phiếu nhập phải có ít nhất 1 dòng hàng.", "OK");
                    return;
                }

                var header = new PhieuXuatKhoHeader
                {
                    So_Phieu_Xuat_Kho = SoPhieuXuatKhoInput.Trim(),
                    Ngay_Xuat_Kho = NgayXuatKhoInput,
                    Kho_ID = khoId,
                    Ghi_Chu = GhiChuInput
                };

                await _repository.SaveAsync(header, DanhSachDong.ToList());

                await Shell.Current.DisplayAlertAsync("Thành công", "Đã lưu phiếu nhập kho.", "OK");

                // Reset form
                SoPhieuXuatKhoInput = string.Empty;
                NgayXuatKhoInput = DateTime.Today;
                KhoIdInput = string.Empty;
                GhiChuInput = string.Empty;
                DanhSachDong.Clear();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}