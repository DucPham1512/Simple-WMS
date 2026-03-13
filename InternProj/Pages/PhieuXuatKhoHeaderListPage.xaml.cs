using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.Inputs;

namespace InternProj.Pages
{
    public partial class PhieuXuatKhoHeaderListPage : ContentPage
    {
        private readonly PhieuXuatKhoHeaderListPageModel _vm;

        public PhieuXuatKhoHeaderListPage(PhieuXuatKhoHeaderListPageModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                // Load whenever you enter/return to this screen
                await _vm.LoadDataCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }

        private async void ActionPicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (sender is not Picker picker) return;
            if (picker.BindingContext is not PhieuXuatKhoHeader item) return;
            if (picker.SelectedItem is not string action) return;
            if (_vm == null) return;

            try
            {
                switch (action)
                {
                    case "Lưu":
                        await _vm.SaveCommand.ExecuteAsync(item);
                        break;
                    case "Sửa":
                        await _vm.OpenEditAsync(item);
                        break;

                    case "Xóa":
                        var confirm = await Shell.Current.DisplayAlertAsync("Xác nhận", $"Bạn có chắc muốn xóa phiếu xuất kho '{item.So_Phieu_Xuat_Kho}'?", "Có", "Không");
                        if (confirm)
                        {
                            await _vm.DeleteCommand.ExecuteAsync(item);
                        }
                        break;

                    case "In":
                        await _vm.OpenPrintPreviewCommand.ExecuteAsync(item);
                        break;
                }
            }
            finally
            {
                // reset so user can pick again later
                picker.SelectedIndex = -1;
            }
        }

        private void KhoCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is not SfComboBox combo) return;
            if (combo.BindingContext is not PhieuXuatKhoHeader row) return;
            if (_vm == null) return;

            _vm.SyncTenKhoForRow(row);
        }
    }
}