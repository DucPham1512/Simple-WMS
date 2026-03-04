using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.DataGrid.Helper;
using Syncfusion.Maui.Inputs;

namespace InternProj.Pages;

public partial class SanPhamPage : ContentPage
{
    private readonly SanPhamPageModel _vm;
    public SanPhamPage(SanPhamPageModel vm)
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
        if (picker.BindingContext is not SanPham item) return;
        if (picker.SelectedItem is not string action) return;
        if (BindingContext is not SanPhamPageModel vm) return;

        try
        {
            switch (action)
            {
                case "Lưu":
                    await vm.EditCommand.ExecuteAsync(item);
                    break;

                case "Xóa":
                    var confirm = await DisplayAlertAsync("Xác nhận", $"Bạn có chắc muốn xóa sản phẩm '{item.Ten_SP}'?", "Có", "Không");
                    if (confirm)
                    {
                        await vm.DeleteCommand.ExecuteAsync(item);
                    }
                    break;
            }
        }
        finally
        {
            // reset so user can pick again later
            picker.SelectedIndex = -1;
        }
    }

    private void LSPCombo_SelectionChanged(object sender, EventArgs e)
    {
        if (sender is not SfComboBox combo) return;
        if (combo.BindingContext is not SanPham row) return;
        if (BindingContext is not SanPhamPageModel vm) return;

        vm.SyncTenLSPForRow(row);
    }

    private void DVTCombo_SelectionChanged(object sender, EventArgs e)
    {
        if (sender is not SfComboBox combo) return;
        if (combo.BindingContext is not SanPham row) return;
        if (BindingContext is not SanPhamPageModel vm) return;
        vm.SyncTenDVTForRow(row);
    }

    private async void SP_CurrentCellEndEdit(object sender, DataGridCurrentCellEndEditEventArgs e)
    {
        if (sender is not SfDataGrid dataGrid)
            return;

        int columnIndex = dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
        if (columnIndex < 0 || columnIndex >= dataGrid.Columns.Count)
            return;

        var mapping = dataGrid.Columns[columnIndex].MappingName;
        if (mapping is not ("Ma_SP" or "Ten_SP" or "Id_LSP" or "Ten_LSP" or "Id_DVT" or "Ten_DVT" or "Ghi_Chu"))
            return;

        if (dataGrid.CurrentRow is not SanPham row)
            return;
        
        try
        {
            if (BindingContext is SanPhamPageModel vm)
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