using rayma_notes.Models;

namespace rayma_notes.Services.Interfaces
{
    public interface IDatabaseService
    {
        Task<List<Note>> GetNotesAsync();

        Task<Note?> GetNoteAsync(int id);

        Task<int> SaveNoteAsync(Note note);

        Task<int> DeleteNoteAsync(Note note);
    }
}
