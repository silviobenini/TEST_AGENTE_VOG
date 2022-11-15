using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Agente_VOG.SottoServizi
{
    class Biometic : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        int idMagazzino = 0;
        int idCalibratrice = 0;
        private Log myLog = new Log();

        public Biometic(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "Biometic";
            _descrizioneLunga = "Biometic";
        }

        protected override void inizializzazioneCiclo()
        {
        }

        protected override void finalizzazioneCiclo()
        {
             
        }

        protected override void cicloEsecuzione()
        {
            scriviMsg("Ciclo in polling", livelloLog.prolisso);

            int ora = DateTime.Now.Hour;
            int minuti = DateTime.Now.Minute;

            // gira solo  alle 6.30 e alle 23:15

            switch (ora)
            {
                case 6:
                    switch (minuti)
                    {
                        case 30:
                            varietaBiometic();
                            calibraturaBiometic();
                            aggiornaTabella_import_DatiLavBio();
                            break;
                    }
                    break;
                case 23:
                    switch (minuti)
                    {
                        case 15:
                            varietaBiometic();
                            calibraturaBiometic();
                            aggiornaTabella_import_DatiLavBio();
                            break;
                    }
                    break;
                default:
                    scriviMsg("Ciclo in polling - Saltato per orario non previsto !", livelloLog.prolisso);
                    break;
            }



        }

        private void varietaBiometic()
        {
            DateTime dBiometic;
            dBiometic = dataUltimoAggiornamento("dBiometicVarieta");
            dBiometic = dBiometic.AddSeconds(1);

           
            DateTime dataNow = DateTime.Now;

            string urlFinale = "/deliveryvariety?lastupdatefrom=" + dBiometic.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.clsBiometic.ListaVarieta_Biometic _lista_biometic = new Common.clsBiometic.ListaVarieta_Biometic();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_biometic = JsonConvert.DeserializeObject<Common.clsBiometic.ListaVarieta_Biometic>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Biometic");
            }

            DateTime lastUpdateTime = DateTime.Now;

            if (_lista_biometic.data.Count == 0)
            {
                return;
            }

            _lista_biometic.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsBiometic.clsVarietaBiometic varieta in _lista_biometic.data)
            {
                

                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Biometic_Varieta_InsMod");
                mysp.parametro("@id", varieta.id);
                mysp.parametro("@code", varieta.code);
                mysp.parametro("@nameDE", varieta.nameDe);
                mysp.parametro("@nameIT", varieta.nameIT);
                mysp.parametro("@item", varieta.item);
                mysp.parametro("@locked", varieta.locked);
                mysp.parametro("@lastUpdateTime", varieta.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("BIOMETIC -  " + varieta.code + varieta.nameDe);
                    scriviMsg("Inserita Varieta Biometic (id/code)" + varieta.id + "/" + varieta.code);
                    lastUpdateTime = varieta.lastUpdateTime;

                }
            }

            oDB.ParamDB.scrivi(NOME_MODULO, "dBiometicVarieta", lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");

        }

     

  private void aggiornaTabella_import_DatiLavBio()
    {

    
            b2a.Database.StoredProcedure myspUpdate = new b2a.Database.StoredProcedure("_DatiLavBioRicrea");
            myspUpdate.parametro("@tipoImport", 2);

            if (oDB.ExecuteSql(myspUpdate))
                scriviMsg("BIOMETIC -  AggiornataTabella  import_DatiLavBio");
    
    }




private void calibraturaBiometic()
        {
            //ogni 5 minuti importa la giacenza in una tabella di appoggio
            //a fine importazione cancella la tabella della giacenza e la ripopola con i dati nuovi 
            DateTime dBiometic;
            dBiometic = dataUltimoAggiornamento("dBiometic");
            dBiometic = dBiometic.AddSeconds(1);

            //CON BARBI SI E' DECISO DI METTERE LA DATA 31/07/2022 !!!! in quanto non riescono a darci solo le nuove informazioni

            dBiometic = new DateTime(2022, 07, 31);

            DateTime dataNow = DateTime.Now;


            if ((dataNow - dBiometic).TotalMinutes <= 10)
            {
                return;
            }

                  

            string urlFinale = "/sortingresults?fromsortingdate=" + dBiometic.ToString("yyyy-MM-dd");


            Common.clsBiometic.Lista_CalibraturaBiometic _lista_calibrazioni = new Common.clsBiometic.Lista_CalibraturaBiometic();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_calibrazioni = JsonConvert.DeserializeObject<Common.clsBiometic.Lista_CalibraturaBiometic>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Biometic");
            }

            if (_lista_calibrazioni.data.Count == 0)
            {
                return;
            }


            b2a.Database.StoredProcedure myspDel = new b2a.Database.StoredProcedure("Agente_VOG_Biometic_Calibratura_DeleteALL");
            if (oDB.ExecuteSql(myspDel))
            {
                scriviMsg("BIOMETIC -  ELIMINATO TUTTO ");
            }



            _lista_calibrazioni.data.Sort((x, y) => x.sortingDate.CompareTo(y.sortingDate));

            DateTime lastUpdateTime = DateTime.Now;

            myLog.esito = Log.resultImport.success;


            int nrRigheSaltate = 0;


            foreach (Common.clsBiometic.clsCalibraturaBiometic calibrazioneDati in _lista_calibrazioni.data)
            {
                 trovaIdCalibratriceMagazzino(calibrazioneDati.sortingPlant);


                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("[Agente_VOG_Biometic_Calibratura_InsMod]");
                mysp.parametro("@sortingDate", calibrazioneDati.sortingDate);
                mysp.parametro("@sortingPlant", calibrazioneDati.sortingPlant);
                mysp.parametro("@variety", calibrazioneDati.variety);
                mysp.parametro("@harvestYear", calibrazioneDati.harvestYear);
                mysp.parametro("@qualityGroupCode", calibrazioneDati.qualityGroupCode);
                mysp.parametro("@qualityGroupName", calibrazioneDati.qualityGroupName);
                mysp.parametro("@colorGroupCode", calibrazioneDati.colorGroupCode);
                mysp.parametro("@colorGroupName", calibrazioneDati.colorGroupName);
                mysp.parametro("@Weight", calibrazioneDati.Weight);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Inserita calibratura Biometic :" + calibrazioneDati.sortingDate.ToString() + " " + calibrazioneDati.sortingPlant, livelloLog.prolisso);
                    //insertLog(idMagazzino, idCalibratrice, "Prodata - WS", "Inserita calibratura Biometic :" + calibrazioneDati.sortingDate.ToString() + " " + calibrazioneDati.sortingPlant);
                }
                else
                {
                    nrRigheSaltate++;
                    scriviMsg("Errore inserimento calibratura Biometic :" + calibrazioneDati.sortingDate.ToString() + " " + calibrazioneDati.variety + " " + calibrazioneDati.Weight.ToString(), livelloLog.errore);
                    myLog.msg =  "Errore inserimento calibratura Biometic :" + calibrazioneDati.sortingDate.ToString() + " " + calibrazioneDati.variety + " " + calibrazioneDati.Weight.ToString();
                    string msgErr1 =  myLog.insertLog(idMagazzino, idCalibratrice, "Prodata - WS",oDB);
                    myLog.esito = Log.resultImport.successWithError;
                    if (msgErr1 != "")
                        scriviMsg(msgErr1, livelloLog.errore);
                }
            }

            scriviMsg("Esportate Calibrazioni Biometic: " + (_lista_calibrazioni.data.Count - nrRigheSaltate).ToString() + " dati al " + lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
           if (nrRigheSaltate > 0)
            {
                scriviMsg("Saltate Calibrazioni Biometic: " + nrRigheSaltate.ToString());
            }

            oDB.ParamDB.scrivi(NOME_MODULO, "dBiometic", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), "");
            myLog.msg += "Esportate Calibrazioni Biometic: " + (_lista_calibrazioni.data.Count - nrRigheSaltate).ToString() + " dati al " + lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss");
            myLog.msg += "Saltate Calibrazioni Biometic: " + nrRigheSaltate.ToString();

            string msgErr = myLog.insertLog(idMagazzino, idCalibratrice, "Prodata - WS", oDB);
            if (msgErr != "")
                scriviMsg(msgErr, livelloLog.errore);


            b2a.Database.StoredProcedure myspUpdate = new b2a.Database.StoredProcedure("Agente_VOG_CalibraturaQuantitativi_Ricrea");
            if (oDB.ExecuteSql(myspUpdate))
            {
                scriviMsg("CalibraturaQuantitativi -  RICREO ");
            }

        }

        private void trovaIdCalibratriceMagazzino(string sortingPlant)
        {

            int nrFile = 0;
            try
            {
                nrFile = int.Parse(sortingPlant.Replace("B",""));

                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Bio_TrovaCalibratrice");
                mysp.parametro("@nrFile", nrFile);
                DataSet myDs = oDB.SelectSqlDataSet(mysp);
                DataRow resultRow = myDs.Tables[0].Rows[0];
                idMagazzino = (int)resultRow["idMagazzino"];
                idCalibratrice = (int)resultRow["idCalibratrice"];

            }
            catch
            {
                idMagazzino = 0;
                idCalibratrice = 0;
            }

  

        }
      


        private string richiestaWEB(string url, string ErrorMsg)
        {
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResponse = Common.Handler.Send(httpRequest, Common.Handler.metodi._GET, null, ErrorMsg);
            if (httpResponse == null)
            {
                scriviMsg(ErrorMsg, livelloLog.errore);
                return null;
            }

            return Common.Handler.Read(httpResponse);
        }

        private DateTime dataUltimoAggiornamento(string parametro)
        {
            DateTime dataUltimoRecord = DateTime.Now.AddDays(-100);
            string esito = "";
            oDB.ParamDB.leggi(NOME_MODULO, parametro, ref esito);
            try
            {
                if (esito.Length > 0)
                {
                    dataUltimoRecord = Convert.ToDateTime(esito);
                }
            }
            catch
            {
            }

            return dataUltimoRecord;
        }
    }
}
