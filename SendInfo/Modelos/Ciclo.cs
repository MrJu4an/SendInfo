using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SendInfo.Modelos
{
    class Ciclo
    {
        public string ISPLACA { get; set; }
        public int ISIDINGSAL { get; set; }
        public string ISFECING { get; set; }
        public string ISHORING { get; set; }
        public string ISFECSAL { get; set; }
        public string ISHORSAL { get; set; }

        public Ciclo()
        {

        }

        public Ciclo(string isplaca, int isidingsal, string isfecing, string ishoring, string isfecsal, string ishorsal)
        {
            this.ISPLACA = isplaca;
            this.ISIDINGSAL = isidingsal;
            this.ISFECING = isfecing;
            this.ISHORING = ishoring;
            this.ISFECSAL = isfecsal;
            this.ISHORSAL = ishorsal;
        }
    }
}
