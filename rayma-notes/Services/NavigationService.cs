using rayma_notes.Models;
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

        private static Page? GetCurrentPage()
        {
            return Application.Current?.Windows.FirstOrDefault()?.Page;
        }

        public async Task PushReviewPageAsync(Note note)
        {
            NoteReviewView noteReviewView = _serviceProvider.GetRequiredService<NoteReviewView>();

            NoteReviewViewModel noteReviewViewModel = (NoteReviewViewModel)noteReviewView.BindingContext;
            noteReviewViewModel.Note = note;
            noteReviewViewModel.IsEdit = note.Id != -1;

            Page? parentPage = GetCurrentPage();
            if (parentPage is not null)
            {
                await parentPage.Navigation.PushModalAsync(noteReviewView);
            }
        }

        public async Task PushViewNotePageAsync(Note note)
        {
            ViewNoteView viewNoteView = _serviceProvider.GetRequiredService<ViewNoteView>();

            ViewNoteViewModel viewNoteViewModel = (ViewNoteViewModel)viewNoteView.BindingContext;
            viewNoteViewModel.Note = note;

            Page? parentPage = GetCurrentPage();
            if (parentPage is not null)
            {
                await parentPage.Navigation.PushAsync(viewNoteView);
            }
        }

        public async Task PopModalAsync()
        {
            Page? parentPage = GetCurrentPage();
            if (parentPage is not null)
            {
                await parentPage.Navigation.PopModalAsync();

            }
        }

        public async Task PopAsync()
        {
            Page? parentPage = GetCurrentPage();
            if (parentPage is not null)
            {
                await parentPage.Navigation.PopAsync();
            }
        }
    }
}
