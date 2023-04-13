using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Servicios;
using System.Data;
using Microsoft.VisualBasic;
using SendInfo.Modelos;

namespace SendInfo.Procesos
{
    class EnvioProtech
    {
        RepositorioProtech iRepositorioProtech;
        RepositorioGeneral iRepositorioGeneral;
        public EnvioProtech()
        {
            iRepositorioProtech = new RepositorioProtech();
            iRepositorioGeneral = new RepositorioGeneral();
        }
        public void consultarTasas()
        {
            //Definimos variables
            DataTable dt, dt2, dt3;
            DataRow dr, dr2, dr3;
            string fecha, hora, FV, fechaV, horaV, fechaTasa, horaTasa, fec;
            string urlTasa, urlCiclo;
            string ciclo, tasa, vigencia;
            int tiempo;

            //Consultamos el web service de Tasas de uso
            urlTasa = consultarUrl("URLWSCOSAL", "URLAPICSTU");
            //consultamos el web services de Ciclos
            urlCiclo = consultarUrl("URLWSCICL", "URLAPICCL");

            //Consultamos el tiempo desde el sistema toamrá en cuenta las tasas a revisar el proceso de Vigencias
            tiempo = Int32.Parse(asignarParametro("MINVALVIG"));
            if (tiempo == null) { return; }

            //Consultamos la fecha del sistema
            fec = asignarParametro("HORASIS");
            if (fec != null)
            {
                fecha = Strings.Mid(fec, 1, 10);
                hora = Strings.Mid(fec, 12, 5);

                //Fechas a utilizar en el control de vigencias
                FV = Strings.Format(DateTime.Parse(fec).AddMinutes(-tiempo), "MM/dd/yyyy HH:mm");
                fechaV = Strings.Mid(FV, 1, 10);
                horaV = Strings.Mid(FV, 12, 5);
            }
            else
            {
                return;
            }

            //Consultamos que procesos están activos para realizar
            ciclo = asignarParametro("PROCIECICL");
            if (ciclo == null) { return; }

            tasa = asignarParametro("PROTUPROT");
            if (tasa == null) { return; }

            vigencia = asignarParametro("PROCLVIG"); 
            if (vigencia == null) { return; }
            
            //Enviamos la información de la Tasa de Uso
            if (tasa == "S")
            {

            }
        }

        public string consultarUrl(string param1, string param2)
        {
            DataRow dr;
            string urlApi = ""; 
            string urlPro = "";

            dr = iRepositorioGeneral.consultarParametro(param1);
            if(dr != null)
            {
                urlApi = dr["psval"].ToString();
            }
            else
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"No se pudo encontrar el parametro: {param1}", "N/A", date);
            }

            dr = iRepositorioGeneral.consultarParametro(param2);
            if(dr != null)
            {
                urlPro = dr["psval"].ToString();
            }
            else
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"No se pudo encontrar el parametro: {param2}", "N/A", date);
            }
            return urlApi + urlPro;
        }

        public string asignarParametro(string param)
        {
            DataRow dr;

            dr = iRepositorioGeneral.consultarParametro(param);
            if (dr != null)
            {
                return dr["psval"].ToString();
            }
            else
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"No se pudo encontrar el parametro: {param}", "N/A", date);
                return null;
            }
        }
    }
}
