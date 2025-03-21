using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ProgettoSettimana5.Models
{
        public enum StatoPrenotazione
        {
            Confermata,
            Cancellata,
            InAttesa,
            Completata
        }

        public class Prenotazione
        {
            public int PrenotazioneId { get; set; }

            [Required]
            public int ClienteId { get; set; }

            [Required]
            public int CameraId { get; set; }

            [Required(ErrorMessage = "La data di inizio è obbligatoria")]
            [DataType(DataType.Date)]
            [Display(Name = "Data Inizio")]
            public DateTime DataInizio { get; set; }

            [Required(ErrorMessage = "La data di fine è obbligatoria")]
            [DataType(DataType.Date)]
            [Display(Name = "Data Fine")]
            public DateTime DataFine { get; set; }

            [Required]
            [EnumDataType(typeof(StatoPrenotazione))]
            public StatoPrenotazione Stato { get; set; }
           
            [ForeignKey("ClienteId")]
            public virtual Cliente Cliente { get; set; }

            [ForeignKey("CameraId")]
            public virtual Camera Camera { get; set; }

            [NotMapped]
            public int GiorniSoggiorno => (DataFine - DataInizio).Days;

            [NotMapped]
            public decimal CostoTotale => Camera != null ? Camera.Prezzo * GiorniSoggiorno : 0;
        }
    
}
