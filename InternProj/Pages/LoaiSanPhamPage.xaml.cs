using InternProj.PageModels;

namespace InternProj.Pages;

public partial class LoaiSanPhamPage : ContentPage
{
    public LoaiSanPhamPage(LoaiSanPhamPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}