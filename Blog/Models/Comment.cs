using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Blog.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required.")]
        [MaxLength(100, ErrorMessage = "User name cannot exceed 100 characters.")]
        public string UserName { get; set; }

        [DataType(DataType.Date)]
        
        public DateTime CommentDate { get; set; } = DateTime.Now;
        [Required]
        public string Content { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        
        public Post Post { get; set; }
    }
}
