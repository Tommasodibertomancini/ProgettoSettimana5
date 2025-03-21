using Microsoft.EntityFrameworkCore;
using ProgettoSettimana5.Data;
using ProgettoSettimana5.Models;

namespace ProgettoSettimana5.Services
{
    public class ClienteService
    {
        private readonly ApplicationDbContext _context;

        public ClienteService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Cliente>> GetAllClientiAsync()
        {
            return await _context.Clienti
                .OrderBy(c => c.Cognome)
                .ThenBy(c => c.Nome)
                .ToListAsync();
        }

        public async Task<Cliente> GetClienteByIdAsync(int id)
        {
            return await _context.Clienti
                .Include(c => c.Prenotazioni)
                .ThenInclude(p => p.Camera)
                .FirstOrDefaultAsync(c => c.ClienteId == id);
        }

        public async Task<Cliente> GetClienteByEmailAsync(string email)
        {
            return await _context.Clienti
                .FirstOrDefaultAsync(c => c.Email == email);
        }

        public async Task<bool> CreateClienteAsync(Cliente cliente)
        {
            // Verificare se esiste già un cliente con la stessa email
            if (await _context.Clienti.AnyAsync(c => c.Email == cliente.Email))
            {
                return false;
            }

            _context.Clienti.Add(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateClienteAsync(Cliente cliente)
        {            
            if (await _context.Clienti.AnyAsync(c => c.Email == cliente.Email && c.ClienteId != cliente.ClienteId))
            {
                return false;
            }

            _context.Entry(cliente).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await ClienteExistsAsync(cliente.ClienteId))
                {
                    return false;
                }
                throw;
            }
        }

        public async Task<bool> DeleteClienteAsync(int id)
        {
            var cliente = await _context.Clienti
                .Include(c => c.Prenotazioni)
                .FirstOrDefaultAsync(c => c.ClienteId == id);

            if (cliente == null)
            {
                return false;
            }

            if (cliente.Prenotazioni != null &&
                cliente.Prenotazioni.Any(p => p.Stato == StatoPrenotazione.Confermata || p.Stato == StatoPrenotazione.InAttesa))
            {
                return false;
            }

            _context.Clienti.Remove(cliente);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> ClienteExistsAsync(int id)
        {
            return await _context.Clienti.AnyAsync(c => c.ClienteId == id);
        }
    }
}
