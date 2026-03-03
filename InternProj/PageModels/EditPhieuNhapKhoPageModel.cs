using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    [QueryProperty(nameof(Header), "Header")]
    public partial class EditPhieuNhapKhoPageModel : ObservableObject
    {
        private readonly PhieuNhapKhoRepository _repository;
        [ObservableProperty]
        private PhieuNhapKhoHeader _header;

        [ObservableProperty]
        private PhieuNhapKhoData? _selectedLine;
        [ObservableProperty]
        private string _sanPhamIdInput;
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

        [ObservableProperty] 
        private string _soLuongInput;
        [ObservableProperty] 
        private string _donGiaInput;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuNhapKhoRawData> _danhSachDong = new();

        public EditPhieuNhapKhoPageModel(PhieuNhapKhoRepository repository)
        {
            _repository = repository;
        }

        partial void OnHeaderChanged(PhieuNhapKhoHeader value)
        {
            if (value != null)
                _ = InitializeAsync(value);
        }

        public async Task InitializeAsync(PhieuNhapKhoHeader header)
        {
            //Header = header;

            SoPhieuNhapKhoInput = header.So_Phieu_Nhap_Kho;
            NgayNhapKhoInput = header.Ngay_Nhap_Kho.ToString("dd/MM/yyyy");
            KhoIdInput = header.Kho_ID.ToString();
            NccInput = header.NCC_ID.ToString();
            GhiChuInput = header.Ghi_Chu ?? "";

            var data = await _repository.GetAsync(header.Id);
            DanhSachDong = new ObservableCollection<PhieuNhapKhoRawData>(data);
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

                PhieuNhapKhoData newLine = new PhieuNhapKhoData
                {
                    NhapKhoId = Header.Id,
                    SanPhamId = sanPhamId,
                    SoLuong = soLuong,
                    DonGia = donGia
                };

                await _repository.EditDataAsync(newLine);
                DanhSachDong.Add(newLine);
                

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
        private async Task RemoveLine(PhieuNhapKhoRawData? item)
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
                else if (!DateTime.TryParseExact(NgayNhapKhoInput, 
                                                 "dd/MM/yyyy", 
                                                  CultureInfo.InvariantCulture, 
                                                  DateTimeStyles.None, 
                                                  out ngayNhap))
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Ngày nhập kho không hợp lệ.", "OK");
                    return;
                }

                var header = new PhieuNhapKhoHeader
                {
                    Id = Header.Id,
                    So_Phieu_Nhap_Kho = SoPhieuNhapKhoInput.Trim(),
                    Ngay_Nhap_Kho = ngayNhap,
                    NCC_ID = nccId,
                    Kho_ID = khoId,
                    Ghi_Chu = GhiChuInput
                };

                await _repository.EditHeaderAsync(header);

                await LoadData();
                await Shell.Current.DisplayAlertAsync("Thành công", "Đã sửa phiếu nhập kho.", "OK");

                // Reset form
                SoPhieuNhapKhoInput = string.Empty;
                NgayNhapKhoInput = string.Empty;
                KhoIdInput = string.Empty;
                NccInput = string.Empty;
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

                var line =(new PhieuNhapKhoRawData
                {
                    Id = _selectedLine.Id,
                    SoLuong = soLuong,
                    DonGia = donGia
                });

                await _repository.EditDataAsync(line);

                // Clear current line inputs after adding
                SoLuongInput = string.Empty;
                DonGiaInput = string.Empty;
                LoadData();
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
            DanhSachDong = new ObservableCollection<PhieuNhapKhoRawData>(data);
        }

        partial void OnSelectedLineChanged(PhieuNhapKhoData? value)
        {
            if (value == null) return;


            SoLuongInput = value.SoLuong.ToString();
            DonGiaInput = value.DonGia.ToString(CultureInfo.InvariantCulture);

            SanPhamIdInput = value.SanPhamId.ToString();

        }
    }
    }