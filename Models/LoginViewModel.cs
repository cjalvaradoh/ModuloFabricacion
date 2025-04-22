using System.ComponentModel.DataAnnotations;

namespace caobaModeloFabricacion.Models
{
    public class LoginViewModel
    {
        [Required]
        public string Usuario { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Recordar { get; set; }
    }
}
