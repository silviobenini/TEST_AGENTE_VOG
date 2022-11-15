
using Agente_VOG.SottoServizi.ImportBinsCalibrati;
using b2a.BlackBox;
using b2a.Interfaccia;


namespace Agente_VOG.SottoServizi
{
    class clsImportQuantitativi : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public clsImportQuantitativi(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "ImportQuantitativiProgrammati";
            _descrizioneLunga = "ImportQuantitativiProgrammati";
        }

        protected override void inizializzazioneCiclo()
        {
            aggiungiSottoservizioFiglio(new ImportPrgQuantitativi("ImportPrgQuantitativi", _controllore));
        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {

        }

    }


}