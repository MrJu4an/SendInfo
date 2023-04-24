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
        DataRow consultarParametro(string param);
        DataRow selectEmpresaVehiculo(string placa);
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

        public DataRow selectEmpresaVehiculo(string placa)
        {
            string QRY = $@"SELECT hvcodemp FROM ophojveh 
                            WHERE hvplaca = '{placa}' ";
            return dbs.OpenRow(QRY);
        }

    }
}
