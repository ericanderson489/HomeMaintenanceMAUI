using Microsoft.Extensions.Logging;
// This is the main entry point for the .NET MAUI application.
namespace HomeMaintenanceMAUI
{   // The MauiProgram class is responsible for configuring and creating the MAUI application.
    public static class MauiProgram
    {   // The CreateMauiApp method sets up the application, including fonts and logging.
        public static MauiApp CreateMauiApp()
        {   // Create a new MAUI application builder.
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {   // Add custom fonts to the application.
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
