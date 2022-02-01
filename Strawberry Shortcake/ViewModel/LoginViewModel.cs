using System.ComponentModel.DataAnnotations;

namespace Strawberry_Shortcake.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage ="Enter your email.")]
        public string UserEmail { get; set; }
        
        [Required(ErrorMessage = "Enter your password.")]
        [DataType(DataType.Password)]
        public string UserPw { get; set; }

        public string ReturnUrl { get; set; }
    }
}
