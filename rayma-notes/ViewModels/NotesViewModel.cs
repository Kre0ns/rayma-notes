using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using rayma_notes.Models;
using rayma_notes.Services.Interfaces;
using rayma_notes.Views;
using System.Collections.ObjectModel;


namespace rayma_notes.ViewModels
{
    public partial class NotesViewModel : ObservableObject
    {
        private readonly IDatabaseService _databaseService;

        public ObservableCollection<Note> Notes { get; } = new();

        public NotesViewModel(IDatabaseService databaseService)
        {
            _databaseService = databaseService;
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
    }
}
