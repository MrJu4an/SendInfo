using System;
using System.IO;
using System.Text;
using System.Security.Cryptography;

namespace Utilidades
{
    public class Seguridad
    {
        private TripleDESCryptoServiceProvider ProvCripto = new TripleDESCryptoServiceProvider();
        private MemoryStream MemoStr = new MemoryStream();
        private CryptoStream CrypCoder;
        private byte[] ByteText;
        /// <summary>
        ///         ''' Contructor de la clase
        ///         ''' </summary>
        public Seguridad()
        {
        }

        public Seguridad(string Key)
        {
            ProvCripto.Key = ComputeHash(Key, ProvCripto.KeySize / (int)8);
            ProvCripto.IV = ComputeHash("", ProvCripto.BlockSize / (int)8);
        }
        /// <summary>
        ///         ''' Metodo private que calcula y tranforma el hash de la llave proporcionada
        ///         ''' </summary>
        ///         ''' <param name="Key">Llave de trabajo</param>
        ///         ''' <param name="Longitud">Longitud del hash</param>
        ///         ''' <returns>Array de Bytes con el hash calculado de la llave</returns>
        ///         ''' <remarks></remarks>
        private byte[] ComputeHash(string Key, int Longitud)
        {
            try
            {
                byte[] KB = Encoding.Unicode.GetBytes(Key);
                byte[] HR = new SHA1CryptoServiceProvider().ComputeHash(KB);
                var oldHR = HR;
                HR = new byte[Longitud - 1 + 1];
                if (oldHR != null)
                    Array.Copy(oldHR, HR, Math.Min(Longitud - 1 + 1, oldHR.Length));
                return HR;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        ///         ''' Encripta los datos suministrados
        ///         ''' </summary>
        ///         ''' <param name="DataInput">Datos a encriptar</param>
        ///         ''' <returns>String con la cadena de datos encriptados</returns>
        ///         ''' <remarks></remarks>
        public string Encriptar(string DataInput)
        {
            try
            {
                ByteText = Encoding.Unicode.GetBytes(DataInput);
                CrypCoder = new CryptoStream(MemoStr, ProvCripto.CreateEncryptor(), CryptoStreamMode.Write);
                CrypCoder.Write(ByteText, 0, ByteText.Length);
                CrypCoder.FlushFinalBlock();
                return Convert.ToBase64String(MemoStr.ToArray());
            }
            catch (Exception ex)
            {
                // MensajeBox(".", MsgBoxStyle.Exclamation, "Advertencia")
                throw new Exception("No se puede encriptar la data proporcionada");
                return string.Empty;
            }
        }

        /// <summary>
        ///         ''' Desencipta la informacion suministrada
        ///         ''' </summary>
        ///         ''' <param name="DataInput">Datos encriptados que se pretenden procesar</param>
        ///         ''' <returns>String con los datos procesados y desencriptados</returns>
        ///         ''' <remarks></remarks>
        public string Desencriptar(string DataInput)
        {
            try
            {
                ByteText = Convert.FromBase64String(DataInput);
                CrypCoder = new CryptoStream(MemoStr, ProvCripto.CreateDecryptor(), CryptoStreamMode.Write);
                CrypCoder.Write(ByteText, 0, ByteText.Length);
                CrypCoder.FlushFinalBlock();
                return Encoding.Unicode.GetString(MemoStr.ToArray());
            }
            catch (Exception ex)
            {
                // MensajeBox("La llave o los datos proporcionados no son correctos.", MsgBoxStyle.Exclamation, "Advertencia")
                throw new Exception("La llave o los datos proporcionados no son correctos");
                return string.Empty;
            }
        }
    }
}
