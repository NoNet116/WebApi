namespace DAL.Entities
{
    public class Article
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string Content { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public string AuthorId { get; set; } = default!;
        public User Author { get; set; } = default!; // Навигационное свойство
        public Tag? Tag { get; set; } = default!; // Навигационное свойство

        public ICollection<Tag> Tags = new List<Tag>();
        public ICollection<Comment> Comments = new List<Comment>();
    }
}
