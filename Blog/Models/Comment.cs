using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace Blog.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        
        public string UserName { get; set; }
        public string UserProfileImage { get; set; }

        [DataType(DataType.Date)]
        
        public DateTime CommentDate { get; set; } = DateTime.Now;
        [Required]
        public string Content { get; set; }
        [ForeignKey("Post")]
        public int PostId { get; set; }
        [ValidateNever]
        public Post Post { get; set; }
    }
}
