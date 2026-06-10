using SQLite;

namespace rayma_notes.Models
{
    [Table("notes")]
    public class Note
    {
        [PrimaryKey,  AutoIncrement]
        [Column("id")]
        public int Id { get; set; }

        [Column("text")]
        [NotNull]
        public string Text { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
