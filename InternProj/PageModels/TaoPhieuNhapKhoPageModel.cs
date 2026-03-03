using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    public partial class TaoPhieuNhapKhoPageModel : ObservableObject
    {
        private readonly PhieuNhapKhoRepository _repository;

        [ObservableProperty] 
        private string _soPhieuNhapKhoInput;
        [ObservableProperty] 
        private string _ngayNhapKhoInput;
        [ObservableProperty] 
        private string _khoIdInput;
        [ObservableProperty] 
        private string _nccInput;
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
        private ObservableCollection<PhieuNhapKhoRawData> danhSachDong = new();

        public TaoPhieuNhapKhoPageModel(PhieuNhapKhoRepository repository)
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

                DanhSachDong.Add(new PhieuNhapKhoRawData
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
        private void RemoveLine(PhieuNhapKhoRawData? item)
        {
            if (item == null) return;
            DanhSachDong.Remove(item);
        }

        [RelayCommand]
        private async Task Save()
        {
            try
            {
                if (string.IsNullOrWhiteSpace(SoPhieuNhapKhoInput))
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Số phiếu nhập không được rỗng.", "OK");
                    return;
                }

                if (!int.TryParse(KhoIdInput, out var khoId) || khoId <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Kho không hợp lệ.", "OK");
                    return;
                }

                if (!int.TryParse(NccInput, out var nccId) || nccId <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "NCC không hợp lệ.", "OK");
                    return;
                }

                DateTime ngayNhap;
                if (string.IsNullOrWhiteSpace(NgayNhapKhoInput))
                {
                    ngayNhap = DateTime.Now;
                }
                else if (!DateTime.TryParseExact(
                         NgayNhapKhoInput,
                         "dd/MM/yyyy",
                         CultureInfo.InvariantCulture,
                         DateTimeStyles.None,
                         out ngayNhap))
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Ngày nhập kho không hợp lệ.", "OK");
                    return;
                }

                if (DanhSachDong.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Phiếu nhập phải có ít nhất 1 dòng hàng.", "OK");
                    return;
                }

                var header = new PhieuNhapKhoHeader
                {
                    So_Phieu_Nhap_Kho = SoPhieuNhapKhoInput.Trim(),
                    Ngay_Nhap_Kho = ngayNhap,
                    NCC_ID = nccId,
                    Kho_ID = khoId,
                    Ghi_Chu = GhiChuInput
                };

                await _repository.SaveAsync(header, DanhSachDong.ToList());

                await Shell.Current.DisplayAlertAsync("Thành công", "Đã lưu phiếu nhập kho.", "OK");

                // Reset form
                SoPhieuNhapKhoInput = string.Empty;
                NgayNhapKhoInput = string.Empty;
                KhoIdInput = string.Empty;
                NccInput = string.Empty;
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