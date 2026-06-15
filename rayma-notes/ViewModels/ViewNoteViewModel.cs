using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Models;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;

namespace rayma_notes.ViewModels
{
    public partial class ViewNoteViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        [ObservableProperty]
        public partial Note Note { get; set; } = new();

        public ViewNoteViewModel(IDatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
        }


        [RelayCommand]
        public async Task EditAsync()
        {
            await _navigationService.PushReviewPageAsync(Note);
        }

        [RelayCommand]
        public async Task DeleteAsync()
        {
            bool IsConfirmed = await Application.Current!.Windows[0].Page!.DisplayAlertAsync("Delete note?", "This note will be gone forever!", "Delete", "Cancel");

            if (!IsConfirmed) return;

            await _databaseService.DeleteNoteAsync(Note);

            await _navigationService.PopAsync();
        }

        public async Task ReloadNoteAsync()
        {
            if (Note.Id != -1)
            {
                Note? updatedNote = await _databaseService.GetNoteAsync(Note.Id);

                if (updatedNote is not null)
                {
                    Note = updatedNote;
                }
            }
        }
    }
}
