using System.ComponentModel.DataAnnotations;


namespace EigeneHotelWebsiteMVC2.Models
{
    public class LoginResponse
    {

        [Required(ErrorMessage="Bitte geben Sie einen Benutzernamen ein!")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie ein Passwort ein!")]
        public string Password { get; set; }
    }
}
