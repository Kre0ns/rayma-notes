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
        public partial string TitleText { get; set; } = string.Empty;

        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(HasBody))]
        public partial string BodyText { get; set; } = string.Empty;

        public bool HasBody => !string.IsNullOrEmpty(BodyText);

        public NoteReviewViewModel(IDatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
        }


        [RelayCommand]
        private async Task SaveAsync()
        {
            Note note = new()
            {
                Title = string.IsNullOrWhiteSpace(TitleText) ? "Untitled Note" : TitleText,
                Body = BodyText
            };

            await _databaseService.SaveNoteAsync(note);
            await _navigationService.PopModalAsync();
        }

        [RelayCommand]
        private async Task DiscardAsync()
        {
            await _navigationService.PopModalAsync();
        }
    }
}
