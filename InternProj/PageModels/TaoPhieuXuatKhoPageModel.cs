using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    public partial class TaoPhieuXuatKhoPageModel : BasePageModel
    {
        private readonly PhieuXuatKhoRepository _repository;
        private readonly KhoRepository _khoRepository;
        private readonly SanPhamRepository _spRepository;

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
        [ObservableProperty]
        private Kho? _selectedKho;
        [ObservableProperty]
        private SanPham? _selectedSP;
        [ObservableProperty]
        private decimal _tongSoLuong;
        [ObservableProperty]
        private decimal _tongThanhTien;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuXuatKhoRawData> danhSachDong = new();

        [ObservableProperty]
        private ObservableCollection<Kho> _danhSachKho = [];

        [ObservableProperty]
        private ObservableCollection<SanPham> _danhSachSP = [];

        public TaoPhieuXuatKhoPageModel (PhieuXuatKhoRepository repository, 
                                        KhoRepository khoRepository, 
                                        SanPhamRepository spRepository,
                                        DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
            _khoRepository = khoRepository;
            _spRepository = spRepository;
        }

        [RelayCommand]
        public override async Task LoadData()
        {
            var listKho = await _khoRepository.ListAsync();
            DanhSachKho = new ObservableCollection<Kho>(listKho);
            var listSP = await _spRepository.ListAsync();
            DanhSachSP = new ObservableCollection<SanPham>(listSP);
        }


        [RelayCommand]
        private async Task AddLine()
        {
            try
            { 

                if (!decimal.TryParse(SoLuongInput, out var soLuong) || soLuong <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Số lượng không hợp lệ.", "OK");
                    return;
                }

                if (!decimal.TryParse(DonGiaInput, out var donGia) || donGia < 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Đơn giá không hợp lệ.", "OK");
                    return;
                }

                if(SelectedSP is null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Chưa chọn sản phẩm", "OK");
                    return;
                }

                DanhSachDong.Add(new PhieuXuatKhoData
                {
                    SanPhamId = SelectedSP.Id,
                    TenSP   = SelectedSP.Ten_SP,
                    MaSP    = SelectedSP.Ma_SP,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    ThanhTien = soLuong * donGia
                });
                TongSoLuong = DanhSachDong.Sum(x => x.SoLuong);
                TongThanhTien = DanhSachDong.Sum(x => (x.SoLuong * x.DonGia));

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
            TongSoLuong = DanhSachDong.Sum(x => x.SoLuong);
            TongThanhTien = DanhSachDong.Sum(x => (x.SoLuong * x.DonGia));
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

                if (DanhSachDong.Count == 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Phiếu nhập phải có ít nhất 1 dòng hàng.", "OK");
                    return;
                }

                if(SelectedKho is null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Chưa chọn kho", "OK");
                    return;
                }

                var header = new PhieuXuatKhoHeader
                {
                    So_Phieu_Xuat_Kho = SoPhieuXuatKhoInput.Trim(),
                    Ngay_Xuat_Kho = NgayXuatKhoInput,
                    Kho_ID = SelectedKho.Id,
                    Ghi_Chu = GhiChuInput
                };

                await _repository.SaveAsync(header, DanhSachDong.ToList());

                await Shell.Current.DisplayAlertAsync("Thành công", "Đã lưu phiếu xuất kho.", "OK");

                SoPhieuXuatKhoInput = string.Empty;
                NgayXuatKhoInput = DateTime.Today;
                GhiChuInput = string.Empty;
                SelectedKho = null;
                SelectedSP = null;
                DanhSachDong.Clear();
            }
            catch
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", "Phiếu xuất kho đã tồn tại", "OK");
                await LoadData();
            }
        }

        [RelayCommand]
        private async Task EditLine(PhieuXuatKhoData line)
        {
            try
            {
                line.ThanhTien = line.SoLuong * line.DonGia;
                TongSoLuong = DanhSachDong.Sum(x => x.SoLuong);
                TongThanhTien = DanhSachDong.Sum(x => (x.SoLuong * x.DonGia));
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}