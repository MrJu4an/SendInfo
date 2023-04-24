using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Servicios;
using SendInfo.Modelos;
using System.Data;

namespace SendInfo.EnvioData
{
    class DataRSN
    {
        IRepositorioRSN iRepositorioRSN;
        IRepositorioGeneral iRepositorioGeneral;

        public DataRSN()
        {
            iRepositorioGeneral = new RepositorioGeneral();
            iRepositorioRSN = new RepositorioRSN();
        }
        
        public List<Concepto> ConsultarConceptos(Caja caja)
        {
            string nit, nitContratante, codEmpresa;
            DataTable dt;
            DataRow dr;
            InfoRSN infoRsn;
            Double totalcre, totalcup, totallin, total;
            List<Concepto> listaConceptos = new List<Concepto>();
            Boolean banderaConvenio = false;

            //Consultamos las empresas
            dt = iRepositorioRSN.consultarEmpresas(caja.FECHA, caja.NOMBRE);
            if (dt != null)
            {
                foreach (DataRow dRow in dt.Rows)
                {
                    try
                    {
                        codEmpresa = dRow["cocodemp"].ToString();

                        //Consultamos cual es el NIT de la empresa
                        dr = iRepositorioRSN.selectEmpresa(codEmpresa);
                        if (dr != null)
                        {
                            nit = dr["henit"].ToString();
                        }
                        else
                        {
                            dr = iRepositorioRSN.selectEmpresaConvenio(codEmpresa);
                            if (dr != null)
                            {
                                nit = dr["henit"].ToString();
                                //Si es una empresa de convenio debe realizar un proceso diferente
                                banderaConvenio = true;
                            }
                        }

                        //Se inicia con la consulta de lo recaudado por dicha empresa
                        if (!banderaConvenio)
                        {

                        }
                    }
                    catch (Exception ex)
                    {
                        string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                        iRepositorioRSN.insertLogRSN(ex.Message, "Error", date);
                    }
                }
            }
        }
    }
}
