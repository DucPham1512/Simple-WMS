using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Helper;

namespace InternProj.Pages;

public partial class LoaiSanPhamPage : ContentPage
{
    private readonly LoaiSanPhamPageModel _vm;
    public LoaiSanPhamPage(LoaiSanPhamPageModel vm)
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

    private async void LSP_CurrentCellEndEdit(object sender, DataGridCurrentCellEndEditEventArgs e)
    {
        if (sender is not SfDataGrid dataGrid)
            return;

        int columnIndex = dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
        if (columnIndex < 0 || columnIndex >= dataGrid.Columns.Count)
            return;

        var mapping = dataGrid.Columns[columnIndex].MappingName;
        if (mapping is not ("Ma_LSP" or "Ten_LSP" or "Ghi_Chu"))
            return;

        if (dataGrid.CurrentRow is not LoaiSanPham row)
            return;

        try
        {
            if (BindingContext is LoaiSanPhamPageModel vm)
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