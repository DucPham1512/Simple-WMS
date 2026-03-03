using InternProj.Data;
using InternProj.PageModels;
using InternProj.Pages;
using Microsoft.Extensions.Logging;
using Syncfusion.Licensing;
using Syncfusion.Maui.Core.Hosting;

namespace InternProj
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureSyncfusionCore()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });
            SyncfusionLicenseProvider.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1JGaF1cXmhKYVppR2NbeU55flBBallXVBYiSV9jS3hTdUVhW35bd3FTQmNbV091XQ==");
#if DEBUG
            builder.Logging.AddDebug();
            builder.Services.AddSingleton<DonViTinhRepository>();
            builder.Services.AddTransient<DonViTinhPageModel>();
            builder.Services.AddTransient<DonViTinhPage>();
            builder.Services.AddSingleton<LoaiSanPhamRepository>();
            builder.Services.AddTransient<LoaiSanPhamPageModel>();
            builder.Services.AddTransient<LoaiSanPhamPage>();
            builder.Services.AddSingleton<PhieuNhapKhoRepository>();
            builder.Services.AddTransient<PhieuNhapKhoHeaderListPageModel>();
            builder.Services.AddTransient<PhieuNhapKhoHeaderListPage>();
            builder.Services.AddTransient<TaoPhieuNhapKhoPageModel>();
            builder.Services.AddTransient<TaoPhieuNhapKhoPage>();
            builder.Services.AddTransient<EditPhieuNhapKhoPageModel>();
            builder.Services.AddTransient<EditPhieuNhapKhoPage>();
            builder.Services.AddTransient<PrintPreviewPageModel>();
            builder.Services.AddTransient<PrintPreviewPage>();

            builder.Services.AddSingleton<KhoUserRepository>();
            builder.Services.AddTransient<KhoUserPageModel>();
            builder.Services.AddTransient<KhoUserPage>();
            builder.Services.AddSingleton<KhoRepository>();
            builder.Services.AddTransient<KhoPageModel>();
            builder.Services.AddTransient<KhoPage>();
            builder.Services.AddSingleton<SanPhamRepository>();
            builder.Services.AddTransient<SanPhamPageModel>();
            builder.Services.AddTransient<SanPhamPage>();
            builder.Services.AddSingleton<NhaCungCapRepository>();
            builder.Services.AddTransient<NhaCungCapPageModel>();
            builder.Services.AddTransient<NhaCungCapPage>();
#endif

            return builder.Build();
        }
    }
}
