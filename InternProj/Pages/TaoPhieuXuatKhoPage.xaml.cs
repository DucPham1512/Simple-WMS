using InternProj.Models;
using InternProj.PageModels;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Helper;

namespace InternProj.Pages
{
    public partial class TaoPhieuXuatKhoPage : ContentPage
    {
        private readonly TaoPhieuXuatKhoPageModel _vm;
        public TaoPhieuXuatKhoPage(TaoPhieuXuatKhoPageModel vm)
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

        private async void ReCalcThanhTien(object sender, DataGridCurrentCellEndEditEventArgs e)
        {
            if (sender is not SfDataGrid dataGrid)
                return;

            int columnIndex = dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            if (columnIndex < 0 || columnIndex >= dataGrid.Columns.Count)
                return;

            var mapping = dataGrid.Columns[columnIndex].MappingName;
            if (mapping is not ("SoLuong" or "DonGia"))
                return;

            if (dataGrid.CurrentRow is not PhieuXuatKhoData row)
                return;

            try
            {
                if (BindingContext is TaoPhieuXuatKhoPageModel vm)
                {
                    Dispatcher.Dispatch(async () =>
                    {
                        await Task.Delay(50);
                        await vm.EditLineCommand.ExecuteAsync(row);
                        dataGrid.Refresh();
                    });
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[CRASH CAUGHT] {ex}");
                await DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}