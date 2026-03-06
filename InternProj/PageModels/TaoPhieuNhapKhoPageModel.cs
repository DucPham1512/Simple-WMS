using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Text.RegularExpressions;

namespace InternProj.PageModels
{
    public partial class TaoPhieuNhapKhoPageModel : ObservableObject
    {
        private readonly PhieuNhapKhoRepository _repository;

        private readonly KhoRepository _khoRepository;

        private readonly NhaCungCapRepository _nccRepository;

        private readonly SanPhamRepository _spRepository;

        [ObservableProperty] 
        private string _soPhieuNhapKhoInput;
        [ObservableProperty] 
        private DateTime _ngayNhapKhoInput = DateTime.Today;
        [ObservableProperty] 
        private string _ghiChuInput;

        // Current line inputs
        [ObservableProperty] 
        private string _soLuongInput;
        [ObservableProperty] 
        private string _donGiaInput;
        [ObservableProperty]
        private Kho? _selectedKho;
        [ObservableProperty]
        private NhaCungCap? _selectedNCC;
        [ObservableProperty]
        private SanPham? _selectedSP;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuNhapKhoRawData> _danhSachDong = new();

        [ObservableProperty]
        private ObservableCollection<Kho> _danhSachKho = [];

        [ObservableProperty]
        private ObservableCollection<NhaCungCap> _danhSachNCC = [];

        [ObservableProperty]
        private ObservableCollection<SanPham> _danhSachSP = [];

        public TaoPhieuNhapKhoPageModel(PhieuNhapKhoRepository repository, KhoRepository khoRepository, NhaCungCapRepository nccRepository, SanPhamRepository spRepository)
        {
            _repository = repository;
            _khoRepository = khoRepository;
            _nccRepository = nccRepository;
            _spRepository = spRepository;
        }

        [RelayCommand]
        private async Task Load()
        {
            var listKho = await _khoRepository.ListAsync();
            DanhSachKho = new ObservableCollection<Kho>(listKho);
            var listNCC = await _nccRepository.ListAsync();
            DanhSachNCC = new ObservableCollection<NhaCungCap>(listNCC);
            var listSP = await _spRepository.ListAsync();
            DanhSachSP = new ObservableCollection<SanPham>(listSP);
        }

        [RelayCommand]
        private async Task AddLine()
        {
            try
            {
                if(SelectedSP is null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Chưa chọn sản phẩm", "OK");
                    return;
                }

                if (!int.TryParse(SoLuongInput, out var soLuong) || soLuong <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Số lượng không hợp lệ.", "OK");
                    return;
                }

                if (!float.TryParse(DonGiaInput, out var donGia) || donGia < 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Đơn giá không hợp lệ.", "OK");
                    return;
                }

                DanhSachDong.Add(new PhieuNhapKhoData
                {
                    SanPhamId = SelectedSP.Id,
                    TenSP = Regex.Replace(SelectedSP.Ten_SP, @"\s+", " ").Trim(),
                    MaSP = SelectedSP.Ma_SP,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    ThanhTien = soLuong * donGia,
                });

                // Clear current line inputs after adding
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

                if (SelectedNCC is null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Chưa chọn nhà cung cấp", "OK");
                    return;
                }

                if (SelectedKho is null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Chưa chọn kho", "OK");
                    return;
                }

                if (DanhSachDong.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Phiếu nhập phải có ít nhất 1 dòng hàng.", "OK");
                    return;
                }

                var header = new PhieuNhapKhoHeader
                {
                    So_Phieu_Nhap_Kho = Regex.Replace(SoPhieuNhapKhoInput, @"\s+", " ").Trim(),
                    Ngay_Nhap_Kho = NgayNhapKhoInput,
                    NCC_ID = SelectedNCC.Id,
                    Kho_ID = SelectedKho.Id,
                    Ghi_Chu = GhiChuInput
                };

                await _repository.SaveAsync(header, DanhSachDong.ToList());

                await Shell.Current.DisplayAlertAsync("Thành công", "Đã lưu phiếu nhập kho.", "OK");

                await Shell.Current.GoToAsync("..");
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}