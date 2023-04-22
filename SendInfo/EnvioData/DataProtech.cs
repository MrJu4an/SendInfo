using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using SendInfo.Modelos;
using SendInfo.Servicios;
using Newtonsoft.Json;
using System.IO;
using System.Data;
using SendInfo.Utiles;
using Microsoft.VisualBasic;
using System.Security.Policy;

namespace SendInfo.EnvioData
{
    class DataProtech
    {
        #region Variables

        IRepositorioProtech iRepositorioProtech;
        IRepositorioGeneral iRepositorioGeneral;

        #endregion

        #region Constructor
        public DataProtech()
        {
            iRepositorioProtech = new RepositorioProtech();
            iRepositorioGeneral = new RepositorioGeneral();
        }
        #endregion

        #region Datos Tasa
        public void EnvioDataTasa(TasaUso tasa, string url)
        {
            try
            {
                ResponseTasa respuesta = ConsumirWSTasa(tasa, url);
                if (respuesta.status_code == 0)
                {
                    iRepositorioProtech.updateTasa(tasa.CONUMERO);
                }
                else
                {
                    string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                    iRepositorioProtech.insertLog($@"{respuesta.msg} - Consumo Tasa - TU Protech", "N/A", date);
                }
            }
            catch (Exception ex)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"{ex.Message} - Envio Tasa - TU Protech", "N/A", date);
            }

        }
        #endregion

        #region Control Ciclos
        public void controlCiclos(TasaUso tasa, string url)
        {
            DataRow dr;
            DataTable dt;
            int cont = 1;
            try
            {
                //Consultamos primero si registro la entrada del vehículo
                dr = iRepositorioProtech.selectEntradaProtech(tasa.COPLACA, tasa.COFECHA, tasa.COTERMINAL);
                if (dr != null)
                {
                    //Si registra entrada actual, verificamos si la entrada anterior está incompleta
                    dt = iRepositorioProtech.selectUltimaEntrada(tasa.COPLACA, tasa.COFECHA);
                    if (dt != null)
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            if (cont == 2)
                            {
                                Ciclo ciclo = new Ciclo
                                    (
                                        dRow["isplaca"].ToString(),
                                        Int32.Parse(dRow["isidingsal"].ToString()),
                                        Strings.Mid(dRow["isfecing"].ToString(), 1, 10),
                                        dRow["ishoring"].ToString(),
                                        Strings.Mid(dRow["isfecsal"].ToString(), 1, 10),
                                        dRow["ishorsal"].ToString()
                                    );

                                if (ciclo.ISFECSAL == "" && ciclo.ISHORSAL == "")
                                {
                                    if (CierreProtech(ciclo, tasa, url))
                                    {
                                        return;
                                    }
                                    else
                                    {
                                        CierreAutomatico(ciclo, tasa);
                                        return;
                                    }
                                }
                            }
                            cont++;
                        }
                        //Si llega a esta parte, es que no cuenta con un ciclo pendiente de cerrar
                        //Por ende se actualiza el estado
                        iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                    }
                    else
                    {
                        iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                    }
                }
                else
                {
                    //Si no registra entrada, verificamos si tiene un ciclo pendiente de cerrar
                    dt = iRepositorioProtech.selectUltimaEntrada(tasa.COPLACA, tasa.COFECHA);
                    if (dt != null)
                    {
                        //Como no registra entrada, procedemos a verificar la última entrada que registra
                        foreach(DataRow dRow in dt.Rows)
                        {
                            Ciclo ciclo = new Ciclo
                                    (
                                        dRow["isplaca"].ToString(),
                                        Int32.Parse(dRow["isidingsal"].ToString()),
                                        Strings.Mid(dRow["isfecing"].ToString(), 1, 10),
                                        dRow["ishoring"].ToString(),
                                        Strings.Mid(dRow["isfecsal"].ToString(), 1, 10),
                                        dRow["ishorsal"].ToString()
                                    );
                            if (ciclo.ISFECSAL == "" && ciclo.ISHORSAL == "")
                            {
                                //Si tiene ciclo incompleto intentamos cerrarlo automáticamente
                                if (CierreAutomatico(ciclo, tasa))
                                {
                                    //Si se logra cerrar, solo mandamos a Protech el registro de la entrada
                                    EntradaProtech(tasa, url);
                                    return;
                                }
                                else
                                {
                                    //Si no se logra realizar el cierre automático, se realiza con los datos de Protech
                                    EntradaCierreProtech(ciclo, tasa, url);
                                    return;
                                }
                            }
                            else
                            {
                                //Si no tiene ciclo pendiente, registramos solo la entrada
                                EntradaProtech(tasa, url);
                                return;
                            }
                        }
                    }
                    else
                    {
                        iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                    }
                }
            }
            catch (Exception ex)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"{ex.Message} - Primer control - Control Ciclos", "N/A", date);
            }

        }

        public Boolean CierreAutomatico(Ciclo ciclo, TasaUso tasa)
        {
            DataRow dr;
            DataTable dt;
            string fechaSalida, horaSalida, casEnt, casSal, fechaTasa, fechaIngreso, fechaCiclo, fechaActual;
            int cont = 1;
            Double tiempo;
            Boolean resultado = false;

            //Declaramos las casetas de entradas y salidas
            if (tasa.COTERMINAL == "C")
            {
                casEnt = ValuesTerminal.Entradas.EntradaCentral;
                casSal = ValuesTerminal.Salidas.SalidaCentral;
            }
            else if (tasa.COTERMINAL == "N")
            {
                casEnt = ValuesTerminal.Entradas.EntradaNorte;
                casSal = ValuesTerminal.Salidas.SalidaNorte;
            }
            else
            {
                casEnt = ValuesTerminal.Entradas.EntradaSur;
                casSal = ValuesTerminal.Salidas.SalidaSur;
            }

            try
            {
                fechaActual = DateTime.Now.ToString("MM/dd/yyyy");

                //Consultamos el tiempo a adicionar según el parametro
                dr = iRepositorioGeneral.consultarParametro("MINDADISAL");
                tiempo = Double.Parse(dr["psval"].ToString());

                dt = iRepositorioProtech.selectUltimaTasa(tasa.COPLACA, ciclo.ISFECING);
                if (dt != null)
                {
                    //Si la fecha del ciclo a cerrar es la misma que la actual, entonces se empieza a validar desde la segunda tasa y no desde la primera
                    if (DateTime.Parse(ciclo.ISFECING.ToString()) == DateTime.Parse(fechaActual))
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            if (cont >= 2)
                            {
                                //Si se encontro una tasa, verificamos que la fecha de venta de esta, sea mayor a la entrada anterior
                                fechaTasa = Strings.Mid(dRow["cofecsal"].ToString(), 1, 10) + " " + Strings.Mid(DateTime.Parse(dRow["cohorsal"].ToString()).AddMinutes(-5).ToString(), 12, 5);
                                fechaIngreso = ciclo.ISFECING + " " + Strings.Mid(ciclo.ISHORING, 12, 5);
                                if (DateTime.Parse(fechaTasa) > DateTime.Parse(fechaIngreso))
                                {
                                    fechaSalida = DateTime.Parse(dRow["cofecsal"].ToString()).ToString("MM/dd/yyyy");
                                    horaSalida = Strings.Mid(DateTime.Parse(dRow["cohorsal"].ToString()).AddMinutes(tiempo).ToString(), 12, 5);
                                    //Marcamos la salida
                                    iRepositorioProtech.updateSalida(ciclo.ISIDINGSAL, ciclo.ISPLACA, fechaSalida, horaSalida, casSal, 0);
                                    //Insertamos en el log el cierre del ciclo
                                    iRepositorioProtech.insertLogCierreCiclo("CIERRE AUTOMATICO", ciclo.ISPLACA, ciclo.ISIDINGSAL, fechaSalida);
                                    //Actualizamos el estado del proceso
                                    iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                                    resultado = true;
                                    break;
                                }
                                else
                                {
                                    //En cuyo caso no se encontro alguna tasa con la que poder cerrar el ciclo, se retorna False
                                    resultado = false;
                                }
                            }
                            cont++;
                        }
                    }
                    else
                    {
                        foreach (DataRow dRow in dt.Rows)
                        {
                            //Si se encontro una tasa, verificamos que la fecha de venta de esta, sea mayor a la entrada anterior
                            fechaTasa = Strings.Mid(dRow["cofecsal"].ToString(), 1, 10) + " " + Strings.Mid(DateTime.Parse(dRow["cohorsal"].ToString()).AddMinutes(-5).ToString(), 12, 5);
                            fechaIngreso = ciclo.ISFECING + " " + Strings.Mid(ciclo.ISHORING, 12, 5);
                            if (DateTime.Parse(fechaTasa) > DateTime.Parse(fechaIngreso))
                            {
                                fechaSalida = DateTime.Parse(dRow["cofecsal"].ToString()).ToString("MM/dd/yyyy");
                                horaSalida = Strings.Mid(DateTime.Parse(dRow["cohorsal"].ToString()).AddMinutes(tiempo).ToString(), 12, 5);
                                //Marcamos la salida
                                iRepositorioProtech.updateSalida(ciclo.ISIDINGSAL, ciclo.ISPLACA, fechaSalida, horaSalida, casSal, 0);
                                //Insertamos en el log el cierre del ciclo
                                iRepositorioProtech.insertLogCierreCiclo("CIERRE AUTOMATICO", ciclo.ISPLACA, ciclo.ISIDINGSAL, fechaSalida);
                                //Actualizamos el estado del proceso
                                iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                                resultado = true;
                                break;
                            }
                            else
                            {
                                //En cuyo caso no se encontro alguna tasa con la que poder cerrar el ciclo, se retorna False
                                resultado = false;
                            }
                        }
                    }
                }
                else
                {
                    resultado = false;
                }
            }
            catch(Exception ex)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"{ex.Message} - Cierre Automatico - Control Ciclos", "N/A", date);
            }
            return resultado;
        }

        public Boolean CierreProtech(Ciclo ciclo, TasaUso tasa, string url)
        {
            Double tiempo, tarifa;
            string fechaIngreso, horaIngreso, fechaSalida, horaSalida, casEnt, casSal, empresa;
            Boolean resultado = false;
            DataRow dr;

            //Declaramos las casetas de entradas y salidas
            if (tasa.COTERMINAL == "C")
            {
                casEnt = ValuesTerminal.Entradas.EntradaCentral;
                casSal = ValuesTerminal.Salidas.SalidaCentral;
            }
            else if (tasa.COTERMINAL == "N")
            {
                casEnt = ValuesTerminal.Entradas.EntradaNorte;
                casSal = ValuesTerminal.Salidas.SalidaNorte;
            }
            else
            {
                casEnt = ValuesTerminal.Entradas.EntradaSur;
                casSal = ValuesTerminal.Salidas.SalidaSur;
            }

            try
            {
                ResponseControlCiclos resp = EnvioDataCtrlCiclo(tasa.COPLACA, tasa.COFECHA, tasa.COTERMINAL, ValuesProtech.ProcesosProtech.CierreProtech, url);
                if (resp.Resultado == "Proceso correcto")
                {
                    fechaIngreso = Strings.Mid(DateTime.Parse(ciclo.ISFECING).ToString(), 1, 10);
                    horaIngreso = Strings.Mid(DateTime.Parse(ciclo.ISHORING).ToString(), 12, 5);
                    fechaSalida = Strings.Mid(DateTime.Parse(resp.FechaHoraSalida).ToString(), 1, 10);
                    horaSalida = Strings.Mid(DateTime.Parse(resp.FechaHoraSalida).ToString(), 12, 5);

                    //Consultamos la empresa que pertenece el vehículo
                    dr = iRepositorioGeneral.selectEmpresaVehiculo(tasa.COPLACA);
                    empresa = dr["hvcodemp"].ToString();

                    //Calculamos el tiempo de permanencia
                    //Si el cobro es mayor a 0, insertamos los registros correspondientes
                    if (resp.CobroPermanencia > 0)
                    {
                        //Buscamos el valor de la tarifa
                        dr = iRepositorioGeneral.consultarParametro("VALPARQ1");
                        tarifa = Double.Parse(dr["psval"].ToString());
                        //Calculamos el tiempo de permanencia
                        TimeSpan time = (DateTime.Parse(fechaSalida + " " + horaSalida) - DateTime.Parse(fechaIngreso + " " + horaIngreso));
                        tiempo = Double.Parse(time.TotalDays.ToString());
                        iRepositorioProtech.insertCoparqueo(tasa.COPLACA, empresa, fechaIngreso + " " + horaIngreso, fechaSalida + " " + horaSalida,
                                                            tiempo, resp.CobroPermanencia, tarifa, casEnt, tasa.COTERMINAL);
                        iRepositorioProtech.insertLogParqueadero(tasa.COPLACA, fechaIngreso, fechaSalida, resp.CobroPermanencia);
                    }
                    else
                    {
                        tiempo = 0;
                    }
                    //Cerramos el ciclo con los datos de Protech
                    iRepositorioProtech.updateSalida(ciclo.ISIDINGSAL, ciclo.ISPLACA, fechaSalida, horaSalida, casSal, tiempo);
                    //Insertamos en el log la salida
                    iRepositorioProtech.insertLogCierreCiclo("CIERRE PROTECH", tasa.COPLACA, ciclo.ISIDINGSAL, fechaSalida);
                    //Si todo se ejecuta correctamente, registramos en el log la transacción con Protech
                    iRepositorioProtech.insertLogEnvProtech(resp.Proceso, tasa.COPLACA, tasa.COFECHA, resp.JSON, resp.JSONResp, resp.Resultado, resp.Mensaje);
                    //Actualizamos el estado del proceso
                    iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                    resultado = true;
                }
                else
                {
                    iRepositorioProtech.insertLogEnvProtech(resp.Proceso, tasa.COPLACA, tasa.COFECHA, resp.JSON, resp.JSONResp, resp.Resultado, resp.Mensaje);
                    resultado = false;
                }
            }
            catch (Exception ex)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"{ex.Message} - Cierre Protech - Control Ciclos", "N/A", date);
            }
            return resultado;
        }

        public void EntradaProtech(TasaUso tasa, string url)
        {
            DataRow dr;
            string casEnt, casSal, fechaP, horaP, fecAnt;
            Double tiempo;

            dr = iRepositorioGeneral.consultarParametro("MINRESENT");
            tiempo = Double.Parse(dr["psval"].ToString());

            //Declaramos las casetas de entradas y salidas
            if (tasa.COTERMINAL == "C")
            {
                casEnt = ValuesTerminal.Entradas.EntradaCentral;
                casSal = ValuesTerminal.Salidas.SalidaCentral;
            }
            else if (tasa.COTERMINAL == "N")
            {
                casEnt = ValuesTerminal.Entradas.EntradaNorte;
                casSal = ValuesTerminal.Salidas.SalidaNorte;
            }
            else
            {
                casEnt = ValuesTerminal.Entradas.EntradaSur;
                casSal = ValuesTerminal.Salidas.SalidaSur;
            }

            try
            {

                ResponseControlCiclos resp = EnvioDataCtrlCiclo(tasa.COPLACA, tasa.COFECHA, tasa.COTERMINAL, ValuesProtech.ProcesosProtech.EntradaProtech, url);
                if (resp.Resultado == "Proceso correcto")
                {
                    //Se solicito por la terminal, que si se registra entrada, sea con un retraso de tiempo según el parametro
                    fecAnt = Strings.Format(DateTime.Parse(resp.FechaHoraIngreso).AddMinutes(-tiempo).ToString(), "MM/dd/yyyy");
                    fechaP = Strings.Mid(fecAnt, 1, 10);
                    horaP = Strings.Mid(fecAnt, 12, 5);
                    //Registramos la entrada
                    iRepositorioProtech.insertEntrada(tasa.COPLACA, fechaP, horaP, casEnt, tasa.COTERMINAL);
                    //Registramos el proceso en el log de ciclos
                    iRepositorioProtech.insertLogEntrada(tasa.COPLACA, tasa.COFECHA);
                    //Registramos en el log la transacción con Protech
                    iRepositorioProtech.insertLogEnvProtech(resp.Proceso, tasa.COPLACA, tasa.COFECHA, resp.JSON, resp.JSONResp, resp.Resultado, resp.Mensaje);
                    //Actualizamos el estado del proceso
                    iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                } 
                else if (resp.Resultado == "Error" && resp.Mensaje == "Movimiento no existe.")
                {
                    //Actualizamos el estado del proceso
                    iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                }
                else
                {
                    iRepositorioProtech.insertLogEnvProtech(resp.Proceso, tasa.COPLACA, tasa.COFECHA, resp.JSON, resp.JSONResp, resp.Resultado, resp.Mensaje);
                }


            } catch (Exception ex)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"{ex.Message} - Cierre Protech - Control Ciclos", "N/A", date);
                
            }

        }

        public void EntradaCierreProtech(Ciclo ciclo, TasaUso tasa, string url)
        {
            Double tiempo, tarifa;
            string fechaIngreso, fechaSalida, horaIngreso, 
                    horaSalida, casEnt, casSal, fec, fechaP, horaP, fecAnt, empresa;
            DataRow dr;

            //Declaramos las casetas de entradas y salidas
            if (tasa.COTERMINAL == "C")
            {
                casEnt = ValuesTerminal.Entradas.EntradaCentral;
                casSal = ValuesTerminal.Salidas.SalidaCentral;
            }
            else if (tasa.COTERMINAL == "N")
            {
                casEnt = ValuesTerminal.Entradas.EntradaNorte;
                casSal = ValuesTerminal.Salidas.SalidaNorte;
            }
            else
            {
                casEnt = ValuesTerminal.Entradas.EntradaSur;
                casSal = ValuesTerminal.Salidas.SalidaSur;
            }

            try
            {
                //Consultamos la empresa a la que pertenece el vehículo
                dr = iRepositorioGeneral.selectEmpresaVehiculo(tasa.COPLACA);
                empresa = dr["hvcodemp"].ToString();

                ResponseControlCiclos resp = EnvioDataCtrlCiclo(tasa.COPLACA, tasa.COFECHA, tasa.COTERMINAL, ValuesProtech.ProcesosProtech.EntradaCierreProtech, url);
                if (resp.Resultado == "Proceso correcto")
                {
                    fechaSalida = Strings.Mid(DateTime.Parse(resp.FechaHoraSalida).ToString(), 1, 10);
                    horaSalida = Strings.Mid(DateTime.Parse(resp.FechaHoraSalida).ToString(), 12, 5);
                    fechaIngreso = Strings.Mid(DateTime.Parse(resp.FechaHoraIngreso).ToString(), 1, 10);
                    horaIngreso = Strings.Mid(DateTime.Parse(resp.FechaHoraIngreso).ToString(), 12, 5);

                    //Consultamos el tiempo de permanencia
                    //Si el cobro es mayor a 0, insertamos los registros correspondientes
                    //Y se registra en el log el proceso
                    if (resp.CobroPermanencia > 0)
                    {
                        tarifa = 0;
                        dr = iRepositorioGeneral.consultarParametro("VALPARQ1");
                        tarifa = Double.Parse(dr["psval"].ToString());
                        //Calculamos el tiempo de la permanencia
                        TimeSpan time = DateTime.Parse(fechaSalida + " " + horaSalida) - DateTime.Parse(fechaIngreso + " " + horaIngreso);
                        tiempo = Double.Parse(time.TotalDays.ToString());
                        //Insertamos la permanencia
                        iRepositorioProtech.insertCoparqueo(tasa.COPLACA, empresa, fechaIngreso + " " + horaIngreso,
                                                            fechaSalida + " " + horaSalida, tiempo, resp.CobroPermanencia, tarifa, casEnt, tasa.COTERMINAL);
                        //Se inserta en el log el cobro de parqueadero
                        iRepositorioProtech.insertLogParqueadero(tasa.COPLACA, fechaIngreso, fechaSalida, resp.CobroPermanencia);
                    }
                    else
                    {
                        tiempo = 0;
                    }

                    //Cerramos el ciclo con los datos de Protech
                    iRepositorioProtech.updateSalida(ciclo.ISIDINGSAL, tasa.COPLACA, fechaSalida, horaSalida, casSal, tiempo);
                    //Insertamos en el log, la salida
                    iRepositorioProtech.insertLogCierreCiclo("CIERRE PROTECH", tasa.COPLACA, ciclo.ISIDINGSAL, fechaSalida);

                    //******************** ENTRADA ********************************
                    //Si todo se ejecuta correctamente, registramos la entrada
                    //Se solicitó por la terminal que si se registra entrada, sea con un retraso de 15 min
                    fecAnt = Strings.Format(DateTime.Parse(resp.FechaHoraIngreso).AddMinutes(-tiempo).ToString(), "MM/dd/yyyy");
                    fechaP = Strings.Mid(fecAnt, 1, 10);
                    horaP = Strings.Mid(fecAnt, 12, 5);
                    //Registamos la entrada
                    iRepositorioProtech.insertEntrada(tasa.COPLACA, fechaP, horaP, casEnt, tasa.COTERMINAL);
                    //Registramos la entrada en el log de ciclos
                    iRepositorioProtech.insertLogEntrada(tasa.COPLACA, tasa.COFECHA);
                    //Registramos en el log la transacción con Protech
                    iRepositorioProtech.insertLogEnvProtech(resp.Proceso, tasa.COPLACA, tasa.COFECHA, resp.JSON, resp.JSONResp, resp.Resultado, resp.Mensaje);
                    //Actualizamos el estado del proceso
                    iRepositorioProtech.updateConducesCiclos(tasa.COPLACA, tasa.CONUMERO, tasa.COFECHA);
                }
            } catch(Exception ex)
            {
                string date = DateTime.Now.ToString("MM/dd/yyyy HH:mm");
                iRepositorioProtech.insertLog($@"{ex.Message} - Cierre Protech - Control Ciclos", "N/A", date);
            }
        }
        #endregion

        #region Consumo Web
        private ResponseTasa ConsumirWSTasa(Object json, string url)
        {
            string objJson = JsonConvert.SerializeObject(json);

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var data = Encoding.UTF8.GetBytes(objJson);
            var result_post = SendRequest(new Uri(url), data, "application/json", "POST");
            ResponseTasa obj = JsonConvert.DeserializeObject<ResponseTasa>(result_post);
            return obj;
        }

        private ResponseControlCiclos EnvioDataCtrlCiclo(string placa, string fecha, string terminal, int proceso, string url)
        {
            string fec;
            int ter;

            fec = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            if (terminal == "C")
            {
                ter = ValuesProtech.TerminalesProtech.TtCentral;
            }
            else if (terminal == "N")
            {
                ter = ValuesProtech.TerminalesProtech.TtNorte;
            }
            else
            {
                ter = ValuesProtech.TerminalesProtech.TtSur;
            }

            //Creamos la clase
            CicloProtech cicloProtech = new CicloProtech(placa, fec, ter, proceso);
            //Realizamos la petición al WS
            ResponseControlCiclos response = ConsumirWsControlCiclos(cicloProtech, url);
            return response;
        }

        private ResponseControlCiclos ConsumirWsControlCiclos(Object json, string url)
        {
            string objJson = JsonConvert.SerializeObject(json);

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var data = Encoding.UTF8.GetBytes(objJson);
            var result_post = SendRequest(new Uri(url), data, "application/json", "POST");
            ResponseControlCiclos obj = JsonConvert.DeserializeObject<ResponseControlCiclos>(result_post);
            obj.JSON = objJson;
            obj.JSONResp = result_post;
            return obj;
        }

        private string SendRequest(Uri uri, byte[] jsonDataBytes, string contentType, string method)
        {
            string response;
            WebRequest request;

            request = WebRequest.Create(uri);
            request.ContentLength = jsonDataBytes.Length;
            request.ContentType = contentType;
            request.Method = method;
            request.Timeout = 10000;

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
        #endregion

    }
}
