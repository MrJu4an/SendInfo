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
        DataTable selectConducesEmpresa(string codEmp, string fecha, string origen, string caja, int conMin, int conMax, string influe);
        DataRow selectConducesCreditoEmpresa(string codEmp, string fecha, string origen, string caja, int conMin, int conMax, string influe);
        DataTable selectAlcoholEmpresa(string codEmp, string fecha, string caja, int conMin, int conMax);
        DataRow selectAlcoholCreditoEmpresa(string codEmp, string fecha, string caja, int conMin, int conMax);
        DataRow selectPermanenciasEmpresa(string codEmp, string fecha, string caja, string placa = "");
        DataRow selectMultaEmpresa(string codEmp, string fecha, string caja, int concepto, string placa = "");
        DataRow selectVigenciaEmpresa(string codEmp, string fecha, string caja, string placa = "");
        DataRow selectTagEmpresa(string codEmp, string fecha, string caja, string placa = "");
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
                                    AND abestado = 'Activo' 
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
                                AND abestado = 'Activo' 
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

        public DataTable selectConducesEmpresa(string codEmp, string fecha, string origen, string caja, int conMin, int conMax, string influe)
        {
            string QRY = $@"SELECT NVL(SUM(covalor), 0) AS TotalConduces, comodo 
                            FROM coconduc 
                            INNER JOIN opruta 
                            ON coconduc.cocodrut = opruta.rucodigo 
                            WHERE conumero BETWEEN {conMin} 
                            AND {conMax} 
                            AND cofecsal = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND cooritra IN ('{origen}') 
                            AND cocodemp = '{codEmp}' 
                            AND conomcaj = '{caja}' 
                            AND cocerrado = 'S' 
                            AND coestado = 'Activo' 
                            AND cocredit = 'N' 
                            AND ruinflue = '{influe}' 
                            GROUP BY comodo ";
            return dbs.OpenData(QRY);
        }

        public DataRow selectConducesCreditoEmpresa(string codEmp, string fecha, string origen, string caja, int conMin, int conMax, string influe)
        {
            string QRY = $@"SELECT NVL(SUM(covalor), 0) AS TotalConduces 
                            FROM coconduc 
                            INNER JOIN opruta 
                            ON coconduc.cocodrut = opruta.rucodigo 
                            WHERE conumero BETWEEN {conMin} 
                            AND {conMax} 
                            AND cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') 
                            AND cooritra IN ('{origen}') 
                            AND cocodemp = '{codEmp}' 
                            AND conomcaj = '{caja}' 
                            AND cocerrado = 'S' 
                            AND coestado = 'Activo' 
                            AND cocredit = 'S' 
                            AND ruinflue = '{influe}' ";
            return dbs.OpenRow(QRY);
        }

        public DataTable selectAlcoholEmpresa(string codEmp, string fecha, string caja, int conMin, int conMax)
        {
            string QRY = $@"SELECT NVL(SUM(covalalc), 0) AS TotalAlcohol, comodo 
                            FROM coconduc 
                            WHERE conumero BETWEEN {conMin} 
                            AND {conMax} 
                            AND cofecsal = TO_DATE('{fecha}','MM/DD/YYYY') 
                            AND cooritra IN ('T', 'O') 
                            AND cocodemp = '{codEmp}' 
                            AND conomcaj = '{caja}' 
                            AND cocerrado = 'S' 
                            AND coestado = 'Activo' 
                            GROUP BY comodo ";
            return dbs.OpenData(QRY);
        }

        public DataRow selectAlcoholCreditoEmpresa(string codEmp, string fecha, string caja, int conMin, int conMax)
        {
            string QRY = $@"SELECT NVL(SUM(covalalc), 0) AS TotalAlcohol 
                            FROM coconduc 
                            WHERE conumero BETWEEN {conMin} 
                            AND {conMax} 
                            AND cofecsal = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND cooritra IN ('T', 'O') 
                            AND cocodemp = '{codEmp}' 
                            AND conomcaj = '{caja}' 
                            AND cocerrado = 'S' 
                            AND coestado = 'Activo' 
                            AND cocredit = 'S' ";
            return dbs.OpenRow(QRY);
        }

        public DataRow selectPermanenciasEmpresa(string codEmp, string fecha, string caja, string placa = "")
        {
            string QRY = $@"SELECT NVL(SUM(pavalor), 0) AS TotalPermanencia 
                            FROM coparqueo 
                            WHERE pafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND pacodemp = '{codEmp}' 
                            AND pacaja = '{caja}' 
                            AND patipo = 'P' 
                            AND paestado = 'C' 
                            AND pacerrado = 'S' ";

            if (placa != "")
            {
                QRY += $@" AND paplaca = '{placa}' ";
            }
            return dbs.OpenRow(QRY);
        }

        public DataRow selectMultaEmpresa(string codEmp, string fecha, string caja, int concepto, string placa = "")
        {
            string QRY = $@"SELECT NVL(SUM(abvalor), 0) AS TotalMulta 
                            FROM opabonos, opmultas 
                            WHERE mucodemp = '{codEmp}' 
                            AND abfecha = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND munumero = abnumero 
                            AND abnomcaj = '{caja}' 
                            AND abcerrado = 'S' 
                            AND abestado = 'Activo' 
                            AND mucodinf = {concepto} ";

            if (placa != "")
            {
                QRY += $@" and munumpla = '{placa}' ";
            }
            return dbs.OpenRow(QRY);
        }

        public DataRow selectVigenciaEmpresa(string codEmp, string fecha, string caja, string placa = "")
        {
            string QRY = $@"SELECT NVL(SUM(pavalor), 0) AS TotalVigencia 
                            FROM coparqueo 
                            WHERE pafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND pacerrado = 'S' 
                            AND patipo = 'V' 
                            AND pacaja = '{caja}' 
                            AND paestado = 'C' 
                            AND pacodemp = '{codEmp}' ";

            if (placa != "")
            {
                QRY += $@" AND paplaca = '{placa}' ";
            }
            return dbs.OpenRow(QRY);
        }

        public DataRow selectTagEmpresa(string codEmp, string fecha, string caja, string placa = "")
        {
            string QRY = $@"SELECT NVL(SUM(tavalor), 0) AS TotalTag 
                            FROM optag 
                            WHERE tafecpag = TO_DATE('{fecha}', 'MM/DD/YYYY') 
                            AND tacodemp = '{codEmp}' 
                            AND tanomcaj = '{caja}' ";

            if (placa != "")
            {
                QRY += $@" AND taplaca = '{placa}' ";
            }
            return dbs.OpenRow(QRY);
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
