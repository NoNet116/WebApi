
namespace DAL.Entities
{
    public  class ArticleTags
    {
        public int ArticleId { get; set; }
        public Article Article { get; set; }

        public Guid TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
