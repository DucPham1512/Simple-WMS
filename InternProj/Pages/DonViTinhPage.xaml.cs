using InternProj.PageModels;

namespace InternProj.Pages;

public partial class DonViTinhPage : ContentPage
{
    public DonViTinhPage(DonViTinhPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}