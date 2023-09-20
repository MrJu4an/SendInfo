using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Servicios;
using System.Data;
using Microsoft.VisualBasic;
using SendInfo.Modelos;
using SendInfo.EnvioData;

namespace SendInfo.Procesos
{
    class EnvioProtech
    {
        IRepositorioProtech iRepositorioProtech;
        IRepositorioGeneral iRepositorioGeneral;
        DataProtech dataProtech;
        public EnvioProtech()
        {
            iRepositorioGeneral = new RepositorioGeneral();
            iRepositorioProtech = new RepositorioProtech();
            dataProtech = new DataProtech();
        }
        public void consultarTasas()
        {
            //Definimos variables
            DataTable dt;
            DataRow dr;
            string fecha, hora, FV, fechaV, horaV, horaTasa, fec;
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

                dt = iRepositorioProtech.consultarConduces(fecha);
                if (dt != null)
                {
                    foreach (DataRow dRow in dt.Rows)
                    {
                        try
                        {
                            TasaUso tasaUso = new TasaUso
                                (
                                    Int32.Parse(dRow["CONUMERO"].ToString()),
                                    dRow["COPLACA"].ToString(),
                                    dRow["COTERMINAL"].ToString(),
                                    Strings.Mid(dRow["COFECSAL"].ToString(), 1, 10),
                                    Strings.Mid(dRow["COHORVEN"].ToString(), 12, 5)
                                );
                            dataProtech.EnvioDataTasa(tasaUso, urlTasa);
                        }
                        catch (Exception e)
                        {
                            string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                            iRepositorioProtech.insertLog($@"{ e } - Envio Información Tasa - Protech", "N/A", date);
                        }
                    }
                }

            }

            if (ciclo == "S")
            {
                try
                {
                    dt = iRepositorioProtech.selectConducesCiclo(fecha);
                    if (dt != null)
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            TasaUso tasaUso = new TasaUso
                                (
                                    Int32.Parse(dRow["CONUMERO"].ToString()),
                                    dRow["COPLACA"].ToString(),
                                    dRow["COTERMINAL"].ToString(),
                                    Strings.Mid(dRow["COFECSAL"].ToString(), 1, 10),
                                    Strings.Mid(dRow["COHORVEN"].ToString(), 12, 5)
                                );
                            dataProtech.controlCiclos(tasaUso, urlCiclo);
                        }
                    }
                }
                catch (Exception e)
                {
                    string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    iRepositorioProtech.insertLog($@"{ e } - Envio Información Tasa - Protech", "N/A", date);
                }
            }

            if (vigencia == "S")
            {
                try
                {
                    dt = iRepositorioProtech.selectConducesVig(fechaV, horaV);
                    if (dt != null)
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            TasaUso tasaUso = new TasaUso
                                (
                                    Int32.Parse(dRow["CONUMERO"].ToString()),
                                    dRow["COPLACA"].ToString(),
                                    dRow["COTERMINAL"].ToString(),
                                    Strings.Mid(dRow["COFECSAL"].ToString(), 1, 10),
                                    Strings.Mid(dRow["COHORVEN"].ToString(), 12, 5)
                                );
                            horaTasa = DateTime.Parse(dRow["cohorsal"].ToString()).ToString("HH:mm");
                            dataProtech.ControlVigencias(tasaUso, horaTasa);
                        }
                    }
                }
                catch (Exception e)
                {
                    string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    iRepositorioProtech.insertLog($@"{ e } - Envio Información Tasa - Protech", "N/A", date);
                }
            }
        }

        public string consultarUrl(string param1, string param2)
        {
            DataRow dr;
            string urlApi = "";
            string urlPro = "";

            dr = iRepositorioGeneral.consultarParametro(param1);
            if (dr != null)
            {
                urlApi = dr["psval"].ToString();
            }
            else
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"No se pudo encontrar el parametro: {param1}", "N/A", date);
            }

            dr = iRepositorioGeneral.consultarParametro(param2);
            if (dr != null)
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
