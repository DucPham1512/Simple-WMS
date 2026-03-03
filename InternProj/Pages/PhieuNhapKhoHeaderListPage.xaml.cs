using InternProj.Models;
using InternProj.PageModels;

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
            if (sender is not Picker picker) return;
            if (picker.BindingContext is not PhieuNhapKhoHeader item) return;
            if (picker.SelectedItem is not string action) return;
            if (BindingContext is not PhieuNhapKhoHeaderListPageModel vm) return;

            try
            {
                switch (action)
                {
                    case "Lưu":
                        await vm.SaveCommand.ExecuteAsync(item);
                        break;
                    case "Sửa":
                        await vm.OpenEditAsync(item);
                        break;

                    case "Xóa":
                        var confirm = await DisplayAlertAsync("Xác nhận", $"Bạn có chắc muốn xóa phiếu nhập kho '{item.So_Phieu_Nhap_Kho}'?", "Có", "Không");
                        if (confirm)
                        {
                            await vm.DeleteCommand.ExecuteAsync(item);
                        }
                        break;

                    case "In":
                        await vm.OpenPrintPreviewCommand.ExecuteAsync(item);
                        break;
                }
            }
            finally
            {
                // reset so user can pick again later
                picker.SelectedIndex = -1;
            }
        }
    }
}