using System.ComponentModel.DataAnnotations;

namespace WebAdvert.Web.Models
{
    public class SignUpModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(8, ErrorMessage = "Password must be 8 characters long")]
        [Display(Name = "Password")]
        public string Password { get; set; }


        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage ="Confirm Password does not match.")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassowrd { get; set; }
    }
}
