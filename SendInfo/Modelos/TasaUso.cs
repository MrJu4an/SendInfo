using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class TasaUso
    {
        public double CONUMERO { get; set; }
        public string COPLACA { get; set; }
        public string COTERMINAL { get; set; }
        public string COFECHA { get; set; }
        public string COHORA { get; set; }

        public TasaUso()
        {

        }

        public TasaUso(double conumero, string coplaca, string coterminal, string cofecha, string cohora)
        {
            this.CONUMERO = conumero;
            this.COPLACA = coplaca;
            this.COTERMINAL = coterminal;
            this.COFECHA = COFECHA;
            this.COHORA = COHORA;
        }
    }
}
