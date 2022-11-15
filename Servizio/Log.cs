using b2a.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG
{

    class Log
     {

        public enum resultImport
        {
            success,
            successWithError,
            NoSuccess
        }



        public enum tipoErroreImport
        {
            noErrore,
            aperturaFile,
            decodificaNomeFile,
            letturaCampiXLS,
            saltataQualcheRiga,
            noCalibratice,
            noBarcode
        }

        public  string getDescrizioneEsito ()
        {
            string result = "";

            switch (esito)
            {
                case resultImport.success:
                    result = "COMPLETATO CON SUCCESSO";
                    break;
                case resultImport.NoSuccess:
                    result = "NON COMPLETATO";
                    break;
                case resultImport.successWithError:
                    result = "COMPLETATO CON ERRORI";
                    break;
                default:
                    result = "";
                break;
            }

            return result;

        }




        public string msg;
        public tipoErroreImport error;
        public resultImport esito;

            

        public Log()
        {
            msg = "";
            error = tipoErroreImport.noErrore;
            esito = resultImport.success;
        }

        public string insertLog(int idMagazzino, int idCalibratrice, string nomefile,SqlSrv_Interface oDB )
        {
            string result = "";
            StoredProcedure mysp = new StoredProcedure("Agente_VOG_Import_LogsIns");
            mysp.parametro("@idMagazzino", idMagazzino);
            mysp.parametro("@idCalibratrice", idCalibratrice);
            mysp.parametro("@nomefile", nomefile);
            mysp.parametro("@logFileImport", msg);
            mysp.parametro("@logEsito", getDescrizioneEsito());


            if (!oDB.ExecuteSql(mysp))
                result = "myLog.errore nell'inserimento log " + nomefile;

            return result;
        }

    }
}
