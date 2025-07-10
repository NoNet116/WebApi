using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebApi.ViewModels.Articles
{
    public class CreateArticleViewModel
    {
        [Required, MaxLength(50)]
        public required string Title { get; set; }
        [Required, MaxLength(255)]
        public required string Content { get; set; }

        [MaxLength(10)]
        public IEnumerable<string>? Tags { get; set; }
    }
}
