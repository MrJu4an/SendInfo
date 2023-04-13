using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SendInfo.Servicios
{
    public interface IRepositorioProtech
    {
        DataTable consultarConduces(string fecha);
        void insertLog(string msg, string value, string fecha);
    }

    class RepositorioProtech : IRepositorioProtech
    {
        Datas dbs;
        public RepositorioProtech()
        {
            dbs = new Datas();
        }

        public DataTable consultarConduces(string fecha)
        {
            string QRY = $@"SELECT conumero, coplaca, cofecsal, coterminal 
                            FROM coconenvp 
                            WHERE cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') AND coestado = 'PEN' ";
            return dbs.OpenData(QRY);
        }

        public void insertLog(string msg, string value, string fecha)
        {
            string QRY = $@"INSERT INTO logprotech 
                            (lotipo, lovalor, lodescripcion, lofecha) 
                            VALUES
                            ('S', '{value}', '{msg}', TO_DATE('{fecha}','MM/DD/YYYY HH24:mi') )";
            dbs.Execute(QRY);
        }
    }
}
