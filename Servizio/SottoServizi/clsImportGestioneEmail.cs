using b2a.BlackBox;
using b2a.Interfaccia;


namespace Agente_VOG.SottoServizi
{
    class clsImportGestioneEmail : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public clsImportGestioneEmail(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "GestioneEmail";
            _descrizioneLunga = "GestioneEmail";
        }

        protected override void inizializzazioneCiclo()
        {
            aggiungiSottoservizioFiglio(new GestioneEmail("GestioneImportEmail", _controllore));

        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {
            
        }       

    }
}