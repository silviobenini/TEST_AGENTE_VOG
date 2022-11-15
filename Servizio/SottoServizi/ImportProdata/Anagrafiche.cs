using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Agente_VOG.SottoServizi
{
    class Anagrafiche : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public Anagrafiche(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "Anagrafiche";
            _descrizioneLunga = "Anagrafiche";
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
            varianteImballaggi();
            workplan();
            //gruppoLineaProduzione();
            stabilimento();
            cooperativa();
            indirizzo();
            varieta_articolo();
            gruppo_qualita();
            gruppo_calibro();
            calibro();
            qualita();
            coltivazione();
            colore();
            codificaArticoli();
            codificaCaratteristiche();
            //associazione_codifiche();
            //associazione_codifiche_varieta();
        }

        private void varianteImballaggi()
        {
            DateTime dVarianteImballaggi;
            dVarianteImballaggi = dataUltimoAggiornamento("dVarianteImballaggi");
            dVarianteImballaggi = dVarianteImballaggi.AddSeconds(1); 

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/packagingvariant?lastupdatefrom=" + dVarianteImballaggi.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss") + "&alsolocked=true";

            Common.Lista_AnagraficaVarianteImballaggi _lista_anagrafiche = new Common.Lista_AnagraficaVarianteImballaggi();
             
            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try { 
                _lista_anagrafiche = JsonConvert.DeserializeObject<Common.Lista_AnagraficaVarianteImballaggi>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            } 
            catch (Exception ex)
            {
                scriviExc(ex, "GET VarianteImballaggi");
            }

            if (_lista_anagrafiche.data.Count == 0) { 
                return; 
            }

            _lista_anagrafiche.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsAnagraficaVarianteImballaggi anagrafica in _lista_anagrafiche.data) {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_VarianteImballaggi_InsMod");
                mysp.parametro("@code", anagrafica.code);
                mysp.parametro("@internalNameDE", anagrafica.internalNameDE);
                mysp.parametro("@internalNameIT", anagrafica.internalNameIT);
                mysp.parametro("@nameDE", anagrafica.nameDE);
                mysp.parametro("@nameIT", anagrafica.nameIT); 
                mysp.parametro("@workPlan", anagrafica.workPlan);
                //mysp.parametro("@packagingProductionGroup", anagrafica.packagingProductionGroup);
                mysp.parametro("@locked", anagrafica.locked);
                mysp.parametro("@lastUpdateTime", anagrafica.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("VarianteImballaggi " + anagrafica.code); 
                    oDB.ParamDB.scrivi(NOME_MODULO, "dVarianteImballaggi", anagrafica.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }

        }




        private void stabilimento()
        {
            DateTime dStabilimento;
            dStabilimento = dataUltimoAggiornamento("dStabilimento");
            dStabilimento = dStabilimento.AddSeconds(1);

            DateTime dataNow = DateTime.Now;
 
            string urlFinale = "/storage?lastupdatefrom=" + dStabilimento.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss") + "&alsolocked=true";

            Common.Lista_Stabilimento _lista_stabilimento = new Common.Lista_Stabilimento();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_stabilimento = JsonConvert.DeserializeObject<Common.Lista_Stabilimento>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Stabilimento");
            }

            if (_lista_stabilimento.data.Count == 0)
            {
                return;
            }

            _lista_stabilimento.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsStabilimento stabilimenti in _lista_stabilimento.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Stabilimenti_InsMod");
                mysp.parametro("@code", stabilimenti.code);
                mysp.parametro("@abbreviation", stabilimenti.abbreviation);
                mysp.parametro("@warehouse", stabilimenti.warehouse);
                mysp.parametro("@locked", stabilimenti.locked);
                mysp.parametro("@lastUpdateTime", stabilimenti.lastUpdateTime); 
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Stabilimento " + stabilimenti.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dStabilimento", stabilimenti.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }

        }

        private void cooperativa() {
            DateTime dCooperativa;
            dCooperativa = dataUltimoAggiornamento("dCooperativa");
            dCooperativa = dCooperativa.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/warehouse?lastupdatefrom=" + dCooperativa.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss") + "&alsolocked=true";

            Common.Lista_Cooperativa _lista_cooperativa = new Common.Lista_Cooperativa();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_cooperativa = JsonConvert.DeserializeObject<Common.Lista_Cooperativa>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Cooperativa");
            }

            if (_lista_cooperativa.data.Count == 0)
            {
                return;
            }

            _lista_cooperativa.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsCooperativa cooperativa in _lista_cooperativa.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Cooperative_InsMod");
                mysp.parametro("@code", cooperativa.code);
                mysp.parametro("@searchString", cooperativa.searchString);
                mysp.parametro("@address", cooperativa.address);
                mysp.parametro("@locked", cooperativa.locked);
                mysp.parametro("@lastUpdateTime", cooperativa.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Cooperativa " + cooperativa.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dCooperativa", cooperativa.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void indirizzo() {
            DateTime dIndirizzo;
            dIndirizzo = dataUltimoAggiornamento("dIndirizzo");
            dIndirizzo = dIndirizzo.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/address?lastupdatefrom=" + dIndirizzo.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_Indirizzi _lista_indirizzi = new Common.Lista_Indirizzi();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_indirizzi = JsonConvert.DeserializeObject<Common.Lista_Indirizzi>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Indirizzi");
            }

            if (_lista_indirizzi.data.Count == 0)
            {
                return;
            }

            _lista_indirizzi.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsIndirizzi indirizzo in _lista_indirizzi.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Indirizzi_InsMod");
                mysp.parametro("@uuid", indirizzo.uuid);
                mysp.parametro("@personNumber", indirizzo.personNumber);
                mysp.parametro("@searchString", indirizzo.searchString);
                mysp.parametro("@street", indirizzo.street);
                mysp.parametro("@zip", indirizzo.zip);
                mysp.parametro("@place", indirizzo.place);
                mysp.parametro("@province", indirizzo.province);
                mysp.parametro("@nation", indirizzo.nation);
                mysp.parametro("@language", indirizzo.language); 
                mysp.parametro("@locked", indirizzo.locked);
                mysp.parametro("@lastUpdateTime", indirizzo.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Indirizzi " + indirizzo.uuid);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dIndirizzo", indirizzo.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void varieta_articolo() {
            DateTime dvarieta_articolo;
            dvarieta_articolo = dataUltimoAggiornamento("dvarieta_articolo");
            dvarieta_articolo = dvarieta_articolo.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/item?lastupdatefrom=" + dvarieta_articolo.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_varieta_articoli _lista_varieta_articoli = new Common.Lista_varieta_articoli();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_varieta_articoli = JsonConvert.DeserializeObject<Common.Lista_varieta_articoli>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET VarietaArticoli");
            }

            if (_lista_varieta_articoli.data.Count == 0)
            {
                return;
            }

            _lista_varieta_articoli.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsVarieta_articoli _VarietaArticoli in _lista_varieta_articoli.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_VarietaArticoli_InsMod");
                mysp.parametro("@code", _VarietaArticoli.code);
                mysp.parametro("@nameDE", _VarietaArticoli.nameDE);
                mysp.parametro("@nameIT", _VarietaArticoli.nameIT);
                mysp.parametro("@type", _VarietaArticoli.type);
                mysp.parametro("@weightInBin", _VarietaArticoli.weightInBin); 
                mysp.parametro("@locked", _VarietaArticoli.locked);
                mysp.parametro("@lastUpdateTime", _VarietaArticoli.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Varieta_articolo " + _VarietaArticoli.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dvarieta_articolo", _VarietaArticoli.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void gruppo_qualita() {
            DateTime dgruppo_qualita;
            dgruppo_qualita = dataUltimoAggiornamento("dgruppo_qualita");
            dgruppo_qualita = dgruppo_qualita.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/qualitygroup?lastupdatefrom=" + dgruppo_qualita.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_gruppo_qualita _lista_gruppo_qualita = new Common.Lista_gruppo_qualita();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_gruppo_qualita = JsonConvert.DeserializeObject<Common.Lista_gruppo_qualita>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Gruppo_qualita");
            }

            if (_lista_gruppo_qualita.data.Count == 0)
            {
                return;
            }
            
            _lista_gruppo_qualita.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsGruppo_qualita _Gruppo_qualita in _lista_gruppo_qualita.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Gruppo_Qualita_InsMod");
                mysp.parametro("@code", _Gruppo_qualita.code);
                mysp.parametro("@nameDE", _Gruppo_qualita.nameDE);
                mysp.parametro("@nameIT", _Gruppo_qualita.nameIT); 
                mysp.parametro("@lastUpdateTime", _Gruppo_qualita.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Varieta_articolo " + _Gruppo_qualita.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dgruppo_qualita", _Gruppo_qualita.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void gruppo_calibro() {
            DateTime dgruppo_calibro;
            dgruppo_calibro = dataUltimoAggiornamento("dgruppo_calibro");
            dgruppo_calibro = dgruppo_calibro.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/calibergroup?lastupdatefrom=" + dgruppo_calibro.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_gruppo_calibro _lista_gruppo_calibro = new Common.Lista_gruppo_calibro();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_gruppo_calibro = JsonConvert.DeserializeObject<Common.Lista_gruppo_calibro>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Gruppo_calibro");
            }

            if (_lista_gruppo_calibro.data.Count == 0)
            {
                return;
            }

            _lista_gruppo_calibro.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));


            foreach (Common.clsGruppo_calibro _Gruppo_calibro in _lista_gruppo_calibro.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Gruppo_Calibro_InsMod");
                mysp.parametro("@code", _Gruppo_calibro.code);
                mysp.parametro("@nameDE", _Gruppo_calibro.nameDE);
                mysp.parametro("@nameIT", _Gruppo_calibro.nameIT);
                mysp.parametro("@lastUpdateTime", _Gruppo_calibro.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Gruppo_calibro " + _Gruppo_calibro.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dgruppo_calibro", _Gruppo_calibro.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void calibro() {
            DateTime dCalibro;
            dCalibro = dataUltimoAggiornamento("dCalibro");
            dCalibro = dCalibro.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/caliber?lastupdatefrom=" + dCalibro.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_calibro _lista_calibro = new Common.Lista_calibro();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_calibro = JsonConvert.DeserializeObject<Common.Lista_calibro>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Calibro");
            }

            if (_lista_calibro.data.Count == 0)
            {
                return;
            }

            _lista_calibro.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsCalibro _Calibro in _lista_calibro.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Calibro_InsMod");
                mysp.parametro("@code", _Calibro.code);
                mysp.parametro("@nameDE", _Calibro.nameDE);
                mysp.parametro("@nameIT", _Calibro.nameIT);
                mysp.parametro("@group", _Calibro.group);
                mysp.parametro("@locked", _Calibro.locked);
                mysp.parametro("@lastUpdateTime", _Calibro.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Calibro " + _Calibro.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dCalibro", _Calibro.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void qualita() {
            DateTime dQualita;
            dQualita = dataUltimoAggiornamento("dQualita");
            dQualita = dQualita.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/quality?lastupdatefrom=" + dQualita.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_qualita _lista_qualita = new Common.Lista_qualita();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_qualita = JsonConvert.DeserializeObject<Common.Lista_qualita>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Qualita");
            }

            if (_lista_qualita.data.Count == 0)
            {
                return;
            }

            _lista_qualita.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));


            foreach (Common.clsQualita _Qualita in _lista_qualita.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Qualita_InsMod");
                mysp.parametro("@code", _Qualita.code);
                mysp.parametro("@nameDE", _Qualita.nameDE);
                mysp.parametro("@nameIT", _Qualita.nameIT);
                mysp.parametro("@group", _Qualita.group);
                mysp.parametro("@type", _Qualita.type);
                mysp.parametro("@locked", _Qualita.locked);
                mysp.parametro("@lastUpdateTime", _Qualita.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Qualita " + _Qualita.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dQualita", _Qualita.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void coltivazione() {
            DateTime dColtivazione;
            dColtivazione = dataUltimoAggiornamento("dColtivazione");
            dColtivazione = dColtivazione.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/growingtype?lastupdatefrom=" + dColtivazione.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_coltivazione _lista_Coltivazione = new Common.Lista_coltivazione();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_Coltivazione = JsonConvert.DeserializeObject<Common.Lista_coltivazione>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Coltivazione");
            }

            if (_lista_Coltivazione.data.Count == 0)
            {
                return;
            }

            _lista_Coltivazione.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));


            foreach (Common.clsColtivazione _Coltivazione in _lista_Coltivazione.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Coltivazione_InsMod");
                mysp.parametro("@code", _Coltivazione.code);
                mysp.parametro("@nameDE", _Coltivazione.nameDE);
                mysp.parametro("@nameIT", _Coltivazione.nameIT); 
                mysp.parametro("@type", _Coltivazione.type);
                mysp.parametro("@locked", _Coltivazione.locked);
                mysp.parametro("@lastUpdateTime", _Coltivazione.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Coltivazione " + _Coltivazione.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dColtivazione", _Coltivazione.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void colore() {
            DateTime dColore;
            dColore = dataUltimoAggiornamento("dColore");
            dColore = dColore.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/colour?lastupdatefrom=" + dColore.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_colore _lista_colore = new Common.Lista_colore();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            { 
                _lista_colore = JsonConvert.DeserializeObject<Common.Lista_colore>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET Colore");
            }

            if (_lista_colore.data.Count == 0)
            {
                return;
            }

            _lista_colore.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));


            foreach (Common.clsColore _colore in _lista_colore.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Colore_InsMod");
                mysp.parametro("@code", _colore.code);
                mysp.parametro("@nameDE", _colore.nameDE);
                mysp.parametro("@nameIT", _colore.nameIT); 
                mysp.parametro("@locked", _colore.locked);
                mysp.parametro("@lastUpdateTime", _colore.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("Colore " + _colore.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dColore", _colore.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }
        }

        private void workplan()
        {
            DateTime dVarianteImballaggi;
            dVarianteImballaggi = dataUltimoAggiornamento("dworkplan");
            dVarianteImballaggi = dVarianteImballaggi.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/workplan?lastupdatefrom=" + dVarianteImballaggi.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_Workplan _lista_workplan = new Common.Lista_Workplan();

            string httpErrorMsg = "";
            string urlVariante = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_workplan = JsonConvert.DeserializeObject<Common.Lista_Workplan>(richiestaWEB(urlVariante, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET workplan");
            }

            if (_lista_workplan.data.Count == 0)
            {
                return;
            }

            _lista_workplan.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsWorkplan anagrafica in _lista_workplan.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_Workplan_InsMod");
                mysp.parametro("@code", anagrafica.code); 
                mysp.parametro("@nameDE", anagrafica.nameDE);
                mysp.parametro("@nameIT", anagrafica.nameIT);  
                mysp.parametro("@locked", anagrafica.locked);
                mysp.parametro("@lastUpdateTime", anagrafica.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("workplan " + anagrafica.code);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dworkplan", anagrafica.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }

        }

        private void codificaArticoli()
        {
            DateTime dCodificaArticoli;
            dCodificaArticoli = dataUltimoAggiornamento("dCodificaArticoli");
            dCodificaArticoli = dCodificaArticoli.AddSeconds(1);

                       
            DateTime dataNow = DateTime.Now;
            string urlFinale = "/ppsitem?lastupdatefrom=" + dCodificaArticoli.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss");

            Common.Lista_CodificaArticoli _lista_codificheArticoli = new Common.Lista_CodificaArticoli();

            string httpErrorMsg = "";
            string urlCodArt = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_codificheArticoli = JsonConvert.DeserializeObject<Common.Lista_CodificaArticoli>(richiestaWEB(urlCodArt, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET codificaArticoli");
                return;
            }

            if (_lista_codificheArticoli.data.Count == 0)
            {
                return;
            }


            _lista_codificheArticoli.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsCodificaArticoli codificaArt in _lista_codificheArticoli.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_CodificaArticoli_InsMod");
                mysp.parametro("@ppsCode", codificaArt.ppsCode);
                mysp.parametro("@warehouse", codificaArt.warehouse);
                mysp.parametro("@qualityGroup", codificaArt.qualityGroup);
                mysp.parametro("@caliberGroup", codificaArt.caliberGroup);
                mysp.parametro("@growingType", codificaArt.growingType);
                mysp.parametro("@colour", codificaArt.colour);
                mysp.parametro("@item", codificaArt.item);
                mysp.parametro("@lastUpdateTime", codificaArt.lastUpdateTime);
                if (oDB.ExecuteSql(mysp))
                {
                    scriviMsg("CodificaArticoli " + codificaArt.ppsCode);
                    oDB.ParamDB.scrivi(NOME_MODULO, "dCodificaArticoli", codificaArt.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                }
            }

        }


        private void codificaCaratteristiche()
        {
            DateTime dCodificaCaratteristiche;
            dCodificaCaratteristiche = dataUltimoAggiornamento("dCodificaCaratteristiche");
            dCodificaCaratteristiche = dCodificaCaratteristiche.AddSeconds(1);

            DateTime dataNow = DateTime.Now;

            string urlFinale = "/ppsitemproperties?lastupdatefrom=" + dCodificaCaratteristiche.ToString("yyyy-MM-ddTHH:mm:ss") + "&lastupdateto=" + dataNow.ToString("yyyy-MM-ddTHH:mm:ss") ;

            Common.Lista_CodificaCaratteristiche _lista_codificheCaratteristiche = new Common.Lista_CodificaCaratteristiche();

            string httpErrorMsg = "";
            string urlCodCar = Common.Handler.URL_WebService + urlFinale;
            try
            {
                _lista_codificheCaratteristiche = JsonConvert.DeserializeObject<Common.Lista_CodificaCaratteristiche>(richiestaWEB(urlCodCar, httpErrorMsg).ToString());
            }
            catch (Exception ex)
            {
                scriviExc(ex, "GET CodificaCaratteristiche");
                return;
            }

            if (_lista_codificheCaratteristiche.data.Count == 0)
            {
                return;
            }

            _lista_codificheCaratteristiche.data.Sort((x, y) => x.lastUpdateTime.CompareTo(y.lastUpdateTime));

            foreach (Common.clsCodificaCaratteristiche codificaCar in _lista_codificheCaratteristiche.data)
            {
                b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_CodificaCaratteristiche_InsMod");
                if (codificaCar.propertyType != null & codificaCar.item == null)
                {
                    mysp.parametro("@propertyType", codificaCar.propertyType);
                    mysp.parametro("@ppsCode", codificaCar.ppsCode);
                    mysp.parametro("@item", codificaCar.item);
                    mysp.parametro("@warehouse", codificaCar.warehouse);
                    mysp.parametro("@qualityGroup", codificaCar.qualityGroup);
                    mysp.parametro("@caliberGroup", codificaCar.caliberGroup);
                    mysp.parametro("@growingType", codificaCar.growingType);
                    mysp.parametro("@colour", codificaCar.colour);
                    mysp.parametro("@lastUpdateTime", codificaCar.lastUpdateTime);
                    if (oDB.ExecuteSql(mysp))
                    {
                        scriviMsg("CodificaCaratteristiche " + codificaCar.ppsCode);
                        oDB.ParamDB.scrivi(NOME_MODULO, "dCodificaCaratteristiche", codificaCar.lastUpdateTime.ToString("yyyy-MM-ddTHH:mm:ss"), "");
                    }
                }




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
            DateTime dataUltimoRecord = DateTime.Now.AddDays(-3600);
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
