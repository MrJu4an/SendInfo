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
        void updateTasa(double conumero);
        DataTable selectConducesCiclo(string fecha);
        DataRow selectEntradaProtech(string placa, string fecha, string terminal);
        DataTable selectUltimaEntrada(string placa, string fecha);
        void insertCoparqueo(string placa, string empresa, string fecini, string fecsal, Double tiempo,
                                    Double valor, Double tarifa, string entrada, string estado, 
                                    string observacion, string terminal);
        void insertLogParqueadero(string placa, string fechaEntrada, string fechaSalida, Double valor);
        void updateSalida(int id, string placa, string fecha, string hora, string casSal, Double permanencia);
        void insertLogCierreCiclo(string tipo, string placa, int idCiclo, string fechaSalida);
        void insertLogEnvProtech(int tipo, string placa, string fecha, string json, string jsonRec, string resultado, string mensaje);
        void updateConducesCiclos(string placa, Double conumero, string fecha);
        DataTable selectUltimaTasa(string placa, string fecha);
        void insertEntrada(string placa, string fecha, string hora, string casEnt, string terminal);
        void insertLogEntrada(string placa, string fecha);
        DataTable selectConducesVig(string fecha, string hora);
        DataTable selectUltEntradaTU(string placa, string fecha, string hora, string terminal);
        DataRow selectUltimaSalida(string placa, string terminal);
        DataRow selectReserva(string placa, string empresa, string fecha);
        void updateEstadoVig(double conumero, string placa, string fecha);
    }

    class RepositorioProtech : IRepositorioProtech
    {
        #region Variables

        Datas dbs;

        #endregion

        #region Constructor

        public RepositorioProtech()
        {
            dbs = new Datas();
        }

        #endregion

        #region Datos Tasa

        public DataTable consultarConduces(string fecha)
        {
            string QRY = $@"SELECT conumero, coplaca, cofecsal, cohorven, coterminal 
                            FROM coconenvp 
                            WHERE cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') AND coestado = 'PEN' ";
            return dbs.OpenData(QRY);
        }
        public void updateTasa(double conumero)
        {
            string QRY = $@"UPDATE coconenvp SET coestado = 'OK' WHERE conumero = {conumero}";
            dbs.Execute(QRY);
        }

        #endregion

        #region Control Ciclos

        public DataTable selectConducesCiclo(string fecha)
        {
            string QRY = $@"SELECT conumero, coplaca, cofecsal, cohorven, coterminal FROM coconenvp 
                            WHERE cofecsal = TO_DATE('{fecha}', 'MM/DD/YYYY') AND covalciclo = 'PEN' ";
            return dbs.OpenData(QRY);
        }

        public DataRow selectEntradaProtech(string placa, string fecha, string terminal)
        {
            string QRY = $@"SELECT isplaca FROM coingsalp 
                            WHERE isfecing >= TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND isfecsal IS NULL 
                            AND isterminal = '{terminal}'
                            AND isplaca = '{placa}' ";
            return dbs.OpenRow(QRY);
        }

        public DataTable selectUltimaEntrada(string placa, string fecha)
        {
            string QRY = $@"SELECT isplaca, isidingsal, isfecing, ishoring, isfecsal, ishorsal FROM coingsalp 
                            WHERE isfecing <= TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND isplaca = '{placa}' 
                            ORDER BY isfecing DESC, ishoring DESC 
                            FETCH FIRST 3 ROWS ONLY ";
            return dbs.OpenData(QRY);
        }

        public DataRow selectUltimaSalida(string placa, string terminal)
        {
            string QRY = $@"SELECT * FROM (
                                SELECT isfecsal, TO_CHAR(ishorsal, 'HH24:MI:SS') AS ishorsal 
                                FROM coingsalp 
                                WHERE isfecsal IS NOT NULL 
                                AND isterminal = '{terminal}' 
                                AND isplaca = '{placa}' 
                                ORDER BY isfecsal DESC, ishorsal DESC 
                            ) WHERE ROWNUM <= 1";
            return dbs.OpenRow(QRY); 
        }

        public void insertCoparqueo(string placa, string empresa, string fecini, string fecsal, Double tiempo, 
                                    Double valor, Double tarifa, string entrada, string estado, string observacion,
                                    string terminal)
        {
            string QRY = $@"INSERT INTO coparqueop 
                            (paplaca, pacodemp, pafecini, pafecsal, patiempo, pavalor, paestado, paobservacion, paregpc, patarifa, patipo, paterminal) 
                            VALUES ('{placa}', '{empresa}', TO_DATE('{fecini}','MM/DD/YYYY HH24:mi:ss'), TO_DATE('{fecsal}','MM/DD/YYYY HH24:mi:ss'),
                            {tiempo}, {valor}, '{estado}', '{observacion}', '{entrada}', {tarifa}, 'P', '{terminal}')";
            dbs.Execute(QRY);
        }

        public void updateSalida(int id, string placa, string fecha, string hora, string casSal, Double permanencia)
        {
            string QRY = $@"UPDATE coingsalp SET 
                            isfecsal = TO_DATE('{fecha}','MM/DD/YYYY') 
                            ishorsal = TO_DATE('{hora}','HH24:MI') 
                            iscassal = '{casSal}' 
                            ispermanencia = {permanencia} 
                            WHERE isfecsal IS NULL 
                            AND isplaca = '{placa}' 
                            AND isidingsal = {id} ";
            dbs.Execute(QRY);
        }

        public void updateConducesCiclos(string placa, Double conumero, string fecha)
        {
            string QRY = $@"UPDATE coconenvp SET 
                            covalciclo = 'OK' 
                            WHERE coplaca = '{placa}' 
                            AND conumero = {conumero} 
                            AND cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') ";
            dbs.Execute(QRY);
        }

        public DataTable selectUltimaTasa(string placa, string fecha)
        {
            string QRY = $@"SELECT conumero, cofecsal, cohorven, cohorsal, covalor FROM coconduc 
                            WHERE coplaca = '{placa}' 
                            AND cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') 
                            ORDER BY cofecsal DESC, cohorsal ASC 
                            FETCH FIRST 3 ROWS ONLY";
            return dbs.OpenData(QRY);
        }

        public void insertEntrada(string placa, string fecha, string hora, string casEnt, string terminal)
        {
            string QRY = $@"INSERT INTO coingsalp 
                            (isplaca, isfecing, ishoring, isingoritra, isprualc, isfuncio, isturno, 
                            iscasent, isterminal, ispermanencia, istipollegada, isrutdes, ispasent) 
                            VALUES ('{placa}', TO_DATE('{fecha}','MM/DD/YYYY'), TO_DATE('{hora}','HH24:mi:ss'),
                            'O', 'N', 'entprotech', 1, '{casEnt}', '{terminal}', 0, 'S', '00', 0) ";
            dbs.Execute(QRY);
        }

        public DataRow selectReserva(string placa, string empresa, string fecha)
        {
            string QRY = $@"SELECT vrfecha 
                                FROM covehreserva 
                                WHERE vrfecha = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                AND vrcodemp = '{empresa}' 
                                AND vrplaca = '{placa}' 
                                AND vrestado = 'A' ";
            return dbs.OpenRow(QRY);
        }

        #endregion

        #region Control Vigencias

        public DataTable selectConducesVig(string fecha, string hora)
        {
            string QRY = $@"SELECT conumero, coplaca, cofecsal, cohorven, cohorsal, coterminal FROM coconenvp 
                            WHERE cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') 
                            AND cohorsal <= TO_DATE('{hora}','HH24:mi') 
                            AND covalvig = 'PEN' ";
            return dbs.OpenData(QRY);
        }

        public DataTable selectUltEntradaTU(string placa, string fecha, string hora, string terminal)
        {
            string QRY = $@"SELECT isplaca, isidingsal, isfecing, ishoring, isfecsal, ishorsal FROM coingsalp 
                            WHERE isplaca = '{placa}' 
                            AND isfecing = TO_DATE('{fecha}','MM/DD/YYYY') 
                            AND ishoring <= TO_DATE('{hora}','HH24:mi') 
                            AND isterminal = '{terminal}' 
                            ORDER BY isfecing DESC, ishoring DESC ";
            return dbs.OpenData(QRY);
        }

        public void updateEstadoVig(double conumero, string placa, string fecha)
        {
            string QRY = $@"UPDATE coconenvp SET 
                            covalvig = 'OK' 
                            WHERE conumero = {conumero} 
                            AND coplaca = {placa} 
                            AND cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') ";
            dbs.Execute(QRY);
        }

        #endregion

        #region General
        public void insertLog(string msg, string value, string fecha)
        {
            string QRY = $@"INSERT INTO logprotech 
                            (lotipo, lovalor, lodescripcion, lofecha) 
                            VALUES
                            ('S', '{value}', '{msg}', TO_DATE('{fecha}','MM/DD/YYYY HH24:mi') )";
            dbs.Execute(QRY);
        }

        public void insertLogParqueadero(string placa, string fechaEntrada, string fechaSalida, Double valor)
        {
            string QRY = $@"INSERT INTO logcierreciclos 
                            (lotipo, loplaca, lofecing, lofecsal, lovalorpar) 
                            VALUES ('PARQUEADERO', '{placa}', TO_DATE('{fechaEntrada}','MM/DD/YYYY'), 
                            TO_DATE('{fechaSalida}','MM/DD/YYYY'), {valor}) ";
            dbs.Execute(QRY);
        }

        public void insertLogCierreCiclo(string tipo, string placa, int idCiclo, string fechaSalida)
        {
            string QRY = $@"INSERT INTO logcierreciclos 
                            (lotipo, loplaca, loidciclo, lofecsal) 
                            VALUES('{tipo}', '{placa}', {idCiclo}, TO_DATE('{fechaSalida}','MM/DD/YYYY') )";
            dbs.Execute(QRY);
        }

        public void insertLogEnvProtech(int tipo, string placa, string fecha, string json, string jsonRec, string resultado, string mensaje)
        {
            string QRY = $@"INSERT INTO logenvprotech 
                            (lotipo, loplaca, lofecha, lojsonenv, lojsonrec, loresultado) 
                            VALUES ({tipo}, '{placa}', TO_DATE('{fecha}','MM/DD/YYYY HH24:mi'), '{json}', '{jsonRec}', {resultado} / {mensaje}) ";
            dbs.Execute(QRY);
        }

        public void insertLogEntrada(string placa, string fecha)
        {
            string QRY = $@"INSERT INTO logcierreciclos 
                            (lotipo, loplaca, lofecing) 
                            VALUES ('ENTRADA PROTECH', '{placa}', TO_DATE('{fecha}','MM/DD/YYYY')) ";
            dbs.Execute(QRY);
        }

        #endregion

    }
}
