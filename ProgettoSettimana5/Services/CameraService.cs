using Microsoft.EntityFrameworkCore;
using ProgettoSettimana5.Data;
using ProgettoSettimana5.Models;

namespace ProgettoSettimana5.Services
{
    public class CameraService
    {
        private readonly ApplicationDbContext _context;

        public CameraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Camera>> GetAllCamereAsync()
        {
            return await _context.Camere
                .OrderBy(c => c.Numero)
                .ToListAsync();
        }

        public async Task<Camera> GetCameraByIdAsync(int id)
        {
            return await _context.Camere
                .Include(c => c.Prenotazioni)
                .ThenInclude(p => p.Cliente)
                .FirstOrDefaultAsync(c => c.CameraId == id);
        }

        public async Task<List<Camera>> GetCamereDisponibiliAsync(DateTime dataInizio, DateTime dataFine)
        {            
            var tutteCamere = await _context.Camere.ToListAsync();

            var camereOccupate = await _context.Prenotazioni
                .Where(p => p.Stato != StatoPrenotazione.Cancellata &&
                           ((p.DataInizio <= dataInizio && p.DataFine > dataInizio) ||
                            (p.DataInizio < dataFine && p.DataFine >= dataFine) ||
                            (p.DataInizio >= dataInizio && p.DataFine <= dataFine)))
                .Select(p => p.CameraId)
                .Distinct()
                .ToListAsync();

            return tutteCamere
                .Where(c => !camereOccupate.Contains(c.CameraId))
                .ToList();
        }

        public async Task<bool> CreateCameraAsync(Camera camera)
        {
            if (await _context.Camere.AnyAsync(c => c.Numero == camera.Numero))
            {
                return false;
            }

            _context.Camere.Add(camera);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateCameraAsync(Camera camera)
        {
            if (await _context.Camere.AnyAsync(c => c.Numero == camera.Numero && c.CameraId != camera.CameraId))
            {
                return false;
            }

            _context.Entry(camera).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await CameraExistsAsync(camera.CameraId))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteCameraAsync(int id)
        {
            var camera = await _context.Camere
                .Include(c => c.Prenotazioni)
                .FirstOrDefaultAsync(c => c.CameraId == id);

            if (camera == null)
            {
                return false;
            }

            if (camera.Prenotazioni != null &&
                camera.Prenotazioni.Any(p =>
                    (p.Stato == StatoPrenotazione.Confermata || p.Stato == StatoPrenotazione.InAttesa) &&
                    p.DataFine >= DateTime.Today))
            {
                return false;
            }

            _context.Camere.Remove(camera);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CameraExistsAsync(int id)
        {
            return await _context.Camere.AnyAsync(c => c.CameraId == id);
        }
    }
}
