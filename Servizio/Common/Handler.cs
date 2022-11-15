using b2a.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.Common
{
    class Handler
    {
 
        //public static string URL_WebService = "https://kw-mon-vog-b2a-ccsp.pdcloud.eu/monitoring-workrooms";
        //public static string Utente_WebService  = "B2A_MonTEST";
        //public static string Password_WebService = "b2@MonT3$t";

        public static string URL_WebService = "https://f8-monit-workr-dfe4xp.vog.it:8643/monitoring-workrooms";
        public static string Utente_WebService = "b2a"; 
        public static string Password_WebService =  "9a\"3RB!V$5+63Q\"&2;n-"; //9a"3RB!V$5+63Q"&2;n-

        public enum metodi
        {
            _GET,
            _POST,
            _PUT,
            _DELETE
        }
         

        public static string Read(HttpWebResponse httpResponse) { 
            string json = "";
            using (System.IO.Stream dataStream = httpResponse.GetResponseStream())
            { 
                StreamReader reader = new StreamReader(dataStream);
                json = reader.ReadToEnd(); 
            }

            httpResponse.Close();
            return json;
        }

        public static HttpWebResponse Send(HttpWebRequest _httpRequest, metodi metodo, string json, string errorMsg)
        {
            switch (metodo)
            {
                case metodi._GET:
                    _httpRequest.Method = "GET";
                    break;

                case metodi._POST:
                    _httpRequest.Method = "POST";
                    break;

                case metodi._DELETE:
                    _httpRequest.Method = "DELETE";
                    break;

                case metodi._PUT:
                    _httpRequest.Method = "PUT";
                    break;
                default:
                    return null; 
            }

            _httpRequest.ContentType = "application/json";
            _httpRequest.Credentials = new System.Net.NetworkCredential(Utente_WebService, Password_WebService);
            HttpWebResponse obj = null;

            if (json != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(json);
                _httpRequest.ContentLength = data.Length;
                try
                {
                    Stream stream = _httpRequest.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                    stream.Close();
                    obj = (HttpWebResponse)_httpRequest.GetResponse();
                }
                catch (WebException webEx)
                {
                    obj = (HttpWebResponse)webEx.Response;
                }
                catch (Exception Ex)
                {
                    errorMsg = Ex.ToString();
                    obj = null;
                }
            }
            else
            {
                try {
                    obj = (HttpWebResponse)_httpRequest.GetResponse();
                } catch (Exception Ex) {
                    errorMsg = Ex.ToString();
                    obj = null;  
                }
            }

            return obj;
        } 
    }
}
