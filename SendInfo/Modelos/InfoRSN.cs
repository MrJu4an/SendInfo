using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class InfoRSN
    {
        public string CAJA { get; set; }
        public string FECHA { get; set; }
        public List<Concepto> DATOS { get; set; }


        public InfoRSN()
        {

        }

        public InfoRSN(string caja, string fecha, List<Concepto> datos)
        {
            this.CAJA = caja;
            this.FECHA = fecha;
            this.DATOS = datos;
        }
    }
}
