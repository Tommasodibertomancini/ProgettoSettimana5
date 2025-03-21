using Microsoft.AspNetCore.Identity;

namespace ProgettoSettimana5.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string NomeCompleto => $"{Nome} {Cognome}";
    }
}
