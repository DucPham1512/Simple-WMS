using InternProj.PageModels;

namespace InternProj.Pages;

public partial class KhoPage : ContentPage
{
    public KhoPage(KhoPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}