using System;
using System.Threading.Tasks;
using ProgettoSettimana5.Models;
using ProgettoSettimana5.Services;
using ProgettoSettimana5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace ProgettoSettimana5.Controllers
{
    [Authorize]
    public class CamereController : Controller
    {
        private readonly CameraService _cameraService;

        public CamereController(CameraService cameraService)
        {
            _cameraService = cameraService;
        }

        // GET: Camere
        public async Task<IActionResult> Index(string tipoFiltro, decimal? prezzoMinimo, decimal? prezzoMassimo)
        {
            var model = new CameraListViewModel
            {
                Camere = await _cameraService.GetAllCamereAsync(),
                TipoFiltro = tipoFiltro,
                PrezzoMinimo = prezzoMinimo,
                PrezzoMassimo = prezzoMassimo
            };

            // Applica filtri se presenti
            if (!string.IsNullOrEmpty(tipoFiltro))
            {
                model.Camere = model.Camere.FindAll(c => c.Tipo.Contains(tipoFiltro, StringComparison.OrdinalIgnoreCase));
            }

            if (prezzoMinimo.HasValue)
            {
                model.Camere = model.Camere.FindAll(c => c.Prezzo >= prezzoMinimo.Value);
            }

            if (prezzoMassimo.HasValue)
            {
                model.Camere = model.Camere.FindAll(c => c.Prezzo <= prezzoMassimo.Value);
            }

            return View(model);
        }

        // GET: Camere/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
            {
                return NotFound();
            }

            var model = new CameraDetailViewModel
            {
                Camera = camera,
                PrenotazioniAttive = camera.Prenotazioni?
                .Where(p => p.Stato == StatoPrenotazione.Confermata && p.DataFine >= DateTime.Today)
                .ToList() ?? new List<Prenotazione>(),

                PrenotazioniFuture = camera.Prenotazioni?
                .Where(p => (p.Stato == StatoPrenotazione.Confermata || p.Stato == StatoPrenotazione.InAttesa) &&
                p.DataInizio > DateTime.Today)
                .ToList() ?? new List<Prenotazione>(),
            };

            return View(model);
        }

        // GET: Camere/Create
        [Authorize(Roles = "Admin,Manager")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Camere/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create(CameraCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var camera = new Camera
                {
                    Numero = model.Numero,
                    Tipo = model.Tipo,
                    Prezzo = model.Prezzo
                };

                var success = await _cameraService.CreateCameraAsync(camera);
                if (success)
                {
                    TempData["SuccessMessage"] = "Camera creata con successo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Numero", "Esiste già una camera con questo numero");
                }
            }
            return View(model);
        }

        // GET: Camere/Edit/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
            {
                return NotFound();
            }

            var model = new CameraEditViewModel
            {
                CameraId = camera.CameraId,
                Numero = camera.Numero,
                Tipo = camera.Tipo,
                Prezzo = camera.Prezzo
            };

            return View(model);
        }

        // POST: Camere/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, CameraEditViewModel model)
        {
            if (id != model.CameraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var camera = new Camera
                {
                    CameraId = model.CameraId,
                    Numero = model.Numero,
                    Tipo = model.Tipo,
                    Prezzo = model.Prezzo
                };

                var success = await _cameraService.UpdateCameraAsync(camera);
                if (success)
                {
                    TempData["SuccessMessage"] = "Camera aggiornata con successo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Numero", "Esiste già un'altra camera con questo numero");
                }
            }
            return View(model);
        }

        // GET: Camere/Delete/5
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
            {
                return NotFound();
            }

            return View(camera);
        }

        // POST: Camere/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _cameraService.DeleteCameraAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Camera eliminata con successo";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Impossibile eliminare la camera. " +
                                          "Assicurarsi che non abbia prenotazioni attive o future.";
                return RedirectToAction(nameof(Index));
            }
        }

        // GET: Camere/CamereDisponibili
        public IActionResult CamereDisponibili()
        {
            var model = new CamereDisponibiliViewModel
            {
                DataInizio = DateTime.Today,
                DataFine = DateTime.Today.AddDays(1),
                CamereDisponibili = new System.Collections.Generic.List<Camera>()
            };

            return View(model);
        }

        // POST: Camere/CamereDisponibili
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CamereDisponibili(CamereDisponibiliViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (model.DataInizio >= model.DataFine)
                {
                    ModelState.AddModelError("DataFine", "La data di fine deve essere successiva alla data di inizio");
                    model.CamereDisponibili = new System.Collections.Generic.List<Camera>();
                    return View(model);
                }

                model.CamereDisponibili = await _cameraService.GetCamereDisponibiliAsync(model.DataInizio, model.DataFine);
            }

            return View(model);
        }
    }
}
