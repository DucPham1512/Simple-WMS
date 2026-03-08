using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.Inputs;

namespace InternProj.Pages
{
    public partial class PhieuNhapKhoHeaderListPage : ContentPage
    {
        private readonly PhieuNhapKhoHeaderListPageModel _vm;

        public PhieuNhapKhoHeaderListPage(PhieuNhapKhoHeaderListPageModel vm)
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
            System.Diagnostics.Debug.WriteLine("[DEBUG] ActionPicker_SelectedIndexChanged triggered");
            
            if (sender is not Picker picker) 
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] Sender is not a Picker");
                return;
            }
            if (picker.BindingContext is not PhieuNhapKhoHeader item) 
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] BindingContext is not PhieuNhapKhoHeader: {picker.BindingContext?.GetType().Name ?? "NULL"}");
                return;
            }
            if (picker.SelectedItem is not string action) 
            {
                System.Diagnostics.Debug.WriteLine($"[DEBUG] SelectedItem is not string: {picker.SelectedItem?.GetType().Name ?? "NULL"}");
                return;
            }
            if (_vm == null) 
            {
                System.Diagnostics.Debug.WriteLine("[DEBUG] _vm is NULL");
                return;
            }

            System.Diagnostics.Debug.WriteLine($"[DEBUG] Action '{action}' picked for Item '{item.Id}'");

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
                        var confirm = await Application.Current.MainPage.DisplayAlert("Xác nhận", $"Bạn có chắc muốn xóa phiếu nhập kho '{item.So_Phieu_Nhap_Kho}'?", "Có", "Không");
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
            if (combo.BindingContext is not PhieuNhapKhoHeader row) return;
            if (_vm == null) return;

            _vm.SyncTenKhoForRow(row);
        }

        private void NccCombo_SelectionChanged(object sender, EventArgs e)
        {
            if (sender is not SfComboBox combo) return;
            if (combo.BindingContext is not PhieuNhapKhoHeader row) return;
            if (_vm == null) return;
            _vm.SyncTenNCCForRow(row);
        }
    }
}