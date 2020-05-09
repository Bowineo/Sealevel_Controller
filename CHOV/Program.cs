using System;
using System.Diagnostics;
using System.Windows.Forms;



namespace CHOV
{
    static class Program
    {
        /// <summary>
        /// Ponto de entrada principal para o aplicativo.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // Obtém o nome do processo atual (esta aplicação)
            string meuProcesso = Process.GetCurrentProcess().ProcessName;

            // Procura o processo atual na lista de processos que estão a ser executados neste momento, no computador
            Process[] processos = Process.GetProcessesByName(meuProcesso);

            // Além desta instância, já existe mais alguma?
            if (processos != null && processos.Length > 1)

            {
                // Mostra uma mensagem, e termina esta instância...
                MessageBox.Show("Já tem uma instância aberta do programa! É permitido apenas umas por vez.", "AVISO");
            }

            else
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmPainel());

            }


        }
    }
}