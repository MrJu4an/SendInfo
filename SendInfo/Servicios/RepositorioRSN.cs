using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace SendInfo.Servicios
{
    public interface IRepositorioRSN
    {
        DataRow selectEmpresa(string codEmp);
        DataRow selectEmpresaConvenio(string codEmp);
        DataRow selectEmpresaContratante(string codEmp, string placa);
        DataTable consultarCierres(string fecha);
        DataTable consultarEmpresas(string fecha, string caja);
        DataTable consultarEmpresasConvenio(string fecha, string caja, string codEmp);
        int insertLogRSN(string msg, string valor, string fecha);
    }

    class RepositorioRSN : IRepositorioRSN
    {
        #region Variables
        Datas dbs;
        #endregion

        #region Constructor
        public RepositorioRSN()
        {
            dbs = new Datas();
        }
        #endregion

        #region Recolección de Información


        public DataRow selectEmpresa(string codEmp)
        {
            string QRY = $@"SELECT henombre, henit FROM ophojemp 
                            WHERE hecodigo = '{codEmp}' ";
            return dbs.OpenRow(QRY);
        }

        public DataRow selectEmpresaConvenio(string codEmp)
        {
            string QRY = $@"SELECT henombre, henit FROM ophojempcon 
                            WHERE hecodigo = '{codEmp}' ";
            return dbs.OpenRow(QRY);
        }

        public DataRow selectEmpresaContratante(string codEmp, string placa)
        {
            string QRY = $@"SELECT hvempcon, henit 
                            FROM ophojveh, ophojemp 
                            WHERE hvplaca = '{placa}' 
                            AND hvcodemp = '{codEmp}' 
                            AND hvempcon = hecodigo ";
            return dbs.OpenRow(QRY);
        }

        public DataTable consultarCierres(string fecha)
        {
            string QRY = $@"SELECT cifecha, MIN(cinumini) AS cinumini, MAX(cinumfin) AS cinumfin, cinomcaj 
                            FROM cocietur 
                            WHERE cifecha = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND ciciecaj = 'S' 
                            AND cinumini > 0 
                            AND cinumfin > 0
                            GROUP BY cifecha, cinomcaj ";
            return dbs.OpenData(QRY);
        }

        public DataTable consultarEmpresas(string fecha, string caja)
        {
            string QRY = $@"SELECT DISTINCT(cocodemp) AS cocodemp, SUM(covalor) AS covalor, SUM(CantTasas) AS CantTasas 
                            FROM (
                                    SELECT DISTINCT(cocodemp) AS cocodemp, SUM(covalor) AS covalor, COUNT(conumero) AS CantTasas 
                                    FROM coconduc 
                                    INNER JOIN ophojemp ON cocodemp = hecodigo 
                                    WHERE cofecsal = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                    AND conomcaj = '{caja}' 
                                    AND cocerrado = 'S' 
                                    AND coestado = 'Activo' 
                                    GROUP BY cocodemp, henombre 
                                UNION 
                                    SELECT DISTINCT(mucodemp) AS cocodemp, 0 AS covalor, 0 AS CantTasas 
                                    FROM opabonos, opmultas 
                                    WHERE abfecha = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                    AND munumero = abnumero 
                                    AND abnomcaj = '{caja}' 
                                    AND abcerrado = 'S' 
                                    AND mucodinf IN (51, 52, 53, 54) 
                                UNION
                                    SELECT DISTINCT(pacodemp) AS cocodemp, 0 AS covalor, 0 AS CantTasas 
                                    FROM coparqueo 
                                    WHERE pafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                    AND pacaja = '{caja}' 
                                    AND patipo = 'P' 
                                UNION
                                    SELECT DISTINCT(tacodemp) AS cocodemp, 0 AS covalor, 0 AS CantTasas 
                                    FROM optag 
                                    WHERE tafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                    AND tanomcaj = '{caja}' 
                            ) GROUP BY cocodemp ";
            return dbs.OpenData(QRY);
        }

        public DataTable consultarEmpresasConvenio(string fecha, string caja, string codEmp)
        {
            string QRY = $@"SELECT DISTINCT(cocodemp) AS cocodemp, placa AS placa FROM( 
                                SELECT DISTINCT(mucodemp) AS cocodemp, munumpla AS placa FROM opabonos, opmultas 
                                WHERE abfecha = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                AND munumero = abnumero 
                                AND abcerrado = 'S' AND mucodinf IN (51, 52, 53, 54) 
                                AND mucodemp = '{codEmp}' 
                                AND abnomcaj = '{caja}' 
                                AND abcerrado = 'S' 
                            UNION 
                                SELECT DISTINCT(pacodemp) AS cocodemp, paplaca AS placa, FROM coparqueo 
                                WHERE pafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                AND patipo = 'P' 
                                AND pacaja = '{caja}' 
                                AND pacodemp = '{codEmp}' 
                            UNION 
                                SELECT DISTINCT(tacodemp) AS cocodemp, taplaca AS placa FROM optag 
                                WHERE tafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                                AND tacodemp = '{codEmp}' 
                            ) GROUP BY cocodemp, placa ";
            return dbs.OpenData(QRY);
        }
        #endregion

        #region General

        public int insertLogRSN(string msg, string valor, string fecha)
        {
            string QRY = $@"INSERT INTO logrsn (lotipo, lovalor, lodescripcion, lofecha) 
                            VALUES ('C' '{valor}', '{msg}', TO_DATE('{fecha}','MM/DD/YYYY') )";
            return dbs.Execute(QRY);
        }

        #endregion
    }
}
