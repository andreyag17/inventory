using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace appInventory.Models.ViewModel
{
    public class listaUsuario
    {
        public int usuarioId { get; set; }
        public string nombre { get; set; }
        public string apellido1 { get; set; }
        public string apellido2 { get; set; }
        public string correoElectronico { get; set; }
        public string nombreUsuario { get; set; }
        public string contrasennaUsuario { get; set; }
        public int rol_Id { get; set; }
    }
}