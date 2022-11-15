using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using b2a.BlackBox;
using b2a.Interfaccia;
using static Servizio.VariabiliGlobali;

namespace Servizio
{
    class clsAvvioConsole
    {
        static void Main(string[] args)
        {
            clsAvvioConsole oRoot = new clsAvvioConsole();
        }

        public clsAvvioConsole()
        {
            myLogger = new clsLogSuFile();
            impostaVariabiliGenerali();
            Run();
        }

        public void Run()
        {
            try {
                Console.WriteLine("Avvio agente");

                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                String nome = assembly.GetName().Name;
                System.Threading.Mutex mtxControlloMultiIstanza = new System.Threading.Mutex(false, nome);

                if(!mtxControlloMultiIstanza.WaitOne(1, false)) {
                    MessageBox.Show("Applicazione <" + nome + "> gia attiva", "Application", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    // Esce dal programma
                    return;
                }

                myServizioAgente = new clsServizioAgente2(clsServizioAgente.tipiAvvio.console, myLogger);
                myServizioAgente.Avvio();
            } catch(Exception ex) {
                myLogger.scriviExc(ex);
            }

            //una volta lanciato il programma, resta in un'attesa indefinita
            bool bRun = true;

            do {
                try {
                    System.Threading.Thread.Sleep(20000);

                } catch (Exception e) {
                    myLogger.scriviExc(e);
                    bRun = true;
                }
            } while (bRun == true);

            GC.Collect();
        }
    }
}
