using Microsoft.Extensions.Logging;
using Plugin.Maui.Audio;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;
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

            builder.Services.AddSingleton<IDatabaseService, SqliteDatabaseService>();
            builder.Services.AddSingleton<IAiService, GroqAiService>();
            builder.Services.AddSingleton<NavigationService>();

            builder.Services.AddTransient<MainTabbedPage>();

            builder.Services.AddTransient<RecordView>();
            builder.Services.AddTransient<RecordViewModel>();

            builder.Services.AddTransient<NoteReviewView>();
            builder.Services.AddTransient<NoteReviewViewModel>();

            builder.Services.AddTransient<NotesView>();
            builder.Services.AddTransient<NotesViewModel>();

            builder.Services.AddTransient<ViewNoteView>();
            builder.Services.AddTransient<ViewNoteViewModel>();

            builder.Services.AddTransient<SettingsView>();
            builder.Services.AddTransient<SettingsViewModel>();

#if DEBUG
            builder.Logging.AddDebug();
#endif

            Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif
            });

            Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (handler, view) =>
            {
#if ANDROID
                handler.PlatformView.Background = null;
#endif
            });

            return builder.Build();
        }
    }
}
