using System.ComponentModel.DataAnnotations;

namespace FoodApp.Services.Identity.API.Models.AccountViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
