using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using InternProj.Pages;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace InternProj.PageModels
{
    public partial class HangNhapReportPageModel : BasePageModel
    {
        private readonly PhieuNhapKhoRepository _repository;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<NhapKhoReportData> _danhSachHang = new();

        [ObservableProperty]
        private decimal _tongSoLuong;

        [ObservableProperty]
        private decimal _tongThanhTien;

        public HangNhapReportPageModel(PhieuNhapKhoRepository repository,
                                        DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
        }


        [RelayCommand]
        public override async Task LoadData()
        {
            var data = await _repository.GetByDateRangeAsync(StartDate, EndDate);
            DanhSachHang = new ObservableCollection<NhapKhoReportData>(data);
            TongSoLuong = DanhSachHang.Sum(h => h.SoLuong);
            TongThanhTien = DanhSachHang.Sum(h => h.ThanhTien);
        }
    }
}