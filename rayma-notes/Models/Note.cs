using SQLite;

namespace rayma_notes.Models
{
    [Table("notes")]
    public class Note
    {
        [PrimaryKey, AutoIncrement]
        [Column("id")]
        public int Id { get; set; } = -1;

        [Column("title")]
        [NotNull]
        public string Title { get; set; } = string.Empty;

        [Column("body")]
        [NotNull]
        public string Body { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
