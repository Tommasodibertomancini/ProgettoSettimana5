using System.ComponentModel.DataAnnotations;

namespace ProgettoSettimana5.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Ricordami")]
        public bool RememberMe { get; set; }
    }

    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "La password è obbligatoria")]
        [StringLength(100, ErrorMessage = "La password deve avere almeno {2} caratteri", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma password")]
        [Compare("Password", ErrorMessage = "Le password non corrispondono")]
        public string ConfirmPassword { get; set; }

        [Required(ErrorMessage = "Il ruolo è obbligatorio")]
        [Display(Name = "Ruolo utente")]
        public string Role { get; set; }
    }

    public class ChangePasswordViewModel
    {
        [Required(ErrorMessage = "La password attuale è obbligatoria")]
        [DataType(DataType.Password)]
        [Display(Name = "Password attuale")]
        public string CurrentPassword { get; set; }

        [Required(ErrorMessage = "La nuova password è obbligatoria")]
        [StringLength(100, ErrorMessage = "La password deve avere almeno {2} caratteri", MinimumLength = 8)]
        [DataType(DataType.Password)]
        [Display(Name = "Nuova password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Conferma nuova password")]
        [Compare("NewPassword", ErrorMessage = "Le password non corrispondono")]
        public string ConfirmPassword { get; set; }
    }

    public class UserViewModel
    {
        public string Id { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
    }
}
