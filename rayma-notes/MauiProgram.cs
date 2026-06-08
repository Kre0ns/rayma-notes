using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using rayma_notes.ViewModels;
using rayma_notes.Views;

namespace rayma_notes
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
                })
                .AddAudio();

            builder.Services.AddTransient<RecordView>();
            builder.Services.AddTransient<RecordViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
