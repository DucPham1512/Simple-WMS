using InternProj.PageModels;
using Microsoft.Maui.Controls;

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
                await _vm.LoadCommand.ExecuteAsync(null);
            }
            catch (Exception ex)
            {
                await DisplayAlertAsync("Lỗi", ex.Message, "OK");
            }
        }
    }
}