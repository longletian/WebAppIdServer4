using System.ComponentModel.DataAnnotations;

namespace WebAppIdServer4.Pages.Account.Regirst
{
    public class RegirstInputModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string AginPassword { get; set; }
    }
}
