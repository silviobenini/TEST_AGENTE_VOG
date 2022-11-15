
using b2a.BlackBox;
using b2a.Database;
using b2a.Interfaccia;
using ExcelDataReader;

using System;
using System.Data;
using System.IO;
using System.Threading;




namespace Agente_VOG.SottoServizi
{

    class ImportPrgQuantitativi : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";
        private static string PATH_QUANTITATIVI = "Quantitativi\\";

        int idMagazzino = 0;
        int idCalibratrice = 0;


        private Log myLog = new Log();

        private readonly string[] path = { "Importati\\", "ImportatiConErrori\\", "NonImportati\\" };

        public ImportPrgQuantitativi(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "ImportPrgQuantitativi";
            _descrizioneLunga = "ImportPrgQuantitativi";
        }

        protected override void cicloEsecuzione()
        {
            scriviMsg("Ciclo in polling", livelloLog.prolisso);

            string pathFile;
            pathFile = oDB.ParamDB.leggi(NOME_MODULO, "pathAttachedEmail");

            string[] fileEntries = Directory.GetFiles(pathFile, "*.xls*");

            if (fileEntries.Length == 0)
                scriviMsg("Nessun File da importare !", livelloLog.prolisso);
            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains("programmaQuantitativi") == false)
                    continue;



                idMagazzino = 0;
                idCalibratrice = 0;
                if (leggiFileExcel(fileName))
                {
                    switch (myLog.error)
                    {
                        case Log.tipoErroreImport.noErrore:
                            myLog.esito = Log.resultImport.success;
                            scriviMsg("Import File :" + fileName + " .Completato con successo !", livelloLog.normale);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " .Completato con successo !";
                            spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                            break;
                        case Log.tipoErroreImport.saltataQualcheRiga:
                            myLog.esito = Log.resultImport.successWithError;
                            scriviMsg("Import File :" + fileName + " .Completato ma saltata qualche riga !", livelloLog.avviso);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " .Completato ma saltata qualche riga !";
                            spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                            break;
                        case Log.tipoErroreImport.noCalibratice:
                            scriviMsg("Import File :" + fileName + " .Completato ma saltata qualche riga per mancanza di calibratrice !", livelloLog.avviso);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " .Completato ma saltata qualche riga per mancanza di calibratrice !";
                            spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                            break;
                        case Log.tipoErroreImport.noBarcode:
                            myLog.esito = Log.resultImport.successWithError;
                            scriviMsg("Import File :" + fileName + " .Completato ma saltata qualche riga per mancanza di barcode !", livelloLog.avviso);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " .Completato ma saltata qualche riga per mancanza di barcode !";
                            spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                            break;
                        default:
                            // code block
                            break;
                    }
                }
                else
                {
                    switch (myLog.error)
                    {
                        case Log.tipoErroreImport.aperturaFile:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO causa problemi in Apertura File!", livelloLog.errore);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " NON COMPLETATO causa problemi in Apertura File!";
                            break;
                        case Log.tipoErroreImport.decodificaNomeFile:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO causa problemi in decodidica nome File!", livelloLog.errore);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " NON COMPLETATO causa problemi in decodidica nome File!";
                            break;
                        case Log.tipoErroreImport.letturaCampiXLS:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO causa problemi di lettura dei campi !", livelloLog.errore);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " NON COMPLETATO causa problemi di lettura dei campi !";
                            break;
                        default:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO!", livelloLog.errore);
                            myLog.msg += "\r\n" + "Import File :" + fileName + " NON COMPLETATO!";
                            break;

                    }
                    myLog.esito = Log.resultImport.NoSuccess;
                    spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                }


                myLog.insertLog(idMagazzino, idCalibratrice, new FileInfo(fileName).Name, oDB);
                Thread.Sleep(1000);
            }
            

        }

       



        protected override void finalizzazioneCiclo()
        {

        }

        protected override void inizializzazioneCiclo()
        {

        }



               private void spostaFile(string pathOrigine, string nomeFile, Log.resultImport tipoSpostamento)
        {
            try
            {
                string pathDestinazione = pathOrigine + PATH_QUANTITATIVI + path[(int)tipoSpostamento];
                string estensione = new FileInfo(nomeFile).Extension;
                string nomeFileDestinazione = nomeFile;
                if (File.Exists(pathDestinazione + nomeFile))
                {
                    string dataOra = DateTime.Now.ToString("yyyyMMddHHmmss");
                    int posExtension = nomeFile.IndexOf(estensione);
                    nomeFileDestinazione = nomeFileDestinazione.Substring(0, posExtension) + "_" + dataOra + "." + estensione;
                }

                Directory.CreateDirectory(pathDestinazione);

                File.Move(pathOrigine + nomeFile, pathDestinazione + nomeFileDestinazione);
            }
            catch (Exception ex)
            {
                scriviExc(ex);
            }
        }



        private Boolean leggiFileExcel(string fileNameConPath)
        {
            myLog.error = Log.tipoErroreImport.noErrore;
            Boolean result = true;
            string nomeFileAssoluto = new FileInfo(fileNameConPath).Name;
            int count = 0;



            b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_CalibraturaQuantitativi_InsMod");

            try
            {
                using (var stream = File.Open(fileNameConPath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        int indexCooperativa = 0;
                        int indexVarieta = 1;
                        int indexAnno = 2;
                        int indexSettimana = 3;
                        int indexTonDaLavorare = 4;
                        int indexTonProgrammate = 5;
                        do
                        {
                            while (reader.Read())
                            {


                                if (count > 0)
                                {



                                    mysp.parametro("@cooperativa", reader.GetString(indexCooperativa));
                                    mysp.parametro("@varietaBiometic", reader.GetString(indexVarieta));
                                    mysp.parametro("@anno", reader.GetValue(indexAnno));
                                    mysp.parametro("@settimana", reader.GetValue(indexSettimana));
                                    mysp.parametro("@tonDaLavorare", reader.GetDouble(indexTonDaLavorare));
                                    mysp.parametro("@tonProgrammate", reader.GetDouble(indexTonProgrammate));


                                    if (oDB.ExecuteSql(mysp))
                                    {
                                        scriviMsg("Salvataggio dei dati alla riga: " + count + " del file " + nomeFileAssoluto, livelloLog.prolisso);

                                    }
                                    else
                                    {
                                        scriviMsg("myLog.errore nel salvataggio dei dati alla riga: " + count + " del file " + nomeFileAssoluto, livelloLog.errore);
                                        myLog.msg += "\r\n" + "myLog.errore nel salvataggio dei dati alla riga: " + count + " del file " + nomeFileAssoluto;
                                        myLog.error = Log.tipoErroreImport.saltataQualcheRiga;
                                    }

                                }
                                count++;
                            }


                        } while (reader.NextResult());
                    }
                }

            }
            catch (Exception ex)
            {
                scriviMsg("myLog.errore generico nell'importazione  del file " + nomeFileAssoluto + " alla riga " + count, livelloLog.errore);
                myLog.msg += "\r\n" + "myLog.errore generico nell'importazione  del file " + nomeFileAssoluto + " alla riga ";
                myLog.error = Log.tipoErroreImport.aperturaFile;
                return false;
            }
            return result;
        }











    }
}
