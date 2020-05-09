using log4net;
using System;
using System.Windows.Forms;

namespace CHOV
{
    public partial class FrmLogs : Form
    {
        readonly FrmPainel frmP;

        public FrmLogs(FrmPainel frm1)
        { InitializeComponent(); frmP = frm1; }

        //variavel para Logs
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        /// <summary>
        /// Load Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLogs_Shown(object sender, EventArgs e)
        { HistStart(); TimerLogs.Start(); log.Debug("Form Logs carregado"); }

        /// <summary>
        /// Função obter novas inserções no histórico
        /// </summary>
        public void Histt()
        {
            LblNitensHistorico.Text = "Diferential historical - Logs:" + (frmP.Historico.Items.Count - Historico1.Items.Count).ToString();
            label1LblNitensLog.Text = "Number of historical readings:" + frmP.Historico.Items.Count.ToString();
            if ((frmP.Historico.Items.Count - Historico1.Items.Count) == 2)
            { Historico1.Items.Insert(0, frmP.ArrayHistoric[frmP.ArrayHistoric.Count - 2]); Historico1.Items.Insert(0, frmP.ArrayHistoric[frmP.ArrayHistoric.Count - 1]); }
            if ((frmP.Historico.Items.Count - Historico1.Items.Count) == 1) { Historico1.Items.Insert(0, frmP.ArrayHistoric[frmP.ArrayHistoric.Count - 1]); }
        }


        /// <summary>
        /// Função inicial para obter o histórico
        /// </summary>
        public void HistStart()
        { string[] vetor0 = frmP.ArrayHistoric.ToArray(); Array.Reverse(vetor0); Historico1.Items.AddRange((vetor0)); }

        /// <summary>
        /// Teclas de atalho
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmLogs_KeyDown(object sender, KeyEventArgs e)
        { if (e.Shift && e.KeyCode == Keys.P) { Funcoes.ReturnPanel(); log.Debug("Atalho painel acionado"); } }

        /// <summary>
        /// Relogio Log
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TimerLogs_Tick(object sender, EventArgs e)
        { Histt(); LblRelogioLog.Text = frmP.Data_pgm + "   " + frmP.Relogio_pgm; LblRelogioLog.Visible = true; }

        /// <summary>
        /// Export Logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnExport_Click(object sender, EventArgs e)
        {
            string path;
            using (FolderBrowserDialog folderDlg = new FolderBrowserDialog
            { ShowNewFolderButton = true })
            {
                DialogResult result = folderDlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    path = folderDlg.SelectedPath.Replace(@"\", @"/");
                    _ = folderDlg.RootFolder;
                    string nome = "Log_Operation_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".log";
                    string nomeArquivo = path + "/" + nome;
                    if (!System.IO.File.Exists(nomeArquivo))
                    { System.IO.File.Create(nomeArquivo).Close(); }
                    System.IO.TextWriter arquivo = System.IO.File.AppendText(nomeArquivo);
                    int n = Historico1.Items.Count;
                    string[] vet = new string[n];
                    Historico1.Items.CopyTo(vet, 0);
                    for (int i = 0; i < Historico1.Items.Count; i++) { arquivo.Write(vet[i]); }
                    arquivo.Close();
                    using (Form MsgBox = new MmsgBox("O Log " + "'" + nome + "'" + " was successfully exported.", "OK", 1, 0))
                    { _ = MsgBox.ShowDialog(); }
                }
            }
        }
    }
}

