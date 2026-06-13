using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Models;
using rayma_notes.Services;
using rayma_notes.Services.Interfaces;
using System.Collections.ObjectModel;


namespace rayma_notes.ViewModels
{
    public partial class NotesViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;
        private readonly NavigationService _navigationService;

        public ObservableCollection<Note> Notes { get; } = new();

        public NotesViewModel(IDatabaseService databaseService, NavigationService navigationService)
        {
            _databaseService = databaseService;
            _navigationService = navigationService;
        }

        public async Task LoadNotesAsync()
        {
            List<Note> notes = await _databaseService.GetNotesAsync();

            Notes.Clear();
            foreach (Note note in notes)
            {
                Notes.Add(note);
            }
        }

        [RelayCommand]
        public async Task SelectNoteAsync(Note note)
        {
            await _navigationService.PushViewNotePageAsync(note);
        }
    }
}
