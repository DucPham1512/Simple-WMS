using InternProj.Models;
using InternProj.PageModels;
using Microsoft.Maui.Controls;

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
    }
}