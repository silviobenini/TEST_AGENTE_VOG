
using Agente_VOG.SottoServizi.ImportBinsCalibrati;
using b2a.BlackBox;
using b2a.Interfaccia;


namespace Agente_VOG.SottoServizi
{
    class clsImportBinsCalibrati : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public clsImportBinsCalibrati(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "ImportBinsCalibrati";
            _descrizioneLunga = "ImportBinsCalibrati";
        }

        protected override void inizializzazioneCiclo()
        {
            aggiungiSottoservizioFiglio(new ImportLavorazioni("ImportLavorazioni", _controllore));
            /*
             SOSTITUITO DA IMPORT WS PRODATA ....VEDI CLASSE Biometic
            aggiungiSottoservizioFiglio(new importBiometric("importBiometric", _controllore));
            */
        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {

        }

    }


}