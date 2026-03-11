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
    public partial class HangXuatReportPageModel : BasePageModel
    {
        private readonly PhieuXuatKhoRepository _repository;

        [ObservableProperty]
        private DateTime _startDate = DateTime.Today;

        [ObservableProperty]
        private DateTime _endDate = DateTime.Today;

        [ObservableProperty]
        private ObservableCollection<XuatKhoReportData> _danhSachHang = new();

        
        public HangXuatReportPageModel  (PhieuXuatKhoRepository repository, 
                                        DatabaseWatcherService databaseWatcherService) : base(databaseWatcherService)
        {
            _repository = repository;
        }

        [RelayCommand]
        public override async Task LoadData()
        {
            var data = await _repository.GetByDateRangeAsync(StartDate, EndDate);
            DanhSachHang = new ObservableCollection<XuatKhoReportData>(data);
        }
    }
}