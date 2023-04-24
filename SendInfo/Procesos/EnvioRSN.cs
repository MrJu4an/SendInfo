using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Servicios;
using SendInfo.EnvioData;

namespace SendInfo.Procesos
{
    class EnvioRSN
    {
        IRepositorioProtech iRepositorioProtech;
        IRepositorioGeneral iRepositorioGeneral;
        DataRSN dataRsn;

        public EnvioRSN()
        {
            iRepositorioGeneral = new RepositorioGeneral();
            iRepositorioProtech = new RepositorioProtech();
            dataRsn = new DataRSN();
        }
    }
}
