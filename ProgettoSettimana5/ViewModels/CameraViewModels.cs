using ProgettoSettimana5.Models;
using System.ComponentModel.DataAnnotations;

namespace ProgettoSettimana5.ViewModels
{
    public class CameraListViewModel
    {
        public List<Camera> Camere { get; set; }
        public string TipoFiltro { get; set; }
        public decimal? PrezzoMinimo { get; set; }
        public decimal? PrezzoMassimo { get; set; }
    }

    public class CameraCreateViewModel
    {
        [Required(ErrorMessage = "Il numero della camera è obbligatorio")]
        [Range(1, 1000, ErrorMessage = "Il numero della camera deve essere compreso tra 1 e 1000")]
        public int Numero { get; set; }

        [Required(ErrorMessage = "Il tipo di camera è obbligatorio")]
        [StringLength(50, ErrorMessage = "Il tipo non può superare i 50 caratteri")]
        public string Tipo { get; set; }

        [Required(ErrorMessage = "Il prezzo è obbligatorio")]
        [Range(0.01, 10000, ErrorMessage = "Il prezzo deve essere maggiore di zero")]
        public decimal Prezzo { get; set; }
    }

    public class CameraDetailViewModel
    {
        public Camera Camera { get; set; }
        public List<Prenotazione> PrenotazioniAttive { get; set; }
        public List<Prenotazione> PrenotazioniFuture { get; set; }
    }

    public class CameraEditViewModel
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
    }

    public class CamereDisponibiliViewModel
    {
        [Required(ErrorMessage = "La data di inizio è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di inizio")]
        public DateTime DataInizio { get; set; }

        [Required(ErrorMessage = "La data di fine è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di fine")]
        public DateTime DataFine { get; set; }

        public List<Camera> CamereDisponibili { get; set; }
    }
}
