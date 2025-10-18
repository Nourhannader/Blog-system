using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Blog.Models
{
    public class Post
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="The Title is Required")]
        [MaxLength(200, ErrorMessage = "The Title cannot exceed 200 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The Content is Required")]
        public string Content { get; set; }
        [Required(ErrorMessage = "The Author is Required")]
        [MaxLength(100, ErrorMessage = "The Author cannot exceed 100 characters")]
        public string Author { get; set; }
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        public string FeatureImageUrl { get; set; }

        // Navigation property for Category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }
        // Navigation property for Comments
        public ICollection<Comment> Comments { get; set; }


    }
}
