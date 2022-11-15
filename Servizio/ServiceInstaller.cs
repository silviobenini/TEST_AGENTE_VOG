using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;
using System.Threading.Tasks;
using static Servizio.VariabiliGlobali;
using b2a.BlackBox;
using b2a.Interfaccia;
using System.ServiceProcess;
using System.Diagnostics;
using Microsoft.VisualBasic;

namespace Agente_TelegramBot
{
    [RunInstaller(true)]
    public partial class ServiceInstaller : System.Configuration.Install.Installer
    {
        //AZIONI  DI RIPRISTINO DEL SERVIZIO IN CASO DI MALFUNZIONAMENTO
        //Formato: AZIONE_DA_ESEGUIRE/RITARDO
        //Possibili azioni:
        //  restart -> riavvia il servizio
        //  reboot  -> riavvia il computer
        //  run     -> avvia un programma esterno (via percorso)
        //NB: RITARDO ESPRESSO IN MS
        //
        //ES: REBOOT/60000 ->RIAVVIA IL COMPUTER DOPO 1 MINUTO

        string azioneRipristinoPrimoTentativo = "restart/60000";
        string azioneRipristinoSecondoTentativo = "restart/30000";
        string azioneRipristinoTerzoTentativo = "restart/6000";

        public ServiceInstaller()
        {
            InitializeComponent();

            impostaVariabiliGenerali();

            serviceInstaller1.ServiceName = Parametri.NOME_SERVIZIO;
            serviceInstaller1.DisplayName = Parametri.NOME2_SERVIZIO;
            serviceInstaller1.Description = Parametri.DESCR_SERVIZIO;
            //definisce la modalità di avvio automatico del sevizio
            serviceInstaller1.StartType = ServiceStartMode.Automatic;

            //-----------------------------------------'
            // Definizione dell'account per il servizio '
            //---------------------------------------- - '
            // Nel caso in cui si abbiano dei parametri che dipendono dall'utente (ad esempio le impostazioni
            //   stampanti, o così via) occorre specificare l'utente, o indicandolo qui con login e pwd
            //   ServiceProcessInstaller1.Username = "DOMINIO\utente"
            // ServiceProcessInstaller1.Password = "password"
            //   oppure facendo comparire la schermata in fase di installazione servizio
            //   ServiceProcessInstaller1.Account = ServiceProcess.ServiceAccount.User
            // altrimenti si può lasciare l'utente <LocalSystem>
            //   ServiceProcessInstaller1.Account = ServiceProcess.ServiceAccount.LocalSystem

            //Nota: devo impostare un utente(e non LocalService o LocalSystem) per avere le impostazioni corrette delle stampanti

            serviceProcessInstaller1.Account = System.ServiceProcess.ServiceAccount.LocalSystem;

            //Necessario per settare le impostazioni di ripristino del servizio in caso di malfunzionamenti
            //Quando l'installazione è effettuata(tutte le modifiche sono 'committed') esegue il metodo ImpostaOpzioniRipristino

            serviceInstaller1.Committed += ImpostaOpzioniRipristino;
        }

        private void ImpostaOpzioniRipristino(object sender, InstallEventArgs e)
        {
            try {
                Process.Start("cmd.exe /c sc failure " + "\"\"" + Parametri.NOME_SERVIZIO + "\"\"" + " reset= 0 actions= " + azioneRipristinoPrimoTentativo + "/" + azioneRipristinoSecondoTentativo + "/" + azioneRipristinoTerzoTentativo + " & echo 'Impostazioni di ripristino servizio settate..' & pause & exit", AppWinStyle.NormalFocus.ToString());
            } catch (Exception ex) { }
        }
    }
}
