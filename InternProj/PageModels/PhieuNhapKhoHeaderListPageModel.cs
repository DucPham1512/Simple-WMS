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
    public partial class PhieuNhapKhoHeaderListPageModel : ObservableObject
    {
        private readonly PhieuNhapKhoRepository _repository;

        [ObservableProperty]
        private ObservableCollection<PhieuNhapKhoHeader> _danhSachPhieu = new();

        [ObservableProperty]
        private PhieuNhapKhoHeader? _selectedItem;

        public PhieuNhapKhoHeaderListPageModel(PhieuNhapKhoRepository repository)
        {
            _repository = repository;
        }

        // This is called automatically when SelectedItem changes
        partial void OnSelectedItemChanged(PhieuNhapKhoHeader? value)
        {
            if (value == null) return;

            // can't make this partial method async, so fire-and-forget safely:
            _ = OpenEditAsync(value);
        }


        [RelayCommand]
        private async Task LoadData()
        {
            var data = await _repository.ListAsync();
            DanhSachPhieu = new ObservableCollection<PhieuNhapKhoHeader>(data);
        }

        [RelayCommand]
        private async Task New()
        {
            await Shell.Current.GoToAsync(nameof(TaoPhieuNhapKhoPage));
        }

        [RelayCommand]
        private async Task Save(PhieuNhapKhoHeader item)
        {
            await _repository.EditHeaderAsync(item);
            await LoadData();
        }

        [RelayCommand]
        private async Task Delete(PhieuNhapKhoHeader item)
        {
            await _repository.DeleteItemAsync(item);
            await LoadData();
        }


        public async Task OpenEditAsync(PhieuNhapKhoHeader item)
        {
            await Shell.Current.GoToAsync(
                nameof(EditPhieuNhapKhoPage),
                new Dictionary<string, object>
                {
                    ["Header"] = item
                });
            SelectedItem = null;
        }

        [RelayCommand]
        private async Task OpenPrintPreview(PhieuNhapKhoHeader? item)
        {
            var lines = await _repository.GetAsync(item.Id);

            var page = App.Current?.Handler?.MauiContext?.Services.GetService<PrintPreviewPage>();
            if (page?.BindingContext is PrintPreviewPageModel vm)
            {
                vm.Load(item, lines);
                await Shell.Current.Navigation.PushAsync(page);
            }
        }

        public IReadOnlyList<string> ActionOptions { get; } =
            new[] {"Lưu","Sửa", "Xóa","In" };


        // Placeholder, await for get method from Kho repository
        public IReadOnlyList<string> DanhSachKho { get;  } =
            new[] {"Kho A", "Kho B", "Kho C" };

    }
}