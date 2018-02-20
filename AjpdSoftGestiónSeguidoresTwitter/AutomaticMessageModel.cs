using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GestiónSeguidoresTwitter
{
    public class AutomaticMessageModel
    {
        public int Id { get; set; }
        public string Tweet { get; set; }
        public int Status { get; set; }
        public int State { get; set; }
        public DateTime Fecha { get; set; }
        
    }
}
