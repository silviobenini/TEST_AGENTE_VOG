using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Agente_VOG.Common;
using Agente_VOG.SottoServizi;
using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;

namespace Servizio
{
    class clsServizioAgente2 : clsServizioAgente
    {
        public clsServizioAgente2(tipiAvvio _avvio, clsLogSuFile _myLogger) : base(_avvio, _myLogger)
        {
        }

        protected override void avvio2()
        {
        }

        protected override void caricaSottoservizi()
        {
        

        controllore.aggiungiServizio(new ImportProdata("ImportProdata", controllore));
        controllore.aggiungiServizio(new clsImportGestioneEmail("ImportGestioneEmail", controllore));
        controllore.aggiungiServizio(new clsImportBinsCalibrati("ImportBinsCalibrati", controllore));
        controllore.aggiungiServizio(new clsImportQuantitativi("ImportQuantitativi", controllore));
        controllore.aggiungiServizio(new ExportProdata("ExportProdata", controllore));
        }

        protected override void chiusura2()
        {
             
        }
    }
}
