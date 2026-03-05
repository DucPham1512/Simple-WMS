using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using InternProj.Pages;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;

namespace InternProj.PageModels
{
    public partial class HangNhapReportPageModel : ObservableObject
    {
        private readonly PhieuNhapKhoRepository _repository;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<NhapKhoReportData> _danhSachHang = new();

        public HangNhapReportPageModel(PhieuNhapKhoRepository repository)
        {
            _repository = repository;
        }


        [RelayCommand]
        private async Task Search()
        {
            var data = await _repository.GetByDateRangeAsync(StartDate, EndDate);
            DanhSachHang = new ObservableCollection<NhapKhoReportData>(data);
        }
    }
}