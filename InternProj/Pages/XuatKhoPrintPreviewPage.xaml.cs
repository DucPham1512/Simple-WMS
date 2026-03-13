using InternProj.PageModels;

namespace InternProj.Pages;

public partial class XuatKhoPrintPreviewPage : ContentPage
{
    public XuatKhoPrintPreviewPage(XuatKhoPrintPreviewPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}