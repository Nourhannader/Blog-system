using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Blog.Data;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

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
        
        [DataType(DataType.Date)]
        public DateTime PublishedDate { get; set; } = DateTime.Now;
        [ValidateNever]
        public string FeatureImageUrl { get; set; }

        //Navigation property for user
        [ValidateNever]
        [ForeignKey("User")]
        public string userId { get; set; }
        [ValidateNever]
        public AppUser User { get; set; }
        // Navigation property for Category
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ValidateNever]
        public Category Category { get; set; }
        [ValidateNever]
        // Navigation property for Comments
        public ICollection<Comment> Comments { get; set; }


    }
}
