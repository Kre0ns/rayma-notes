using rayma_notes.ViewModels;
using rayma_notes.Views;

namespace rayma_notes.Services
{
    public class NavigationService
    {
        private readonly IServiceProvider _serviceProvider;

        public NavigationService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task PushReviewPageAsync(string cleanText)
        {
            NoteReviewView noteReviewView = _serviceProvider.GetRequiredService<NoteReviewView>();

            NoteReviewViewModel noteReviewViewModel = (NoteReviewViewModel)noteReviewView.BindingContext;
            noteReviewViewModel.BodyText = cleanText;
            noteReviewViewModel.TitleText = string.Empty;

            Page parentPage = Application.Current!.Windows[0]!.Page!;
            await parentPage.Navigation.PushModalAsync(noteReviewView);
        }

        public async Task PopModalAsync()
        {
            Page parentPage = Application.Current!.Windows[0]!.Page!;

            await parentPage.Navigation.PopModalAsync();
        }
    }
}
