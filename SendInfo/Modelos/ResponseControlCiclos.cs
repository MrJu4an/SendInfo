using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class ResponseControlCiclos
    {
        public string Resultado { get; set; }
        public string Mensaje { get; set; }
        public string FechaHoraIngreso { get; set; }
        public string FechaHoraSalida { get; set; }
        public string Terminal { get; set; }
        public Double CobroPermanencia { get; set; }
        public string JSON { get; set; }
        public int Proceso { get; set; }
        public string JSONResp { get; set; }
    }
}
