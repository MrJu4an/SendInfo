using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class Caja
    {
        public string NOMBRE { get; set; }
        public string FECHA { get; set; }
        public int NUMINI { get; set; }
        public int NUMFIN { get; set; }

        public Caja()
        {

        }

        public Caja(string nombre, string fecha, int numini, int numfin)
        {
            this.NOMBRE = nombre;
            this.FECHA = fecha;
            this.NUMINI = numini;
            this.NUMFIN = numfin;
        }
    }
}
