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
        public int Status { get; set; }
        public DateTime Fecha { get; set; }
        public int State { get; set; }
        public bool Follow { get; set; }
        public bool Favorites { get; set; }
        public bool Autorizar { get; set; }
    }
}
