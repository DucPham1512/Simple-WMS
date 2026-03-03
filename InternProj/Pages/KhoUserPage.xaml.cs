using InternProj.PageModels;

namespace InternProj.Pages;

public partial class KhoUserPage : ContentPage
{
    public KhoUserPage(KhoUserPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}