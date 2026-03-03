using Microsoft.Extensions.Logging;

using InternProj.Data;
using InternProj.PageModels;
using InternProj.Pages;

namespace InternProj
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
            builder.Services.AddSingleton<DonViTinhRepository>();
            builder.Services.AddTransient<DonViTinhPageModel>();
            builder.Services.AddTransient<DonViTinhPage>();
            builder.Services.AddSingleton<LoaiSanPhamRepository>();
            builder.Services.AddTransient<LoaiSanPhamPageModel>();
            builder.Services.AddTransient<LoaiSanPhamPage>();
            builder.Services.AddSingleton<SanPhamRepository>();
            builder.Services.AddTransient<SanPhamPageModel>();
            builder.Services.AddTransient<SanPhamPage>();
#endif

            return builder.Build();
        }
    }
}
