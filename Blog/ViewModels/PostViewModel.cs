using Blog.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Blog.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Blog.ViewModels
{
    public class PostViewModel
    {
        [ValidateNever]
        public int Id { get; set; }
        [Required(ErrorMessage = "The Title is Required")]
        [MaxLength(200, ErrorMessage = "The Title cannot exceed 200 characters")]
        public string Title { get; set; }
        [Required(ErrorMessage = "The Content is Required")]
        public string Content { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
        public IFormFile? FeatureImage { get; set; }
    }
}
