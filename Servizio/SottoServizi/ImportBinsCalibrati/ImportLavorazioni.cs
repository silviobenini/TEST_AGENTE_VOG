
using b2a.BlackBox;
using b2a.Database;
using b2a.Interfaccia;
using ExcelDataReader;

using System;
using System.Data;
using System.IO;
using System.Threading;




namespace Agente_VOG.SottoServizi.ImportBinsCalibrati
{

    class ImportLavorazioni : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";
        private static string PATH_LAVORAZIONI = "Lavorazioni\\";

        

        int idMagazzino = 0;
        int idCalibratrice = 0;
    

   
        private readonly string[] path = { "Importati\\", "ImportatiConErrori\\", "NonImportati\\" };

        private Log myLog = new Log();

        public ImportLavorazioni(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "ImportLavorazioni";
            _descrizioneLunga = "ImportazioneLavorazioni";
        }

        protected override void cicloEsecuzione()
        {
            scriviMsg("Ciclo in polling", livelloLog.prolisso);

            string pathFile;
            pathFile = oDB.ParamDB.leggi(NOME_MODULO, "pathAttachedEmail");

            string[] fileEntries = Directory.GetFiles(pathFile, "*.xls*");

            if (fileEntries.Length == 0)
            { 
                scriviMsg("Nessun File da importare !", livelloLog.prolisso);
                return;
            }
            foreach (string fileName in fileEntries)
            {
                if (fileName.Contains("programmaQuantitativi"))
                    continue;



  
                idMagazzino = 0;
                idCalibratrice = 0;
                if (nuoveoEX(fileName))
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
                            myLog.esito = Log.resultImport.successWithError;
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


                string msgErr = myLog.insertLog(idMagazzino, idCalibratrice, new FileInfo(fileName).Name, oDB);
                if (msgErr != "")
                    scriviMsg(msgErr, livelloLog.errore);
                Thread.Sleep(1000);
            }

            aggiornaTabella_import_DatiLavBio();
        }


        private void aggiornaTabella_import_DatiLavBio()
        {


            b2a.Database.StoredProcedure myspUpdate = new b2a.Database.StoredProcedure("_DatiLavBioRicrea");
            myspUpdate.parametro("@tipoImport", 1);

            if (oDB.ExecuteSql(myspUpdate))
                scriviMsg("Importa lavorazioni -  AggiornataTabella  import_DatiLavBio");

        }


        protected override void finalizzazioneCiclo()
        {

        }

        protected override void inizializzazioneCiclo()
        {

        }



        private int trovaIdCalibratrice(string nomeSuFileExcel)
        {
            int result = 0;
            int nrFile = 0;
            try
            {


                StoredProcedure mysp = new StoredProcedure("Agente_VOG_DatiLavorazione_TrovaCalibratrice");
                mysp.parametro("@nomeCalibratrice", nomeSuFileExcel);
                DataSet myDs = oDB.SelectSqlDataSet(mysp);
                DataRow resultRow = myDs.Tables[0].Rows[0];
                idCalibratrice = (int)resultRow["idCalibratrice"];
                idMagazzino = (int)resultRow["idMagazzino"];
                result = idCalibratrice;


            }
            catch
            {
                result = 0;
            }

            return result;

        }


        private void spostaFile(string pathOrigine, string nomeFile, Log.resultImport tipoSpostamento)
        {
            try
            {
                string pathDestinazione = pathOrigine + PATH_LAVORAZIONI + path[(int)tipoSpostamento];
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



        private Boolean nuoveoEX(string fileNameConPath)
        {
            myLog.error = Log.tipoErroreImport.noErrore;
            Boolean result = true;
            string nomeFileAssoluto = new FileInfo(fileNameConPath).Name;
            int count = 0;
            b2a.Database.StoredProcedure mysp = new b2a.Database.StoredProcedure("Agente_VOG_DatiLavorazione_InsMod");

            try
            {
                using (var stream = File.Open(fileNameConPath, FileMode.Open, FileAccess.Read))
                {
                    // Auto-detect format, supports:
                    //  - Binary Excel files (2.0-2003 format; *.xls)
                    //  - OpenXml Excel files (2007 format; *.xlsx, *.xlsb)
                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {

                        int indexBarCode = int.MaxValue;
                        int indexData = int.MaxValue;
                        int indexQualita = int.MaxValue;
                        int indexCalibro = int.MaxValue;
                        int indexColore = int.MaxValue;
                        int indexTrattamento = int.MaxValue;
                        int indexCalibratrice = int.MaxValue;
                        int indexVarieta = int.MaxValue;
                        int indexPeso = int.MaxValue;
                        int indexNrFrutti = int.MaxValue;
                        int indexPercentuale = int.MaxValue;
                        int indexPesoMedio = int.MaxValue;

                        do
                        {
                            while (reader.Read())
                            {


                                if (count == 0)
                                {
                                    int index = 1;


                                    for (int c = 0; c < reader.FieldCount; c++)
                                    {
                                        if (reader.GetString(c) != null)
                                        {
                                            try
                                            {
                                                index = c;
                                                if (c == 0)
                                                    //if ( ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, c].Value.ToString().Equals("+"))
                                                    index = 1;
                                                //else
                                                // index = 1;
                                                if (reader.GetString(c).Equals("Kistencode"))
                                                    indexBarCode = index;
                                                if (reader.GetString(c).Equals("Sortierdatum"))
                                                    indexData = index;
                                                if (reader.GetString(c).Equals("Qualitätbeschr."))
                                                    indexQualita = index;
                                                if (reader.GetString(c).Equals("Farbebeschr.") || reader.GetString(c).Equals("Farbe"))
                                                    indexColore = index;
                                                if (reader.GetString(c).Equals("Beschr. Anbauart") || reader.GetString(c).Equals("Anbauart"))
                                                    indexTrattamento = index;
                                                if (reader.GetString(c).Equals("Kaliberbeschr."))
                                                    indexCalibro = index;
                                                if (reader.GetString(c).Equals("Lieferant"))
                                                    indexCalibratrice = index;
                                                if (reader.GetString(c).Equals("Sortenbeschr.") || reader.GetString(c).Equals("Sortenbeschr. (etikett)"))
                                                    indexVarieta = index;
                                                if (reader.GetString(c).Equals("Kistengewicht"))
                                                    indexPeso = index;
                                                if (reader.GetString(c).Equals("Anzahl Äpfel"))
                                                    indexNrFrutti = index;
                                                if (reader.GetString(c).Equals("Füllung %"))
                                                    indexPercentuale = index;
                                                if (reader.GetString(c).Equals("Durchschnittsgewicht Apfel"))
                                                    indexPesoMedio = index;
                                            }
                                            catch (Exception e)
                                            {
                                                myLog.error = Log.tipoErroreImport.letturaCampiXLS;
                                                result = false;
                                                return false;
                                                scriviMsg(e.Message, livelloLog.errore);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    try
                                    {
                                        string x = reader.GetString(indexBarCode);
                                    }
                                    catch (System.InvalidCastException ex)
                                    {
                                        scriviMsg("arrivato alla fine del file " + nomeFileAssoluto + " dopo righe: " + count, livelloLog.normale);
                                        myLog.error = Log.tipoErroreImport.noErrore;
                                        return true;
                                    }


                                    if (reader.GetString(indexBarCode) != null)
                                    {
                                        if (indexBarCode == int.MaxValue)
                                        {
                                            myLog.error = Log.tipoErroreImport.letturaCampiXLS;
                                            scriviMsg("nessun barcode nel file " + nomeFileAssoluto, livelloLog.normale);
                                            myLog.msg += "\r\n" + "nessun barcode nel file " + nomeFileAssoluto;
                                            return false;
                                        }
                                        if (indexCalibratrice == int.MaxValue)
                                        {
                                            myLog.error = Log.tipoErroreImport.letturaCampiXLS;
                                            scriviMsg("nessuna calibratrice nel file " + nomeFileAssoluto, livelloLog.normale);
                                            myLog.msg += "\r\n" + "nessuna calibratrice nel file " + nomeFileAssoluto;
                                            return false;
                                        }




                                        if ((indexBarCode == 0 || indexBarCode == 1) && reader.GetString(indexBarCode).Equals("+"))
                                        {
                                            if (indexBarCode == 0)
                                                indexBarCode = 1;
                                            else
                                                indexBarCode = 0;
                                        }




                                        if (reader.GetString(indexVarieta) != null)
                                        {
                                            if ((indexVarieta == 0 || indexVarieta == 1) && reader.GetString(indexVarieta).Equals("+"))
                                            {
                                                if (indexVarieta == 0)
                                                    indexVarieta = 1;
                                                else
                                                    indexVarieta = 0;
                                            }
                                        }
                                        else
                                            indexVarieta = 0;



                                        if (indexBarCode != int.MaxValue)
                                            mysp.parametro("@Barcode", reader.GetString(indexBarCode));

                                        if (indexData != int.MaxValue)
                                            mysp.parametro("@Data", reader.GetDateTime(indexData));

                                        if (indexColore != int.MaxValue)
                                            mysp.parametro("@Colore", reader.GetString(indexColore));

                                        if (indexTrattamento != int.MaxValue)
                                            mysp.parametro("@Trattamento", reader.GetString(indexTrattamento));

                                        if (indexCalibratrice != int.MaxValue)
                                        {
                                            mysp.parametro("@Calibratrice", reader.GetString(indexCalibratrice));

                                            int idCalibratrice = trovaIdCalibratrice((reader.GetString(indexCalibratrice)));
                                            if (idCalibratrice == 0)
                                            {
                                                scriviMsg("dati nel file " + nomeFileAssoluto + " alla riga " + count + " non salvati per mancanza calibratrice", livelloLog.errore);
                                                myLog.msg += "\r\n" + "dati nel file " + nomeFileAssoluto + " alla riga " + count + " non salvati per mancanza calibratrice";
                                                myLog.error = Log.tipoErroreImport.noCalibratice;
                                                //return false;
                                            }
                                        }

                                        if (indexCalibro != int.MaxValue)
                                            mysp.parametro("@Calibro", reader.GetString(indexCalibro));

                                        if (indexVarieta != int.MaxValue)
                                            mysp.parametro("@Varieta", reader.GetString(indexVarieta));

                                        if (indexPeso != int.MaxValue)
                                            mysp.parametro("@Peso", reader.GetDouble(indexPeso));

                                        if (indexNrFrutti != int.MaxValue)
                                            mysp.parametro("@NrFrutti", reader.GetDouble(indexNrFrutti));

                                        if (indexPercentuale != int.MaxValue)
                                            mysp.parametro("@Percentuale", reader.GetString(indexPercentuale));

                                        if (indexQualita != int.MaxValue)
                                            mysp.parametro("@Qualita", reader.GetString(indexQualita));

                                        if (indexPesoMedio != int.MaxValue)
                                            mysp.parametro("@PesoMedioMela", reader.GetDouble(indexPesoMedio));

                                        mysp.parametro("@FileName", nomeFileAssoluto);
                                        mysp.parametro("@nrRigaFile", count);

                                        if (oDB.ExecuteSql(mysp))
                                        {
                                            if (indexBarCode != int.MaxValue && indexCalibratrice != int.MaxValue)
                                                scriviMsg("dati Riga nr :" + count.ToString() + " salvati correttamente", livelloLog.prolisso);
                                            else
                                            {
                                                if (indexBarCode == int.MaxValue)
                                                {
                                                    scriviMsg("dati nel file " + nomeFileAssoluto + " alla riga " + count + " non salvati per mancanza barcode", livelloLog.errore);
                                                    myLog.msg += "\r\n" + "dati nel file " + nomeFileAssoluto + " alla riga " + count + " non salvati per mancanza barcode";
                                                    myLog.error = Log.tipoErroreImport.noBarcode;
                                                }
                                                if (indexCalibratrice == int.MaxValue)
                                                {
                                                    scriviMsg("dati nel file " + nomeFileAssoluto + " alla riga " + count + " non salvati per mancanza calibratrice", livelloLog.errore);
                                                    myLog.msg += "\r\n" + "dati nel file " + nomeFileAssoluto + " alla riga " + count + " non salvati per mancanza calibratrice";
                                                    myLog.error = Log.tipoErroreImport.noCalibratice;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            scriviMsg("myLog.errore nel salvataggio dei dati alla riga: " + count + " del file " + nomeFileAssoluto, livelloLog.errore);
                                            myLog.msg += "\r\n" + "myLog.errore nel salvataggio dei dati alla riga: " + count + " del file " + nomeFileAssoluto;
                                            myLog.error = Log.tipoErroreImport.saltataQualcheRiga;
                                        }

                                    }
                                    //else
                                    //    count++;

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
