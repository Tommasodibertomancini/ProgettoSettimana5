using Microsoft.EntityFrameworkCore;
using ProgettoSettimana5.Data;
using ProgettoSettimana5.Models;

namespace ProgettoSettimana5.Services
{
    public class PrenotazioneService
    {
        private readonly ApplicationDbContext _context;

        public PrenotazioneService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Prenotazione>> GetAllPrenotazioniAsync()
        {
            return await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .OrderByDescending(p => p.DataInizio)
                .ToListAsync();
        }

        public async Task<List<Prenotazione>> GetPrenotazioniAttualiAsync()
        {
            var oggi = DateTime.Today;
            return await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .Where(p => p.Stato == StatoPrenotazione.Confermata &&
                           p.DataInizio <= oggi &&
                           p.DataFine >= oggi)
                .OrderBy(p => p.DataFine)
                .ToListAsync();
        }

        public async Task<List<Prenotazione>> GetPrenotazioniFutureAsync()
        {
            var oggi = DateTime.Today;
            return await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .Where(p => (p.Stato == StatoPrenotazione.Confermata || p.Stato == StatoPrenotazione.InAttesa) &&
                           p.DataInizio > oggi)
                .OrderBy(p => p.DataInizio)
                .ToListAsync();
        }

        public async Task<Prenotazione> GetPrenotazioneByIdAsync(int id)
        {
            return await _context.Prenotazioni
                .Include(p => p.Cliente)
                .Include(p => p.Camera)
                .FirstOrDefaultAsync(p => p.PrenotazioneId == id);
        }

        public async Task<List<Prenotazione>> GetPrenotazioniByClienteIdAsync(int clienteId)
        {
            return await _context.Prenotazioni
                .Include(p => p.Camera)
                .Where(p => p.ClienteId == clienteId)
                .OrderByDescending(p => p.DataInizio)
                .ToListAsync();
        }

        public async Task<bool> IsCameraDisponibileAsync(int cameraId, DateTime dataInizio, DateTime dataFine, int? prenotazioneId = null)
        {
            var prenotazioniSovrapposte = await _context.Prenotazioni
                .Where(p => p.CameraId == cameraId &&
                           p.Stato != StatoPrenotazione.Cancellata &&
                           ((p.DataInizio <= dataInizio && p.DataFine > dataInizio) ||
                            (p.DataInizio < dataFine && p.DataFine >= dataFine) ||
                            (p.DataInizio >= dataInizio && p.DataFine <= dataFine)))
                .ToListAsync();

            if (prenotazioneId.HasValue)
            {
                prenotazioniSovrapposte = prenotazioniSovrapposte
                    .Where(p => p.PrenotazioneId != prenotazioneId.Value)
                    .ToList();
            }

            return !prenotazioniSovrapposte.Any();
        }

        public async Task<bool> CreatePrenotazioneAsync(Prenotazione prenotazione)
        {
            if (!await IsCameraDisponibileAsync(prenotazione.CameraId, prenotazione.DataInizio, prenotazione.DataFine))
            {
                return false;
            }

            prenotazione.Stato = StatoPrenotazione.Confermata;

            _context.Prenotazioni.Add(prenotazione);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePrenotazioneAsync(Prenotazione prenotazione)
        {
            if (!await IsCameraDisponibileAsync(
                prenotazione.CameraId,
                prenotazione.DataInizio,
                prenotazione.DataFine,
                prenotazione.PrenotazioneId))
            {
                return false;
            }

            _context.Entry(prenotazione).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await PrenotazioneExistsAsync(prenotazione.PrenotazioneId))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> CancellaPrenotazioneAsync(int id)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null)
            {
                return false;
            }

            prenotazione.Stato = StatoPrenotazione.Cancellata;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePrenotazioneAsync(int id)
        {
            var prenotazione = await _context.Prenotazioni.FindAsync(id);
            if (prenotazione == null)
            {
                return false;
            }

            _context.Prenotazioni.Remove(prenotazione);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> PrenotazioneExistsAsync(int id)
        {
            return await _context.Prenotazioni.AnyAsync(p => p.PrenotazioneId == id);
        }
    }
}
