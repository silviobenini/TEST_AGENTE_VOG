using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using static Servizio.VariabiliGlobali;
using b2a.BlackBox;
using b2a.Interfaccia;
using System.Threading;

/// <summary>
/// Classe di avvio per <b>Servizio Windows</b>
/// </summary>
/// <remarks>Selezionare questo come Oggetto di avvio nelle impostazioni di progetto</remarks>
namespace Servizio
{
    partial class clsAvvioServizio : ServiceBase
    {
        static void Main(string[] args)
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] { new clsAvvioServizio() };
            ServiceBase.Run(ServicesToRun);
        }

        public clsAvvioServizio()
        {
            InitializeComponent();

            myLogger = new clsLogSuFile();
            impostaVariabiliGenerali();
        }

        protected override void OnStart(string[] args)
        {
            myLogger.scriviLog("Avvio servizio " + Parametri.NOME_SERVIZIO);

            try {
                myServizioAgente = new clsServizioAgente2(clsServizioAgente.tipiAvvio.servizio, myLogger);
                myServizioAgente.Avvio();
            } catch(Exception e) {
                myLogger.scriviExc(e, "Onstart");
            }

            Thread.Sleep(1000);
        }

        protected override void OnStop()
        {
            myLogger.scriviLog("Chiusura Servizio " + Parametri.NOME_SERVIZIO);

            if(myServizioAgente != null) {
                myServizioAgente.Chiudi();
                myServizioAgente = null;
            }

            oDB = null;

            Thread.Sleep(1000);
        }
    }
}