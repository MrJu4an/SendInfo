using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class CicloProtech
    {
        public string placa { get; set; }
        public string fechaHora { get; set; }
        public int terminal { get; set; }
        public int tipoCiclo { get; set; }

        public CicloProtech()
        {

        }

        public CicloProtech(string placa, string fechaHora, int terminal, int tipoCiclo)
        {
            this.placa = placa;
            this.fechaHora = fechaHora;
            this.terminal = terminal;
            this.tipoCiclo = tipoCiclo;
        }
    }
}
