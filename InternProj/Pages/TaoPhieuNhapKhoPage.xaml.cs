using InternProj.PageModels;
using Microsoft.Maui.Controls;

namespace InternProj.Pages
{
    public partial class TaoPhieuNhapKhoPage : ContentPage
    {
        public TaoPhieuNhapKhoPage(TaoPhieuNhapKhoPageModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}