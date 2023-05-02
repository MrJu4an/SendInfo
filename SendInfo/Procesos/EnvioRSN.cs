using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Servicios;
using SendInfo.EnvioData;
using SendInfo.Modelos;
using System.Data;
using Microsoft.VisualBasic;

namespace SendInfo.Procesos
{
    class EnvioRSN
    {
        IRepositorioRSN iRepositorioRSN;
        IRepositorioGeneral iRepositorioGeneral;
        DataRSN dataRsn;

        public EnvioRSN()
        {
            iRepositorioGeneral = new RepositorioGeneral();
            iRepositorioRSN = new RepositorioRSN();
            dataRsn = new DataRSN();
        }

        public void ConsultarCierres()
        {
            InfoRSN infoRSN = new InfoRSN();
            DataRSN dataRSN = new DataRSN();
            Caja caja;
            string fecha;
            List<Concepto> listaDatos;
            DataTable dt;
            DataRow dr;
            string urlWS, urlPr, url;
            string token;
            string xml;

            //Consultamos el Web Services a consumir de RSN
            url = consultarUrl("URLWSRSN", "URLWSCITUR");

            //consultamos el token
            dr = iRepositorioGeneral.consultarParametro("TOKENRSN");
            token = dr["psval"].ToString();

            //Consultamos la fecha del día anterior
            dr = iRepositorioGeneral.consultarParametro("HORASIS");
            fecha = DateTime.Parse(Strings.Mid(dr["psval"].ToString(), 1, 10)).AddDays(-1).ToString("MM/dd/yyyy");

            //Consultamos el cierre de turno para la hora actual
            dt = iRepositorioRSN.consultarCierres(fecha);

            if (dt != null)
            {
                foreach(DataRow dRow in dt.Rows)
                {
                    try
                    {
                        //Creamos el objeto caja
                        caja = new Caja(
                                            dRow["cinomcaj"].ToString(),
                                            Strings.Mid(dRow["cifecha"].ToString(), 1, 10),
                                            Int32.Parse(dRow["cinumini"].ToString()),
                                            Int32.Parse(dRow["cinumfin"].ToString())
                                        );

                        //Creamos el objeto a enviar a RSN
                        infoRSN.CAJA = sacarCaja(caja.NOMBRE);
                        infoRSN.FECHA = DateTime.Parse(fecha).ToString("dd/MM/yyyy");
                        infoRSN.DATOS = dataRSN.ConsultarConceptos(caja);

                        //Creamos el XML
                        xml = armadoXML(infoRSN, token);

                        //Consumimos el Web Service de RSN
                        ResponseRSN response = dataRSN.ConsumirWSRSN(xml, url);

                        if (response.Codigo == 0)
                        {
                            iRepositorioRSN.insertLogRSN(response.Caso + $@" Caja: {caja.NOMBRE}", caja.NOMBRE, caja.FECHA);
                        }
                        else
                        {
                            iRepositorioRSN.insertLogRSN(response.Caso + $@" Caja: {caja.NOMBRE}", caja.NOMBRE, caja.FECHA);
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

        public string armadoXML(InfoRSN infoRSN, string token)
        {
            string XML;
            int i;
            List<Concepto> list;

            //Armamos el encabezado
            XML = $@"<?xml version=""1.0"" encoding=""ISO-8859-1""?>
                            <SOAP-ENV:Envelope SOAP-ENV:encodingStyle=""http://schemas.xmlsoap.org/soap/encoding/"" 
                                xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"" 
                                xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" 
                                xmlns:xsi=""http://www.w3.org/2001/XMLSchema-instance"" 
                                xmlns:SOAP-ENC=""http://schemas.xmlsoap.org/soap/encoding/""> 
                                <SOAP-ENV:Body> 
                                    <ns1944:setTasasUso 
                                        xmlns:ns1944=""http://tempuri.org"">
                                        <datos xsi:type=""SOAP-ENC:Array"" SOAP-ENC:arrayType=""unnamed_struct_use_soapval[1]"">
                                            <item>
                                                <Caja xsi:type=""xsd:string"">{infoRSN.CAJA}</Caja>
                                                <Fecha xsi:type=""xsd:string"">{infoRSN.FECHA}</Fecha>
                                                <Datos xsi:type=""SOAP-ENC:Array"" SOAP-ENC:arrayType=""unnamed_struct_use_soapval[5]"">";

            //Recorremos la lista de objetos
            if (infoRSN.DATOS != null)
            {
                list = infoRSN.DATOS;
                i = list.Count;
                for (i = 0; i <= list.Count - 1; i++)
                {
                    XML += $@"<item>
                                    <Nit xsi:type=""xsd:string"">{list[i].NIT}</Nit>
                                    <Concepto xsi:type=""xsd:string"">{list[i].CONCEPTO}</Concepto>
                                    <Valor xsi:type=""xsd:string"">{list[i].VALOR}</Valor>";

                    if (list[i].VALOR_CREDITO > 0)
                    {
                        XML += $@"<Valor_credito xsi:type=""xsd:string"">{list[i].VALOR_CREDITO}</Valor_credito>";
                    }
                    else
                    {
                        XML += $@"<Valor_credito xsi:type=""xsd:string""></Valor_credito>";
                    }

                    if (list[i].VALOR_CUPO > 0)
                    {
                        XML += $@"<Valor_cupo xsi:type=""xsd:string"">{list[i].VALOR_CUPO}</Valor_cupo>";
                    }
                    else
                    {
                        XML += $@"<Valor_cupo xsi:type=""xsd:string""></Valor_cupo>";
                    }

                    if (list[i].VALOR_CONTADO > 0)
                    {
                        XML += $@"<Valor_contado xsi:type=""xsd:string"">{list[i].VALOR_CONTADO}</Valor_contado>";
                    }
                    else
                    {
                        XML += $@"<Valor_contado xsi:type=""xsd:string""></Valor_contado>";
                    }

                    XML += $@"</item>";
                }
            }

            //Armamos el pie del XML
            XML += $@"</Datos>
                     </item>
                    </datos>
                   <token xsi:type=""xsd:string"">{token}</token>
                  </ns1944:setTasasUso>
                 </SOAP-ENV:Body>
                </SOAP-ENV:Envelope>";

            return XML;
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
                iRepositorioRSN.insertLogRSN($@"No se pudo encontrar el parametro: {param1}", "Error", date);
            }

            dr = iRepositorioGeneral.consultarParametro(param2);
            if (dr != null)
            {
                urlPro = dr["psval"].ToString();
            }
            else
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioRSN.insertLogRSN($@"No se pudo encontrar el parametro: {param2}", "Error", date);
            }
            return urlApi + urlPro;
        }

        public string sacarCaja(string caja)
        {
            return Strings.Mid(caja, 5);
        }
    }
}
