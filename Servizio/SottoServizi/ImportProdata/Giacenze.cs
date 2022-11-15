using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.SottoServizi
{
    class Giacenze : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public Giacenze(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "Giacenze";
            _descrizioneLunga = "Giacenze";
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
            //giacenze(); 
            giacenzeComplete();
        }

        private void giacenze()
        {
            DateTime dGiacenze;
            dGiacenze = dataUltimoAggiornamento("dGiacenze");
            dGiacenze = dGiacenze.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/ppssortedstock?lastupdatefrom=" + dGiacenze.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.clsGiacenze.Lista_Giacenze _lista_giacenza = new Common.clsGiacenze.Lista_Giacenze();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_giacenza = JsonConvert.DeserializeObject<Common.clsGiacenze.Lista_Giacenze>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Giacenze");
            }

            if (_lista_giacenza.data.Count == 0)
            {
                return;
            }

            _lista_giacenza.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsGiacenze.clsGiacenza giacenzaDati in _lista_giacenza.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Giacenza_InsMod");
                mysp.parametro("@warehouse", giacenzaDati.warehouse);
                mysp.parametro("@storage", giacenzaDati.storage);
                mysp.parametro("@cropYear", giacenzaDati.cropYear);
                mysp.parametro("@variety", giacenzaDati.variety);
                mysp.parametro("@caliber", giacenzaDati.caliber);
                mysp.parametro("@colour", giacenzaDati.colour);
                mysp.parametro("@quality", giacenzaDati.quality);
                mysp.parametro("@growingType", giacenzaDati.growingType);
                mysp.parametro("@inStock", giacenzaDati.inStock);
                mysp.parametro("@available", giacenzaDati.available);
                mysp.parametro("@closed", giacenzaDati.closed);
                mysp.parametro("@calendarWeekDay", giacenzaDati.calendarWeekDay);
                mysp.parametro("@age", giacenzaDati.age);
                mysp.parametro("@lastUpdateTime", giacenzaDati.lastUpdateTime); 
                if (oDB.ExecuteSql(mysp))
                {
                    string articolo = giacenzaDati.variety + " " + giacenzaDati.caliber + " " + giacenzaDati.colour + " " + giacenzaDati.quality;
                    scriviMsg("Giacenza " + giacenzaDati.cropYear.ToString() + " articolo: " + articolo);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dGiacenze", giacenzaDati.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
             
        }

        private void giacenzeComplete()
        {
            //ogni 5 minuti importa la giacenza in una tabella di appoggio
            //a fine importazione cancella la tabella della giacenza e la ripopola con i dati nuovi 
            DateTime dGiacenze;
            dGiacenze = dataUltimoAggiornamento("dGiacenze");
            dGiacenze = dGiacenze.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            if ((dataNow - dGiacenze).TotalMinutes <= 10)
            {
                return;
            }

            string urlFinale = "/ppssortedstock";

            Common.clsGiacenze.Lista_Giacenze _lista_giacenza = new Common.clsGiacenze.Lista_Giacenze();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_giacenza = JsonConvert.DeserializeObject<Common.clsGiacenze.Lista_Giacenze>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Giacenze");
            }

            if (_lista_giacenza.data.Count == 0)
            {
                return;
            }

            _lista_giacenza.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            DateTime lastUpdateTime = DateTime.Now;

            foreach (Common.clsGiacenze.clsGiacenza giacenzaDati in _lista_giacenza.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_GiacenzaCompleta_InsMod");
                mysp.parametro("@warehouse", giacenzaDati.warehouse);
                mysp.parametro("@storage", giacenzaDati.storage);
                mysp.parametro("@cropYear", giacenzaDati.cropYear);
                mysp.parametro("@variety", giacenzaDati.variety);
                mysp.parametro("@caliber", giacenzaDati.caliber);
                mysp.parametro("@colour", giacenzaDati.colour);
                mysp.parametro("@quality", giacenzaDati.quality);
                mysp.parametro("@growingType", giacenzaDati.growingType);
                mysp.parametro("@inStock", giacenzaDati.inStock);
                mysp.parametro("@available", giacenzaDati.available);
                mysp.parametro("@closed", giacenzaDati.closed);
                mysp.parametro("@calendarWeekDay", giacenzaDati.calendarWeekDay);
                mysp.parametro("@age", giacenzaDati.age);
                mysp.parametro("@lastUpdateTime", giacenzaDati.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    lastUpdateTime = giacenzaDati.lastUpdateTime;
                }
            }

            if (oDB.ExecuteSql(new b2a.Database.StoredProcedure("Agente_VOG_GiacenzaCompleta_Concludi")))
            {
                scriviMsg("Giacenza Esportati: " + _lista_giacenza.data.Count.ToString() + " dati al " + lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
                oDB.ParamDB.scrivi(NOME_MODULO, "dGiacenze", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), "");
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
            DateTime dataUltimoRecord = DateTime.Now;
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
