using System.ComponentModel.DataAnnotations;

namespace ProgettoSettimana5.Models
{
    public class Camera
    {
        public int CameraId { get; set; }

        [Required(ErrorMessage = "Il numero della camera è obbligatorio")]
        [Range(1, 1000, ErrorMessage = "Il numero della camera deve essere compreso tra 1 e 1000")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "Il tipo di camera è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il tipo non può superare i 50 caratteri")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Il prezzo è obbligatorio")]
        [Range(0.01, 10000, ErrorMessage = "Il prezzo deve essere maggiore di zero")]
        public decimal Prezzo { get; set; }

        public virtual ICollection<Prenotazione> Prenotazioni { get; set; }

        public string DescrizioneCompleta => $"Camera {Numero} - {Tipo} (€{Prezzo}/notte)";
    }
}
