using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.Inputs;

namespace InternProj.Pages
{
    public partial class HangNhapReportPage : ContentPage
    {
        private readonly HangNhapReportPageModel _vm;

        public HangNhapReportPage(HangNhapReportPageModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }
    }
}