using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SendInfo.Servicios
{
    public interface IRepositorioGeneral
    {

    }
    class RepositorioGeneral : IRepositorioGeneral
    {
        Datas dbs;

        public RepositorioGeneral()
        {
            dbs = new Datas();
        }

        public DataRow consultarParametro(string param)
        {
            var QRY = "SELECT psval FROM geparsis WHERE pscod = '" + param + "' ";
            return dbs.OpenRow(QRY);
        }
    }
}
