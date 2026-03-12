using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using InternProj.Pages;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace InternProj.PageModels
{
    public partial class XuatNhapTonDataPageModel : ObservableObject
    {
        private readonly XuatNhapTonDataRepository _repository;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<XuatNhapTonData> _danhSachHang = new();

        [ObservableProperty]
        private decimal _tongSoLuongDauKy;
        [ObservableProperty]
        private decimal _tongSoLuongNhap;
        [ObservableProperty]
        private decimal _tongSoLuongXuat;
        [ObservableProperty]
        private decimal _tongSoLuongCuoiKy;
        public XuatNhapTonDataPageModel(XuatNhapTonDataRepository repository)
        {
            _repository = repository;
        }


        [RelayCommand]
        private async Task Search()
        {
            var data = await _repository.GetByDateRangeAsync(StartDate, EndDate);
            DanhSachHang = new ObservableCollection<XuatNhapTonData>(data);
            TongSoLuongNhap = DanhSachHang.Sum(x => x.SoLuongNhap);
            TongSoLuongXuat = DanhSachHang.Sum(x => x.SoLuongXuat);
            TongSoLuongDauKy = DanhSachHang.Sum(x => x.SoLuongDauKy);
            TongSoLuongCuoiKy = DanhSachHang.Sum(x => x.SoLuongCuoiKy);
        }
    }
}