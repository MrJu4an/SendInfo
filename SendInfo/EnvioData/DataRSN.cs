using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Servicios;
using SendInfo.Modelos;
using SendInfo.Utiles;
using System.Data;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using System.Text;
using System.Xml;

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
            string codEmpresa;
            int nit = 0, nitContratante = 0;
            DataTable dt, dt2;
            DataRow dr;
            Double totalcre = 0, totalcup = 0, totallin = 0;
            Double total = 0;
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
                            nit = Int32.Parse(dr["henit"].ToString());
                        }
                        else
                        {
                            dr = iRepositorioRSN.selectEmpresaConvenio(codEmpresa);
                            if (dr != null)
                            {
                                nit = Int32.Parse(dr["henit"].ToString());
                                //Si es una empresa de convenio debe realizar un proceso diferente
                                banderaConvenio = true;
                            }
                        }

                        //Se inicia con la consulta de lo recaudado por dicha empresa
                        if (!banderaConvenio)
                        {
                            // ----- Tasas de Uso Origen -----
                            dt2 = iRepositorioRSN.selectConducesEmpresa(codEmpresa, caja.FECHA, "O", caja.NOMBRE, caja.NUMINI, caja.NUMFIN, "No");
                            if (dt2 != null)
                            {
                                foreach (DataRow dRow2 in dt2.Rows)
                                {
                                    //Asignamos valores por cupos
                                    if (dRow2["comodo"].ToString() == "P")
                                    {
                                        totalcup = Double.Parse(dRow2["TotalConduces"].ToString());
                                    }
                                    if (dRow2["comodo"].ToString() == "D")
                                    {
                                        totallin = Double.Parse(dRow2["TotalConduces"].ToString());
                                    }
                                }
                            }
                            //Consultamos el total por credito
                            dr = iRepositorioRSN.selectConducesCreditoEmpresa(codEmpresa, caja.FECHA, "O", caja.NOMBRE, caja.NUMINI, caja.NUMFIN, "No");
                            totalcre = Double.Parse(dr["TotalConduces"].ToString());

                            //Teniendo los valores, calculamos el total y guardamos
                            total = totalcup + totallin + totalcre;

                            //El sistema aveces encuentra valores en cero, si el total es mayor a cero, se añade el concepto a la lista
                            if (total > 0)
                            {
                                listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.TasasUsoOrigen, total, totalcre, totalcup, totallin));
                            }

                            //Reiniciamos valores
                            totalcre = 0;
                            totalcup = 0;
                            totallin = 0;
                            total = 0;

                            // ----- Tasas de Uso Transito -----
                            dt2 = iRepositorioRSN.selectConducesEmpresa(codEmpresa, caja.FECHA, "T", caja.NOMBRE, caja.NUMINI, caja.NUMFIN, "No");
                            if (dt2 != null)
                            {
                                foreach (DataRow dRow2 in dt2.Rows)
                                {
                                    if (dRow2["comodo"].ToString() == "P")
                                    {
                                        totalcup = Double.Parse(dRow2["TotalConduces"].ToString());
                                    }
                                    if (dRow2["comodo"].ToString() == "D")
                                    {
                                        totallin = Double.Parse(dRow2["TotalConduces"].ToString());
                                    }
                                }
                            }
                            //Consultamos el total por crédito
                            dr = iRepositorioRSN.selectConducesCreditoEmpresa(codEmpresa, caja.FECHA, "T", caja.NOMBRE, caja.NUMINI, caja.NUMFIN, "No");
                            totalcre = Double.Parse(dr["TotalConduces"].ToString());

                            total = totalcup + totallin + totalcre;

                            //El sistema aveces encuentra valores en cero, si el total es mayor a cero se añade el concepto a la lista
                            if (total > 0)
                            {
                                listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.TasasUsoTransito, total, totalcre, totalcup, totallin));
                            }

                            //Reiniciamos valores
                            totalcre = 0;
                            totalcup = 0;
                            totallin = 0;
                            total = 0;

                            //Sebastián Rondón - Enero 25 de 2023
                            //Por petición del Área de Financiera de la terminal de Bogotá
                            //Se empieza a mandar por separado, lo recaudado por rutas de influencia
                            // ----- Rutas de Influencia -----
                            dt2 = iRepositorioRSN.selectConducesEmpresa(codEmpresa, caja.FECHA, "O', 'T", caja.NOMBRE, caja.NUMINI, caja.NUMFIN, "Si");
                            if (dt2 != null)
                            {
                                foreach (DataRow dRow2 in dt2.Rows)
                                {
                                    if (dRow2["comodo"].ToString() == "P")
                                    {
                                        totalcup = Double.Parse(dRow2["TotalConduces"].ToString());
                                    }
                                    if (dRow2["comodo"].ToString() == "D")
                                    {
                                        totallin = Double.Parse(dRow2["TotalConduces"].ToString());
                                    }
                                }
                            }

                            //Consultamos el total por crédito
                            dr = iRepositorioRSN.selectConducesCreditoEmpresa(codEmpresa, caja.FECHA, "O', 'T", caja.NOMBRE, caja.NUMINI, caja.NUMFIN, "Si");
                            totalcre = Double.Parse(dr["TotalConduces"].ToString());

                            total = totalcup + totallin + totalcre;

                            //El sistema aveces encuentra valores en cero, si el total es mayor a cero, se añade el concepto
                            if (total > 0)
                            {
                                listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Influencia, total, totalcre, totalcup, totallin));
                            }

                            //Reiniciamos valores
                            totalcre = 0;
                            totalcup = 0;
                            totallin = 0;
                            total = 0;

                            // ----- Alcoholimetría -----
                            dt2 = iRepositorioRSN.selectAlcoholEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, caja.NUMINI, caja.NUMFIN);
                            if (dt2 != null)
                            {
                                foreach (DataRow dRow2 in dt2.Rows)
                                {
                                    if (dRow2["comodo"].ToString() == "P")
                                    {
                                        totalcup = Double.Parse(dRow2["TotalAlcohol"].ToString());
                                    }
                                    if (dRow2["comodo"].ToString() == "D")
                                    {
                                        totallin = Double.Parse(dRow2["TotalAlcohol"].ToString());
                                    }
                                }

                                //Para alcoholimetría, multas y demás conceptos no se envian valores credito
                                total = totalcup + totallin + totalcre;

                                //El sistema aveces encuentra valores en cero, si el total es mayor a cero, se añade el concepto
                                if (total > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Alcoholimetria, total, totalcre, totalcup, totallin));
                                }
                                //Reiniciamos valores
                                totalcre = 0;
                                totalcup = 0;
                                totallin = 0;
                                total = 0;
                            }

                            // ----- Permanencias -----
                            dr = iRepositorioRSN.selectPermanenciasEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE);
                            if (dr != null)
                            {
                                if(Double.Parse(dr["TotalPermanencia"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Permanencia, Double.Parse(dr["TotalPermanencia"].ToString()), 0, 0, Double.Parse(dr["TotalPermanencia"].ToString())));
                                }
                            }

                            // ----- Elusión -----
                            dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.Elusion);
                            if (dr != null)
                            {
                                if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Elusion, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                }
                            }

                            // ----- Evasión -----
                            dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.Evasion);
                            if (dr != null)
                            {
                                if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Evasion, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                }
                            }

                            // ----- Perdida Documento -----
                            dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.Duplicado);
                            if (dr != null)
                            {
                                if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Duplicado, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                }
                            }

                            // ----- Evasiones Transito -----
                            dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.EvasionTransito);
                            if (dr != null)
                            {
                                if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.EvasionTransito, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                }
                            }

                            // ----- Vigencias -----
                            dr = iRepositorioRSN.selectVigenciaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE);
                            if (dr != null)
                            {
                                if (Double.Parse(dr["TotalVigencia"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Vigencia, Double.Parse(dr["TotalVigencia"].ToString()), 0, 0, Double.Parse(dr["TotalVigencia"].ToString())));
                                }
                            }

                            // ----- TAG -----
                            dr = iRepositorioRSN.selectTagEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE);
                            if (dr != null)
                            {
                                if (Double.Parse(dr["TotalTag"].ToString()) > 0)
                                {
                                    listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Tag, Double.Parse(dr["TotalTag"].ToString()), 0, 0, Double.Parse(dr["TotalTag"].ToString())));
                                }
                            }
                        }
                        else
                        {
                            //Realizamos la consulta de los conceptos de la empresa convenio
                            dt2 = iRepositorioRSN.consultarEmpresasConvenio(caja.FECHA, caja.NOMBRE, codEmpresa);
                            if (dt2 != null)
                            {
                                foreach (DataRow dRow2 in dt2.Rows)
                                {
                                    //Buscamos primero la empresa a cargar los conceptos
                                    dr = iRepositorioRSN.selectEmpresaContratante(codEmpresa, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        nitContratante = Int32.Parse(dr["henit"].ToString());
                                    }
                                    else
                                    {
                                        continue;
                                    }

                                    // ----- Permanencias -----
                                    dr = iRepositorioRSN.selectPermanenciasEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalPermanencia"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Permanencia, Double.Parse(dr["TotalPermanencia"].ToString()), 0, 0, Double.Parse(dr["TotalPermanencia"].ToString())));
                                        }
                                    }

                                    // ----- Elusión -----
                                    dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.Elusion, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Elusion, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                        }
                                    }

                                    // ----- Evasión -----
                                    dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.Evasion, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Evasion, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                        }
                                    }

                                    // ----- Perdida Documento -----
                                    dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.Duplicado, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Duplicado, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                        }
                                    }

                                    // ----- Evasiones Transito -----
                                    dr = iRepositorioRSN.selectMultaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, ValuesRSN.TiposConceptos.EvasionTransito, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalMulta"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.EvasionTransito, Double.Parse(dr["TotalMulta"].ToString()), 0, 0, Double.Parse(dr["TotalMulta"].ToString())));
                                        }
                                    }

                                    // ----- Vigencias -----
                                    dr = iRepositorioRSN.selectVigenciaEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalVigencia"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Vigencia, Double.Parse(dr["TotalVigencia"].ToString()), 0, 0, Double.Parse(dr["TotalVigencia"].ToString())));
                                        }
                                    }

                                    // ----- TAG -----
                                    dr = iRepositorioRSN.selectTagEmpresa(codEmpresa, caja.FECHA, caja.NOMBRE, dRow2["placa"].ToString());
                                    if (dr != null)
                                    {
                                        if (Double.Parse(dr["TotalTag"].ToString()) > 0)
                                        {
                                            listaConceptos.Add(new Concepto(nit, ValuesRSN.TiposConceptos.Tag, Double.Parse(dr["TotalTag"].ToString()), 0, 0, Double.Parse(dr["TotalTag"].ToString())));
                                        }
                                    }
                                }
                            }
                            banderaConvenio = false;
                        }
                    }
                    catch (Exception ex)
                    {
                        string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                        iRepositorioRSN.insertLogRSN(ex.Message, "Error", date);
                    }
                }
            }
            return listaConceptos;
        }

        private string SendRequest(Uri uri, byte[] jsonDataBytes, string contentType, string method)
        {
            string response;
            WebRequest request;

            request = WebRequest.Create(uri);
            request.Headers.Add("SOAPAction", "urn:terminalwsdl#setTasasUso");
            request.ContentLength = jsonDataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;
            request.Timeout = 60000;

            using (var requestStream = request.GetRequestStream())
            {
                requestStream.Write(jsonDataBytes, 0, jsonDataBytes.Length);
                requestStream.Close();

                using (var responseStream = request.GetResponse().GetResponseStream())
                {
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        response = reader.ReadToEnd();
                    }
                }
            }

            return response;
        }

        //public ResponseRSN desencriptarResponse(string r)
        //{
        //    XmlNode elemento;
        //    XmlElement subnodo;
        //    XmlDocument obj = new XmlElement();
        //    string cadena;
        //    ResponseRSN responseRSN = new ResponseRSN();

        //    obj.LoadXml(r);
        //}
    }
}
