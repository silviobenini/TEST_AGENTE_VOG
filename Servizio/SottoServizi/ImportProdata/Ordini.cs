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
    class Ordini : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public Ordini(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "Ordini";
            _descrizioneLunga = "Ordini";
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
            ordini();
            statoOrdini();
        }

        private void ordini()
        {
            DateTime dOrdini;
            dOrdini = dataUltimoAggiornamento("dOrdini");
            dOrdini = dOrdini.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/orderLine?lastupdatefrom=" + dOrdini.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss"); 

            Common.clsOrdini.Lista_RigheOrdini _lista_ordini = new Common.clsOrdini.Lista_RigheOrdini();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_ordini = JsonConvert.DeserializeObject<Common.clsOrdini.Lista_RigheOrdini>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Ordini");
            }

            if (_lista_ordini.data.Count == 0)
            {
                return;
            }

            _lista_ordini.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            //DateTime lastUpdateTime = DateTime.Now;

            foreach (Common.clsOrdini.clsRigheOrdine ordiniDati in _lista_ordini.data)
            { 
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Ordini_InsMod");
                mysp.parametro("@systemName", ordiniDati.systemName);
                mysp.parametro("@orderNumber", ordiniDati.orderNumber);
                mysp.parametro("@positionNumber", Convert.ToInt32(ordiniDati.positionNumber));
                mysp.parametro("@numerationCode", ordiniDati.numerationCode);
                mysp.parametro("@warehouse", ordiniDati.warehouse);
                mysp.parametro("@internalOrderNumber", ordiniDati.internalOrderNumber);
                mysp.parametro("@internalPositionNumber", ordiniDati.internalPositionNumber);
                mysp.parametro("@internalOrderYear", ordiniDati.internalOrderYear);
                mysp.parametro("@orderDate", ordiniDati.orderDate);
                mysp.parametro("@loadingDate", ordiniDati.loadingDate);
                if (ordiniDati.deliveryDate != null)
                {
                    mysp.parametro("@deliveryDate", DateTime.Parse(ordiniDati.deliveryDate).ToString("yyyy-MM-ddTHH:mm:ss"));
                }

                //mysp.parametro("@deliveryDate", ordiniDati.deliveryDate);
                mysp.parametro("@invoiceRecipient", ordiniDati.invoiceRecipient);
                mysp.parametro("@consignee", ordiniDati.consignee);
                mysp.parametro("@deliveryAddress", ordiniDati.deliveryAddress);
                mysp.parametro("@freighter", ordiniDati.freighter);
                mysp.parametro("@employee", ordiniDati.employee); 
                mysp.parametro("@storage", ordiniDati.storage);
                mysp.parametro("@variety", ordiniDati.variety);
                mysp.parametro("@quality", ordiniDati.quality);
                mysp.parametro("@caliber", ordiniDati.caliber);
                mysp.parametro("@growing", ordiniDati.growing);
                mysp.parametro("@colour", ordiniDati.colour);
                mysp.parametro("@cropYear", ordiniDati.cropYear);
                mysp.parametro("@palletization", ordiniDati.palletization);
                mysp.parametro("@packagingVariant", ordiniDati.packagingVariant);
                mysp.parametro("@kgGros", ordiniDati.kgGros);
                mysp.parametro("@kgNet", ordiniDati.kgNet);
                mysp.parametro("@quantityPallets", ordiniDati.quantityPallets);
                mysp.parametro("@quantityPackaging", ordiniDati.quantityPackaging);
                mysp.parametro("@cancelled", ordiniDati.cancelled);
                mysp.parametro("@closed", ordiniDati.closed);
                mysp.parametro("@lastUpdateTime", ordiniDati.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                { 
                    //lastUpdateTime = ordiniDati.lastUpdateTime;

                    string articolo = ordiniDati.internalOrderNumber.ToString() + " " + ordiniDati.internalPositionNumber.ToString();
                    scriviMsg("Ordine " + articolo);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dOrdini", ordiniDati.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }

            //scriviMsg("Ordine aggiornati " + _lista_ordini.data.Count.ToString() + " ordini");
            //oDB.ParamDB.scrivi(NOME_MODULO, "dOrdini", lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");

        }


        private void statoOrdini()
        {
            DateTime dStatoOrdini;
            dStatoOrdini = dataUltimoAggiornamento("dStatoOrdini");
            dStatoOrdini = dStatoOrdini.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/orderStatus?lastupdatefrom=" + dStatoOrdini.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss"); 

            Common.Lista_StatoOrdini _lista_stato_ordini = new Common.Lista_StatoOrdini();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_stato_ordini = JsonConvert.DeserializeObject<Common.Lista_StatoOrdini>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET StatusOrdini");
            }

            if (_lista_stato_ordini.data.Count == 0)
            {
                return;
            }

            _lista_stato_ordini.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            //DateTime lastUpdateTime = DateTime.Now;

            foreach (Common.clsStatoOrdine ordiniStato in _lista_stato_ordini.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Ordini_Stato_InsMod");
                mysp.parametro("@systemName", ordiniStato.systemName);
                mysp.parametro("@ppsLocation", ordiniStato.ppsLocation);
                mysp.parametro("@orderID", ordiniStato.orderID); 
                mysp.parametro("@orderNumber", ordiniStato.orderNumber);
                mysp.parametro("@positionNumber", Convert.ToInt32(ordiniStato.positionNumber));
                mysp.parametro("@warehouse", ordiniStato.warehouse);
                mysp.parametro("@InternalOrderNumber", ordiniStato.InternalOrderNumber);
                mysp.parametro("@InternalPositionNumber", ordiniStato.InternalPositionNumber);
                mysp.parametro("@internalOrderYear", ordiniStato.internalOrderYear);
                mysp.parametro("@splitNumber", ordiniStato.splitNumber);
                mysp.parametro("@productionState", ordiniStato.productionState);
                mysp.parametro("@lineId", ordiniStato.lineId);
                mysp.parametro("@quantityPallets", ordiniStato.quantityPallets);
                mysp.parametro("@quantityPackagingProduced", ordiniStato.quantityPackagingProduced);
                mysp.parametro("@primOrderRef", ordiniStato.primOrderRef);
                mysp.parametro("@primOrderPrio", ordiniStato.primOrderPrio);
                mysp.parametro("@LastUpdateTime", ordiniStato.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    //lastUpdateTime = ordiniStato.lastUpdateTime;

                    string articolo = ordiniStato.orderID.ToString() + " " + ordiniStato.productionState;
                    scriviMsg("Ordine Stato " + articolo);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dStatoOrdini", ordiniStato.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
            
            //scriviMsg("Ordine Stato aggiornati " + _lista_stato_ordini.data.Count.ToString() + " ordini");
            //oDB.ParamDB.scrivi(NOME_MODULO, "dStatoOrdini", lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
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
