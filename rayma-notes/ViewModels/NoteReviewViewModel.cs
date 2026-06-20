using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Models;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;

namespace rayma_notes.ViewModels
{
    public partial class NoteReviewViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasBody))]
        public partial Note Note { get; set; } = new();

        public bool IsEdit { get; set; } = false; 

        public bool HasBody => !string.IsNullOrEmpty(Note.Body);

        public NoteReviewViewModel(IDatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
        }


        [RelayCommand]
        private async Task SaveAsync()
        {
            if (string.IsNullOrEmpty(Note.Title)) Note.Title = "Untitled Note";

            await _databaseService.SaveNoteAsync(Note);
            await _navigationService.PopModalAsync();
        }

        [RelayCommand]
        private async Task DiscardAsync()
        {
            string alertTitle = IsEdit ? "Discard changes?" : "Discard note?";
            string alertBody = IsEdit ? "The changes you made will be gone forever." : "This note will be gone forever.";

            bool IsConfirmed = await DialogService.ShowConfirmationAsync(alertTitle, alertBody, "Discard", "Cancel");

            if (!IsConfirmed) return;

            await _navigationService.PopModalAsync();
        }
    }
}
