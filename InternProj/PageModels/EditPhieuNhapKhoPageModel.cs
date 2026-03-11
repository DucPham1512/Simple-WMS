using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    [QueryProperty(nameof(Header), "Header")]
    public partial class EditPhieuNhapKhoPageModel : BasePageModel
    {
        private readonly PhieuNhapKhoRepository _repository;
        private readonly SanPhamRepository _spRepository;
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
        private SanPham? _selectedSanPham;
        [ObservableProperty]
        private string _soLuongInput;
        [ObservableProperty]
        private string _donGiaInput;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuNhapKhoRawData> _danhSachDong = new();

        [ObservableProperty]
        private ObservableCollection<SanPham> _danhSachSP = new();

        public EditPhieuNhapKhoPageModel    (PhieuNhapKhoRepository repository, 
                                            SanPhamRepository spRepository, 
                                            DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
            _spRepository = spRepository;
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
            var listSP = await _spRepository.ListAsync();
            DanhSachSP = new ObservableCollection<SanPham>(listSP);
        }

        [RelayCommand]
        private async Task AddLine()
        {
            try
            {
                if (SelectedSanPham is null)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Chưa chọn sản phẩm", "OK");
                    return;
                }

                if (!float.TryParse(SoLuongInput, out var soLuong) || soLuong <= 0)
                {
                    await Shell.Current.DisplayAlertAsync("Lỗi", "Số lượng không hợp lệ.", "OK");
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
                    SanPhamId = SelectedSanPham.Id,
                    TenSP = SelectedSanPham.Ten_SP,
                    MaSP = SelectedSanPham.Ma_SP,
                    SoLuong = soLuong,
                    DonGia = donGia,
                    ThanhTien = soLuong * donGia
                };

                await _repository.EditDataAsync(newLine);

                // Clear current line inputs after adding
                SanPhamIdInput = string.Empty;
                SoLuongInput = string.Empty;
                DonGiaInput = string.Empty;
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", $"Không thể thêm '{SelectedSanPham.Ten_SP}' ", "OK");
            }
        }


        [RelayCommand]
        private async Task RemoveLine(PhieuNhapKhoRawData? item)
        {
            if (item == null) return;
            await _repository.DeleteDataAsync(item);
        }

        [RelayCommand]
        public override async Task LoadData()
        {
            var data = await _repository.GetAsync(Header.Id);
            DanhSachDong = new ObservableCollection<PhieuNhapKhoRawData>(data);
        }

        [RelayCommand]
        private async Task Edit(PhieuNhapKhoData item)
        {
            try
            {
                await _repository.EditDataAsync(item);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}