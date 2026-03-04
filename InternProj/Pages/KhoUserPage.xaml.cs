using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.DataGrid;
using Syncfusion.Maui.Inputs;
using Syncfusion.Maui.DataGrid.Helper;

namespace InternProj.Pages;

public partial class KhoUserPage : ContentPage
{
    private readonly KhoUserPageModel _vm;
    public KhoUserPage(KhoUserPageModel vm)
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

    private void KhoCombo_SelectionChanged(object sender, EventArgs e)
    {
        if (sender is not SfComboBox combo) return;
        if (combo.BindingContext is not KhoUser row) return;
        if (BindingContext is not KhoUserPageModel vm) return;

        vm.SyncTenKhoForRow(row);
    }

    private async void KhoUser_CurrentCellEndEdit(object sender, DataGridCurrentCellEndEditEventArgs e)
    {
        if (sender is not SfDataGrid dataGrid)
            return;

        int columnIndex = dataGrid.ResolveToGridVisibleColumnIndex(e.RowColumnIndex.ColumnIndex);
        if (columnIndex < 0 || columnIndex >= dataGrid.Columns.Count)
            return;

        var mapping = dataGrid.Columns[columnIndex].MappingName;
        if (mapping is not ("MaDangNhap" or "KhoId" or "Ten_Kho"))
            return;

        if (dataGrid.CurrentRow is not KhoUser row)
            return;

        try
        {
            if (BindingContext is KhoUserPageModel vm)
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