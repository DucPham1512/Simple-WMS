using InternProj.PageModels;

namespace InternProj.Pages;

public partial class NhaCungCapPage : ContentPage
{
    public NhaCungCapPage(NhaCungCapPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}