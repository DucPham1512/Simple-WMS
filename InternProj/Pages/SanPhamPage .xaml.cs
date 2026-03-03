using InternProj.PageModels;

namespace InternProj.Pages;

public partial class SanPhamPage : ContentPage
{
    public SanPhamPage(SanPhamPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}