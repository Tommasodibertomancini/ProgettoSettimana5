using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ProgettoSettimana5.Models;
using ProgettoSettimana5.Services;
using ProgettoSettimana5.ViewModels;

namespace ProgettoSettimana5.Controllers
{
    [Authorize]
    public class PrenotazioniController : Controller
    {
        private readonly PrenotazioneService _prenotazioneService;
        private readonly ClienteService _clienteService;
        private readonly CameraService _cameraService;

        public PrenotazioniController(
            PrenotazioneService prenotazioneService,
            ClienteService clienteService,
            CameraService cameraService)
        {
            _prenotazioneService = prenotazioneService;
            _clienteService = clienteService;
            _cameraService = cameraService;
        }

        // GET: Prenotazioni
        public async Task<IActionResult> Index(string statoFiltro, DateTime? dataInizioFiltro, DateTime? dataFineFiltro)
        {
            var prenotazioni = await _prenotazioneService.GetAllPrenotazioniAsync();

            // Applica filtri se presenti
            if (!string.IsNullOrEmpty(statoFiltro) && Enum.TryParse<StatoPrenotazione>(statoFiltro, out var stato))
            {
                prenotazioni = prenotazioni.FindAll(p => p.Stato == stato);
            }

            if (dataInizioFiltro.HasValue)
            {
                prenotazioni = prenotazioni.FindAll(p => p.DataInizio >= dataInizioFiltro.Value);
            }

            if (dataFineFiltro.HasValue)
            {
                prenotazioni = prenotazioni.FindAll(p => p.DataFine <= dataFineFiltro.Value);
            }

            var model = new PrenotazioneListViewModel
            {
                Prenotazioni = prenotazioni,
                StatoFiltro = statoFiltro,
                DataInizioFiltro = dataInizioFiltro,
                DataFineFiltro = dataFineFiltro
            };

            return View(model);
        }

        // GET: Prenotazioni/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var prenotazione = await _prenotazioneService.GetPrenotazioneByIdAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }

            var model = new PrenotazioneDetailViewModel
            {
                Prenotazione = prenotazione
            };

            return View(model);
        }

        // GET: Prenotazioni/Create
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Create(int? clienteId, int? cameraId, DateTime? dataInizio, DateTime? dataFine)
        {
            var clienti = await _clienteService.GetAllClientiAsync();
            var camere = await _cameraService.GetAllCamereAsync();

            var model = new PrenotazioneCreateViewModel
            {
                ClienteId = clienteId ?? 0,
                CameraId = cameraId ?? 0,
                DataInizio = dataInizio ?? DateTime.Today,
                DataFine = dataFine ?? DateTime.Today.AddDays(1),
                Stato = StatoPrenotazione.InAttesa,
                Clienti = new SelectList(clienti, "ClienteId", "NomeCompleto", clienteId),
                Camere = new SelectList(camere, "CameraId", "DescrizioneCompleta", cameraId),
                PrezzoCamera = cameraId.HasValue ?
                    camere.Find(c => c.CameraId == cameraId.Value)?.Prezzo ?? 0 : 0
            };

            return View(model);
        }

        // POST: Prenotazioni/Create
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Create(PrenotazioneCreateViewModel model)
        {
            var clienti = await _clienteService.GetAllClientiAsync();
            var camere = await _cameraService.GetAllCamereAsync();
            model.Clienti = new SelectList(clienti, "ClienteId", "NomeCompleto", model.ClienteId);
            model.Camere = new SelectList(camere, "CameraId", "DescrizioneCompleta", model.CameraId);

            var camera = camere.Find(c => c.CameraId == model.CameraId);
            model.PrezzoCamera = camera?.Prezzo ?? 0;

            if (ModelState.IsValid)
            {
                var disponibile = await _prenotazioneService.IsCameraDisponibileAsync(
                    model.CameraId, model.DataInizio, model.DataFine);

                if (!disponibile)
                {
                    ModelState.AddModelError(string.Empty, "La camera non è disponibile nelle date selezionate");
                    return View(model);
                }

                StatoPrenotazione statoPrenotazione = StatoPrenotazione.Confermata;

                var prenotazione = new Prenotazione
                {
                    ClienteId = model.ClienteId,
                    CameraId = model.CameraId,
                    DataInizio = model.DataInizio,
                    DataFine = model.DataFine,
                    Stato = statoPrenotazione
                };

               
                var success = await _prenotazioneService.CreatePrenotazioneAsync(prenotazione);

                if (success)
                {
                 
                    TempData["SuccessMessage"] = "Prenotazione creata con successo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "La camera non è disponibile nelle date selezionate");
                }
            }

            return View(model);
        }

        // GET: Prenotazioni/Edit/5
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Edit(int id)
        {
            var prenotazione = await _prenotazioneService.GetPrenotazioneByIdAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }

            var clienti = await _clienteService.GetAllClientiAsync();
            var camere = await _cameraService.GetAllCamereAsync();

            var model = new PrenotazioneEditViewModel
            {
                PrenotazioneId = prenotazione.PrenotazioneId,
                ClienteId = prenotazione.ClienteId,
                CameraId = prenotazione.CameraId,
                DataInizio = prenotazione.DataInizio,
                DataFine = prenotazione.DataFine,
                Stato = prenotazione.Stato,
                Clienti = new SelectList(clienti, "ClienteId", "NomeCompleto", prenotazione.ClienteId),
                Camere = new SelectList(camere, "CameraId", "DescrizioneCompleta", prenotazione.CameraId),
                PrezzoCamera = prenotazione.Camera.Prezzo
            };

            return View(model);
        }

        // POST: Prenotazioni/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Edit(int id, PrenotazioneEditViewModel model)
        {
            if (id != model.PrenotazioneId)
            {
                return NotFound();
            }

            var clienti = await _clienteService.GetAllClientiAsync();
            var camere = await _cameraService.GetAllCamereAsync();
            model.Clienti = new SelectList(clienti, "ClienteId", "NomeCompleto", model.ClienteId);
            model.Camere = new SelectList(camere, "CameraId", "DescrizioneCompleta", model.CameraId);

            // Aggiorna il prezzo della camera per il calcolo del costo totale
            var camera = camere.Find(c => c.CameraId == model.CameraId);
            model.PrezzoCamera = camera?.Prezzo ?? 0;

            if (ModelState.IsValid)
            {
                if (model.DataInizio >= model.DataFine)
                {
                    ModelState.AddModelError("DataFine", "La data di fine deve essere successiva alla data di inizio");
                    return View(model);
                }

                var prenotazione = new Prenotazione
                {
                    PrenotazioneId = model.PrenotazioneId,
                    ClienteId = model.ClienteId,
                    CameraId = model.CameraId,
                    DataInizio = model.DataInizio,
                    DataFine = model.DataFine,
                    Stato = model.Stato
                };

                var success = await _prenotazioneService.UpdatePrenotazioneAsync(prenotazione);
                if (success)
                {
                    TempData["SuccessMessage"] = "Prenotazione aggiornata con successo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "La camera non è disponibile nelle date selezionate");
                }
            }

            return View(model);
        }

        // GET: Prenotazioni/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var prenotazione = await _prenotazioneService.GetPrenotazioneByIdAsync(id);
            if (prenotazione == null)
            {
                return NotFound();
            }

            return View(prenotazione);
        }

        // POST: Prenotazioni/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _prenotazioneService.DeletePrenotazioneAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Prenotazione eliminata con successo";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Impossibile eliminare la prenotazione";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Prenotazioni/Cancella/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Cancella(int id)
        {
            var success = await _prenotazioneService.CancellaPrenotazioneAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Prenotazione cancellata con successo";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Impossibile cancellare la prenotazione";
                return RedirectToAction(nameof(Index));
            }
        }

        // AJAX: Prenotazioni/GetCameraInfo/5
        [HttpGet]
        public async Task<IActionResult> GetCameraInfo(int id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
            {
                return NotFound();
            }

            return Json(new { prezzo = camera.Prezzo, tipo = camera.Tipo });
        }

        // AJAX: Prenotazioni/VerificaDisponibilita
        [HttpGet]
        public async Task<IActionResult> VerificaDisponibilita(int cameraId, DateTime dataInizio, DateTime dataFine, int? prenotazioneId = null)
        {
            if (dataInizio >= dataFine)
            {
                return Json(new { disponibile = false, messaggio = "La data di fine deve essere successiva alla data di inizio" });
            }

            var disponibile = await _prenotazioneService.IsCameraDisponibileAsync(cameraId, dataInizio, dataFine, prenotazioneId);
            return Json(new { disponibile });
        }

        // AJAX: Prenotazioni/PrenotazioniPartial
        [HttpGet]
        public async Task<IActionResult> PrenotazioniPartial(string stato = null)
        {
            var prenotazioni = await _prenotazioneService.GetAllPrenotazioniAsync();

            if (!string.IsNullOrEmpty(stato) && Enum.TryParse<StatoPrenotazione>(stato, out var statoEnum))
            {
                prenotazioni = prenotazioni.FindAll(p => p.Stato == statoEnum);
            }

            return PartialView("_PrenotazioniPartial", prenotazioni);
        }
    }
}
