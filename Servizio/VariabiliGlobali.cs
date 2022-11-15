using System;
using System.Collections.Generic;
using System.Text;
using b2a.Interfaccia;
using b2a.BlackBox;
using b2a.Database;

namespace Servizio
{
    static class VariabiliGlobali
    { 
        public static clsServizioAgente myServizioAgente;
        public static clsLogSuFile myLogger;

        public static SqlSrv_Interface oDB;

        public static void impostaVariabiliGenerali()
        {
            Parametri.NOME_CANALE = "CanaleAgente_VOG";
            Parametri.NOME_SERVIZIO = "Agente_VOG";
            Parametri.NOME2_SERVIZIO = "B2A Agente VOG";
            Parametri.DESCR_SERVIZIO = "Servizio di dialogo con Prodata";

            Parametri.CHIAVE_REGISTRO = @"Software\B2A\BTrace\Agente_VOG";
            Parametri.NOME_MODULO = "Agente_VOG";
        }
    }
}