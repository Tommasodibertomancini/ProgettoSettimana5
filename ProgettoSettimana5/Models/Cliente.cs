using System.ComponentModel.DataAnnotations;

namespace ProgettoSettimana5.Models
{
    public class Cliente
    {
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "Il nome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il nome non può superare i 50 caratteri")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Il cognome è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il cognome non può superare i 50 caratteri")]
        public string Cognome { get; set; }

        [Required(ErrorMessage = "L'email è obbligatoria")]
        [EmailAddress(ErrorMessage = "Formato email non valido")]
        [StringLength(100, ErrorMessage = "L'email non può superare i 100 caratteri")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Il telefono è obbligatorio")]
        [Phone(ErrorMessage = "Formato telefono non valido")]
        [StringLength(20, ErrorMessage = "Il telefono non può superare i 20 caratteri")]
        public string Telefono { get; set; }

        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }

        public string NomeCompleto => $"{Nome} {Cognome}";
    }
}
