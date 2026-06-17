using rayma_notes.Models;
using rayma_notes.Services.Interfaces;
using SQLite;

namespace rayma_notes.Services
{
    public class SqliteDatabaseService : IDatabaseService
    {
        private const string DatabaseName = "RaymaNotes.sqlite";

        private SQLiteAsyncConnection? _database;

        private async Task InitAsync()
        {
            if (_database is not null)
            {
                return;
            }

            string dbPath = Path.Combine(FileSystem.AppDataDirectory, DatabaseName);
            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<Note>();
        }

        public async Task<int> DeleteNoteAsync(Note note)
        {
            await InitAsync();

            string sql = "DELETE FROM notes WHERE id = ?";

            return await _database!.ExecuteAsync(sql, note.Id);
        }

        public async Task<Note?> GetNoteAsync(int id)
        {
            await InitAsync();

            string sql = "SELECT * FROM notes WHERE id = ?";

            List<Note> notes = await _database!.QueryAsync<Note>(sql, id);
            return notes.FirstOrDefault();
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            await InitAsync();

            string sql = "SELECT * FROM notes ORDER BY created_at DESC";

            return await _database!.QueryAsync<Note>(sql);
        }

        public async Task<int> SaveNoteAsync(Note note)
        {
            await InitAsync();

            if (note.Id == -1)
            {
                string sql = "INSERT INTO notes (title, body, created_at) VALUES (?, ?, ?)";

                return await _database!.ExecuteAsync(sql, note.Title, note.Body, note.CreatedAt);
            }
            else
            {
                string sql = "UPDATE notes SET title = ?, body = ?, created_at = ? WHERE id = ?";

                return await _database!.ExecuteAsync(sql, note.Title, note.Body, note.CreatedAt, note.Id);
            }
        }
    }
}
