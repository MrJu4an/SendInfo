using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class ResponseRSN
    {
        public string Caso { get; set; }
        public int Codigo { get; set; }
        public string Observacion { get; set; }

        public ResponseRSN()
        {

        }

        public ResponseRSN(string caso, int codigo, string observacion)
        {
            this.Caso = caso;
            this.Codigo = codigo;
            this.Observacion = observacion;
        }
    }
}
