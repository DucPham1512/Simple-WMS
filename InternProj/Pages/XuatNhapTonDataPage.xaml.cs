using InternProj.Models;
using InternProj.PageModels;
using Syncfusion.Maui.Inputs;

namespace InternProj.Pages
{
    public partial class XuatNhapTonDataPage : ContentPage
    {
        private readonly XuatNhapTonDataPageModel _vm;

        public XuatNhapTonDataPage(XuatNhapTonDataPageModel vm)
        {
            InitializeComponent();
            BindingContext = _vm = vm;
        }
    }
}