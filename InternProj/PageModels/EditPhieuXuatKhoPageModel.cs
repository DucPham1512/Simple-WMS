using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using System.Collections.ObjectModel;
using System.Globalization;

namespace InternProj.PageModels
{
    [QueryProperty(nameof(Header), "Header")]
    public partial class EditPhieuXuatKhoPageModel : BasePageModel
    {
        private readonly PhieuXuatKhoRepository _repository;
        private readonly SanPhamRepository _spRepository;
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
        [ObservableProperty]
        private SanPham _selectedSP;

        // Temporary lines shown on screen before saving
        [ObservableProperty]
        private ObservableCollection<PhieuXuatKhoRawData> _danhSachDong = new();
        [ObservableProperty]
        private ObservableCollection<SanPham> _danhSachSP = new();

        public EditPhieuXuatKhoPageModel    (PhieuXuatKhoRepository repository, 
                                            SanPhamRepository spRepository, 
                                            DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
            _spRepository = spRepository;
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
            var listSP = await _spRepository.ListAsync();
            DanhSachSP = new ObservableCollection<SanPham>(listSP);

        }

        [RelayCommand]
        private async Task AddLine()
        {
            try
            {
                if (SelectedSP is null)
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


                PhieuXuatKhoData newLine = new PhieuXuatKhoData
                {
                    XuatKhoId = Header.Id,
                    SanPhamId = SelectedSP.Id,
                    TenSP = SelectedSP.Ten_SP,
                    MaSP = SelectedSP.Ma_SP,
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
                await Shell.Current.DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }


        [RelayCommand]
        private async Task RemoveLine(PhieuXuatKhoRawData? item)
        {
            if (item == null) return;
            await _repository.DeleteDataAsync(item);
        }

        [RelayCommand]
        public override async Task LoadData()
        {
            var data = await _repository.GetAsync(Header.Id);
            DanhSachDong = new ObservableCollection<PhieuXuatKhoRawData>(data);
        }

    [RelayCommand]
        private async Task Edit(PhieuXuatKhoData item)
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