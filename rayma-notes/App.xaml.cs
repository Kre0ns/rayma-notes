using Microsoft.Extensions.DependencyInjection;

namespace rayma_notes
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        protected override Window CreateWindow(IActivationState? activationState)
        {
            var mainPage = activationState!.Context.Services.GetRequiredService<MainTabbedPage>();
            return new Window(new NavigationPage(mainPage));
        }
    }
}