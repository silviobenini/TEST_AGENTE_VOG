using b2a.BlackBox;
using b2a.Interfaccia;
using MailKit;
using MailKit.Net.Imap;
using MailKit.Search;
using MimeKit;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace Agente_VOG.SottoServizi
{

    class GestioneEmail : cls_SottoServizio
    {
        private static string NOME_MODULO = "Agente_VOG";

        public GestioneEmail(string nome, cls_Controllore controllore) : base(nome, controllore)
        {
            _descrizioneBreve = "GestioneEmail";
            _descrizioneLunga = "GestioneEmail";
        }

        protected override void inizializzazioneCiclo()
        {

        }

        protected override void finalizzazioneCiclo()
        {

        }

        protected override void cicloEsecuzione()
        {
            scriviMsg("Ciclo in polling", livelloLog.prolisso);
            leggiAllegati();
        }


        //protected void leggiAllegati()
        //{

        //    String esito = null;

        //    oDB.ParamDB.leggi(NOME_MODULO, "pathAttachedEmail", ref esito);

        //    if (esito.Length > 0)
        //    {
        //        using (Imap imap = new Imap())
        //        {
                    
        //                imap.Connect("imaps.aruba.it");   // or ConnectSSL for SSL
        //                imap.UseBestLogin("b2a@vogcloud.it", "VogCloudB2@2022!");
        //            }
        //            catch (Exception e) {
        //                scriviMsg(e.Message, livelloLog.errore);
        //            }
                    

        //            imap.SelectInbox();
        //            List<long> uids = imap.Search(Flag.All);

        //            foreach (long uid in uids)
        //            {
        //                IMail email = new MailBuilder().CreateFromEml(imap.GetMessageByUID(uid));
        //                CommonFolders common = new CommonFolders(imap.GetFolders());

        //                // save all attachments to disk
        //                foreach (MimeData mime in email.Attachments)
        //                {
        //                    try {
        //                        mime.Save(@esito + mime.SafeFileName);
        //                        scriviMsg("file salvato in " + esito + ": " + mime.SafeFileName, livelloLog.normale);
        //                    } catch (Exception e) {
        //                        scriviMsg(e.Message, livelloLog.errore);
        //                    } 
        //                }
        //                // imap.DeleteMessageByUID(uid);
        //                //imap.MoveByUID(uid, common.Trash);
        //            }
        //            imap.Close();
        //        }
        //    }
        //    else
        //        scriviMsg("percorso di salvataggio file non trovato!", livelloLog.errore);
        //}



        private void leggiAllegati()
        {
            String esito = null;

            oDB.ParamDB.leggi(NOME_MODULO, "pathAttachedEmail", ref esito);

            if (esito.Length > 0)
            {
                using (var client = new ImapClient())
                {
                    try
                    {
                        client.Connect("imaps.aruba.it", 993);
                        client.Authenticate("b2a@vogcloud.it", "VogCloudB2@2022!");
                    }
                    catch (Exception e)
                    {
                        scriviMsg(e.Message, livelloLog.errore);
                    }

                    client.Inbox.Open(MailKit.FolderAccess.ReadWrite);
                    IList<UniqueId> uids = client.Inbox.Search(SearchQuery.All);

                    foreach (UniqueId uid in uids)
                    {
                        MimeMessage message = client.Inbox.GetMessage(uid);

                        foreach (var attachment in message.Attachments)
                        {
                            var fileName = attachment.ContentDisposition?.FileName ?? attachment.ContentType.Name;

                            using (var stream = File.Create(esito + fileName))
                            {
                                if (attachment is MessagePart)
                                {
                                    var rfc822 = (MessagePart)attachment;
                                    rfc822.Message.WriteTo(stream);
                                }
                                else
                                {
                                    var part = (MimePart)attachment;
                                    part.Content.DecodeTo(stream);
                                }

                                if (File.Exists(esito + fileName))
                                {
                                    scriviMsg("file creato: " + esito + fileName, livelloLog.normale);                                    
                                }
                                else
                                    scriviMsg("file non creato: " + esito + fileName, livelloLog.errore);
                            }
                        }

                        try
                        {
                            client.Inbox.MoveTo(uid, client.GetFolder(SpecialFolder.Trash));
                        }
                        catch (Exception e) {
                            scriviMsg("email non spostata nel cestino", livelloLog.normale);
                        }
                    }
                }
            }else
                scriviMsg("percorso di salvataggio file non trovato!", livelloLog.errore);
        }



    }
}