using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Configuration;
using System.Data.OracleClient;
using System.Web.Configuration;
using System.Data;
using Utilidades;
using Oracle.ManagedDataAccess;

public class Datas
{
    // Variables de administracion de las conexiones a la base de datos
    private string _ConectionString;
    private OracleConnection Conexion;
    private OracleCommand Comando;
    private OracleTransaction Transaccion = null/* TODO Change to default(_) if this is not a reference type */;
    private object[] Parametros = null;
    private OracleParameterCollection _Parameters = null/* TODO Change to default(_) if this is not a reference type */;
    private bool _WriteLog = false;
    public string QRY = "";
    const string CKFEDObjectNameFileOrWebSystemTrick = "$E&u4!)A)c%3C_FZ";
    /// <summary>
    ///     ''' Crea una nueva instacia de la clase de conexion.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    public Datas()
    {
        try
        {
            // _ConectionString = ConfigurationManager.ConnectionStrings("ConectionString").ConnectionString
            _ConectionString = Cadena();
        }
        catch (Exception ex)
        {
        }
        finally
        {
        }
    }

    public string Cadena()
    {
        try
        {
            var encriptacion = new Seguridad(CKFEDObjectNameFileOrWebSystemTrick);

            ConnectionStringSettings ConfigStr = ConfigurationManager.ConnectionStrings["ConnectionString"];
            // Llenamos el combo

            try
            {
                //return encriptacion.Desencriptar(ConfigStr.ConnectionString);
                return(ConfigStr.ConnectionString);
            }
            catch (Exception ex)
            {
                return "";
            }

            try
            {
                // La pirmera vez no se decripta
                string CadenaFinal = ConfigurationManager.ConnectionStrings["ConectionString"].ConnectionString;
                return encriptacion.Desencriptar(CadenaFinal);
            }
            catch (Exception ex)
            {
                return ConfigurationManager.ConnectionStrings["ConnectionString"].ToString();
            }
        }
        catch (Exception exWeb)
        {
            return string.Empty;
        }
    }
    /// <summary>
    ///     ''' Metodo que Conecta el Aplicativo de la Base de Datos.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void Conectar()
    {
        // If Me._ConectionString = String.Empty Then
        // _log.RegistrarLog("AccesoDatos", "Consultas", "Consultas", "No ha especificado una cadena de conexión válida o no se encuentra app.config.")
        // Exit Sub
        // End If

        // If Not Me.Conexion Is Nothing Then
        // If Me.Conexion.State.Equals(ConnectionState.Open) Then
        // _log.RegistrarLog("AccesoDatos", "Consultas", "Consultas", "No se puede abrir la conexión. Ya se encuentra abierta.")
        // Exit Sub
        // End If
        // End If

        try
        {
            if (this.Conexion == null)
            {
                this.Conexion = new OracleConnection();
                this.Conexion.ConnectionString = this._ConectionString;
            }
            else
                this.Conexion.Close();
                this.Conexion.ConnectionString = this._ConectionString;
                Conexion.Open();

            // Despues de conectar el motor llamamos este proceso y alteramos la sesion
            SessionAltered();
        }
        catch (Exception ex)
        {
        }
    }

    /// <summary>
    ///     ''' Metodo que Desconeta el Aplicativo de la Base de Datos.
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void Desconectar()
    {
        if (this.Conexion == null)
        {
            if (this.Conexion.State.Equals(ConnectionState.Open))
            {
                this.Conexion.Close();
                this.Conexion.Dispose();
                this.Conexion = null;
            }
        }
    }

    /// <summary>
    ///     ''' Crea un comando de Ejecucion de la Base de Datos
    ///     ''' </summary>
    ///     ''' <param name="SQL">Sentencia SQL a ejecutar en el servidor.</param>
    ///     ''' <remarks></remarks>
    private void CrearComando(string SQL)
    {
        this.Comando = new OracleCommand();
        this.Comando.Connection = this.Conexion;
        this.Comando.CommandType = CommandType.Text;
        this.Comando.CommandText = SQL;

        // Verificamos si entra como transaccion
        if (this.Transaccion == null)
            this.Comando.Transaction = this.Transaccion;
    }

    /// <summary>
    ///     ''' Crear un comando de Ejecucion de la base de Datos.
    ///     ''' </summary>
    ///     ''' <param name="SP">Nombre del Procedimiento Almacenado.</param>
    ///     ''' <remarks></remarks>
    private void CrearComandoSP(string SP)
    {
        this.Comando = new OracleCommand();
        this.Comando.Connection = this.Conexion;
        this.Comando.CommandText = SP;
        this.Comando.CommandType = CommandType.StoredProcedure;

        // Verificamos si entra como transaccion
        if (this.Transaccion == null)
            this.Comando.Transaction = this.Transaccion;
    }

    /// <summary>
    ///     ''' Altera la session de conexion al motor para la mascara de fecha, lenguaje y formato de numeros. (Inicialmente Oracle)
    ///     ''' </summary>
    ///     ''' <remarks></remarks>
    private void SessionAltered()
    {
        // Despues de que se conecta, el sistema debe cambiar la entrada de fechas a oracle, la cambiamos aca.
        // Si hay mas condiciones en la inicializacion para los motores, se debe hacer aca
        this.Comando = new OracleCommand();
        this.Comando.Connection = this.Conexion;
        this.Comando.CommandType = CommandType.Text;
        this.Comando.CommandText = "ALTER SESSION SET NLS_DATE_FORMAT = 'DDMMYYYY HH24:MI:SS'";
        this.Comando.ExecuteNonQuery();
        this.Comando.CommandText = "ALTER SESSION SET NLS_LANGUAGE=SPANISH";
        this.Comando.ExecuteNonQuery();
    }

    /// <summary>
    ///     ''' Metodo que ejecuta las sentecias a la base de datos.
    ///     ''' </summary>
    ///     ''' <param name="SQL">Cadena SQL a ejecutar</param>
    ///     ''' <returns>Cantidad de Datos Afectados</returns>
    ///     ''' <remarks></remarks>
    private int Ejecutar(string SQL)
    {
        try
        {
            // Conectamos a la base de datos siempre y cuando no este en transacción
            if (this.Transaccion == null)
                Conectar();
            // Creamos el comando para ejecutar
            CrearComando(SQL);
            // Recorremos los parametros para la ejecucion
            if (Parametros != null)
            {
                for (var i = 0; i <= Parametros.Length - 1; i++)
                    this.Comando.Parameters.Add((OracleParameter)Parametros[i]);
            }

            // Ejecutamos la sentencia
            return this.Comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            return 1;
        }
        // _log.RegistrarLog("AccesoDatos", "Consultas", "Consultas", ex.Message)
        finally
        {
            if (this.Transaccion == null)
                Desconectar();
            if (Parametros != null)
            {
                _Parameters = this.Comando.Parameters;
                Parametros = null;
            }
            else
                _Parameters = null;
        }
    }

    /// <summary>
    ///     ''' Metodo para ejecutar una consulta que retorna datos.
    ///     ''' </summary>
    ///     ''' <param name="SQL"></param>
    ///     ''' <returns></returns>
    ///     ''' <remarks></remarks>
    private DataTable ObtenerDatos(string SQL)
    {
        OracleDataAdapter Adapter = new OracleDataAdapter();
        DataTable Dta = new DataTable();

        try
        {
            // Conectamos a la base de datos siempre y cuando no este en transacción
            if (this.Transaccion == null)
                Conectar();
            // Creamos el comando para ejecutar
            CrearComando(SQL);
            // Establecemos el adaptador
            Adapter.SelectCommand = this.Comando;
            // Llenamos el datatable
            Adapter.Fill(Dta);
            // Retornamos el valor
            if (Dta.Rows.Count > 0)
                return Dta;
            else
                return null/* TODO Change to default(_) if this is not a reference type */;
        }
        catch (Exception ex)
        {
            // _log.RegistrarLog("AccesoDatos", "Consultas", "Consultas", ex.Message)
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
        finally
        {
            if (this.Transaccion == null)
                Desconectar();
        }
    }

    /// <summary>
    ///     ''' Metodo que ejecuta una funcion que retorna un scalar.
    ///     ''' </summary>
    ///     ''' <param name="SQL">Cadena SQL a ejecutar</param>
    ///     ''' <returns>Scalar resultado de la operacion.</returns>
    ///     ''' <remarks></remarks>
    private object Escalar(string SQL)
    {
        try
        {
            // Conectamos a la base de datos siempre y cuando no este en transacción
            if (this.Transaccion == null)
                Conectar();
            // Creamos el comando para ejecutar
            CrearComando(SQL);
            // Ejecutamos la sentencia
            return this.Comando.ExecuteScalar();
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
            return null;
        }
        finally
        {
            if (this.Transaccion == null)
                Desconectar();
        }
    }
    /// <summary>
    ///     ''' Obtiene los datos de la ejecución de una sentencia SQL. Utilice esta función cuando se ha de ejecutar sentencias de consulta de datos
    ///     ''' </summary>
    ///     ''' <param name="SQL">Sentencia SQL que se va a Ejecutar. Sentencias de tipo [SELECT]</param>
    ///     ''' <returns>Retorna los datos obtenidos en la consulta mediante un DataTable</returns>
    ///     ''' <remarks></remarks>
    public DataTable OpenData(string SQL)
    {
        // Verificamos que la sentencia tenga algo
        if (SQL.Trim() == string.Empty)
            return null/* TODO Change to default(_) if this is not a reference type */;
        else
            return ObtenerDatos(SQL);
    }

    /// <summary>
    ///     ''' Devuelve los datos resultado de una consulta SQL. 
    ///     ''' </summary>
    ///     ''' <param name="SQL">Sentencia SQL que se va a Ejecutar. Sentencias de tipo [SELECT]</param>
    ///     ''' <returns>Devuelve el dato especificado sin abstracción</returns>
    ///     ''' <remarks></remarks>
    public DataRow OpenRow(string SQL)
    {
        DataTable Resulta = new DataTable();
        DataRow Fila;
        try
        {
            if (SQL.Trim() == string.Empty)
                return null/* TODO Change to default(_) if this is not a reference type */;
            else
            {
                Resulta = OpenData(SQL);
                Fila = Resulta.Rows[0];

                return Fila;
            }
        }
        catch (Exception ex)
        {
            return null/* TODO Change to default(_) if this is not a reference type */;
        }
    }

    /// <summary>
    ///     ''' Ejecuta la sentencia SQL especificada. Utilice esta función cuando se ha de ejecutar sentencias de modificacion de datos.
    ///     ''' </summary>
    ///     ''' <param name="SQL">Sentencia SQL que se va a Ejecutar. Sentencias de tipo [UPDATE, INSERT, DELETE]</param>
    ///     ''' <remarks></remarks>
    public int Execute(string SQL)
    {
        return Ejecutar(SQL);
    }

    /// <summary>
    ///     ''' Ejecuta una sentencia SQL especificada. Utilice esta función cuando se ejecutan sentencias que retornan scalar
    ///     ''' </summary>
    ///     ''' <param name="SQL">Sentencia SQL que se va a Ejecutar. Sentencias de tipo [SELECT, UPDATE, INSERT, DELETE]</param>
    ///     ''' <returns>Retorna el valor capturado por el scalar.</returns>
    ///     ''' <remarks></remarks>
    public object Scalar(string SQL)
    {
        return Escalar(SQL);
    }
}
