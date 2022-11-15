using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Agente_VOG.SottoServizi
{
    class ExportProdata : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public ExportProdata(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "ExportProdata";
            _descrizioneLunga = "ExportProdata";
        }

        protected override void inizializzazioneCiclo()
        {
            aggiungiSottoservizioFiglio(new Conferimenti("Conferimenti", _controllore));

        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {

        }

    }


}