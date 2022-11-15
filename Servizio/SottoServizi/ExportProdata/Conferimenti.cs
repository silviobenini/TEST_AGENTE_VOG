using Agente_VOG.Common;
using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Agente_VOG.SottoServizi
{
    class Conferimenti : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public Conferimenti(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "Conferimenti";
            _descrizioneLunga = "Conferimenti";
        }

        protected override void inizializzazioneCiclo()
        {
            string json = JsonConvert.SerializeObject(new Umlagerungernteware());
            bool a = false;
        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {
            scriviMsg("Ciclo in polling", livelloLog.prolisso);

            HttpClient client = new HttpClient();
            string URL = "https://jp-b2a-rest-8yrza2yf43.vog.it:8643/rest/b2a_rest/monitarbr/umlagerungernteware";

            b2a.Database.StoredProcedure myspLista = new b2a.Database.StoredProcedure("[AgenteProData_Melix_richiestaDDT_lista]");
            b2a.Database.StoredProcedure myspTermina = new b2a.Database.StoredProcedure("[AgenteProData_Melix_richiestaDDT_termina]");
            DataSet myds = oDB.SelectSqlDataSet(myspLista);
            Finale f = null;
            List<SubFields> subFields = null;

            foreach (DataRow row in myds.Tables[0].Rows)
            {
                f = new Finale();
                f.Testata.VerarbeitungsbetriebKuerzel = "MEL";
                f.Testata.LagerCode = "L1";
                if(row.Field<int>("idMagazzino") == 1)
                    f.Testata.WarenempfaengerNummer = "118587";
                if (row.Field<int>("idMagazzino") == 3)
                    f.Testata.WarenempfaengerNummer = "100836";

                myspLista.parametro("@idPrenotazione", row.Field<int>("idPrenotazione"));
                myds = oDB.SelectSqlDataSet(myspLista);

                subFields = new List<SubFields>();

                foreach (DataRow barcode in myds.Tables[1].Rows) {
                    subFields.Add(new SubFields() { Fields = new SubFieldsData() { Kistennummer = barcode.Field<string>("barcode") } });
                }
                f.Collections.UmlagerungErntewarePosition.Data = subFields.ToArray();
                f.Testata.Vollstaendig = true;

                myspTermina.parametro("@id", row.Field<int>("id"));
                if (oDB.ExecuteSql(myspTermina)) {
                    scriviMsg("termina richiesta ddt", livelloLog.normale);
                }
                else
                    scriviMsg("richiesta ddt non terminata", livelloLog.errore);

                string json = JsonConvert.SerializeObject(f, Formatting.Indented);
                client.BaseAddress = new Uri(URL);

                // Add an Accept header for JSON format.
                //client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("BASIC", Convert.ToBase64String(Encoding.Default.GetBytes("B2A_REST:X171YyB#b*a56rF8")));
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage result = client.PostAsync(URL, content).Result;
                f = null;
                subFields = null;
            }
            
        }

        //private void Conferimenti()
        //{
        //    DateTime dConferimenti;
        //    dConferimenti = dataUltimoAggiornamento("dConferimenti");
        //    dConferimenti = dConferimenti.AddSeconds(1);

        //    DateTime dataNow = DateTime.Now;

        //    string urlFinale = "/ppssortedstock?lastupdatefrom=" + dConferimenti.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

        //    Common.clsConferimenti.Lista_Conferimenti _lista_giacenza = new Common.clsConferimenti.Lista_Conferimenti();

        //    string httpErrorMsg = "";
        //    string urlVariante = Common.Handler.URL_WebService + urlFinale;
        //    try
        //    {
        //        _lista_giacenza = JsonConvert.DeserializeObject<Common.clsConferimenti.Lista_Conferimenti>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        scriviExc(ex, "GET Conferimenti");
        //    }

        //    if (_lista_giacenza.data.Count == 0)
        //    {
        //        return;
        //    }

        //    _lista_giacenza.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

        //    foreach (Common.clsConferimenti.clsGiacenza giacenzaDati in _lista_giacenza.data)
        //    {
        //        b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Giacenza_InsMod");
        //        mysp.parametro("@warehouse", giacenzaDati.warehouse);
        //        mysp.parametro("@storage", giacenzaDati.storage);
        //        mysp.parametro("@cropYear", giacenzaDati.cropYear);
        //        mysp.parametro("@variety", giacenzaDati.variety);
        //        mysp.parametro("@caliber", giacenzaDati.caliber);
        //        mysp.parametro("@colour", giacenzaDati.colour);
        //        mysp.parametro("@quality", giacenzaDati.quality);
        //        mysp.parametro("@growingType", giacenzaDati.growingType);
        //        mysp.parametro("@inStock", giacenzaDati.inStock);
        //        mysp.parametro("@available", giacenzaDati.available);
        //        mysp.parametro("@closed", giacenzaDati.closed);
        //        mysp.parametro("@calendarWeekDay", giacenzaDati.calendarWeekDay);
        //        mysp.parametro("@age", giacenzaDati.age);
        //        mysp.parametro("@lastUpdateTime", giacenzaDati.lastUpdateTime); 
        //        if (oDB.ExecuteSql(mysp))
        //        {
        //            string articolo = giacenzaDati.variety + " " + giacenzaDati.caliber + " " + giacenzaDati.colour + " " + giacenzaDati.quality;
        //            scriviMsg("Giacenza " + giacenzaDati.cropYear.ToString() + " articolo: " + articolo);
        //            oDB.ParamDB.scrivi(NOME_MODULO, "dConferimenti", giacenzaDati.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
        //        }
        //    }

        //}

        //private void ConferimentiComplete()
        //{
        //    //ogni 5 minuti importa la giacenza in una tabella di appoggio
        //    //a fine importazione cancella la tabella della giacenza e la ripopola con i dati nuovi 
        //    DateTime dConferimenti;
        //    dConferimenti = dataUltimoAggiornamento("dConferimenti");
        //    dConferimenti = dConferimenti.AddSeconds(1);

        //    DateTime dataNow = DateTime.Now;

        //    if ((dataNow - dConferimenti).TotalMinutes <= 10)
        //    {
        //        return;
        //    }

        //    string urlFinale = "/ppssortedstock";

        //    Common.clsConferimenti.Lista_Conferimenti _lista_giacenza = new Common.clsConferimenti.Lista_Conferimenti();

        //    string httpErrorMsg = "";
        //    string urlVariante = Common.Handler.URL_WebService + urlFinale;
        //    try
        //    {
        //        _lista_giacenza = JsonConvert.DeserializeObject<Common.clsConferimenti.Lista_Conferimenti>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
        //    }
        //    catch (Exception ex)
        //    {
        //        scriviExc(ex, "GET Conferimenti");
        //    }

        //    if (_lista_giacenza.data.Count == 0)
        //    {
        //        return;
        //    }

        //    _lista_giacenza.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

        //    DateTime lastUpdateTime = DateTime.Now;

        //    foreach (Common.clsConferimenti.clsGiacenza giacenzaDati in _lista_giacenza.data)
        //    {
        //        b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_GiacenzaCompleta_InsMod");
        //        mysp.parametro("@warehouse", giacenzaDati.warehouse);
        //        mysp.parametro("@storage", giacenzaDati.storage);
        //        mysp.parametro("@cropYear", giacenzaDati.cropYear);
        //        mysp.parametro("@variety", giacenzaDati.variety);
        //        mysp.parametro("@caliber", giacenzaDati.caliber);
        //        mysp.parametro("@colour", giacenzaDati.colour);
        //        mysp.parametro("@quality", giacenzaDati.quality);
        //        mysp.parametro("@growingType", giacenzaDati.growingType);
        //        mysp.parametro("@inStock", giacenzaDati.inStock);
        //        mysp.parametro("@available", giacenzaDati.available);
        //        mysp.parametro("@closed", giacenzaDati.closed);
        //        mysp.parametro("@calendarWeekDay", giacenzaDati.calendarWeekDay);
        //        mysp.parametro("@age", giacenzaDati.age);
        //        mysp.parametro("@lastUpdateTime", giacenzaDati.lastUpdateTime);
        //        if (oDB.ExecuteSql(mysp))
        //        {
        //            lastUpdateTime = giacenzaDati.lastUpdateTime;
        //        }
        //    }

        //    if (oDB.ExecuteSql(new b2a.Database.StoredProcedure("Agente_VOG_GiacenzaCompleta_Concludi")))
        //    {
        //        scriviMsg("Giacenza Esportati: " + _lista_giacenza.data.Count.ToString() + " dati al " + lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"));
        //        oDB.ParamDB.scrivi(NOME_MODULO, "dConferimenti", DateTime.Now.ToString("yyyy-MM-ddTHH:mm:ss"), "");
        //    }
        //}

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
