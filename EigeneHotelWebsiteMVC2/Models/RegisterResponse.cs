using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace EigeneHotelWebsiteMVC2.Models
{
    public class RegisterResponse
    {
        [Required(ErrorMessage = "Bitte geben Sie einen Benutzernamen ein!")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie einen Benutzernamen ein!")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Email-Adresse ein!")]
        [RegularExpression(".+\\@.+\\..+", ErrorMessage = "Bitte geben Sie eine Gültige Email-Adresse ein!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie ein Passwort ein!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$"
            , ErrorMessage = "Ihr Passwort muss mind. 8 Zeichen lang sein, mind." +
            " eine Zahl, mind. einen Großbuchstaben, mind. einen Kleinbuchstaben und ein Sonderzeichen enthalten")]
        public string Password { get; set; } 

        [Required(ErrorMessage = "Bitte geben Sie ein Passwort ein!")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#$^+=!*()@%&]).{8,}$"
            , ErrorMessage = "Ihr Passwort muss mind. 8 Zeichen lang sein, mind." +
            " eine Zahl, mind. einen Großbuchstaben, mind. einen Kleinbuchstaben und ein Sonderzeichen enthalten")]
        public string PasswordRepeat { get; set; }

        [Required(ErrorMessage = "Bitte geben Sie eine Telefonnummer ein!")]
        public string PhoneNumber { get; set; }


    }
}
