using Microsoft.AspNetCore.Identity;

namespace caobaModeloFabricacion.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Nombre { get; set; }
        public string Rol { get; set; }
    }
}