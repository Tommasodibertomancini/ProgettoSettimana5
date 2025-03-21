using System.Threading.Tasks;
using ProgettoSettimana5.Models;
using ProgettoSettimana5.Services;
using ProgettoSettimana5.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProgettoSettimana5.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {
        private readonly ClienteService _clienteService;
        private readonly PrenotazioneService _prenotazioneService;

        public ClientiController(ClienteService clienteService, PrenotazioneService prenotazioneService)
        {
            _clienteService = clienteService;
            _prenotazioneService = prenotazioneService;
        }

        // GET: Clienti
        public async Task<IActionResult> Index(string searchString)
        {
            var model = new ClienteListViewModel
            {
                Clienti = await _clienteService.GetAllClientiAsync(),
                SearchString = searchString
            };

            if (!string.IsNullOrEmpty(searchString))
            {
                searchString = searchString.ToLower();
                model.Clienti = model.Clienti.FindAll(c =>
                    c.Nome.ToLower().Contains(searchString) ||
                    c.Cognome.ToLower().Contains(searchString) ||
                    c.Email.ToLower().Contains(searchString) ||
                    c.Telefono.Contains(searchString));
            }

            return View(model);
        }

        // GET: Clienti/Details/
        public async Task<IActionResult> Details(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var prenotazioni = await _prenotazioneService.GetPrenotazioniByClienteIdAsync(id);

            var model = new ClienteDetailViewModel
            {
                Cliente = cliente,
                Prenotazioni = prenotazioni
            };

            return View(model);
        }

        // GET: Clienti/Create
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Clienti/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Create(ClienteCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var cliente = new Cliente
                {
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    Email = model.Email,
                    Telefono = model.Telefono
                };

                var success = await _clienteService.CreateClienteAsync(cliente);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cliente creato con successo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Email", "Esiste già un cliente con questa email");
                }
            }
            return View(model);
        }

        // GET: Clienti/Edit
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            var model = new ClienteEditViewModel
            {
                ClienteId = cliente.ClienteId,
                Nome = cliente.Nome,
                Cognome = cliente.Cognome,
                Email = cliente.Email,
                Telefono = cliente.Telefono
            };

            return View(model);
        }

        // POST: Clienti/Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager,Receptionist")]
        public async Task<IActionResult> Edit(int id, ClienteEditViewModel model)
        {
            if (id != model.ClienteId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var cliente = new Cliente
                {
                    ClienteId = model.ClienteId,
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    Email = model.Email,
                    Telefono = model.Telefono
                };

                var success = await _clienteService.UpdateClienteAsync(cliente);
                if (success)
                {
                    TempData["SuccessMessage"] = "Cliente aggiornato con successo";
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    ModelState.AddModelError("Email", "Esiste già un altro cliente con questa email");
                }
            }
            return View(model);
        }

        // GET: Clienti/Delete
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var cliente = await _clienteService.GetClienteByIdAsync(id);
            if (cliente == null)
            {
                return NotFound();
            }

            return View(cliente);
        }

        // POST: Clienti/Delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var success = await _clienteService.DeleteClienteAsync(id);
            if (success)
            {
                TempData["SuccessMessage"] = "Cliente eliminato con successo";
                return RedirectToAction(nameof(Index));
            }
            else
            {
                TempData["ErrorMessage"] = "Impossibile eliminare il cliente. " +
                                          "Assicurarsi che non abbia prenotazioni attive.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
