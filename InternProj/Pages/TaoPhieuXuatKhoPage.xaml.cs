using InternProj.PageModels;
using Microsoft.Maui.Controls;

namespace InternProj.Pages
{
    public partial class TaoPhieuXuatKhoPage : ContentPage
    {
        public TaoPhieuXuatKhoPage(TaoPhieuXuatKhoPageModel vm)
        {
            InitializeComponent();
            BindingContext = vm;
        }
    }
}