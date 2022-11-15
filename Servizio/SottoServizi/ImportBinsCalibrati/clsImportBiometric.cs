using b2a.BlackBox;
using b2a.Database;
using b2a.Interfaccia;
using Newtonsoft.Json;
using System;
using System.Data;
using System.IO;
using System.Net;
using System.Threading;

namespace Agente_VOG.SottoServizi
{
    class importBiometric : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";
        private static string PATH_BIOMETRIC = "Biometric\\";


        int idMagazzino = 0;
        int idCalibratrice = 0;

        private Log myLog = new Log();

        private readonly string[] path =  {"Importati\\","ImportatiConErrori\\","NonImportati\\"};

        public importBiometric(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "importBiometric";
            _descrizioneLunga = "importBiometric";
        }

        protected override void inizializzazioneCiclo()
        { 
        }

        protected override void finalizzazioneCiclo()
        {
             
        }

        protected override void cicloEsecuzione()
        {
            string pathFile;
            pathFile = oDB.ParamDB.leggi(NOME_MODULO, "pathAttachedEmail");

            string[] fileEntries =  Directory.GetFiles(pathFile,"*.txt");

            if (fileEntries.Length == 0)
                scriviMsg("Nessun File da importare !", livelloLog.prolisso);
            foreach (string fileName in fileEntries)
            {
                idMagazzino = 0;
                idCalibratrice = 0;
                if (importFileBiometric(fileName))
                {


                    switch (myLog.error)
                    {
                        case Log.tipoErroreImport.noErrore:
                            myLog.esito = Log.resultImport.success;
                            scriviMsg("Import File :" + fileName + " .Completato con successo !", livelloLog.normale);
                             myLog.msg  += "\r\n" + "Import File :" + fileName + " .Completato con successo !";
                            spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                            break;
                        case Log.tipoErroreImport.saltataQualcheRiga:
                            myLog.esito = Log.resultImport.successWithError;
                            scriviMsg("Import File :" + fileName + " .Completato ma saltata qualche riga !", livelloLog.avviso);
                            myLog.msg += "\r\n" +  "Import File :" + fileName + " .Completato ma saltata qualche riga !";
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
                            myLog.msg += "\r\n" +  "Import File: " + fileName + " NON COMPLETATO causa problemi in Apertura File!";
                            break;
                        case Log.tipoErroreImport.decodificaNomeFile:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO causa problemi in decodidica nome File!", livelloLog.errore);
                            myLog.msg += "\r\n" +  "Import File :" + fileName + " NON COMPLETATO causa problemi in decodidica nome File!";
                            break;
                        case Log.tipoErroreImport.noCalibratice:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO causa problemi calibratrice non identificata !", livelloLog.errore);
                            myLog.msg += "\r\n" +  "Import File :" + fileName + " NON COMPLETATO causa problemi calibratrice non identificata !";
                            break;
                        default:
                            scriviMsg("Import File :" + fileName + " NON COMPLETATO!", livelloLog.errore);
                            myLog.msg += "\r\n" +  "Import File :" + fileName + " NON COMPLETATO!";
                            break;

                    }
                    myLog.esito = Log.resultImport.NoSuccess;
                    spostaFile(pathFile, new FileInfo(fileName).Name, myLog.esito);
                }
                Thread.Sleep(1000);

                string msgErr = myLog.insertLog(idMagazzino, idCalibratrice, new FileInfo(fileName).Name, oDB);
                if (msgErr != "")
                    scriviMsg(msgErr, livelloLog.errore);
            }
        }


               private Boolean importFileBiometric (string fileNameConPath)
        {
            myLog.error = Log.tipoErroreImport.noErrore;

            Boolean result = true;

            scriviMsg("Trovato nuovo File:" + fileNameConPath, livelloLog.prolisso);

            string nomeFileAssoluto = new FileInfo(fileNameConPath).Name;

            scriviMsg("Inizio Import File:" + nomeFileAssoluto, livelloLog.normale);

            try
            {
                

                string nrFileBiometric = "";
                string varieta = "";
                string strData = "";

                int lenDate = 0;

                int anno = 0;
                int giorno = 0;
                int mese = 0;

                DateTime data;
                try
                {

                    string[] decodFile = nomeFileAssoluto.Split('-');

                    nrFileBiometric = decodFile[0];
                    varieta = decodFile[1];
                    strData = decodFile[2].Substring(0, 8);





                    lenDate = strData.Length;

                    anno = int.Parse(strData.Substring(0, 4));
                    mese = int.Parse(strData.Substring(4, 2));
                    giorno = int.Parse(strData.Substring(6, 2));
                    data = new DateTime(anno, mese, giorno);

                }
                catch
                {
                    myLog.error = Log.tipoErroreImport.decodificaNomeFile;
                    result = false;
                    return result;
                }


                

                int idCalibratrice = trovaIdCalibratrice(nrFileBiometric);

                if (idCalibratrice > 0) // significa che ha identificato la calibratrice e quindi procedo
                {
                    int counter = 0;

                    if (File.Exists(fileNameConPath))
                        // Read the file and display it line by line.  
                        foreach (string line in System.IO.File.ReadLines(fileNameConPath, System.Text.Encoding.Unicode))
                        {
                            scriviMsg("nr Riga" + counter.ToString() + " - " + line,livelloLog.prolisso);
                            counter++;

                            string[] valoriRiga = line.Split(',');

                            string codice ;
                            string nrFrutti ;
                            string peso ;
                            string percentuale;

                            try
                            {
                                codice = valoriRiga[0];
                                nrFrutti = valoriRiga[1];
                                peso = valoriRiga[2];
                                percentuale = valoriRiga[3];
                            }
                            catch
                            {
                                scriviMsg("Errore Formato Riga. File:" + nomeFileAssoluto + ", Riga:" + counter.ToString(), livelloLog.errore);
                                myLog.msg += "\r\n" + "Errore Formato Riga. File:" + nomeFileAssoluto + ", Riga:" + counter.ToString();
                                myLog.error = Log.tipoErroreImport.saltataQualcheRiga;
                                continue;
                            }

                            string classe;
                            string qualita;
                            string colore;
                            string calibro;

                            StoredProcedure mysp = new StoredProcedure("Agente_VOG_Bio_TrovaDati");
                            mysp.parametro("@codice", codice);
                            DataSet myDs = oDB.SelectSqlDataSet(mysp);
                            if (myDs.Tables != null &&  myDs.Tables.Count > 0 && myDs.Tables[0].Rows.Count > 0)
                            {
                                DataRow resultRow = myDs.Tables[0].Rows[0];
                                classe = resultRow["classe"].ToString();
                                qualita = resultRow["qualita"].ToString();
                                colore = resultRow["colore"].ToString();
                                calibro = resultRow["calibro"].ToString();

                            }
                            else
                            {
                                scriviMsg("Non trovo la decodifica Agente_VOG_Bio_TrovaDati "+ codice.ToString()+". File:" + nomeFileAssoluto + ", Riga:" + counter.ToString(), livelloLog.errore);
                                myLog.msg += "\r\n" + "Non trovo la decodifica Agente_VOG_Bio_TrovaDati " + codice.ToString() + ". File:" + nomeFileAssoluto + ", Riga:" + counter.ToString();
                                myLog.error = Log.tipoErroreImport.saltataQualcheRiga;
                                continue;
                            }
                            

                            mysp = new StoredProcedure("Agente_VOG_Bio_Dati_InsMod");
                            mysp.parametro("@dataCalibrazione", data);
                            mysp.parametro("@idCalibratrice", idCalibratrice);
                            mysp.parametro("@varieta", varieta);
                            mysp.parametro("@classe", classe);
                            mysp.parametro("@qualita", qualita);
                            mysp.parametro("@colore", colore);
                            mysp.parametro("@calibro", calibro);
                            mysp.parametro("@peso", peso);
                            mysp.parametro("@nrFrutti", nrFrutti);
                            mysp.parametro("@percentuale", percentuale);
                            mysp.parametro("@nomeFile", nomeFileAssoluto);
                            mysp.parametro("@rigaFile", counter);


                            if (oDB.ExecuteSql(mysp))
                            {
                                scriviMsg("Inserita nuova riga dati Biometric. File:" + nomeFileAssoluto + ", Riga:" + counter.ToString(), livelloLog.prolisso);

                            }
                            else
                            {
                                myLog.error = Log.tipoErroreImport.saltataQualcheRiga;
                                scriviMsg("Fallito Inserimento Dati Biometric. File:" + nomeFileAssoluto + ", Riga:" + counter.ToString(), livelloLog.errore);
                                myLog.msg += "\r\n" +  "Fallito Inserimento Dati Biometric. File:" + nomeFileAssoluto + ", Riga:" + counter.ToString();
                            }

                        }


                    else
                    {
                        result = false;
                        myLog.error = Log.tipoErroreImport.aperturaFile;
                    }
                }
                else
                {
                    result = false;
                    myLog.error = Log.tipoErroreImport.noCalibratice;
                }
                    
            }
            catch (Exception ex)
            {
                scriviExc(ex);
                result = false;
            }


            return result;

        }


        private int trovaIdCalibratrice(string nrFileBiometric)
        {
            int result = 0;
            int nrFile = 0;
            try
            {
                nrFile = int.Parse(nrFileBiometric);

                StoredProcedure mysp = new StoredProcedure("Agente_VOG_Bio_TrovaCalibratrice");
                mysp.parametro("@nrFile", nrFile);
                DataSet myDs = oDB.SelectSqlDataSet(mysp);
                DataRow resultRow = myDs.Tables[0].Rows[0];
                idMagazzino = (int)resultRow["idMagazzino"];
                idCalibratrice = (int)resultRow["idCalibratrice"];
                result = idCalibratrice;

            }
            catch
            {
                result = 0;
            }
            
            return result;

        }

        private void spostaFile (string pathOrigine, string nomeFile , Log.resultImport tipoSpostamento )
        {
            try
            {
                string pathDestinazione = pathOrigine + PATH_BIOMETRIC + path[(int)tipoSpostamento];
                string nomeFileDestinazione = nomeFile;
                string estensione = new FileInfo(nomeFile).Extension;
                if (File.Exists(pathDestinazione + nomeFile))
                    {
                    string dataOra = DateTime.Now.ToString("yyyyMMddHHmmss");
                    int posExtension = nomeFile.IndexOf(estensione);
                    nomeFileDestinazione = nomeFileDestinazione.Substring(0, posExtension) + "_" + dataOra + estensione;
                }
               
                
                Directory.CreateDirectory(pathDestinazione);
                File.Move(pathOrigine + nomeFile, pathDestinazione + nomeFileDestinazione);
            }
            catch(Exception ex)
            {
                scriviExc(ex);
            }


        }


    }
}
