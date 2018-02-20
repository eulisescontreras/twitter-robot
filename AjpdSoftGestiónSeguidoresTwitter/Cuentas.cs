using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestiónSeguidoresTwitter
{
    public class Cuentas
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public DateTime Fecha { get; set; }
        public int State { get; set; }
    }
}
