using InternProj.PageModels;
using Microsoft.Maui.Controls;

namespace InternProj.Pages
{
    public partial class TaoPhieuNhapKhoPage : ContentPage
    {
        private readonly TaoPhieuNhapKhoPageModel _vm;
        public TaoPhieuNhapKhoPage(TaoPhieuNhapKhoPageModel vm)
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
                await _vm.LoadCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}