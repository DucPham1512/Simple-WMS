
using Syncfusion.Maui.Toolkit.SegmentedControl;
using InternProj.Pages;
using Microsoft.Maui.Controls;
//using Org.Apache.Http.Conn.Routing;

namespace InternProj
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(TaoPhieuNhapKhoPage), typeof(TaoPhieuNhapKhoPage));
            Routing.RegisterRoute(nameof(EditPhieuNhapKhoPage), typeof(EditPhieuNhapKhoPage));
            Routing.RegisterRoute(nameof(TaoPhieuXuatKhoPage), typeof(TaoPhieuXuatKhoPage));
            Routing.RegisterRoute(nameof(EditPhieuXuatKhoPage), typeof(EditPhieuXuatKhoPage));
        }


    }
}
