using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.Inputs;

namespace InternProj.Pages
{
    public partial class HangXuatReportPage : ContentPage
    {
        private readonly HangXuatReportPageModel _vm;

        public HangXuatReportPage(HangXuatReportPageModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }
    }
}