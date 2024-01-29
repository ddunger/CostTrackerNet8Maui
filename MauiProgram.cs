using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using Miljokaz.Data;
using Miljokaz.Views;
using Microcharts.Maui;
using Miljokaz.ViewModels;

namespace Miljokaz
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMicrocharts()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<AllItems>();
            builder.Services.AddTransient<EditItem>();
            builder.Services.AddTransient<NewItem>();

            builder.Services.AddSingleton<MainPageViewModel>();


            string dbPath = Path.Combine(FileSystem.AppDataDirectory, "sqldata.db");

            builder.Services.AddSingleton(s => ActivatorUtilities.CreateInstance<DataRepository>(s, dbPath));


#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
