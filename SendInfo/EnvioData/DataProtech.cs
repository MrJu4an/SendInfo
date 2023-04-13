using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SendInfo.Modelos;
using SendInfo.Servicios;
using Newtonsoft.Json;

namespace SendInfo.EnvioData
{
    class DataProtech
    {
        RepositorioProtech iRepositorioProtech;
        public DataProtech()
        {
            iRepositorioProtech = new RepositorioProtech();
        }
        public void EnvioDataTasa(TasaUso tasa)
        {

        }

        public ResponseTasa consumirWSTasa(Object json, string url)
        {
            var objJson = JsonConvert.SerializeObject(json);

            System.Net.ServicePointManager.SecurityProtocol = System.Net.SecurityProtocolType.Tls12;
            var data = Encoding.UTF8.GetBytes(objJson);
            //var result_post = 
        }
    }
}
