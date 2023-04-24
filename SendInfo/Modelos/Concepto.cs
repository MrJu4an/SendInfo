using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class Concepto
    {
        public int NIT { get; set; }
        public int CONCEPTO { get; set; }
        public Double VALOR { get; set; }
        public Double VALOR_CREDITO { get; set; }
        public Double VALOR_CUPO { get; set; }
        public Double VALOR_CONTADO { get; set; }

        public Concepto()
        {

        }

        public Concepto(int nit, int concepto, Double valor, Double valor_credito, Double valor_contado)
        {
            this.NIT = nit;
            this.CONCEPTO = concepto;
            this.VALOR = valor;
            this.VALOR_CREDITO = valor_credito;
            this.VALOR_CONTADO = valor_contado;
        }
    }
}
