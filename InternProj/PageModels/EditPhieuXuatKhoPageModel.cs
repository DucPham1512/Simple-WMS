using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    [QueryProperty(nameof(Header), "Header")]
    public partial class EditPhieuXuatKhoPageModel : ObservableObject
    {
        private readonly PhieuXuatKhoRepository _repository;
        [ObservableProperty]
        private PhieuXuatKhoHeader _header;

        [ObservableProperty]
        private PhieuXuatKhoData? _selectedLine;
        [ObservableProperty]
        private string _sanPhamIdInput;
        [ObservableProperty]
        private string _soPhieuXuatKhoInput;
        [ObservableProperty]
        private string _ngayXuatKhoInput;
        [ObservableProperty]
        private string _khoIdInput;
        [ObservableProperty]
        private string _ghiChuInput;

        [ObservableProperty]
        private string _soLuongInput;
        [ObservableProperty]
        private string _donGiaInput;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuXuatKhoRawData> _danhSachDong = new();

        public EditPhieuXuatKhoPageModel(PhieuXuatKhoRepository repository)
        {
            _repository = repository;

        }

        partial void OnHeaderChanged(PhieuXuatKhoHeader value)
        {
            if (value != null)
                _ = InitializeAsync(value);
        }

        public async Task InitializeAsync(PhieuXuatKhoHeader header)
        {
            //Header = header;

            SoPhieuXuatKhoInput = header.So_Phieu_Xuat_Kho;
            NgayXuatKhoInput = header.Ngay_Xuat_Kho.ToString("dd/MM/yyyy");
            KhoIdInput = header.Kho_ID.ToString();
            GhiChuInput = header.Ghi_Chu ?? "";

            var data = await _repository.GetAsync(header.Id);
            DanhSachDong = new ObservableCollection<PhieuXuatKhoRawData>(data);

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

                PhieuXuatKhoData newLine = new PhieuXuatKhoData
                {
                    XuatKhoId = Header.Id,
                    SanPhamId = sanPhamId,
                    SoLuong = soLuong,
                    DonGia = donGia
                };

                await _repository.EditDataAsync(newLine);
                DanhSachDong.Add(newLine);

                await LoadData();

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
        private async Task RemoveLine(PhieuXuatKhoRawData? item)
        {
            if (item == null) return;
            await _repository.DeleteDataAsync(item);
            DanhSachDong.Remove(item);
        }

        [RelayCommand]
        private async Task EditHeader()
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

                DateTime ngayXuat;
                if (string.IsNullOrWhiteSpace(NgayXuatKhoInput))
                {
                    ngayXuat = DateTime.Now;
                }
                else if (!DateTime.TryParseExact(NgayXuatKhoInput,
                                                 "dd/MM/yyyy",
                                                  CultureInfo.InvariantCulture,
                                                  DateTimeStyles.None,
                                                  out ngayXuat))
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Ngày nhập kho không hợp lệ.", "OK");
                    return;
                }

                var header = new PhieuXuatKhoHeader
                {
                    Id = Header.Id,
                    So_Phieu_Xuat_Kho = SoPhieuXuatKhoInput.Trim(),
                    Ngay_Xuat_Kho = ngayXuat,
                    Kho_ID = khoId,
                    Ghi_Chu = GhiChuInput
                };

                await _repository.EditHeaderAsync(header);

                await LoadData();
                await Shell.Current.DisplayAlertAsync("Thành công", "Đã sửa phiếu nhập kho.", "OK");

                // Reset form
                SoPhieuXuatKhoInput = string.Empty;
                NgayXuatKhoInput = string.Empty;
                KhoIdInput = string.Empty;
                GhiChuInput = string.Empty;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }

        [RelayCommand]
        private async Task EditLine()
        {
            try
            {
                if (SelectedLine == null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Vui lòng chọn dòng cần sửa.", "OK");
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

                var line = (new PhieuXuatKhoRawData
                {
                    Id = _selectedLine.Id,
                    SoLuong = soLuong,
                    DonGia = donGia
                });

                await _repository.EditDataAsync(line);

                // Clear current line inputs after adding
                SoLuongInput = string.Empty;
                DonGiaInput = string.Empty;
                await LoadData();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.GetAsync(Header.Id);
            DanhSachDong = new ObservableCollection<PhieuXuatKhoRawData>(data);
        }

        partial void OnSelectedLineChanged(PhieuXuatKhoData? value)
        {
            if (value == null) return;


            SoLuongInput = value.SoLuong.ToString();
            DonGiaInput = value.DonGia.ToString(CultureInfo.InvariantCulture);

            SanPhamIdInput = value.SanPhamId.ToString();

        }

    [RelayCommand]
        private async Task Edit(PhieuXuatKhoData item)
        {
            try
            {

                await _repository.EditDataAsync(item);

                await LoadData();

            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}