using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using InternProj.Data;
using InternProj.Models;
using InternProj.Pages;
using Microsoft.Maui.Controls;
using System.Collections.ObjectModel;
//using static Android.Preferences.PreferenceActivity;

namespace InternProj.PageModels
{
    public partial class HangXuatReportPageModel : ObservableObject
    {
        private readonly PhieuXuatKhoRepository _repository;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<XuatKhoReportData> _danhSachHang = new();

        
        public HangXuatReportPageModel(PhieuXuatKhoRepository repository)
        {
            _repository = repository;
        }

        [RelayCommand]
        private async Task Search()
        {
            var data = await _repository.GetByDateRangeAsync(StartDate, EndDate);
            DanhSachHang = new ObservableCollection<XuatKhoReportData>(data);
        }
    }
}