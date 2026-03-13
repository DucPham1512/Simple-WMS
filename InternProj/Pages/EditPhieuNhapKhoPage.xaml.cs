using InternProj.Models;
using InternProj.PageModels;
using Microsoft.Maui.Controls;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Helper;
using Syncfusion.Maui.Inputs;


namespace InternProj.Pages
{
    public partial class EditPhieuNhapKhoPage : ContentPage
    {
        private readonly EditPhieuNhapKhoPageModel _vm;

        public EditPhieuNhapKhoPage(EditPhieuNhapKhoPageModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }

        public async Task InitializeAsync(PhieuNhapKhoHeader header)
        {
            await _vm.InitializeAsync(header);
        }
        private async void EditPNK_CurrentCellEndEdit(object sender, DataGridCurrentCellEndEditEventArgs e)
        {
            if (sender is not SfDataGrid dataGrid)
                return;

            int columnIndex = dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
            if (columnIndex < 0 || columnIndex >= dataGrid.Columns.Count)
                return;

            var mapping = dataGrid.Columns[columnIndex].MappingName;
            if (mapping is not ("MaSP" or "TenSP" or "SoLuong" or "DonGia" or "ThanhTien"))
                return;

            if (dataGrid.CurrentRow is not PhieuNhapKhoData row)
                return;

            try
            {
                if (BindingContext is EditPhieuNhapKhoPageModel vm)
                {
                    Dispatcher.Dispatch(async () =>
                    {
                        await vm.EditCommand.ExecuteAsync(row);
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