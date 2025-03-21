using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgettoSettimana5.Models;
using System.ComponentModel.DataAnnotations;

namespace ProgettoSettimana5.ViewModels
{
    public class PrenotazioneListViewModel
    {
        public List<Prenotazione> Prenotazioni { get; set; }
        public string StatoFiltro { get; set; }
        public DateTime? DataInizioFiltro { get; set; }
        public DateTime? DataFineFiltro { get; set; }
    }

    public class PrenotazioneCreateViewModel
    {
        [Required(ErrorMessage = "Il cliente è obbligatorio")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "La camera è obbligatoria")]
        [Display(Name = "Camera")]
        public int CameraId { get; set; }

        [Required(ErrorMessage = "La data di inizio è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di inizio")]
        public DateTime DataInizio { get; set; }

        [Required(ErrorMessage = "La data di fine è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di fine")]
        public DateTime DataFine { get; set; }

        [Display(Name = "Stato")]
        public StatoPrenotazione? Stato { get; set; }
        [ValidateNever]
        public SelectList Clienti { get; set; }
        [ValidateNever]
        public SelectList Camere { get; set; }

        public decimal PrezzoCamera { get; set; }
        public int GiorniSoggiorno => (DataFine - DataInizio).Days;
        public decimal CostoTotale => PrezzoCamera * GiorniSoggiorno;
    }

    public class PrenotazioneDetailViewModel
    {
        public Prenotazione Prenotazione { get; set; }
        public decimal CostoTotale => Prenotazione.Camera.Prezzo * (Prenotazione.DataFine - Prenotazione.DataInizio).Days;
    }

    public class PrenotazioneEditViewModel
    {
        public int PrenotazioneId { get; set; }

        [Required(ErrorMessage = "Il cliente è obbligatorio")]
        [Display(Name = "Cliente")]
        public int ClienteId { get; set; }

        [Required(ErrorMessage = "La camera è obbligatoria")]
        [Display(Name = "Camera")]
        public int CameraId { get; set; }

        [Required(ErrorMessage = "La data di inizio è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di inizio")]
        public DateTime DataInizio { get; set; }

        [Required(ErrorMessage = "La data di fine è obbligatoria")]
        [DataType(DataType.Date)]
        [Display(Name = "Data di fine")]
        public DateTime DataFine { get; set; }

        [Display(Name = "Stato")]
        public StatoPrenotazione Stato { get; set; }
        [ValidateNever]
        public SelectList Clienti { get; set; }
        [ValidateNever]
        public SelectList Camere { get; set; }

        public decimal PrezzoCamera { get; set; }
        public int GiorniSoggiorno => (DataFine - DataInizio).Days;
        public decimal CostoTotale => PrezzoCamera * GiorniSoggiorno;
    }
}
