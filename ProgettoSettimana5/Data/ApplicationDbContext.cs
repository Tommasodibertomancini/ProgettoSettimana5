using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ProgettoSettimana5.Models;

namespace ProgettoSettimana5.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Cliente> Clienti { get; set; }
        public DbSet<Camera> Camere { get; set; }
        public DbSet<Prenotazione> Prenotazioni { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Prenotazione>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Prenotazioni)
                .HasForeignKey(p => p.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prenotazione>()
                .HasOne(p => p.Camera)
                .WithMany(c => c.Prenotazioni)
                .HasForeignKey(p => p.CameraId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Camera>()
                .HasIndex(c => c.Numero)
                .IsUnique();

            modelBuilder.Entity<Cliente>()
                .HasIndex(c => c.Email)
                .IsUnique();

            modelBuilder.Entity<Camera>()
               .Property(c => c.Prezzo)
               .HasColumnType("decimal(10,2)");
        }
    }
}
