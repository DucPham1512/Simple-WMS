using Syncfusion.Maui.Toolkit.SegmentedControl;
using InternProj.Pages;
using Microsoft.Maui.Controls;
using System;
//using Org.Apache.Http.Conn.Routing;

namespace InternProj
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute("donvitinh", typeof(DonViTinhPage));
            Routing.RegisterRoute("loaisanpham", typeof(LoaiSanPhamPage));
            Routing.RegisterRoute("sanpham", typeof(SanPhamPage));
            Routing.RegisterRoute("nhacungcap", typeof(NhaCungCapPage));
            Routing.RegisterRoute("kho", typeof(KhoPage));
            Routing.RegisterRoute("khouser", typeof(KhoUserPage));
            Routing.RegisterRoute("phieunhapkho", typeof(PhieuNhapKhoHeaderListPage));
            Routing.RegisterRoute("phieuxuatkho", typeof(PhieuXuatKhoHeaderListPage));
            Routing.RegisterRoute("hangnhapreport", typeof(HangNhapReportPage));
            Routing.RegisterRoute("hangxuatreport", typeof(HangXuatReportPage));
            Routing.RegisterRoute("xuatnhaptonreport", typeof(XuatNhapTonDataPage));

            Routing.RegisterRoute(nameof(TaoPhieuNhapKhoPage), typeof(TaoPhieuNhapKhoPage));
            Routing.RegisterRoute(nameof(EditPhieuNhapKhoPage), typeof(EditPhieuNhapKhoPage));
            Routing.RegisterRoute(nameof(TaoPhieuXuatKhoPage), typeof(TaoPhieuXuatKhoPage));
            Routing.RegisterRoute(nameof(EditPhieuXuatKhoPage), typeof(EditPhieuXuatKhoPage));
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            var target = args.Target.Location.OriginalString;
            
            // Allow the initial tabbed page to load
            if (target.Contains("maintabbedpage") || target == "//maintabbedpage")
            {
                base.OnNavigating(args);
                return;
            }

            // Cancel standard navigation
            args.Cancel();

            // Close Flyout Menu
            Shell.Current.FlyoutIsPresented = false;

            // Strip out routing prefixes like "//" to just get the route name
            var targetRoute = target.TrimStart('/');
            
            if (MainTabbedPage.Current != null)
            {
                Type pageType = ResolveTypeFromRoute(targetRoute);
                if (pageType != null)
                {
                    var services = this.Handler?.MauiContext?.Services ?? Application.Current?.MainPage?.Handler?.MauiContext?.Services;
                    if (services != null)
                    {
                        var page = (Page)services.GetService(pageType);
                        MainTabbedPage.Current.LoadPageIntoActiveTab(page, targetRoute);
                    }
                }
            }
        }

        private void OnSidebarMenuItemClicked(object sender, EventArgs e)
        {
            var menuItem = (MenuItem)sender;
            string targetRoute = (string)menuItem.CommandParameter;
            
            Shell.Current.FlyoutIsPresented = false;
            
            if (MainTabbedPage.Current != null)
            {
                Type pageType = ResolveTypeFromRoute(targetRoute);
                if (pageType != null)
                {
                    var services = this.Handler?.MauiContext?.Services ?? Application.Current?.MainPage?.Handler?.MauiContext?.Services;
                    if (services != null)
                    {
                        var page = (Page)services.GetService(pageType);
                        MainTabbedPage.Current.LoadPageIntoActiveTab(page, targetRoute);
                    }
                }
            }
        }

        private Type ResolveTypeFromRoute(string route)
        {
            return route switch
            {
                "donvitinh" => typeof(DonViTinhPage),
                "loaisanpham" => typeof(LoaiSanPhamPage),
                "sanpham" => typeof(SanPhamPage),
                "nhacungcap" => typeof(NhaCungCapPage),
                "kho" => typeof(KhoPage),
                "khouser" => typeof(KhoUserPage),
                "phieunhapkho" => typeof(PhieuNhapKhoHeaderListPage),
                "phieuxuatkho" => typeof(PhieuXuatKhoHeaderListPage),
                "hangnhapreport" => typeof(HangNhapReportPage),
                "hangxuatreport" => typeof(HangXuatReportPage),
                "xuatnhaptonreport" => typeof(XuatNhapTonDataPage),
                nameof(TaoPhieuNhapKhoPage) => typeof(TaoPhieuNhapKhoPage),
                nameof(EditPhieuNhapKhoPage) => typeof(EditPhieuNhapKhoPage),
                nameof(TaoPhieuXuatKhoPage) => typeof(TaoPhieuXuatKhoPage),
                nameof(EditPhieuXuatKhoPage) => typeof(EditPhieuXuatKhoPage),
                _ => null
            };
        }
    }
}
