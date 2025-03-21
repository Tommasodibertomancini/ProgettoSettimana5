using Microsoft.AspNetCore.Identity;

namespace ProgettoSettimana5.Models
{
    public class ApplicationRole : IdentityRole
    {
        public string Descrizione { get; set; }

        public ApplicationRole() : base() { }

        public ApplicationRole(string roleName) : base(roleName) { }

        public ApplicationRole(string roleName, string descrizione) : base(roleName)
        {
            Descrizione = descrizione;
        }
    }
}
