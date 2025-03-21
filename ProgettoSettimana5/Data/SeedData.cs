using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProgettoSettimana5.Models;

namespace ProgettoSettimana5.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            using (var context = new ApplicationDbContext(
                serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>()))
            {
                context.Database.Migrate();

                var roleManager = serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();
                await SeedRolesAsync(roleManager);

                var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                await SeedAdminUserAsync(userManager);

                await SeedCamereAsync(context);
            }
        }

        private static async Task SeedRolesAsync(RoleManager<ApplicationRole> roleManager)
        {           
            string[] roleNames = { "Admin", "Manager", "Receptionist", "Viewer" };
            string[] roleDescriptions = {
                "Accesso completo a tutte le funzionalità",
                "Gestione completa di clienti, camere e prenotazioni",
                "Gestione clienti e prenotazioni",
                "Solo visualizzazione dati"
            };

            for (int i = 0; i < roleNames.Length; i++)
            {
                var roleName = roleNames[i];
                var roleExists = await roleManager.RoleExistsAsync(roleName);
                if (!roleExists)
                {
                    var role = new ApplicationRole(roleName, roleDescriptions[i]);
                    await roleManager.CreateAsync(role);
                }
            }
        }

        private static async Task SeedAdminUserAsync(UserManager<ApplicationUser> userManager)
        {
          
            var adminEmail = "admin@hotel.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    Nome = "Admin",
                    Cognome = "System"
                };

                var result = await userManager.CreateAsync(adminUser, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }

        private static async Task SeedCamereAsync(ApplicationDbContext context)
        {
           
            if (!context.Camere.Any())
            {
               
                var tipiCamera = new[]
                {
                    new { Tipo = "Singola", Prezzo = 50m },
                    new { Tipo = "Doppia", Prezzo = 80m },
                    new { Tipo = "Matrimoniale", Prezzo = 90m },
                    new { Tipo = "Suite", Prezzo = 150m },
                    new { Tipo = "Deluxe", Prezzo = 200m }
                };

                for (int i = 1; i <= 20; i++)
                {
                    var tipoIndex = (i - 1) % tipiCamera.Length;
                    var camera = new Camera
                    {
                        Numero = i,
                        Tipo = tipiCamera[tipoIndex].Tipo,
                        Prezzo = tipiCamera[tipoIndex].Prezzo
                    };

                    context.Camere.Add(camera);
                }

                await context.SaveChangesAsync();
            }
        }
    }
}
