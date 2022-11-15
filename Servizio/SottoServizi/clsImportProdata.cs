using b2a.BlackBox;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Net;

namespace Agente_VOG.SottoServizi
{
    class ImportProdata : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public ImportProdata(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "ImportProdata";
            _descrizioneLunga = "ImportProdata";
        }

        protected override void inizializzazioneCiclo()
        {
            aggiungiSottoservizioFiglio(new Anagrafiche("Anagrafiche", _controllore));
            aggiungiSottoservizioFiglio(new Giacenze("Giacenze", _controllore));
            aggiungiSottoservizioFiglio(new Ordini("Ordini", _controllore));
            aggiungiSottoservizioFiglio(new Biometic("Biometic", _controllore));
        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {

        }

    }


}