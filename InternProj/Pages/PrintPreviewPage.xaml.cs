using InternProj.PageModels;

namespace InternProj.Pages;

public partial class PrintPreviewPage : ContentPage
{
    public PrintPreviewPage(PrintPreviewPageModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}