using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Windows.Forms;

namespace CHOV
{
    public partial class FrmConfiguracoes : Form
    {
        readonly FrmPainel frmC;
        //Variavel para logs
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        private OpenFileDialog openFileDialog1;
        public OpenFileDialog OpenFileDialog1 { get => openFileDialog1; set => openFileDialog1 = value; }
        public ContextMenu PMenu { get; set; }

        #region Call Forms
        public FrmConfiguracoes(FrmPainel frm2)
        {
            InitializeComponent();
            frmC = frm2;
        }

        /// <summary>
        /// Load form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmConfiguracoes_Load(object sender, EventArgs e)
        {
            //Lendo variaveis de config
            ReadSettingsFull();
            //Selecionando TabePage
            TabPageIndex();
            //verificando tipo de sistema e habilitando o q necessario
            EnableDisable(Properties.Settings.Default.System);
            //Recebe dados dos Arrays do pgm
            RecebeNomesPainel(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
            //Recebe nome Slots
            AssignNames();
            log.Debug("Form Configurações carregado");
            // Cria menu popup
            PMenu = new ContextMenu();
            // Cria eventos do menu
            PMenu.MenuItems.Add("Configuration").Enabled = false;
            PMenu.MenuItems.Add("   Import", new System.EventHandler(this.Item1_clicked));
            PMenu.MenuItems.Add("   Export", new System.EventHandler(this.Item2_clicked));
        }

        public static byte[] GeraKey()
        {
            var salt = new byte[] { 99, 21, 3, 4, 5, 6, 7, 8, 9, 10, 111, 12, 13, 14, 255, 16 };
            string password = "my-password";
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt))
            { var key = pdb.GetBytes(32); return key; }
        }

        public static byte[] GeraIv()
        {
            var salt = new byte[] { 13, 2, 3, 4, 5, 6, 255, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            string password = "my-password";
            using (Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(password, salt))
            { var iv = pdb.GetBytes(16); return iv; }
        }

        /// <summary>
        /// Mundança de tab no Menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TbcConfiguration_Selected(object sender, TabControlEventArgs e)
        { TabPageIndex(); }

        /// <summary>
        /// Habilita o que necessário de acordo com a seleção do system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CbTypeSystem_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (CbTypeSystem.Text)
            {
                case "Change Over":
                    pnlsyschgo.Visible = true; pnlsysmtx.Location = new Point(48, 109);
                    break;
                case "Matrix of Signals":
                    pnlsyschgo.Visible = false; pnlsysmtx.Location = new Point(237, 109);
                    break;
            }
        }
        #endregion

        #region Action buttons

        #region Hardware

        /// <summary>
        /// Escreve nas saidas
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnWrite_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Write acionado");
            using (Form MsgBox2 = new MmsgBox("Do you want to write in outputs?", "OK&CANCEL", 4, 0))
            {   //Mensagem de confirmação para o usuário
                DialogResult resultado2 = MsgBox2.ShowDialog();
                if (resultado2 == DialogResult.OK) { log.Debug("Confirmação 'Write' nas saidas"); }
                if (resultado2 == DialogResult.Cancel) { log.Debug("Cancelamento 'Write' nas saidas"); }
            }
        }

        /// <summary>
        /// Limpa o campo das informações
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnClear_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Clear acionado");
            using (Form MsgBox3 = new MmsgBox("Do you want to clear the information?", "OK&CANCEL", 4, 0))
            {   //Mensagem de confirmação para o usuário
                DialogResult resultado3 = MsgBox3.ShowDialog();
                if (resultado3 == DialogResult.OK)
                {
                    log.Debug("Confirmação 'Clear' das informações");
                    //Limpa as informações
                    Informações.Items.Clear();
                }
                if (resultado3 == DialogResult.Cancel) { log.Debug("Cancelamento 'Clear' das informações"); }
            }
        }

        public static byte[] EncryptStringToBytes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Rijndael object
            // with the specified key and IV.
            using (Rijndael rijAlg = Rijndael.Create())
            {
                rijAlg.Key = Key;
                rijAlg.IV = IV;
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }
            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static string DecryptStringFromBytes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            // Declare the string used to hold
            // the decrypted text.
            string plaintext = null;

            using (Rijndael rijAlg = Rijndael.Create())
            {

                rijAlg.Key = GeraKey();
                rijAlg.IV = GeraIv();
                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);
                try
                {
                    // Create the streams used for decryption.
                    using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                    {
                        using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                            { plaintext = srDecrypt.ReadToEnd(); }
                        }
                    }
                }
                catch (Exception)
                {
                    plaintext = "Erro na decriptografia";
                    MessageBox.Show("Erro na decriptografia");
                }
            }
            return plaintext;
        }

        /// <summary>
        /// Realiza a busca do dispositivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSearch_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Search acionado");
            using (Form MsgBox4 = new MmsgBox("Do you want to search for devices?", "OK&CANCEL", 4, 0))
            {   //Mensagem de confirmação para o usuário
                DialogResult resultado4 = MsgBox4.ShowDialog();
                if (resultado4 == DialogResult.OK) { log.Debug("Confirmação do 'Search' "); }
                if (resultado4 == DialogResult.Cancel) { log.Debug("Cancelamento do 'Search' "); }
            }
        }

        /// <summary>
        /// Acessa o dispositivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOpen_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Open acionado");
            using (Form MsgBox5 = new MmsgBox("Do you want to open the device?", "OK&CANCEL", 4, 0))
            {
                //Mensagem de confirmação para o usuário
                DialogResult resultado5 = MsgBox5.ShowDialog();
                if (resultado5 == DialogResult.OK) { log.Debug("Confirmação do 'Open' "); }
                if (resultado5 == DialogResult.Cancel) { log.Debug("Cancelamento do 'Open' "); }
            }
        }

        /// <summary>
        /// Realiza a leitura das entradas do dispositivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnRead_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Read acionado");
            using (Form MsgBox6 = new MmsgBox("Do you want to read the inputs?", "OK&CANCEL", 4, 0))
            {   //Mensagem de confirmação para o usuário
                DialogResult resultado6 = MsgBox6.ShowDialog();
                if (resultado6 == DialogResult.OK) { log.Debug("Confirmação do 'Read' "); }
                if (resultado6 == DialogResult.Cancel) { log.Debug("Cancelamento do 'Read' "); }
            }
        }

        #endregion

        #region System

        /// <summary>
        /// Salva as configurações do sistema
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave_system_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Save acionado");
            using (Form MsgBox = new MmsgBox("Do you want to save the settings?", "SAVE&CANCEL", 4, 0))
            {   //Mensagem de confirmação para o usuário.
                DialogResult resultado = MsgBox.ShowDialog();
                if (resultado == DialogResult.Yes)
                {
                    log.Debug("Confirmado o 'Save' pelo usuário");
                    //Criar vetor bool com os dados a serem salvos no Log e inserido no vetor pgm dos logs
                    frmC.ArrayDataLog = CreateDataLogString();
                    //Escrevendo variaveis do settings.
                    WritevarSetting();
                    //Salvando Settings.
                    SaveSettings();
                    //Selecionando TabePage.
                    TabPageIndex();
                    //verificando tipo de sistema e habilitando o q necessario.
                    EnableDisable(Properties.Settings.Default.System);
                    //Função verifica em qual posição está "CHG0   ".
                    PosicaoChg0noVetor(Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput);
                    //Função que insere o CHG0   .
                    InsertChg0(Properties.Settings.Default.System, Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput, Properties.Settings.Default.InputDeviceChg0);
                    CompletaString();
                    Properties.Settings.Default.Save();
                    //Função utilizada para atualizar as info dos settings e paineis.
                    AtualizaSettingsnoSistem();
                    ShowConfiginLogs();
                    if (Properties.Settings.Default.System == "Change Over")
                    {
                        ReplicaNomesINOUTS();
                        PassaNomesParaArray();
                        GravaNomesSettings(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                        SaveSettings();
                        AtualizaSettingsnoSistem();
                    }
                    else
                    {
                        //Função verifica em qual posição está "CHG0   "
                        PosicaoChg0noVetor(Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput);
                        CompletaString();
                        //Função utilizada para atualizar as info dos settings e paineis.
                        AtualizaSettingsnoSistem();
                    }
                    //Insere as informções atuais salvas no settings.
                    frmC.GetInfoSettings();
                    frmC.AtivaPnl(Properties.Settings.Default.System);
                    using (MmsgBox mmsgBox = new MmsgBox("Settings have been saved successfully!", "OK", 1, 0))
                    { _ = mmsgBox.ShowDialog(); }
                }
                if (resultado == DialogResult.Cancel)
                {   //Lê as variáveis de sistema - imagem device
                    ReadSettingSample();
                    log.Debug("Cancelado o 'Save' pelo usuário");
                }
            }
        }

        /// <summary>
        /// Cancela as confiuraçoes do sistema.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelSystem_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Cancel acionado");
            Close();
        }

        /// <summary>
        /// Click botao browser
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Browser_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Browser acionado");
            string pathselect;
            using (FolderBrowserDialog folderDlg = new FolderBrowserDialog { ShowNewFolderButton = true })
            {
                DialogResult result = folderDlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    pathselect = folderDlg.SelectedPath.Replace(@"\", @"/");
                    _ = folderDlg.RootFolder;
                    Tbpathselect.Text = pathselect;
                    log.Debug("Escolhido novo path para salvar os logs");
                }
            }
        }

        #endregion 

        #region Inputs&Outputs
        /// <summary>
        /// função para completar para 7 caracteres os campos dos nomes das entradas e saidas
        /// </summary>
        public void CompletaString()
        {
            CmpTIN01.Text = Funcoes.String7chars(CmpTIN01.Text);
            CmpTIN02.Text = Funcoes.String7chars(CmpTIN02.Text);
            CmpTIN03.Text = Funcoes.String7chars(CmpTIN03.Text);
            CmpTIN04.Text = Funcoes.String7chars(CmpTIN04.Text);
            CmpTIN05.Text = Funcoes.String7chars(CmpTIN05.Text);
            CmpTIN06.Text = Funcoes.String7chars(CmpTIN06.Text);
            CmpTIN07.Text = Funcoes.String7chars(CmpTIN07.Text);
            CmpTIN08.Text = Funcoes.String7chars(CmpTIN08.Text);
            CmpTIN09.Text = Funcoes.String7chars(CmpTIN09.Text);
            CmpTIN010.Text = Funcoes.String7chars(CmpTIN010.Text);
            CmpTIN011.Text = Funcoes.String7chars(CmpTIN011.Text);
            CmpTIN012.Text = Funcoes.String7chars(CmpTIN012.Text);
            CmpTIN013.Text = Funcoes.String7chars(CmpTIN013.Text);
            CmpTIN014.Text = Funcoes.String7chars(CmpTIN014.Text);
            CmpTIN015.Text = Funcoes.String7chars(CmpTIN015.Text);
            CmpTIN016.Text = Funcoes.String7chars(CmpTIN016.Text);
            CmpRIN1.Text = Funcoes.String7chars(CmpRIN1.Text);
            CmpRIN2.Text = Funcoes.String7chars(CmpRIN2.Text);
            CmpRIN3.Text = Funcoes.String7chars(CmpRIN3.Text);
            CmpRIN4.Text = Funcoes.String7chars(CmpRIN4.Text);
            CmpRIN5.Text = Funcoes.String7chars(CmpRIN5.Text);
            CmpRIN6.Text = Funcoes.String7chars(CmpRIN6.Text);
            CmpRIN7.Text = Funcoes.String7chars(CmpRIN7.Text);
            CmpRIN8.Text = Funcoes.String7chars(CmpRIN8.Text);
            CmpRIN9.Text = Funcoes.String7chars(CmpRIN9.Text);
            CmpRIN10.Text = Funcoes.String7chars(CmpRIN10.Text);
            CmpRIN11.Text = Funcoes.String7chars(CmpRIN11.Text);
            CmpRIN12.Text = Funcoes.String7chars(CmpRIN12.Text);
            CmpRIN13.Text = Funcoes.String7chars(CmpRIN13.Text);
            CmpRIN14.Text = Funcoes.String7chars(CmpRIN14.Text);
            CmpRIN15.Text = Funcoes.String7chars(CmpRIN15.Text);
            CmpRIN16.Text = Funcoes.String7chars(CmpRIN16.Text);
            CmpOut1.Text = Funcoes.String7chars(CmpOut1.Text);
            CmpOut2.Text = Funcoes.String7chars(CmpOut2.Text);
            CmpOut3.Text = Funcoes.String7chars(CmpOut3.Text);
            CmpOut4.Text = Funcoes.String7chars(CmpOut4.Text);
            CmpOut5.Text = Funcoes.String7chars(CmpOut5.Text);
            CmpOut6.Text = Funcoes.String7chars(CmpOut6.Text);
            CmpOut7.Text = Funcoes.String7chars(CmpOut7.Text);
            CmpOut8.Text = Funcoes.String7chars(CmpOut8.Text);
            CmpOut9.Text = Funcoes.String7chars(CmpOut9.Text);
            CmpOut10.Text = Funcoes.String7chars(CmpOut10.Text);
            CmpOut11.Text = Funcoes.String7chars(CmpOut11.Text);
            CmpOut12.Text = Funcoes.String7chars(CmpOut12.Text);
            CmpOut13.Text = Funcoes.String7chars(CmpOut13.Text);
            CmpOut14.Text = Funcoes.String7chars(CmpOut14.Text);
            CmpOut15.Text = Funcoes.String7chars(CmpOut15.Text);
            CmpOut16.Text = Funcoes.String7chars(CmpOut16.Text);
        }

        /// <summary>
        /// Aplica os nomes para os Inouts.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtAplicar_Click(object sender, EventArgs e)
        {
            PassaNomesParaArray();
            if (Funcoes.NullOrempty(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput))
            {
                using (Form MsgBox23 = new MmsgBox("Unable to save with null name (s)!", "OK", 3, 0))
                {
                    //Mensagem de confirmação para o usuário
                    _ = MsgBox23.ShowDialog();
                }
            }
            else
            {
                using (Form MsgBox3 = new MmsgBox("Do you want to apply the new names?", "OK&CANCEL", 4, 0))
                {
                    //Mensagem de confirmação para o usuário
                    DialogResult resultado3 = MsgBox3.ShowDialog();
                    if (resultado3 == DialogResult.OK)
                    {
                        CompletaString();
                        switch (Properties.Settings.Default.System)
                        {
                            case "Change Over":
                                ReplicaNomesINOUTS();
                                PassaNomesParaArray();
                                if (Somente1Chg0Vetor(frmC.ArrayInputPrimary) && Somente1Chg0Vetor(frmC.ArrayInputSecondary) && Somente1Chg0Vetor(frmC.ArrayOutput))
                                {
                                    //Limpa vetor de sistema e insere o vetor de pgm  no vetor de sistema
                                    GravaNomesSettings(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                                    SaveSettings();
                                    AtualizaSettingsnoSistem();
                                    using (Form MsgBox4 = new MmsgBox("New names have been applied.", "OK", 1, 0))
                                    { DialogResult resultado4 = MsgBox4.ShowDialog(); }
                                }
                                else
                                {
                                    string str = Funcoes.IdentificaTrueFalse(Somente1Chg0Vetor(frmC.ArrayInputPrimary), Somente1Chg0Vetor(frmC.ArrayInputSecondary), Somente1Chg0Vetor(frmC.ArrayOutput));
                                    MensagemAdequadaChangeOver(str);
                                }
                                return;
                            case "Matrix of Signals":
                                PassaNomesParaArray();
                                //Limpa vetor de sistema e insere o vetor de pgm  no vetor de sistema
                                GravaNomesSettings(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                                SaveSettings();
                                AtualizaSettingsnoSistem();
                                DialogResult resultado11 = new DialogResult();
                                using (Form MsgBox11 = new MmsgBox("New names have been appliedes.", "OK", 1, 0))
                                { resultado11 = MsgBox11.ShowDialog(); }
                                return;
                        }
                        //Obtém as informações gravadas nos settings
                        frmC.GetInfoSettings();
                    }
                    if (resultado3 == DialogResult.Cancel)
                    {
                        //Recebe dados dos Arrays do pgm
                        RecebeNomesPainel(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                    }
                }
            }
            log.Debug("Botão Aplicar acionado");
        }

        /// <summary>
        /// Replica os nomes para secundário e saída.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnReply_Click(object sender, EventArgs e)
        {
            using (Form MsgBox4 = new MmsgBox("Do you want to replicate the names of the primary to the secondary and to output?", "OK&CANCEL", 4, 0))
            {
                //Mensagem de confirmação para o usuário
                DialogResult resultado4 = MsgBox4.ShowDialog();
                if (resultado4 == DialogResult.OK)
                {
                    //Replica os nomes do primário para o secundário e output
                    ReplicaNomesINOUTS();
                    //Mensagem de confirmação para o usuário
                    using (Form MsgBox5 = new MmsgBox("The names have been replicated.", "OK", 1, 0))
                    { _ = MsgBox5.ShowDialog(); }
                }
                if (resultado4 == DialogResult.Cancel)
                {
                    //Recebe dados dos Arrays do pgm
                    RecebeNomesPainel(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                }
                log.Debug("Botão Reply acionado");
            }
        }

        /// <summary>
        /// Retorna as entradas e saidas para default de acordo com a confiuração do sistema.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDefault_Click(object sender, EventArgs e)
        {
            using (Form MsgBox5 = new MmsgBox("Do you want to return the names to default?", "OK&CANCEL", 4, 0))
            {
                DialogResult resultado5 = MsgBox5.ShowDialog();
                if (resultado5 == DialogResult.OK)
                {
                    //Retorna as entradas e saidas para default de acordo com a confiuração do sistema
                    RetornaDefault(Properties.Settings.Default.System, Properties.Settings.Default.InputDeviceChg0);
                    //Mensagem de confirmação para o usuário
                    using (Form MsgBox15 = new MmsgBox("Names have been set to default.", "OK", 1, 0))
                    { _ = MsgBox15.ShowDialog(); }
                }
                if (resultado5 == DialogResult.Cancel)
                {
                    //Recebe dados dos Arrays do pgm
                    RecebeNomesPainel(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                }
                log.Debug("Botão Default acionado");
            }
        }

        /// <summary>
        /// Cancela form e o fecha.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelIputOutput_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Cancel Inout's acionado");
            Close();
        }

        #endregion

        #region Matrix
        /// <summary>
        /// Escrever na saida
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtWrite_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Write Mtx acionado");
            //frmC.Saida1a16(Funcoes.Slotnot(frmC.ArraySlotsP, frmC.ArraySlotsOp, frmC.ArraySlotsS, frmC.ArraySlotsO, cBp1a8.Text, cBp9a16.Text, cBs1a8.Text, cBs9a16.Text));
            frmC.Saida1a16(Funcoes.ExeCombinacoes(Properties.Settings.Default.Combinations, cBp1a8.Text, cBp9a16.Text, cBs1a8.Text, cBs9a16.Text));
        }

        /// <summary>
        /// Adiciona a combinação para Datagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            log.Debug("Adicionado combinação de matrix");
            bool val = ValidaCombinations(cmBdeviceposicao1.Text, cmBposicao1.Text, cmbOperamtx.Text, cmBdeviceposicao2.Text, cmBposicao2.Text, cmbSaida.Text);
            if (dGv.RowCount >= 16)
            {
                using (Form MsgBox12w = new MmsgBox("Condition could not be recorded, limit of 16 conditions exceeded!", "OK", 4, 0))
                {
                    DialogResult resultado12w = MsgBox12w.ShowDialog();
                    if (resultado12w == DialogResult.OK)
                    {
                        ClearCamposMtx();
                        VisibleBotoes(false, false, false, false, false);
                    }
                }
            }
            else
            {
                if (val)
                {
                    string linha1 = Environment.NewLine + Environment.NewLine + "( " + cmBdeviceposicao1.Text.ToUpper() + ": " + cmBposicao1.Text + " )  < " + cmbOperamtx.Text + " >  ( " + cmBdeviceposicao2.Text.ToUpper() + "  : " + cmBposicao2.Text + " )  ->   " + cmbSaida.Text;
                    string linha2 = Environment.NewLine + Environment.NewLine + "' " + LblPosicao1.Text + " '" + " < " + cmbOperamtx.Text + " > " + "' " + LblPosicao2.Text + " '" + "  ->  " + "' " + LblOutput.Text + " '";
                    using (Form MsgBox1w = new MmsgBox("Do you want to add the condition?" + linha1 + linha2, "OK&CANCEL", 4, 0))
                    {
                        DialogResult resultado1w = MsgBox1w.ShowDialog();
                        if (resultado1w == DialogResult.OK)
                        {
                            AlimentaDgv();
                            Properties.Settings.Default.SizeDgv = dGv.Height;
                            Properties.Settings.Default.Save();
                            ComboBoxMtx();
                            VisibleBotoes(false, false, false, false, true);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Deleta a linha selecionado no Datagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnDel_Click(object sender, EventArgs e)
        {
            if (dGv.Rows[dGv.CurrentRow.Index].Cells[0].Value != null && dGv.Rows[0].Cells[0].Value.ToString() != "")
            {
                log.Debug("Botão 'Delete' foi acionado");
                string[] msg = GetLineDgV();
                using (Form MsgBox43 = new MmsgBox("Do you want to delete this combination?" + Environment.NewLine + Environment.NewLine + "( " + msg[0] + ": " + msg[1] + " ) <" + msg[3] + "> ( " + msg[4] + "  : " + msg[5] + " )  -> " + msg[7] + Environment.NewLine + Environment.NewLine + " ' " + msg[2] + " ' <" + msg[3] + "> ' " + msg[6] + " '  -> ' " + msg[8] + " '", "OK&CANCEL", 3, 0))
                {
                    DialogResult resultado2p = MsgBox43.ShowDialog();
                    if (resultado2p == DialogResult.OK)
                    {
                        log.Debug("Confirmado a operação 'Delete' da matrix");
                        int delindex = dGv.CurrentRow.Index;
                        dGv.Rows.RemoveAt(delindex);
                        VisibleBotoes(false, false, false, false, true);
                    }
                    else
                    {
                        log.Debug("Cancelado a operação 'Delete' da matrix");
                        VisibleBotoes(false, false, false, false, false);
                    }
                }
            }
        }

        /// <summary>
        /// Salva as combinações do Datagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSave__Click(object sender, EventArgs e)
        {
            log.Debug("Botão 'Save' da matrix foi acionado");
            if (dGv.Rows.Count - 1 < 0)
            {
                using (Form MsgBox98y = new MmsgBox("It is not possible to record with any combination configured!", "OK", 3, 0))
                {
                    DialogResult resultado98y = MsgBox98y.ShowDialog();
                    //registo log
                    log.Debug("Não há combinações configuradas para serem salvas");
                }
            }
            else
            {
                using (Form MsgBoxhf = new MmsgBox(GetDgv(dGv.Rows.Count), "Do you save the combinations?", "OK&CANCEL", 1, 13))
                {
                    DialogResult resultadohf = MsgBoxhf.ShowDialog();
                    if (resultadohf == DialogResult.OK)
                    {
                        SaveCombinations();
                        frmC.RecebeNomeSettings();
                        AssignNames();
                        ClearCamposMtx();
                        VisibleBotoes(false, false, false, false, false);
                        log.Debug("Confirmado a operação 'Save' das combinações");

                        DialogResult resultado8y = new DialogResult();

                        using (Form MsgBox8y = new MmsgBox("The combinations have been saved!", "OK", 1, 0))
                        {
                            resultado8y = MsgBox8y.ShowDialog();
                            if (resultado8y == DialogResult.OK)
                            {
                                log.Debug("Mensagem de sucesso da gravaçao das combinações");
                            }
                        }
                    }
                    else
                    {
                        log.Debug("Cancelado a operação 'Save' das combinações");
                        VisibleBotoes(false, false, false, false, false);
                    }
                    CountCombinacoes();
                }
            }
        }

        /// <summary>
        /// Edita a linha selecionada no Datagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnEdit_Click(object sender, EventArgs e)
        { EditLineMtx(); }

        /// <summary>
        /// Cancela o adição da combinação noDatagridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancel_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Cancel Matrix acionado");
            ClearCamposMtx();
            Replecear();
            AlimenteLIST();
            VisibleBotoes(false, false, false, false, false);
        }

        #endregion

        #endregion

        #region Settings
        /// <summary>
        /// Escreve nas variavies de sistema
        /// </summary>
        public void WritevarSetting()
        {
            //Escrevendo variaveis do settings
            Properties.Settings.Default.IP_Primary = IpPrimaryControl.Text;
            Properties.Settings.Default.IP_Secondary = IpSecondaryControl.Text;
            Properties.Settings.Default.IP_Output = IpOutputControl.Text;
            Properties.Settings.Default.System = CbTypeSystem.Text;
            Properties.Settings.Default.DeviceChg0 = CbSelecaoCh0.Text;
            Properties.Settings.Default.InputDeviceChg0 = CbSelecaoInputCh0.Text;
            Properties.Settings.Default.EnableLog = Ch_EnableLog.Checked;
            Properties.Settings.Default.SystemLog = Tbpathselect.Text;
            Properties.Settings.Default.MaxSizeLog = Convert.ToInt32(Cb_MaxSizeLog.Text);
            Properties.Settings.Default.EnableCombinationsLog = ch_Showcombinacoes.Checked;
            //Limpa Vetor do sistema
            Properties.Settings.Default.DatawrittenLog.Clear();
            //Insere vetor de pgm no vetor de sistema
            Properties.Settings.Default.DatawrittenLog.AddRange(frmC.ArrayDataLog);
        }

        /// <summary>
        /// Escreve nas variavies de sistema
        /// </summary>
        public int WritevarSetting(string[] entrada)
        {
            int v = 0;
            if (entrada[0] == "ERRO_CRIPTOGRAFIA")
            {
                // using (MmsgBox mmsgBox = new MmsgBox("Import Canceled!", "OK", 1, 0)){ _ = mmsgBox.ShowDialog(); }
            }
            else
            {
                v = 1;
                CbTypeSystem.Text = entrada[1];
                IpPrimaryControl.Text = entrada[2];
                IpSecondaryControl.Text = entrada[3];
                IpOutputControl.Text = entrada[4];
                CbSelecaoInputCh0.Text = entrada[5];
                CbSelecaoCh0.Text = entrada[6];

                Ch_EnableLog.Checked = Funcoes.TrueFalse(entrada[9]);
                Tbpathselect.Text = entrada[10];
                Cb_MaxSizeLog.Text = entrada[11];
                ch_Showcombinacoes.Checked = Funcoes.TrueFalse(entrada[12]);

                Ch_Username.Checked = Funcoes.TrueFalse(entrada[13]);
                Ch_Level.Checked = Funcoes.TrueFalse(entrada[14]);
                Ch_Method.Checked = Funcoes.TrueFalse(entrada[15]);
                Ch_Message.Checked = Funcoes.TrueFalse(entrada[16]);
                Ch_Thread.Checked = Funcoes.TrueFalse(entrada[17]);
                Ch_Line.Checked = Funcoes.TrueFalse(entrada[18]);
                Ch_identity.Checked = Funcoes.TrueFalse(entrada[19]);
                Ch_Location.Checked = Funcoes.TrueFalse(entrada[20]);
                string[] oit = { entrada[13], entrada[14], entrada[15], entrada[16], entrada[17], entrada[18], entrada[19], entrada[20] };
                Properties.Settings.Default.DatawrittenLog.Clear();
                Properties.Settings.Default.DatawrittenLog.AddRange(oit);

                List<string> Nprimary = new List<string>(entrada);
                List<string> Nsecondary = new List<string>(entrada);
                List<string> Noutput = new List<string>(entrada);
                List<string> Combinacoes = new List<string>(entrada);
                string[] Comb = Combinacoes.GetRange(73, (entrada.Length - 73)).ToArray();

                RecebeNomesPainel(Nprimary.GetRange(22, 16).ToArray(), Nsecondary.GetRange(39, 16).ToArray(), Noutput.GetRange(56, 16).ToArray());
                PassaNomesParaArray();
                GravaNomesSettings(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
                Properties.Settings.Default.Combinations.Clear();
                Properties.Settings.Default.Combinations.AddRange(Comb);
                AtualizaSettingsnoSistem();
            }
            return v;
        }

        /// <summary>
        /// Escreve nas variavies de sistema
        /// </summary>
        public void ReadSettingsFull()
        {
            //Função que recebe info atualizadas do settings
            frmC.RefreshSettings();
            //Load imagem empty device
            picSealevel.Image = CHOV.Properties.Resources.Empty_device;
            IpPrimaryControl.Text = frmC.IP_Primary_pgm;
            IpSecondaryControl.Text = frmC.IP_Secondary_pgm;
            IpOutputControl.Text = frmC.IP_OutputIP_pgm;
            CbTypeSystem.Text = frmC.System_pgm;
            CbSelecaoCh0.Text = frmC.DeviceChg0_pgm;
            CbSelecaoInputCh0.Text = frmC.InputDeviceChg0_pgm;
            Ch_EnableLog.Checked = frmC.EnableLog_pgm;
            Tbpathselect.Text = frmC.SystemLog_pgm;
            Cb_MaxSizeLog.Text = Convert.ToString(frmC.MaxSizeLog_pgm);
            ch_Showcombinacoes.Checked = frmC.EnableCombinations_pgm;
            bool[] vre = CheckLog(frmC.ArrayDataLog);
            Ch_Username.Checked = vre[0];
            Ch_Level.Checked = vre[1];
            Ch_Method.Checked = vre[2];
            Ch_Message.Checked = vre[3];
            Ch_Thread.Checked = vre[4];
            Ch_Line.Checked = vre[5];
            Ch_identity.Checked = vre[6];
            Ch_Location.Checked = vre[7];
        }

        /// <summary>
        /// Lê as variáveis de sistema - imagem device
        /// </summary>
        public void ReadSettingSample()
        {
            //Função que recebe info atualizadas do settings
            frmC.RefreshSettings();
            IpPrimaryControl.Text = frmC.IP_Primary_pgm;
            IpSecondaryControl.Text = frmC.IP_Secondary_pgm;
            IpOutputControl.Text = frmC.IP_OutputIP_pgm;
            CbTypeSystem.Text = frmC.System_pgm;
            CbSelecaoCh0.Text = frmC.DeviceChg0_pgm;
            CbSelecaoInputCh0.Text = frmC.InputDeviceChg0_pgm;
            Ch_EnableLog.Checked = frmC.EnableLog_pgm;
            Tbpathselect.Text = frmC.SystemLog_pgm;
            Cb_MaxSizeLog.Text = Convert.ToString(frmC.MaxSizeLog_pgm);
            bool[] vre = CheckLog(frmC.ArrayDataLog);
            Ch_Username.Checked = vre[0];
            Ch_Level.Checked = vre[1];
            Ch_Method.Checked = vre[2];
            Ch_Message.Checked = vre[3];
            Ch_Thread.Checked = vre[4];
            Ch_Line.Checked = vre[5];
            Ch_identity.Checked = vre[6];
            Ch_Location.Checked = vre[7];
        }

        /// <summary>
        /// Comando para salvar as settings
        /// </summary>
        public void SaveSettings()
        {
            //Salvando nas variaveis de settings
            Properties.Settings.Default.Save();
            log.Debug("função para salvar as Configurações foi ativada");
        }

        /// <summary>
        /// Limpa vetor de sistema e insere o vetor de pgm
        /// </summary>
        public void GravaNomesSettings(string[] Nprimary, string[] Nsecondary, string[] Noutput)
        {
            //Limpa Vetor do sistema
            Properties.Settings.Default.NamesInputPrimary.Clear();
            //Insere vetor de pgm no vetor de sistema
            Properties.Settings.Default.NamesInputPrimary.AddRange(Nprimary);
            Properties.Settings.Default.NamesInputSecondary.Clear();
            Properties.Settings.Default.NamesInputSecondary.AddRange(Nsecondary);
            Properties.Settings.Default.NamesOutput.Clear();
            Properties.Settings.Default.NamesOutput.AddRange(Noutput);
        }

        /// <summary>
        /// Atribui os nomes dos INs&OUTs para Arrays do pgm
        /// </summary>
        public void PassaNomesParaArray()
        {
            frmC.ArrayInputPrimary[0] = CmpTIN01.Text;
            frmC.ArrayInputPrimary[1] = CmpTIN02.Text;
            frmC.ArrayInputPrimary[2] = CmpTIN03.Text;
            frmC.ArrayInputPrimary[3] = CmpTIN04.Text;
            frmC.ArrayInputPrimary[4] = CmpTIN05.Text;
            frmC.ArrayInputPrimary[5] = CmpTIN06.Text;
            frmC.ArrayInputPrimary[6] = CmpTIN07.Text;
            frmC.ArrayInputPrimary[7] = CmpTIN08.Text;
            frmC.ArrayInputPrimary[8] = CmpTIN09.Text;
            frmC.ArrayInputPrimary[9] = CmpTIN010.Text;
            frmC.ArrayInputPrimary[10] = CmpTIN011.Text;
            frmC.ArrayInputPrimary[11] = CmpTIN012.Text;
            frmC.ArrayInputPrimary[12] = CmpTIN013.Text;
            frmC.ArrayInputPrimary[13] = CmpTIN014.Text;
            frmC.ArrayInputPrimary[14] = CmpTIN015.Text;
            frmC.ArrayInputPrimary[15] = CmpTIN016.Text;
            frmC.ArrayInputSecondary[0] = CmpRIN1.Text;
            frmC.ArrayInputSecondary[1] = CmpRIN2.Text;
            frmC.ArrayInputSecondary[2] = CmpRIN3.Text;
            frmC.ArrayInputSecondary[3] = CmpRIN4.Text;
            frmC.ArrayInputSecondary[4] = CmpRIN5.Text;
            frmC.ArrayInputSecondary[5] = CmpRIN6.Text;
            frmC.ArrayInputSecondary[6] = CmpRIN7.Text;
            frmC.ArrayInputSecondary[7] = CmpRIN8.Text;
            frmC.ArrayInputSecondary[8] = CmpRIN9.Text;
            frmC.ArrayInputSecondary[9] = CmpRIN10.Text;
            frmC.ArrayInputSecondary[10] = CmpRIN11.Text;
            frmC.ArrayInputSecondary[11] = CmpRIN12.Text;
            frmC.ArrayInputSecondary[12] = CmpRIN13.Text;
            frmC.ArrayInputSecondary[13] = CmpRIN14.Text;
            frmC.ArrayInputSecondary[14] = CmpRIN15.Text;
            frmC.ArrayInputSecondary[15] = CmpRIN16.Text;
            frmC.ArrayOutput[0] = CmpOut1.Text;
            frmC.ArrayOutput[1] = CmpOut2.Text;
            frmC.ArrayOutput[2] = CmpOut3.Text;
            frmC.ArrayOutput[3] = CmpOut4.Text;
            frmC.ArrayOutput[4] = CmpOut5.Text;
            frmC.ArrayOutput[5] = CmpOut6.Text;
            frmC.ArrayOutput[6] = CmpOut7.Text;
            frmC.ArrayOutput[7] = CmpOut8.Text;
            frmC.ArrayOutput[8] = CmpOut9.Text;
            frmC.ArrayOutput[9] = CmpOut10.Text;
            frmC.ArrayOutput[10] = CmpOut11.Text;
            frmC.ArrayOutput[11] = CmpOut12.Text;
            frmC.ArrayOutput[12] = CmpOut13.Text;
            frmC.ArrayOutput[13] = CmpOut14.Text;
            frmC.ArrayOutput[14] = CmpOut15.Text;
            frmC.ArrayOutput[15] = CmpOut16.Text;
        }

        /// <summary>
        /// Recebe dados direto dos Arrays do pgm
        /// </summary>
        public void RecebeNomePainel()
        {
            CmpTIN01.Text = frmC.ArrayInputPrimary[0];
            CmpTIN02.Text = frmC.ArrayInputPrimary[1];
            CmpTIN03.Text = frmC.ArrayInputPrimary[2];
            CmpTIN04.Text = frmC.ArrayInputPrimary[3];
            CmpTIN05.Text = frmC.ArrayInputPrimary[4];
            CmpTIN06.Text = frmC.ArrayInputPrimary[5];
            CmpTIN07.Text = frmC.ArrayInputPrimary[6];
            CmpTIN08.Text = frmC.ArrayInputPrimary[7];
            CmpTIN09.Text = frmC.ArrayInputPrimary[8];
            CmpTIN010.Text = frmC.ArrayInputPrimary[9];
            CmpTIN011.Text = frmC.ArrayInputPrimary[10];
            CmpTIN012.Text = frmC.ArrayInputPrimary[11];
            CmpTIN013.Text = frmC.ArrayInputPrimary[12];
            CmpTIN014.Text = frmC.ArrayInputPrimary[13];
            CmpTIN015.Text = frmC.ArrayInputPrimary[14];
            CmpTIN016.Text = frmC.ArrayInputPrimary[15];
            CmpRIN1.Text = frmC.ArrayInputSecondary[0];
            CmpRIN2.Text = frmC.ArrayInputSecondary[1];
            CmpRIN3.Text = frmC.ArrayInputSecondary[2];
            CmpRIN4.Text = frmC.ArrayInputSecondary[3];
            CmpRIN5.Text = frmC.ArrayInputSecondary[4];
            CmpRIN6.Text = frmC.ArrayInputSecondary[5];
            CmpRIN7.Text = frmC.ArrayInputSecondary[6];
            CmpRIN8.Text = frmC.ArrayInputSecondary[7];
            CmpRIN9.Text = frmC.ArrayInputSecondary[8];
            CmpRIN10.Text = frmC.ArrayInputSecondary[9];
            CmpRIN11.Text = frmC.ArrayInputSecondary[10];
            CmpRIN12.Text = frmC.ArrayInputSecondary[11];
            CmpRIN13.Text = frmC.ArrayInputSecondary[12];
            CmpRIN14.Text = frmC.ArrayInputSecondary[13];
            CmpRIN15.Text = frmC.ArrayInputSecondary[14];
            CmpRIN16.Text = frmC.ArrayInputSecondary[15];
            CmpOut1.Text = frmC.ArrayOutput[0];
            CmpOut2.Text = frmC.ArrayOutput[1];
            CmpOut3.Text = frmC.ArrayOutput[2];
            CmpOut4.Text = frmC.ArrayOutput[3];
            CmpOut5.Text = frmC.ArrayOutput[4];
            CmpOut6.Text = frmC.ArrayOutput[5];
            CmpOut7.Text = frmC.ArrayOutput[6];
            CmpOut8.Text = frmC.ArrayOutput[7];
            CmpOut9.Text = frmC.ArrayOutput[8];
            CmpOut10.Text = frmC.ArrayOutput[9];
            CmpOut11.Text = frmC.ArrayOutput[10];
            CmpOut12.Text = frmC.ArrayOutput[11];
            CmpOut13.Text = frmC.ArrayOutput[12];
            CmpOut14.Text = frmC.ArrayOutput[13];
            CmpOut15.Text = frmC.ArrayOutput[14];
            CmpOut16.Text = frmC.ArrayOutput[15];
        }

        /// <summary>
        /// Recebe os nomes via dados importados
        /// </summary>
        public void RecebeNomesPainel(string[] Nprimary, string[] Nsecondary, string[] Noutput)
        {
            CmpTIN01.Text = Nprimary[0];
            CmpTIN02.Text = Nprimary[1];
            CmpTIN03.Text = Nprimary[2];
            CmpTIN04.Text = Nprimary[3];
            CmpTIN05.Text = Nprimary[4];
            CmpTIN06.Text = Nprimary[5];
            CmpTIN07.Text = Nprimary[6];
            CmpTIN08.Text = Nprimary[7];
            CmpTIN09.Text = Nprimary[8];
            CmpTIN010.Text = Nprimary[9];
            CmpTIN011.Text = Nprimary[10];
            CmpTIN012.Text = Nprimary[11];
            CmpTIN013.Text = Nprimary[12];
            CmpTIN014.Text = Nprimary[13];
            CmpTIN015.Text = Nprimary[14];
            CmpTIN016.Text = Nprimary[15];

            CmpRIN1.Text = Nsecondary[0];
            CmpRIN2.Text = Nsecondary[1];
            CmpRIN3.Text = Nsecondary[2];
            CmpRIN4.Text = Nsecondary[3];
            CmpRIN5.Text = Nsecondary[4];
            CmpRIN6.Text = Nsecondary[5];
            CmpRIN7.Text = Nsecondary[6];
            CmpRIN8.Text = Nsecondary[7];
            CmpRIN9.Text = Nsecondary[8];
            CmpRIN10.Text = Nsecondary[9];
            CmpRIN11.Text = Nsecondary[10];
            CmpRIN12.Text = Nsecondary[11];
            CmpRIN13.Text = Nsecondary[12];
            CmpRIN14.Text = Nsecondary[13];
            CmpRIN15.Text = Nsecondary[14];
            CmpRIN16.Text = Nsecondary[15];

            CmpOut1.Text = Noutput[0];
            CmpOut2.Text = Noutput[1];
            CmpOut3.Text = Noutput[2];
            CmpOut4.Text = Noutput[3];
            CmpOut5.Text = Noutput[4];
            CmpOut6.Text = Noutput[5];
            CmpOut7.Text = Noutput[6];
            CmpOut8.Text = Noutput[7];
            CmpOut9.Text = Noutput[8];
            CmpOut10.Text = Noutput[9];
            CmpOut11.Text = Noutput[10];
            CmpOut12.Text = Noutput[11];
            CmpOut13.Text = Noutput[12];
            CmpOut14.Text = Noutput[13];
            CmpOut15.Text = Noutput[14];
            CmpOut16.Text = Noutput[15];
        }

        /// <summary>
        /// Determina qual aba do Config esta selecionada
        /// </summary>
        /// <returns> int representando o numero da pagina atual do Tabpage</returns>
        public int TabPageIndex()
        {
            String page = TbcConfiguration.SelectedTab.ToString().Substring(10).Replace("}", "");
            int TbIndexPage;
            if (page != "Null")
            {
                switch (page)
                {
                    case "Hardware":
                        TbIndexPage = 1;
                        return TbIndexPage;
                    case "System":
                        //Lê as variáveis de sistema - imagem device
                        ReadSettingSample();
                        TbIndexPage = 2;
                        return TbIndexPage;
                    case "Inputs & Outputs":
                        //Função verifica em qual posição está "CHG0   "
                        PosicaoChg0noVetor(Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput);
                        //Função que insere o CHG0   
                        InsertChg0(Properties.Settings.Default.System, Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput, Properties.Settings.Default.InputDeviceChg0);
                        //TESTE
                        CompletaString();
                        //Função utilizada para atualizar as info dos settings e paineis.
                        AtualizaSettingsnoSistem();
                        TbIndexPage = 3;
                        return TbIndexPage;
                    case "Matrix":
                        TbIndexPage = 4;
                        if (frmC.System_pgm == "Matrix of Signals")
                        {
                            Replecear();
                            AlimenteLIST();
                            CountCombinacoes();
                        }
                        return TbIndexPage;
                    default:
                        TbIndexPage = 0;
                        return TbIndexPage;
                }
            }
            else
            {
                TbIndexPage = 0;
                return TbIndexPage;
            }
        }

        /// <summary>
        /// Recebe as combinações salvas no settings e preenche o datagridview
        /// </summary>
        public void AlimenteLIST()
        {
            dGv.Rows.Clear();
            int cont = Properties.Settings.Default.Combinations.Count;
            for (int i = 0; i < cont; i++)
            {
                string combi = Properties.Settings.Default.Combinations[i];
                if (Properties.Settings.Default.Combinations[i] != null && combi.Length == 100)
                { dGv.Rows.Add(Funcoes.Dvet(combi)); }
            }
        }

        /// <summary>
        /// verificando tipo de sistema e habilitando o q necessario
        /// </summary>
        /// <param name="System">string representando o tipo de sistema configurado</param>
        public void EnableDisable(string System)
        {
            switch (System)
            {
                case "Change Over":
                    btnReply.Enabled = false;
                    PnlSecondary.Enabled = false;
                    PnlOut.Enabled = false;
                    pnlTabMatrix.Visible = false;
                    PnlMtxoff.Visible = true;
                    break;
                case "Matrix of Signals":
                    pnlTabMatrix.Visible = true;
                    PnlMtxoff.Visible = false;
                    btnReply.Enabled = true;
                    PnlSecondary.Enabled = true;
                    PnlOut.Enabled = true;
                    frmC.AtivaPnlMtx();
                    break;
            }
        }

        /// <summary>
        /// Função utilizada para atualizar as info dos settings e paineis.
        /// </summary>
        public void AtualizaSettingsnoSistem()
        {
            //Passa novos nomes para painel principal 
            frmC.RecebeNomeSettings();
            //Passa p Paniel principal
            frmC.AtribuindoNomesPainel();
            //painel inuots
            RecebeNomesPainel(frmC.ArrayInputPrimary, frmC.ArrayInputSecondary, frmC.ArrayOutput);
            //registo log
            log.Debug("Settings foram atualizadas no sistema");
        }

        /// <summary>
        /// Obtem dados do Datagridview e retorna um string[]
        /// </summary>
        /// <param name="entrada"> recebe string de entrada representando o DatagridView.count </param>
        /// <returns>string[] com os dados do datagridView</returns>
        public string[] GetDgv(int entrada)
        {
            int l = entrada;
            string[] sasuke = new string[l];
            string[] k = new string[9];
            for (Int32 i = 0; i < l; i++)
            {
                for (int h = 0; h <= 8; h++) { k[h] = dGv.Rows[i].Cells[h].Value.ToString(); }
                string p1;
                if (k[1].Length > 5) { p1 = k[1]; }
                else { p1 = "    " + k[1]; }
                string p2;
                if (k[5].Length > 5) { p2 = k[5]; }
                else { p2 = "    " + k[5]; }
                sasuke[i] = Funcoes.Cvet(k[0], p1, k[2], k[3], k[4], p2, k[6], k[7], k[8]);
            }
            return sasuke;
        }

        /// <summary>
        /// Função para discretar o array string slots p seus respectivos arrays o settings
        /// </summary>
        public void SaveCombinations()
        {
            string[] sasuke = GetDgv(dGv.Rows.Count);
            //Limpa Vetor do sistema
            Properties.Settings.Default.Combinations.Clear();
            //Insere vetor de pgm no vetor de sistema
            Properties.Settings.Default.Combinations.AddRange((sasuke));
            //Save
            Properties.Settings.Default.Save();
            //registo log
            log.Debug("Combinações foram salvas");
        }

        /// <summary>
        /// Verifica quais opções estão selecionadas, retornado um vetor string
        /// </summary>
        /// <returns>string[] com o datalog</returns>
        public string[] CreateDataLogString()
        {
            string[] vet = new string[] { "FALSE", "FALSE", "FALSE", "FALSE", "FALSE", "FALSE", "FALSE", "FALSE" };
            if (Ch_Username.CheckState == CheckState.Checked) { vet[0] = "TRUE"; }
            if (Ch_Level.CheckState == CheckState.Checked) { vet[1] = "TRUE"; }
            if (Ch_Method.CheckState == CheckState.Checked) { vet[2] = "TRUE"; }
            if (Ch_Message.CheckState == CheckState.Checked) { vet[3] = "TRUE"; }
            if (Ch_Thread.CheckState == CheckState.Checked) { vet[4] = "TRUE"; }
            if (Ch_Line.CheckState == CheckState.Checked) { vet[5] = "TRUE"; }
            if (Ch_identity.CheckState == CheckState.Checked) { vet[6] = "TRUE"; }
            if (Ch_Location.CheckState == CheckState.Checked) { vet[7] = "TRUE"; }
            return vet;
        }

        /// <summary>
        /// converte um vetor string para vetor bool
        /// </summary>
        /// <param name="ent">string[] a ser convertida em bool[]</param>
        /// <returns>bool[] convertido da entrada</returns>
        public bool[] CheckLog(string[] ent)
        {
            bool[] said = new bool[8];
            for (int i = 0; i < ent.Length; i++)
            {
                if (ent[i] == "TRUE") { said[i] = true; }
                else { said[i] = false; }
            }
            return said;
        }

        /// <summary>
        /// Função para gravar nos logs as informações de configuração
        /// </summary>
        public void ShowConfiginLogs()
        {
            string config1 = "System: " + Properties.Settings.Default.System + " - " + "DeviceChg0: " + Properties.Settings.Default.DeviceChg0 + " - " + "Input deviceChg0: " + Properties.Settings.Default.InputDeviceChg0 + " - " + "CurrentSelection: " + Properties.Settings.Default.CurrentSelection + " - " + "IP_Primary: " + Properties.Settings.Default.IP_Primary + " - " + "IP_Secondary: " + Properties.Settings.Default.IP_Secondary + " - " + "IP_Output: " + Properties.Settings.Default.IP_Output;
            string config2 = "Enable log: " + Properties.Settings.Default.EnableCombinationsLog.ToString() + " - " + "Max size log: " + Properties.Settings.Default.MaxSizeLog.ToString() + "Mb" + " - " + "Path Log operation: " + Properties.Settings.Default.Path_LogOperation + " - " + "Enable combinations log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            log.Debug(config1);
            log.Debug(config2);
        }

        /// <summary>
        /// Expandir form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBoxMore_Click(object sender, EventArgs e)
        {
            if (frmC.exp1 == 0)
            {
                pictureBoxMore.Image = CHOV.Properties.Resources.icons8_para_cima_com_quadrado_100;
                pnlBtns.Location = new Point(6, 84);
                pnlSimulaRead.Visible = true;
                frmC.exp1 = 1;
            }
            else
            {
                pictureBoxMore.Image = CHOV.Properties.Resources.icons8_para_baixo_com_quadrado_100;
                pnlBtns.Location = new Point(221, 84);
                pnlSimulaRead.Visible = false;
                frmC.exp1 = 0;
            }
        }

        /// <summary>
        /// Habilita a gravação dos logs
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Ch_EnableLog_CheckedChanged(object sender, EventArgs e)
        {
            if (Ch_EnableLog.Checked == true) { Pnl_ConfigLog.Enabled = true; }
            else { Pnl_ConfigLog.Enabled = false; }
        }

        /// <summary>
        /// Popula o vetor default
        /// </summary>
        /// <returns>string[] com vertor populado</returns>
        public string[] PopulaVetorDefault()
        {
            string[] vet = new string[32];
            vet[0] = "    IN 01";
            vet[1] = "    IN 02";
            vet[2] = "    IN 03";
            vet[3] = "    IN 04";
            vet[4] = "    IN 05";
            vet[5] = "    IN 06";
            vet[6] = "    IN 07";
            vet[7] = "    IN 08";
            vet[8] = "    IN 09";
            vet[9] = "    IN 10";
            vet[10] = "    IN 11";
            vet[11] = "    IN 12";
            vet[12] = "    IN 13";
            vet[13] = "    IN 14";
            vet[14] = "    IN 15";
            vet[15] = "    IN 16";
            vet[16] = "NOT-IN 01";
            vet[17] = "NOT-IN 02";
            vet[18] = "NOT-IN 03";
            vet[19] = "NOT-IN 04";
            vet[20] = "NOT-IN 05";
            vet[21] = "NOT-IN 06";
            vet[22] = "NOT-IN 07";
            vet[23] = "NOT-IN 08";
            vet[24] = "NOT-IN 09";
            vet[25] = "NOT-IN 10";
            vet[26] = "NOT-IN 11";
            vet[27] = "NOT-IN 12";
            vet[28] = "NOT-IN 13";
            vet[29] = "NOT-IN 14";
            vet[30] = "NOT-IN 15";
            vet[31] = "NOT-IN 16";
            //vet[32] = "IN 00";
            return vet;
        }

        /// <summary>
        /// Preenchimento do comboBox posição1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmBdeviceposicao1_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nulo = "- null  -";
            cmBposicao1.Items.Clear();
            string[] v = PopulaVetorDefault();
            if (cmBdeviceposicao1.Text == "Primary  " || cmBdeviceposicao1.Text == "Secondary")
            {
                cmBposicao1.Items.AddRange(v);
                LblPosicao1.Text = "";
                cmbSaida.Items.Clear();
                LblOutput.Text = "";
            }
            else
            {
                cmbOperamtx.Text = " OR";
                cmBposicao1.Items.Add(nulo);
                cmBposicao1.Text = nulo;
                LblPosicao1.Text = "-null!-";
                cmbSaida.Items.Clear();
                LblOutput.Text = "";
                AlimentaCmbOut();
            }
            VisibleBotoes(true, false, false, false, false);
        }

        /// <summary>
        /// Preenche ComboBoxOut
        /// </summary>
        public void AlimentaCmbOut()
        {
            LblOutput.Text = "";
            string[] vry = new string[16];
            vry[0] = "OUT 01";
            vry[1] = "OUT 02";
            vry[2] = "OUT 03";
            vry[3] = "OUT 04";
            vry[4] = "OUT 05";
            vry[5] = "OUT 06";
            vry[6] = "OUT 07";
            vry[7] = "OUT 08";
            vry[8] = "OUT 09";
            vry[9] = "OUT 10";
            vry[10] = "OUT 11";
            vry[11] = "OUT 12";
            vry[12] = "OUT 13";
            vry[13] = "OUT 14";
            vry[14] = "OUT 15";
            vry[15] = "OUT 16";
            cmbSaida.Items.Clear();
            cmbSaida.Items.AddRange(vry);
        }

        /// <summary>
        /// Muda seleção do Cmbdeviceposição2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmBdeviceposicao2_SelectedIndexChanged(object sender, EventArgs e)
        {
            string nulo = "- null  -";
            string[] v = PopulaVetorDefault();
            cmBposicao2.Items.Clear();
            if (cmBdeviceposicao2.Text == "Primary  " || cmBdeviceposicao2.Text == "Secondary")
            {
                cmBposicao2.Items.AddRange(v);
                LblPosicao2.Text = "";
                cmbSaida.Items.Clear();
                LblOutput.Text = "";
            }
            else
            {
                cmbOperamtx.Text = " OR";
                cmBposicao2.Items.Add(nulo);
                cmBposicao2.Text = nulo;
                LblPosicao2.Text = "-null!-";
                cmbSaida.Items.Clear();
                LblOutput.Text = "";
                AlimentaCmbOut();
            }
            VisibleBotoes(true, false, false, false, false);
        }

        /// <summary>
        /// Muda seleção do Cmbdeviceposição1
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmBposicao1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmBdeviceposicao1.Text == "Primary  ")
            {
                LblPosicao1.Text = Funcoes.PosicaoIndexNEW(Properties.Settings.Default.NamesInputPrimary, cmBposicao1.Text);
                frmC.indxCmbP1 = Funcoes.QualIN(cmBposicao1.SelectedIndex.ToString());
                if (cmbOperamtx.Text != "")
                {
                    cmbSaida.Text = "";
                    LblOutput.Text = "";
                    AlimentaCmbOut();
                }
            }
            if (cmBdeviceposicao1.Text == "Secondary")
            {
                LblPosicao1.Text = Funcoes.PosicaoIndexNEW(Properties.Settings.Default.NamesInputSecondary, cmBposicao1.Text);
                frmC.indxCmbP1 = Funcoes.QualIN(cmBposicao1.SelectedIndex.ToString());
                if (cmbOperamtx.Text != "")
                {
                    cmbSaida.Text = "";
                    LblOutput.Text = "";
                    AlimentaCmbOut();
                }
            }
            if (cmBdeviceposicao1.Text == "- null  -")
            {
                cmbSaida.Text = "";
                LblOutput.Text = "";
                AlimentaCmbOut();
            }
        }

        /// <summary>
        /// Muda seleção do Cmbposição2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmBposicao2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmBdeviceposicao2.Text == "Primary  ")
            {
                LblPosicao2.Text = Funcoes.PosicaoIndexNEW(Properties.Settings.Default.NamesInputPrimary, cmBposicao2.Text);
                frmC.indxCmbP2 = Funcoes.QualIN(cmBposicao2.SelectedIndex.ToString());
                if (cmbOperamtx.Text != "")
                {
                    cmbSaida.Text = "";
                    LblOutput.Text = "";
                    AlimentaCmbOut();
                }
                VisibleBotoes(true, false, false, false, false);
            }
            if (cmBdeviceposicao2.Text == "Secondary")
            {
                LblPosicao2.Text = Funcoes.PosicaoIndexNEW(Properties.Settings.Default.NamesInputSecondary, cmBposicao2.Text);
                frmC.indxCmbP2 = Funcoes.QualIN(cmBposicao2.SelectedIndex.ToString());
                if (cmbOperamtx.Text != "")
                {
                    cmbSaida.Text = "";
                    LblOutput.Text = "";
                    AlimentaCmbOut();
                }
                VisibleBotoes(true, false, false, false, false);
            }
            if (cmBdeviceposicao2.Text == "- null  -")
            {
                cmbSaida.Text = "";
                LblOutput.Text = "";
                AlimentaCmbOut();
            }
        }

        /// <summary>
        /// Muda seleção do Cmbposição3
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbSaida_SelectedIndexChanged(object sender, EventArgs e)
        {
            VisibleBotoes(true, false, false, false, false);
            if (cmbSaida.Text != "") { LblOutput.Text = Funcoes.PosicaoIndexOut(Properties.Settings.Default.NamesOutput, cmbSaida.Text); }
            frmC.indxCmbOut = Funcoes.QualOUT(cmbSaida.SelectedIndex.ToString());
            if (cmbSaida.Text != "" && cmbOperamtx.Text != "") { VisibleBotoes(true, false, false, true, false); }
        }

        /// <summary>
        /// Edit Matrix
        /// </summary>
        public void EditLineMtx()
        {
            if (dGv.Rows[dGv.CurrentRow.Index].Cells[0].Value != null && dGv.CurrentRow.Index >= 0 && (dGv.Rows[dGv.CurrentRow.Index].Cells[0].Value.ToString() != ""))
            {
                string[] LineDev = GetDgV();
                int indexDgv = dGv.CurrentRow.Index;
                string L1 = Environment.NewLine + Environment.NewLine + "( " + LineDev[0].ToUpper() + ": " + LineDev[1] + " )  < " + LineDev[3] + " >  ( " + LineDev[4].ToUpper() + "  : " + LineDev[5] + " )  ->   " + LineDev[7];
                string L2 = Environment.NewLine + Environment.NewLine + "' " + LineDev[2] + " '" + " < " + LineDev[3] + " > " + "' " + LineDev[6] + " '" + "  ->  " + "' " + LineDev[8] + " '";
                using (Form MsgBoxKM = new MmsgBox("Do you want to edit the condition?" + L1 + L2, "OK&CANCEL", 4, 0))
                {
                    DialogResult resultadoKM = MsgBoxKM.ShowDialog();
                    if (resultadoKM == DialogResult.OK)
                    {
                        cmBdeviceposicao1.Text = LineDev[0];
                        cmBposicao1.Text = LineDev[1];
                        cmbOperamtx.Text = LineDev[3];
                        cmBdeviceposicao2.Text = LineDev[4];
                        cmBposicao2.Text = LineDev[5];
                        cmbSaida.Text = LineDev[7];
                        dGv.Rows.RemoveAt(indexDgv);
                        VisibleBotoes(true, false, false, true, true);
                    }
                }
            }
        }

        /// <summary>
        /// Obtém os dados do Datagridview e retorna um string[]
        /// </summary>
        /// <returns>string[] com os dados do Datagridview</returns>
        public string[] GetDgV()
        {
            int k = dGv.CurrentRow.Index;
            string[] V = new string[10];
            V[0] = "-";
            if (dGv.Rows[k].Cells[0].Value != null && k >= 0 && (dGv.Rows[k].Cells[0].Value.ToString() != ""))
            {
                string k1 = dGv.Rows[k].Cells[1].Value.ToString();
                string k2 = dGv.Rows[k].Cells[5].Value.ToString();
                if (k1.Length > 5) { }
                else { k1 = "    " + k1; }
                if (k2.Length > 5) { }
                else { k2 = "    " + k2; }
                V = Funcoes.Dvet(Funcoes.Cvet(dGv.Rows[k].Cells[0].Value.ToString(), k1, dGv.Rows[k].Cells[2].Value.ToString(), dGv.Rows[k].Cells[3].Value.ToString(), dGv.Rows[k].Cells[4].Value.ToString(), k2, dGv.Rows[k].Cells[6].Value.ToString(), dGv.Rows[k].Cells[7].Value.ToString(), dGv.Rows[k].Cells[8].Value.ToString()));
                if (V[1].Length > 5) { V[1] = V[1]; }
                else { V[1] = "    " + V[1]; }
                if (V[5].Length > 5) { V[5] = V[5]; }
                else { V[5] = "    " + V[5]; }
                return V;
            }
            else
            { return V; }
        }

        /// <summary>
        /// Decompoe cada linha do Datagridview e retorna um string[] com cada elemento separado da combinação
        /// </summary>
        /// <returns>string[] com cada elemento separado da combinação</returns>
        public string[] GetLineDgV()
        {
            string[] T = { "null", "null", "null", "null", "null", "null", "null", "null", "null" };
            int k = dGv.CurrentRow.Index;
            if (dGv.Rows[k].Cells[0].Value != null)
            {
                if (k >= 0)
                {
                    if ((dGv.Rows[k].Cells[0].Value.ToString() != ""))
                    {
                        string k1 = dGv.Rows[k].Cells[1].Value.ToString();
                        string k2 = dGv.Rows[k].Cells[5].Value.ToString();
                        string p1;
                        if (k1.Length > 5) { p1 = k1; }
                        else { p1 = "    " + k1; }
                        string p2;
                        if (k2.Length > 5) { p2 = k2; }
                        else { p2 = "    " + k2; }
                        string[] V = Funcoes.Dvet(Funcoes.Cvet(dGv.Rows[k].Cells[0].Value.ToString(), p1, dGv.Rows[k].Cells[2].Value.ToString(), dGv.Rows[k].Cells[3].Value.ToString(), dGv.Rows[k].Cells[4].Value.ToString(), p2, dGv.Rows[k].Cells[6].Value.ToString(), dGv.Rows[k].Cells[7].Value.ToString(), dGv.Rows[k].Cells[8].Value.ToString()));
                        if (V[1].Length > 5) { V[1] = V[1]; }
                        else { V[1] = "    " + V[1]; }
                        if (V[5].Length > 5) { V[5] = V[5]; }
                        else { V[5] = "    " + V[5]; }
                        return V;
                    }
                    return T;
                }
                return T;
            }
            else { return T; }
        }

        /// <summary>
        /// Muda o index do CmbOperaMtx
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CmbOperamtx_SelectedIndexChanged(object sender, EventArgs e)
        {
            VisibleBotoes(true, false, false, false, false);
            if (cmBdeviceposicao1.Text != "" || cmBdeviceposicao2.Text != "") { AlimentaCmbOut(); }
        }

        /// <summary>
        /// Altera os nomes que estejam associados em alguma combinação
        /// </summary>
        public void Replecear()
        {
            string[] Pain = ReplaceNamesCombinations();
            //Limpa Vetor do sistema
            Properties.Settings.Default.Combinations.Clear();
            //Insere vetor de pgm no vetor de sistema
            Properties.Settings.Default.Combinations.AddRange(Pain);
            //Save
            Properties.Settings.Default.Save();
            //registo log
            log.Debug("As atualizações das Combinações foram salvas");
        }

        /// <summary>
        /// Substitui os nomes que estejam associados em alguma combinação e retorna um string[]
        /// </summary>
        /// <returns>string[] com os nomes já alterados</returns>
        public string[] ReplaceNamesCombinations()
        {
            System.Collections.Specialized.StringCollection Combinados = Properties.Settings.Default.Combinations;
            System.Collections.Specialized.StringCollection NamesPrimary = Properties.Settings.Default.NamesInputPrimary;
            System.Collections.Specialized.StringCollection NamesSecondary = Properties.Settings.Default.NamesInputSecondary;
            System.Collections.Specialized.StringCollection NamesOutput = Properties.Settings.Default.NamesOutput;
            int ind1;
            int ind2;
            string dev1;
            string pos1;
            string operacao;
            string dev2;
            string pos2;
            string name1;
            string name2;
            string namesaida;
            string saida;
            for (int i = 0; i < Combinados.Count; i++)
            {
                if ((Combinados[i] != null) && (Combinados[i] != ""))
                {
                    string[] k = Funcoes.Dvet(Combinados[i]);
                    dev1 = k[0];
                    pos1 = k[1];
                    ind1 = Math.Abs(Funcoes.ZeroNegativo(Funcoes.TToNintINnot(k[1])));
                    _ = k[2];
                    dev2 = k[4];
                    pos2 = k[5];
                    ind2 = Math.Abs(Funcoes.ZeroNegativo(Funcoes.TToNintINnot(k[5])));
                    _ = k[6];
                    operacao = k[3];
                    saida = k[7];
                    namesaida = NamesOutput[Funcoes.ToNintOUT(k[7])];
                    switch (dev1)
                    {
                        case "Primary  ": name1 = NamesPrimary[ind1]; break;
                        case "Secondary": name1 = NamesSecondary[ind1]; break;
                        case "- null -": name1 = "-null!-"; break;
                        default: name1 = k[2]; break;
                    }
                    switch (dev2)
                    {
                        case "Primary  ": name2 = NamesPrimary[ind2]; break;
                        case "Secondary": name2 = NamesSecondary[ind2]; break;
                        case "- null -": name2 = "-null!-"; break;
                        default: name2 = k[6]; break;
                    }

                    string Po1;
                    if (pos1.Length > 5) { Po1 = pos1; }
                    else { Po1 = "    " + pos1; }

                    string Po2;
                    if (pos2.Length > 5) { Po2 = pos2; }
                    else { Po2 = "    " + pos2; }
                    Combinados[i] = Funcoes.Cvet(dev1, Po1, name1, operacao, dev2, Po2, name2, saida, namesaida);
                }
                else { Combinados[i] = ""; }
            }
            List<string> shukaku = new List<string>();
            int ct = 0;
            for (int i = 0; i <= Combinados.Count - 1; i++)
            {
                if (Combinados[i] != "")
                {
                    shukaku.Add(Combinados[i]);
                    ct++;
                }
            }
            int ctt = ct;
            if (ctt >= 0)
            {
                string[] minato = new string[ctt];
                for (int i = 0; i < ctt; i++) { minato[i] = shukaku[i]; }
                return minato;
            }
            else
            {
                string[] majula = new string[1];
                majula[0] = " - null - ";
                return majula;
            }
        }

        /// <summary>
        /// Click dategridview
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DGv_Click(object sender, EventArgs e)
        {
            if (dGv.RowCount <= 0) { }
            else
            {
                if (dGv.Rows[dGv.CurrentRow.Index].Cells[0].Value != null && dGv.Rows[0].Cells[0].Value.ToString() != "" && cmBdeviceposicao1.Text == "" && cmBdeviceposicao2.Text == "")
                { VisibleBotoes(true, true, true, false, true); if (dGv.CurrentRow.Index < 0) { VisibleBotoes(false, false, false, false, true); } }
                else { VisibleBotoes(false, false, false, true, true); }
            }
        }

        /// <summary>
        /// Limpa os campos de preenchimento das combinações
        /// </summary>
        public void ClearCamposMtx()
        {
            cmBdeviceposicao1.Text = "";
            cmBdeviceposicao1.Items.Clear();
            cmBdeviceposicao1.Items.Add("Primary  ");
            cmBdeviceposicao1.Items.Add("Secondary");
            cmBdeviceposicao1.Items.Add("- null  -");
            cmBposicao1.Text = "";
            cmBposicao1.Items.Clear();
            cmBdeviceposicao2.Text = "";
            cmBdeviceposicao2.Items.Clear();
            cmBdeviceposicao2.Items.Add("Primary  ");
            cmBdeviceposicao2.Items.Add("Secondary");
            cmBdeviceposicao2.Items.Add("- null  -");
            cmBposicao2.Text = "";
            cmBposicao2.Items.Clear();
            cmbOperamtx.Items.Clear();
            cmbOperamtx.Items.Add("AND");
            cmbOperamtx.Items.Add(" OR");
            cmbSaida.Items.Clear();
            LblPosicao1.Text = "";
            LblPosicao2.Text = "";
            LblOutput.Text = "";
        }

        /// <summary>
        /// Recebe string dos campos preenchidos, criar vetor e insere no datagridview
        /// </summary>
        public void AlimentaDgv()
        {
            string dados = Funcoes.Cvet(cmBdeviceposicao1.Text, cmBposicao1.Text, LblPosicao1.Text, cmbOperamtx.Text, cmBdeviceposicao2.Text, cmBposicao2.Text, LblPosicao2.Text, cmbSaida.Text, LblOutput.Text);
            if (dados.Length == 100)
            {
                string[] ds = Funcoes.Dvet(dados);
                dGv.Rows.Add(ds);
            }
            else
            {
                using (Form MsgBox1w = new MmsgBox("erro tamanho string: " + dados.Length.ToString(), "OK", 4, 0))
                {
                    DialogResult resultado1w = MsgBox1w.ShowDialog();
                    if (resultado1w == DialogResult.OK)
                    { }
                }
            }
        }

        /// <summary>
        /// Habilita a visualização dos botões
        /// </summary>
        /// <param name="n0">bool btn Cancel</param>
        /// <param name="n1">bool btn Del</param>
        /// <param name="n2">bool btn Edit</param>
        /// <param name="n3">bool btn Add</param>
        /// <param name="n4">bool btn Save</param>
        public void VisibleBotoes(bool n0, bool n1, bool n2, bool n3, bool n4)
        {
            if (n0 == true) { btnCancel.Enabled = true; }
            else { btnCancel.Enabled = false; }
            if (n1 == true) { btnDel.Enabled = true; }
            else { btnDel.Enabled = false; }
            if (n2 == true) { btnEdit.Enabled = true; }
            else { btnEdit.Enabled = false; }
            if (n3 == true) { btnAdd.Enabled = true; }
            else { btnAdd.Enabled = false; }
            if (n4 == true) { btnSave_.Enabled = true; }
            else { btnSave_.Enabled = false; }
        }

        /// <summary>
        /// Verifica as condições para gravação das combinações, retorna TRUE se estiver OK para gravação
        /// </summary>
        /// <param name="dev1"> string device 1</param>
        /// <param name="pos1">string posição 1</param>
        /// <param name="operacao">string operação</param>
        /// <param name="dev2"> string device 2</param>
        /// <param name="pos2">string posição 2</param>
        /// <param name="saida">string saida</param>
        /// <returns>Retorna TRUE se atender a todas as condições para gravação</returns>
        public bool ValidaCombinations(string dev1, string pos1, string operacao, string dev2, string pos2, string saida)
        {
            if (dev1 == "- null  -" && dev2 == "- null  -")
            {
                using (Form MsgBox1w = new MmsgBox("Cannot have the condition of 2 devices 'null'.", "OK", 3, 0))
                {
                    //Não pode ter a condição de 2 devices null
                    DialogResult resultado1w = MsgBox1w.ShowDialog();
                    if (resultado1w == DialogResult.OK) { return false; }
                    return false;
                }
            }
            if (((dev1 == "- null  -" || dev2 == "- null  -") || (pos1 == "- null  -" || pos2 == "- null  -")) && operacao == "AND")
            {
                using (Form MsgBox2w = new MmsgBox("Condition cannot have AND function with some position 'null'.", "OK", 3, 0))
                {
                    //Não pode função ter função AND com 2 devices/posicao null
                    DialogResult resultado2w = MsgBox2w.ShowDialog();
                    if (resultado2w == DialogResult.OK) { return false; }
                    return false;
                }
            }
            if (pos1 == "" || pos2 == "" || operacao == "" || saida == "")
            {
                using (Form MsgBox3w = new MmsgBox("It is not possible to record the condition with any of the fields empty.", "OK", 3, 0))
                {
                    //Não pode gravar condição quando alguns dos campos pos1, pos2, operacao e saida estiverem ""
                    DialogResult resultado3w = MsgBox3w.ShowDialog();
                    if (resultado3w == DialogResult.OK) { return false; }
                    return false;
                }
            }
            if (pos1 == "- null  -" && pos2 == "- null  -")
            {
                using (Form MsgBox4w = new MmsgBox("You cannot record when the 2 positions are '- null -'.", "OK", 3, 0))
                {
                    //Não pode gravar quando as 2 posições forem  "- null -"
                    DialogResult resultado4w = MsgBox4w.ShowDialog();
                    if (resultado4w == DialogResult.OK) { return false; }
                    return false;
                }
            }
            else { return true; }
        }

        /// <summary>
        /// Obtém numero de combinações
        /// </summary>
        public void CountCombinacoes()
        { CountComb.Text = Properties.Settings.Default.Combinations.Count.ToString(); }

        /// <summary>
        /// Preenche os comboBox das combinações
        /// </summary>
        public void ComboBoxMtx()
        {
            cmBdeviceposicao1.Text = "";
            cmBdeviceposicao1.Items.Clear();
            cmBdeviceposicao1.Items.Add("Primary  ");
            cmBdeviceposicao1.Items.Add("Secondary");
            cmBdeviceposicao1.Items.Add("- null  -");
            LblPosicao1.Text = "";
            cmBposicao1.Text = "";
            cmBposicao1.Items.Clear();
            cmBdeviceposicao2.Text = "";
            cmBdeviceposicao2.Items.Clear();
            cmBdeviceposicao2.Items.Add("Primary  ");
            cmBdeviceposicao2.Items.Add("Secondary");
            cmBdeviceposicao2.Items.Add("- null  -");
            LblPosicao2.Text = "";
            cmBposicao2.Text = "";
            cmBposicao2.Items.Clear();
            cmbOperamtx.Items.Clear();
            cmbOperamtx.Items.Add("AND");
            cmbOperamtx.Items.Add(" OR");
            cmbSaida.Items.Clear();
            LblOutput.Text = "";
        }

        //Função para obter as configuração e retornar em um vertor string criptografado
        public string[] GetConfigCripto()
        {
            string[] Config = new string[21];
            Config[0] = "System: " + Properties.Settings.Default.System;
            Config[1] = "IP Primary: " + Properties.Settings.Default.IP_Primary;
            Config[2] = "IP Secondary: " + Properties.Settings.Default.IP_Secondary;
            Config[3] = "IP Output: " + Properties.Settings.Default.IP_Output;
            Config[4] = "Input device Chg0: " + Properties.Settings.Default.InputDeviceChg0;
            Config[5] = "Device Chg0: " + Properties.Settings.Default.DeviceChg0;
            Config[6] = "Current selection: " + Properties.Settings.Default.CurrentSelection;
            Config[7] = "Log Operation: " + Properties.Settings.Default.Path_LogOperation;
            Config[8] = "Enable Log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            Config[9] = "System Log: " + Properties.Settings.Default.SystemLog;
            Config[10] = "MaxSize Log: " + Properties.Settings.Default.MaxSizeLog.ToString() + "MB";
            Config[11] = "Enable Combinations Log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            Config[12] = "Data log:";
            Config[13] = "Username: " + Properties.Settings.Default.DatawrittenLog[0];
            Config[14] = "Level: " + Properties.Settings.Default.DatawrittenLog[1];
            Config[15] = "Method: " + Properties.Settings.Default.DatawrittenLog[2];
            Config[16] = "Message: " + Properties.Settings.Default.DatawrittenLog[3];
            Config[17] = "Thread: " + Properties.Settings.Default.DatawrittenLog[4];
            Config[18] = "Line: " + Properties.Settings.Default.DatawrittenLog[5];
            Config[19] = "Identity: " + Properties.Settings.Default.DatawrittenLog[6];
            Config[20] = "Location: " + Properties.Settings.Default.DatawrittenLog[7];

            System.Collections.Specialized.StringCollection NamesPrimario = Properties.Settings.Default.NamesInputPrimary;
            System.Collections.Specialized.StringCollection NamesSecundario = Properties.Settings.Default.NamesInputSecondary;
            System.Collections.Specialized.StringCollection NamesOutput = Properties.Settings.Default.NamesOutput;
            System.Collections.Specialized.StringCollection Combinations = Properties.Settings.Default.Combinations;

            string[] allFiles = new string[Config.Length + NamesPrimario.Count + NamesSecundario.Count + NamesOutput.Count + Combinations.Count + 5];
            allFiles[0] = "->Configuration";
            Config.CopyTo(allFiles, 01);
            allFiles[22] = "->Names Primary";
            NamesPrimario.CopyTo(allFiles, 23);
            allFiles[39] = "->Names Secondary";
            NamesSecundario.CopyTo(allFiles, 40);
            allFiles[56] = "->Names Output";
            NamesOutput.CopyTo(allFiles, 57);
            allFiles[73] = "->Combinations";
            Combinations.CopyTo(allFiles, 74);

            var key = GeraKey();
            var IV = GeraIv();
            using (Rijndael myRijndael = Rijndael.Create())
            {
                for (int i = 0; i < allFiles.Length; i++)
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes(allFiles[i], key, IV);
                    // Informações.Items.Add(Convert.ToBase64String(encrypted));
                    allFiles[i] = Convert.ToBase64String(encrypted);
                }
            }
            return allFiles;
        }

        //Função para obter as configuração e retornar em um vertor string criptografado
        public string[] GetCfgCripto()
        {
            string[] Config = new string[21];
            Config[0] = "System: " + Properties.Settings.Default.System;
            Config[1] = "IP Primary: " + Properties.Settings.Default.IP_Primary;
            Config[2] = "IP Secondary: " + Properties.Settings.Default.IP_Secondary;
            Config[3] = "IP Output: " + Properties.Settings.Default.IP_Output;
            Config[4] = "Input device Chg0: " + Properties.Settings.Default.InputDeviceChg0;
            Config[5] = "Device Chg0: " + Properties.Settings.Default.DeviceChg0;
            Config[6] = "Current selection: " + Properties.Settings.Default.CurrentSelection;
            Config[7] = "Log Operation: " + Properties.Settings.Default.Path_LogOperation;
            Config[8] = "Enable Log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            Config[9] = "System Log: " + Properties.Settings.Default.SystemLog;
            Config[10] = "MaxSize Log: " + Properties.Settings.Default.MaxSizeLog.ToString() + "MB";
            Config[11] = "Enable Combinations Log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            Config[12] = "Data log:";
            Config[13] = "Username: " + Properties.Settings.Default.DatawrittenLog[0];
            Config[14] = "Level: " + Properties.Settings.Default.DatawrittenLog[1];
            Config[15] = "Method: " + Properties.Settings.Default.DatawrittenLog[2];
            Config[16] = "Message: " + Properties.Settings.Default.DatawrittenLog[3];
            Config[17] = "Thread: " + Properties.Settings.Default.DatawrittenLog[4];
            Config[18] = "Line: " + Properties.Settings.Default.DatawrittenLog[5];
            Config[19] = "Identity: " + Properties.Settings.Default.DatawrittenLog[6];
            Config[20] = "Location: " + Properties.Settings.Default.DatawrittenLog[7];

            System.Collections.Specialized.StringCollection NamesPrimario = Properties.Settings.Default.NamesInputPrimary;
            System.Collections.Specialized.StringCollection NamesSecundario = Properties.Settings.Default.NamesInputSecondary;
            System.Collections.Specialized.StringCollection NamesOutput = Properties.Settings.Default.NamesOutput;
            System.Collections.Specialized.StringCollection Combinations = Properties.Settings.Default.Combinations;

            string[] allFiles = new string[Config.Length + NamesPrimario.Count + NamesSecundario.Count + NamesOutput.Count + Combinations.Count + 5];
            allFiles[0] = "->Configuration";
            Config.CopyTo(allFiles, 01);
            allFiles[22] = "->Names Primary";
            NamesPrimario.CopyTo(allFiles, 23);
            allFiles[39] = "->Names Secondary";
            NamesSecundario.CopyTo(allFiles, 40);
            allFiles[56] = "->Names Output";
            NamesOutput.CopyTo(allFiles, 57);
            allFiles[73] = "->Combinations";
            Combinations.CopyTo(allFiles, 74);


            var key = GeraKey();
            var IV = GeraIv();
            using (Rijndael myRijndael = Rijndael.Create())
            {
                for (int i = 0; i < allFiles.Length; i++)
                {
                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes(allFiles[i], key, IV);
                    // Informações.Items.Add(Convert.ToBase64String(encrypted));
                    allFiles[i] = Convert.ToBase64String(encrypted);
                }
            }
            return allFiles;
        }

        //Função para obter as configuração e retornar em um vertor string
        public string[] GetConfig()
        {
            string[] Config = new string[21];
            Config[0] = "System: " + Properties.Settings.Default.System;
            Config[1] = "IP Primary: " + Properties.Settings.Default.IP_Primary;
            Config[2] = "IP Secondary: " + Properties.Settings.Default.IP_Secondary;
            Config[3] = "IP Output: " + Properties.Settings.Default.IP_Output;
            Config[4] = "Input device Chg0: " + Properties.Settings.Default.InputDeviceChg0;
            Config[5] = "Device Chg0: " + Properties.Settings.Default.DeviceChg0;
            Config[6] = "Current selection: " + Properties.Settings.Default.CurrentSelection;
            Config[7] = "Log Operation: " + Properties.Settings.Default.Path_LogOperation;
            Config[8] = "Enable Log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            Config[9] = "System Log: " + Properties.Settings.Default.SystemLog;
            Config[10] = "MaxSize Log: " + Properties.Settings.Default.MaxSizeLog.ToString() + "MB";
            Config[11] = "Enable Combinations Log: " + Properties.Settings.Default.EnableCombinationsLog.ToString();
            Config[12] = "Data log:";
            Config[13] = "Username: " + Properties.Settings.Default.DatawrittenLog[0];
            Config[14] = "Level: " + Properties.Settings.Default.DatawrittenLog[1];
            Config[15] = "Method: " + Properties.Settings.Default.DatawrittenLog[2];
            Config[16] = "Message: " + Properties.Settings.Default.DatawrittenLog[3];
            Config[17] = "Thread: " + Properties.Settings.Default.DatawrittenLog[4];
            Config[18] = "Line: " + Properties.Settings.Default.DatawrittenLog[5];
            Config[19] = "Identity: " + Properties.Settings.Default.DatawrittenLog[6];
            Config[20] = "Location: " + Properties.Settings.Default.DatawrittenLog[7];

            System.Collections.Specialized.StringCollection NamesPrimario = Properties.Settings.Default.NamesInputPrimary;
            System.Collections.Specialized.StringCollection NamesSecundario = Properties.Settings.Default.NamesInputSecondary;
            System.Collections.Specialized.StringCollection NamesOutput = Properties.Settings.Default.NamesOutput;
            System.Collections.Specialized.StringCollection Combinations = Properties.Settings.Default.Combinations;

            string[] allFiles = new string[Config.Length + NamesPrimario.Count + NamesSecundario.Count + NamesOutput.Count + Combinations.Count + 5];
            allFiles[0] = "->Configuration";
            Config.CopyTo(allFiles, 01);
            allFiles[22] = "->Names Primary";
            NamesPrimario.CopyTo(allFiles, 23);
            allFiles[39] = "->Names Secondary";
            NamesSecundario.CopyTo(allFiles, 40);
            allFiles[56] = "->Names Output";
            NamesOutput.CopyTo(allFiles, 57);
            allFiles[73] = "->Combinations";
            Combinations.CopyTo(allFiles, 74);

            return allFiles;
        }

        /// <summary>
        /// Funçõa para exportar as configurações para um arquivo .chg0
        /// </summary>
        /// <param name="entrada">string[] entrada com as configurações salvas no sistema</param>
        public void ExportConfig(string[] entrada)
        {
            string[] lines = entrada;
            log.Debug("Botão Export Configuration acionado");
            string pathselect;
            string ID;
            using (FolderBrowserDialog folderDlg = new FolderBrowserDialog { ShowNewFolderButton = true })
            {
                DialogResult result = folderDlg.ShowDialog();
                if (result == DialogResult.OK)
                {
                    pathselect = folderDlg.SelectedPath.Replace(@"\", @"/");
                    _ = folderDlg.RootFolder;
                    ID = pathselect + @"/" + Funcoes.GetDateSystem().Replace(@", ", @" ").Replace(@"\", @"_").Replace(@"/", @"_") + "_" + DateTime.Now.ToLongTimeString().Replace(@":", @"_") + "_confg.chg0";
                    //textBox1.Text = ID;
                    File.WriteAllLines(ID, lines);
                    log.Debug("Escolhido novo path para salvar as configurações");
                    using (Form MsgBox = new MmsgBox("Configuration were exported in:" + Environment.NewLine + ID, "OK", 1, 0))
                    { DialogResult resultado = MsgBox.ShowDialog(); }
                }
                else
                {
                    using (Form MsgBox = new MmsgBox("Export configuration canceled", "OK", 1, 0))
                    { DialogResult resultado = MsgBox.ShowDialog(); }
                }
            }
        }

        /// <summary>
        /// Funçõa para import as configurações para um arquivo .chg0
        /// </summary>
        public string[] ImportConfig()
        {
            string[] configura = new string[1];
            using (OpenFileDialog1 = new OpenFileDialog())
            {
                OpenFileDialog1.Filter = "Text files (*.chg0)|*.chg0";
                if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var sr = new StreamReader(OpenFileDialog1.FileName))
                        {
                            Array.Resize(ref configura, File.ReadAllLines(OpenFileDialog1.FileName).Length);
                            configura = File.ReadAllLines(OpenFileDialog1.FileName);
                        }
                    }
                    catch (SecurityException ex)
                    {
                        MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" +
                        $"Details:\n\n{ex.StackTrace}");
                    }
                }
            }

            return configura;
        }

        public string[] ImportCripto()
        {
            string[] configurar = { "ERRO_CRIPTOGRAFIA" };
            string[] configura = new string[70];
            bool chk = true;
            using (OpenFileDialog1 = new OpenFileDialog())
            {
                OpenFileDialog1.Filter = "Text files (*.chg0)|*.chg0";
                if (OpenFileDialog1.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (var sr = new StreamReader(OpenFileDialog1.FileName))
                        {
                            configura = File.ReadAllLines(OpenFileDialog1.FileName);
                            var key = GeraKey();
                            var IV = GeraIv();
                            using (Rijndael myRijndael = Rijndael.Create())
                            {
                                try
                                {
                                    Convert.FromBase64String(configura[0]);
                                    for (int i = 0; i < configura.Length && chk; i++)
                                    {
                                        byte[] enc = Convert.FromBase64String(configura[i]);
                                        // Decrypt the bytes to a string.
                                        string roundtrip = DecryptStringFromBytes(enc, key, IV);
                                        if (roundtrip == "Erro na decriptografia")
                                        { chk = false; }
                                        else
                                        { configura[i] = roundtrip; }
                                    }
                                }
                                catch (Exception)
                                {
                                    configura = configurar;
                                    using (MmsgBox mmsgBox = new MmsgBox("Encryption error!", "OK", 1, 0))
                                    { _ = mmsgBox.ShowDialog(); }
                                }
                            }
                        }
                    }
                    catch (SecurityException ex)
                    { MessageBox.Show($"Security error.\n\nError message: {ex.Message}\n\n" + $"Details:\n\n{ex.StackTrace}"); }
                }
                else
                { configura = configurar; }
                if (chk == false) { configura = configurar; }
                return configura;
            }
        }

        public string Encrypto(string entrada)
        {
            var key = GeraKey();
            var IV = GeraIv();
            string original = entrada;
            using (Rijndael myRijndael = Rijndael.Create())
            {
                // Encrypt the string to an array of bytes.
                byte[] encrypted = EncryptStringToBytes(original, key, IV);
                // Informações.Items.Add(Convert.ToBase64String(encrypted));
                return Convert.ToBase64String(encrypted);
            }
        }

        public string Decrypto(string entrada)
        {
            var key = GeraKey();
            var IV = GeraIv();
            try
            {
                byte[] enc = Convert.FromBase64String(entrada);
                // Decrypt the bytes to a string.
                string roundtrip = DecryptStringFromBytes(enc, key, IV);
                return ("Resultado: " + roundtrip);
            }
            catch (Exception)
            {
                MessageBox.Show("Erro na codificação");
                return ("Resultado: " + "Erro decriptografia");
            }
        }

        public string[] SetConfig(string[] entrada)
        {
            string[] saida = new string[entrada.Length - 1];
            if (entrada[0] == "ERRO_CRIPTOGRAFIA")
            {
                Array.Resize(ref saida, 1);
                saida[0] = "ERRO_CRIPTOGRAFIA";
            }
            else
            {
                saida[0] = "->Configuration";
                //system
                saida[1] = entrada[1].Substring(8);
                //Ip's
                saida[2] = entrada[2].Substring(12);
                saida[3] = entrada[3].Substring(14);
                saida[4] = entrada[4].Substring(11);
                //Config Chg0
                saida[5] = entrada[5].Substring(19);
                saida[6] = entrada[6].Substring(13);
                saida[7] = entrada[7].Substring(19);
                //Current Selection
                saida[8] = entrada[8].Substring(15);
                //Log's
                saida[9] = entrada[9].Substring(12);
                saida[10] = entrada[10].Substring(12);
                saida[11] = entrada[11].Substring(13, 3);
                saida[12] = entrada[12].Substring(25);
                //Data_Logs
                saida[13] = entrada[14].Substring(10);
                saida[14] = entrada[15].Substring(7);
                saida[15] = entrada[16].Substring(8);
                saida[16] = entrada[17].Substring(9);
                saida[17] = entrada[18].Substring(8);
                saida[18] = entrada[19].Substring(6);
                saida[19] = entrada[20].Substring(10);
                saida[20] = entrada[21].Substring(10);

                // Extracting a slice into another array
                string[] Slice = new List<string>(entrada).GetRange(22, (entrada.Length - 22)).ToArray();
                Slice.CopyTo(saida, 21);
            }


            return saida;
        }

        public string[] SetCfg(string[] entrada)
        {
            string[] saida = entrada;
            //system
            saida[1] = entrada[1].Substring(8);
            //Ip's
            saida[2] = entrada[2].Substring(12);
            saida[3] = entrada[3].Substring(14);
            saida[4] = entrada[4].Substring(11);
            //Config Chg0
            saida[5] = entrada[5].Substring(19);
            saida[6] = entrada[6].Substring(13);
            saida[7] = entrada[7].Substring(19);
            //Current Selection
            saida[8] = entrada[8].Substring(15);
            //Log's
            saida[9] = entrada[9].Substring(12);
            saida[10] = entrada[10].Substring(12);
            saida[11] = entrada[11].Substring(13, 3);
            saida[12] = entrada[12].Substring(25);
            //Data_Logs
            saida[13] = entrada[14].Substring(10);
            saida[14] = entrada[15].Substring(7);
            saida[15] = entrada[16].Substring(7);
            saida[16] = entrada[17].Substring(9);
            saida[17] = entrada[18].Substring(8);
            saida[18] = entrada[19].Substring(6);
            saida[19] = entrada[20].Substring(10);
            saida[20] = entrada[21].Substring(10);
            saida[21] = entrada[22].Substring(19);
            return saida;
        }

        public string[] Nprimary(string[] entrada)
        {
            string[] saida = new string[16];
            for (int i = 0; i < 16; i++)
            { saida[i] = entrada[14 + i]; }
            return saida;
        }

        public string[] Nsecondary(string[] entrada)
        {
            string[] saida = new string[16];
            for (int i = 0; i < 16; i++)
            { saida[i] = entrada[31 + i]; }
            return saida;
        }

        public string[] Noutput(string[] entrada)
        {
            string[] saida = new string[16];
            for (int i = 0; i < 16; i++)
            { saida[i] = entrada[48 + i]; }
            return saida;
        }

        public string[] Ncombinacoes(string[] entrada)
        {
            string[] saida = new string[entrada.Length - 65];
            for (int i = 0; i < saida.Length; i++)
            { saida[i] = entrada[65 + i]; }
            return saida;
        }

        #endregion

        #region Names INOUTs
        /// <summary>
        /// Função que insere o CHG0   .
        /// </summary>
        /// <param name="systemType">string systemType</param>
        /// <param name="vtPrimary">stringCollection com dados do vetor primário </param>
        /// <param name="vtSecondary">stringCollection com dados do vetor secundário </param>
        /// <param name="vtOutput">stringCollection com dados do vetor output </param>
        /// <param name="CHG0">posiççao com do CHG0</param>
        public void InsertChg0(string systemType, System.Collections.Specialized.StringCollection vtPrimary, System.Collections.Specialized.StringCollection vtSecondary, System.Collections.Specialized.StringCollection vtOutput, string CHG0)
        {
            switch (systemType)
            {
                case "Change Over":
                    string change = "  CHG0    ";
                    if (CHG0 == "IN 01") { vtPrimary[0] = change; vtSecondary[0] = change; vtOutput[0] = change; CmpTIN01.Enabled = false; }
                    if (CHG0 == "IN 02") { vtPrimary[1] = change; vtSecondary[1] = change; vtOutput[1] = change; CmpTIN02.Enabled = false; }
                    if (CHG0 == "IN 03") { vtPrimary[2] = change; vtSecondary[2] = change; vtOutput[2] = change; CmpTIN03.Enabled = false; }
                    if (CHG0 == "IN 04") { vtPrimary[3] = change; vtSecondary[3] = change; vtOutput[3] = change; CmpTIN04.Enabled = false; }
                    if (CHG0 == "IN 05") { vtPrimary[4] = change; vtSecondary[4] = change; vtOutput[4] = change; CmpTIN05.Enabled = false; }
                    if (CHG0 == "IN 06") { vtPrimary[5] = change; vtSecondary[5] = change; vtOutput[5] = change; CmpTIN06.Enabled = false; }
                    if (CHG0 == "IN 07") { vtPrimary[6] = change; vtSecondary[6] = change; vtOutput[6] = change; CmpTIN07.Enabled = false; }
                    if (CHG0 == "IN 08") { vtPrimary[7] = change; vtSecondary[7] = change; vtOutput[7] = change; CmpTIN08.Enabled = false; }
                    if (CHG0 == "IN 09") { vtPrimary[8] = change; vtSecondary[8] = change; vtOutput[8] = change; CmpTIN09.Enabled = false; }
                    if (CHG0 == "IN 10") { vtPrimary[9] = change; vtSecondary[9] = change; vtOutput[9] = change; CmpTIN010.Enabled = false; }
                    if (CHG0 == "IN 11") { vtPrimary[10] = change; vtSecondary[10] = change; vtOutput[10] = change; CmpTIN011.Enabled = false; }
                    if (CHG0 == "IN 12") { vtPrimary[11] = change; vtSecondary[11] = change; vtOutput[11] = change; CmpTIN012.Enabled = false; }
                    if (CHG0 == "IN 13") { vtPrimary[12] = change; vtSecondary[12] = change; vtOutput[12] = change; CmpTIN013.Enabled = false; }
                    if (CHG0 == "IN 14") { vtPrimary[13] = change; vtSecondary[13] = change; vtOutput[13] = change; CmpTIN014.Enabled = false; }
                    if (CHG0 == "IN 15") { vtPrimary[14] = change; vtSecondary[14] = change; vtOutput[14] = change; CmpTIN015.Enabled = false; }
                    if (CHG0 == "IN 16") { vtPrimary[15] = change; vtSecondary[15] = change; vtOutput[15] = change; CmpTIN016.Enabled = false; }
                    break;
                case "Matrix of Signals":
                    { EnableFields(); }
                    break;
                default:
                    using (Form MsgBox = new MmsgBox("Standard error!", "OK", 3, 0))
                    { _ = MsgBox.ShowDialog(); }
                    break;
            }
        }

        public void Enablefields_titular()
        {
            CmpTIN01.Enabled = true;
            CmpTIN02.Enabled = true;
            CmpTIN03.Enabled = true;
            CmpTIN04.Enabled = true;
            CmpTIN05.Enabled = true;
            CmpTIN06.Enabled = true;
            CmpTIN07.Enabled = true;
            CmpTIN08.Enabled = true;
            CmpTIN09.Enabled = true;
            CmpTIN010.Enabled = true;
            CmpTIN011.Enabled = true;
            CmpTIN012.Enabled = true;
            CmpTIN013.Enabled = true;
            CmpTIN014.Enabled = true;
            CmpTIN015.Enabled = true;
            CmpTIN016.Enabled = true;
        }

        public void Enablefields_Reserva()
        {
            CmpRIN1.Enabled = true;
            CmpRIN2.Enabled = true;
            CmpRIN3.Enabled = true;
            CmpRIN4.Enabled = true;
            CmpRIN5.Enabled = true;
            CmpRIN6.Enabled = true;
            CmpRIN7.Enabled = true;
            CmpRIN8.Enabled = true;
            CmpRIN9.Enabled = true;
            CmpRIN10.Enabled = true;
            CmpRIN11.Enabled = true;
            CmpRIN12.Enabled = true;
            CmpRIN13.Enabled = true;
            CmpRIN14.Enabled = true;
            CmpRIN15.Enabled = true;
            CmpRIN16.Enabled = true;
        }

        public void EnableFields_Output()
        {
            CmpOut1.Enabled = true;
            CmpOut2.Enabled = true;
            CmpOut3.Enabled = true;
            CmpOut4.Enabled = true;
            CmpOut5.Enabled = true;
            CmpOut6.Enabled = true;
            CmpOut7.Enabled = true;
            CmpOut8.Enabled = true;
            CmpOut9.Enabled = true;
            CmpOut10.Enabled = true;
            CmpOut11.Enabled = true;
            CmpOut12.Enabled = true;
            CmpOut13.Enabled = true;
            CmpOut14.Enabled = true;
            CmpOut15.Enabled = true;
            CmpOut16.Enabled = true;
        }

        public void EnableFields()
        {
            Enablefields_titular();
            Enablefields_Reserva();
            EnableFields_Output();
        }

        /// <summary>
        /// Função verifica se tem mais de um "  CHG0    ".
        /// </summary>
        /// <param name="vetorIn">string[] com inputs</param>
        /// <returns>retorna TRUE se possuir apenas um "  CHG0    ". </returns>
        public bool Somente1Chg0Vetor(string[] vetorIn)
        {
            int qtd = 0;
            for (int i = 0; i < vetorIn.Length; i++)
            { if (vetorIn[i] == "  CHG0    ") { qtd += 1; } }
            if (qtd == 0) { return false; }
            if (qtd == 1) { return true; }
            if (qtd > 1) { return false; }
            else
            {
                using (Form MsgBox = new MmsgBox("No '  CHG0    '! ", "OK", 3, 0))
                {
                    DialogResult resultado = MsgBox.ShowDialog();
                    return false;
                }
            }
        }

        /// <summary>
        /// Função verifica se tem mais de um "  CHG0    " com 2 retornos.
        /// </summary>
        /// <param name="vetorIn">string[] com inputs</param>
        /// <returns>retorna TRUE se atender as condições</returns>
        public string Somente1Chg0Vetor2ret(string[] vetorIn)
        {
            int qtd = 0;
            for (int i = 0; i < vetorIn.Length; i++)
            { if (vetorIn[i] == "  CHG0    ") { qtd += 1; } }
            string ret;
            if (qtd == 0) { ret = "(false, 0)"; return ret; }
            if (qtd == 1)
            { ret = "(true, 1)"; return ret; }
            if (qtd > 1) { ret = "(false, 2)"; return ret; }
            else
            {
                using (Form MsgBox = new MmsgBox("No '  CHG0    '!", "OK", 3, 0))
                {
                    DialogResult resultado = MsgBox.ShowDialog();
                    ret = "(false, -1)";
                    return ret;
                }
            }
        }

        /// <summary>
        /// Função verifica em qual posição está "  CHG0    ".
        /// </summary>
        /// <param name="vetor1In">string[] com inputs primário</param>
        /// <param name="vetor2In">string[] com inputs secundário</param>
        /// <param name="vetorOut">string[] com out</param>
        public void PosicaoChg0noVetor(System.Collections.Specialized.StringCollection vetor1In, System.Collections.Specialized.StringCollection vetor2In, System.Collections.Specialized.StringCollection vetorOut)
        {
            switch (vetor1In.IndexOf("  CHG0    ") + 1)
            {
                case 1: Properties.Settings.Default.NamesInputPrimary[0] = "IN 01  "; CmpTIN01.Enabled = true; break;
                case 2: Properties.Settings.Default.NamesInputPrimary[1] = "IN 02  "; CmpTIN02.Enabled = true; break;
                case 3: Properties.Settings.Default.NamesInputPrimary[2] = "IN 03  "; CmpTIN03.Enabled = true; break;
                case 4: Properties.Settings.Default.NamesInputPrimary[3] = "IN 04  "; CmpTIN04.Enabled = true; break;
                case 5: Properties.Settings.Default.NamesInputPrimary[4] = "IN 05  "; CmpTIN05.Enabled = true; break;
                case 6: Properties.Settings.Default.NamesInputPrimary[5] = "IN 06  "; CmpTIN06.Enabled = true; break;
                case 7: Properties.Settings.Default.NamesInputPrimary[6] = "IN 07  "; CmpTIN07.Enabled = true; break;
                case 8: Properties.Settings.Default.NamesInputPrimary[7] = "IN 08  "; CmpTIN08.Enabled = true; break;
                case 9: Properties.Settings.Default.NamesInputPrimary[8] = "IN 09  "; CmpTIN09.Enabled = true; break;
                case 10: Properties.Settings.Default.NamesInputPrimary[9] = "IN 10  "; CmpTIN010.Enabled = true; break;
                case 11: Properties.Settings.Default.NamesInputPrimary[10] = "IN 11    "; CmpTIN011.Enabled = true; break;
                case 12: Properties.Settings.Default.NamesInputPrimary[11] = "IN 12  "; CmpTIN012.Enabled = true; break;
                case 13: Properties.Settings.Default.NamesInputPrimary[12] = "IN 13  "; CmpTIN013.Enabled = true; break;
                case 14: Properties.Settings.Default.NamesInputPrimary[13] = "IN 14  "; CmpTIN014.Enabled = true; break;
                case 15: Properties.Settings.Default.NamesInputPrimary[14] = "IN 15  "; CmpTIN015.Enabled = true; break;
                case 16: Properties.Settings.Default.NamesInputPrimary[15] = "IN 16  "; CmpTIN016.Enabled = true; break;
                default: break;
            }
            switch (vetor2In.IndexOf("  CHG0    ") + 1)
            {
                case 1: Properties.Settings.Default.NamesInputSecondary[0] = "IN 01  "; CmpRIN1.Enabled = true; break;
                case 2: Properties.Settings.Default.NamesInputSecondary[1] = "IN 02  "; CmpRIN2.Enabled = true; break;
                case 3: Properties.Settings.Default.NamesInputSecondary[2] = "IN 03  "; CmpRIN3.Enabled = true; break;
                case 4: Properties.Settings.Default.NamesInputSecondary[3] = "IN 04  "; CmpRIN4.Enabled = true; break;
                case 5: Properties.Settings.Default.NamesInputSecondary[4] = "IN 05  "; CmpRIN5.Enabled = true; break;
                case 6: Properties.Settings.Default.NamesInputSecondary[5] = "IN 06  "; CmpRIN6.Enabled = true; break;
                case 7: Properties.Settings.Default.NamesInputSecondary[6] = "IN 07  "; CmpRIN7.Enabled = true; break;
                case 8: Properties.Settings.Default.NamesInputSecondary[7] = "IN 08  "; CmpRIN8.Enabled = true; break;
                case 9: Properties.Settings.Default.NamesInputSecondary[8] = "IN 09  "; CmpRIN9.Enabled = true; break;
                case 10: Properties.Settings.Default.NamesInputSecondary[9] = "IN 10  "; CmpRIN10.Enabled = true; break;
                case 11: Properties.Settings.Default.NamesInputSecondary[10] = "IN 11  "; CmpRIN11.Enabled = true; break;
                case 12: Properties.Settings.Default.NamesInputSecondary[11] = "IN 12  "; CmpRIN12.Enabled = true; break;
                case 13: Properties.Settings.Default.NamesInputSecondary[12] = "IN 13  "; CmpRIN13.Enabled = true; break;
                case 14: Properties.Settings.Default.NamesInputSecondary[13] = "IN 14  "; CmpRIN14.Enabled = true; break;
                case 15: Properties.Settings.Default.NamesInputSecondary[14] = "IN 15  "; CmpRIN15.Enabled = true; break;
                case 16: Properties.Settings.Default.NamesInputSecondary[15] = "IN 16  "; CmpRIN16.Enabled = true; break;
                default: break;
            }
            switch (vetorOut.IndexOf("  CHG0    ") + 1)
            {
                case 1: Properties.Settings.Default.NamesOutput[0] = "IN 01  "; break;
                case 2: Properties.Settings.Default.NamesOutput[1] = "IN 02  "; break;
                case 3: Properties.Settings.Default.NamesOutput[2] = "IN 03  "; break;
                case 4: Properties.Settings.Default.NamesOutput[3] = "IN 04  "; break;
                case 5: Properties.Settings.Default.NamesOutput[4] = "IN 05  "; break;
                case 6: Properties.Settings.Default.NamesOutput[5] = "IN 06  "; break;
                case 7: Properties.Settings.Default.NamesOutput[6] = "IN 07  "; break;
                case 8: Properties.Settings.Default.NamesOutput[7] = "IN 08  "; break;
                case 9: Properties.Settings.Default.NamesOutput[8] = "IN 09  "; break;
                case 10: Properties.Settings.Default.NamesOutput[9] = "IN 10  "; break;
                case 11: Properties.Settings.Default.NamesOutput[10] = "IN 11  "; break;
                case 12: Properties.Settings.Default.NamesOutput[11] = "IN 12  "; break;
                case 13: Properties.Settings.Default.NamesOutput[12] = "IN 13  "; break;
                case 14: Properties.Settings.Default.NamesOutput[13] = "IN 14  "; break;
                case 15: Properties.Settings.Default.NamesOutput[14] = "IN 15  "; break;
                case 16: Properties.Settings.Default.NamesOutput[15] = "IN 16  "; break;
                default: break;
            }
        }

        /// <summary>
        /// Retorna as entradas e saidas de acordo com a confiuração do sistema.
        /// </summary>
        /// <param name="TypeSystem">string systemType</param>
        /// <param name="inputDevice">string inputDevice</param>
        void RetornaDefault(string TypeSystem, string inputDevice)
        {
            string[] vetor = new string[16];
            #region populando os vetores com '  CHG0    '
            int InputSelecionado = Convert.ToInt32(inputDevice.Substring(3));
            vetor[00] = "IN 01  ";
            vetor[01] = "IN 02  ";
            vetor[02] = "IN 03  ";
            vetor[03] = "IN 04  ";
            vetor[04] = "IN 05  ";
            vetor[05] = "IN 06  ";
            vetor[06] = "IN 07  ";
            vetor[07] = "IN 08  ";
            vetor[08] = "IN 09  ";
            vetor[09] = "IN 10  ";
            vetor[10] = "IN 11  ";
            vetor[11] = "IN 12  ";
            vetor[12] = "IN 13  ";
            vetor[13] = "IN 14  ";
            vetor[14] = "IN 15  ";
            vetor[15] = "IN 16  ";
            vetor[InputSelecionado - 1] = "  CHG0    ";
            string[] vetorDefault = new string[16];
            vetorDefault[00] = "IN 01  ";
            vetorDefault[01] = "IN 02  ";
            vetorDefault[02] = "IN 03  ";
            vetorDefault[03] = "IN 04  ";
            vetorDefault[04] = "IN 05  ";
            vetorDefault[05] = "IN 06  ";
            vetorDefault[06] = "IN 07  ";
            vetorDefault[07] = "IN 08  ";
            vetorDefault[08] = "IN 09  ";
            vetorDefault[09] = "IN 10  ";
            vetorDefault[10] = "IN 11  ";
            vetorDefault[11] = "IN 12  ";
            vetorDefault[12] = "IN 13  ";
            vetorDefault[13] = "IN 14  ";
            vetorDefault[14] = "IN 15  ";
            vetorDefault[15] = "IN 16  ";
            string[] vetorOut = new string[16];
            vetorOut[00] = "OUT 01 ";
            vetorOut[01] = "OUT 02 ";
            vetorOut[02] = "OUT 03 ";
            vetorOut[03] = "OUT 04 ";
            vetorOut[04] = "OUT 05 ";
            vetorOut[05] = "OUT 06 ";
            vetorOut[06] = "OUT 07 ";
            vetorOut[07] = "OUT 08 ";
            vetorOut[08] = "OUT 09 ";
            vetorOut[09] = "OUT 10 ";
            vetorOut[10] = "OUT 11 ";
            vetorOut[11] = "OUT 12 ";
            vetorOut[12] = "OUT 13 ";
            vetorOut[13] = "OUT 14 ";
            vetorOut[14] = "OUT 15 ";
            vetorOut[15] = "OUT 16 ";
            vetorOut[InputSelecionado - 1] = "  CHG0    ";
            string[] vetorOutDefault = new string[16];
            vetorOutDefault[00] = "OUT 01 ";
            vetorOutDefault[01] = "OUT 02 ";
            vetorOutDefault[02] = "OUT 03 ";
            vetorOutDefault[03] = "OUT 04 ";
            vetorOutDefault[04] = "OUT 05 ";
            vetorOutDefault[05] = "OUT 06 ";
            vetorOutDefault[06] = "OUT 07 ";
            vetorOutDefault[07] = "OUT 08 ";
            vetorOutDefault[08] = "OUT 09 ";
            vetorOutDefault[09] = "OUT 10 ";
            vetorOutDefault[10] = "OUT 11 ";
            vetorOutDefault[11] = "OUT 12 ";
            vetorOutDefault[12] = "OUT 13 ";
            vetorOutDefault[13] = "OUT 14 ";
            vetorOutDefault[14] = "OUT 15 ";
            vetorOutDefault[15] = "OUT 16 ";
            #endregion
            switch (TypeSystem)
            {
                case "Change Over":
                    CmpTIN01.Text = vetor[00];
                    CmpTIN02.Text = vetor[01];
                    CmpTIN03.Text = vetor[02];
                    CmpTIN04.Text = vetor[03];
                    CmpTIN05.Text = vetor[04];
                    CmpTIN06.Text = vetor[05];
                    CmpTIN07.Text = vetor[06];
                    CmpTIN08.Text = vetor[07];
                    CmpTIN09.Text = vetor[08];
                    CmpTIN010.Text = vetor[09];
                    CmpTIN011.Text = vetor[10];
                    CmpTIN012.Text = vetor[11];
                    CmpTIN013.Text = vetor[12];
                    CmpTIN014.Text = vetor[13];
                    CmpTIN015.Text = vetor[14];
                    CmpTIN016.Text = vetor[15];
                    CmpRIN1.Text = vetor[0];
                    CmpRIN2.Text = vetor[01];
                    CmpRIN3.Text = vetor[02];
                    CmpRIN4.Text = vetor[03];
                    CmpRIN5.Text = vetor[04];
                    CmpRIN6.Text = vetor[05];
                    CmpRIN7.Text = vetor[06];
                    CmpRIN8.Text = vetor[07];
                    CmpRIN9.Text = vetor[08];
                    CmpRIN10.Text = vetor[09];
                    CmpRIN11.Text = vetor[10];
                    CmpRIN12.Text = vetor[11];
                    CmpRIN13.Text = vetor[12];
                    CmpRIN14.Text = vetor[13];
                    CmpRIN15.Text = vetor[14];
                    CmpRIN16.Text = vetor[15];
                    CmpOut1.Text = vetorOut[0];
                    CmpOut2.Text = vetorOut[01];
                    CmpOut3.Text = vetorOut[02];
                    CmpOut4.Text = vetorOut[03];
                    CmpOut5.Text = vetorOut[04];
                    CmpOut6.Text = vetorOut[05];
                    CmpOut7.Text = vetorOut[06];
                    CmpOut8.Text = vetorOut[07];
                    CmpOut9.Text = vetorOut[08];
                    CmpOut10.Text = vetorOut[09];
                    CmpOut11.Text = vetorOut[10];
                    CmpOut12.Text = vetorOut[11];
                    CmpOut13.Text = vetorOut[12];
                    CmpOut14.Text = vetorOut[13];
                    CmpOut15.Text = vetorOut[14];
                    CmpOut16.Text = vetorOut[15];
                    return;

                case "Matrix of Signals":
                    CmpTIN01.Text = vetorDefault[0];
                    CmpTIN02.Text = vetorDefault[01];
                    CmpTIN03.Text = vetorDefault[02];
                    CmpTIN04.Text = vetorDefault[03];
                    CmpTIN05.Text = vetorDefault[04];
                    CmpTIN06.Text = vetorDefault[05];
                    CmpTIN07.Text = vetorDefault[06];
                    CmpTIN08.Text = vetorDefault[07];
                    CmpTIN09.Text = vetorDefault[08];
                    CmpTIN010.Text = vetorDefault[09];
                    CmpTIN011.Text = vetorDefault[10];
                    CmpTIN012.Text = vetorDefault[11];
                    CmpTIN013.Text = vetorDefault[12];
                    CmpTIN014.Text = vetorDefault[13];
                    CmpTIN015.Text = vetorDefault[14];
                    CmpTIN016.Text = vetorDefault[15];
                    CmpRIN1.Text = vetorDefault[0];
                    CmpRIN2.Text = vetorDefault[01];
                    CmpRIN3.Text = vetorDefault[02];
                    CmpRIN4.Text = vetorDefault[03];
                    CmpRIN5.Text = vetorDefault[04];
                    CmpRIN6.Text = vetorDefault[05];
                    CmpRIN7.Text = vetorDefault[06];
                    CmpRIN8.Text = vetorDefault[07];
                    CmpRIN9.Text = vetorDefault[08];
                    CmpRIN10.Text = vetorDefault[09];
                    CmpRIN11.Text = vetorDefault[10];
                    CmpRIN12.Text = vetorDefault[11];
                    CmpRIN13.Text = vetorDefault[12];
                    CmpRIN14.Text = vetorDefault[13];
                    CmpRIN15.Text = vetorDefault[14];
                    CmpRIN16.Text = vetorDefault[15];
                    CmpOut1.Text = vetorOutDefault[0];
                    CmpOut2.Text = vetorOutDefault[01];
                    CmpOut3.Text = vetorOutDefault[02];
                    CmpOut4.Text = vetorOutDefault[03];
                    CmpOut5.Text = vetorOutDefault[04];
                    CmpOut6.Text = vetorOutDefault[05];
                    CmpOut7.Text = vetorOutDefault[06];
                    CmpOut8.Text = vetorOutDefault[07];
                    CmpOut9.Text = vetorOutDefault[08];
                    CmpOut10.Text = vetorOutDefault[09];
                    CmpOut11.Text = vetorOutDefault[10];
                    CmpOut12.Text = vetorOutDefault[11];
                    CmpOut13.Text = vetorOutDefault[12];
                    CmpOut14.Text = vetorOutDefault[13];
                    CmpOut15.Text = vetorOutDefault[14];
                    CmpOut16.Text = vetorOutDefault[15];
                    return;
            }
        }

        /// <summary>
        /// Retorna as entradas e saidas para 'WWWWWWW' para debug.
        /// </summary>
        void RetornaDebug()
        {
            string vetor = "WWWWWWW";
            CmpTIN01.Text = vetor;
            CmpTIN02.Text = vetor;
            CmpTIN03.Text = vetor;
            CmpTIN04.Text = vetor;
            CmpTIN05.Text = vetor;
            CmpTIN06.Text = vetor;
            CmpTIN07.Text = vetor;
            CmpTIN08.Text = vetor;
            CmpTIN09.Text = vetor;
            CmpTIN010.Text = vetor;
            CmpTIN011.Text = vetor;
            CmpTIN012.Text = vetor;
            CmpTIN013.Text = vetor;
            CmpTIN014.Text = vetor;
            CmpTIN015.Text = vetor;
            CmpTIN016.Text = vetor;
            //Replica nomes do primário para secundário e output
            ReplicaNomesINOUTS();
        }

        /// <summary>
        /// Retorna as entradas e saidas para names simulando o sistema para debug.
        /// </summary>
        void RetornaDebugTest()
        {
            CmpTIN01.Text = "SW1 PL";
            CmpTIN02.Text = "SW1 PS";
            CmpTIN03.Text = "SW2 PL";
            CmpTIN04.Text = "SW2 PS";
            CmpTIN05.Text = "SW3 PL";
            CmpTIN06.Text = "SW3 PS";
            CmpTIN07.Text = "SW4 PL";
            CmpTIN08.Text = "SW4 PS";
            CmpTIN09.Text = "K2P1 PL";
            CmpTIN010.Text = "K2P1 PS";
            CmpTIN011.Text = "K2P2 PL";
            CmpTIN012.Text = "K2P2 PS";
            CmpTIN013.Text = "VDR-1";
            CmpTIN014.Text = "VDR-2";
            CmpTIN015.Text = "MVWR-1";
            CmpTIN016.Text = "MVWR-2";
            //Replica nomes do primário para secundário e output
            ReplicaNomesINOUTS();
        }

        /// <summary>
        /// Replica nomes do primário para secundário e output.
        /// </summary>
        void ReplicaNomesINOUTS()
        {
            CmpRIN1.Text = CmpTIN01.Text;
            CmpRIN2.Text = CmpTIN02.Text;
            CmpRIN3.Text = CmpTIN03.Text;
            CmpRIN4.Text = CmpTIN04.Text;
            CmpRIN5.Text = CmpTIN05.Text;
            CmpRIN6.Text = CmpTIN06.Text;
            CmpRIN7.Text = CmpTIN07.Text;
            CmpRIN8.Text = CmpTIN08.Text;
            CmpRIN9.Text = CmpTIN09.Text;
            CmpRIN10.Text = CmpTIN010.Text;
            CmpRIN11.Text = CmpTIN011.Text;
            CmpRIN12.Text = CmpTIN012.Text;
            CmpRIN13.Text = CmpTIN013.Text;
            CmpRIN14.Text = CmpTIN014.Text;
            CmpRIN15.Text = CmpTIN015.Text;
            CmpRIN16.Text = CmpTIN016.Text;
            CmpOut1.Text = CmpTIN01.Text;
            CmpOut2.Text = CmpTIN02.Text;
            CmpOut3.Text = CmpTIN03.Text;
            CmpOut4.Text = CmpTIN04.Text;
            CmpOut5.Text = CmpTIN05.Text;
            CmpOut6.Text = CmpTIN06.Text;
            CmpOut7.Text = CmpTIN07.Text;
            CmpOut8.Text = CmpTIN08.Text;
            CmpOut9.Text = CmpTIN09.Text;
            CmpOut10.Text = CmpTIN010.Text;
            CmpOut11.Text = CmpTIN011.Text;
            CmpOut12.Text = CmpTIN012.Text;
            CmpOut13.Text = CmpTIN013.Text;
            CmpOut14.Text = CmpTIN014.Text;
            CmpOut15.Text = CmpTIN015.Text;
            CmpOut16.Text = CmpTIN016.Text;
        }

        /// <summary>
        /// Alimenta comboBox Slots com array 'frmC.ArraySlotsP'
        /// </summary>
        void AssignNames()
        { LblPosicao1.Text = ""; LblPosicao2.Text = ""; LblOutput.Text = ""; }

        //MENSAGENS
        /// <summary>
        /// Função auxiliar para imprimir mensagens Change Over.
        /// </summary>
        /// <param name="str"></param>
        void MensagemAdequadaChangeOver(string str)
        {
            switch (str)
            {
                case "VFF":
                    using (Form MsgBox = new MmsgBox("Secondary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputSecondary))) + Environment.NewLine + "Output: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayOutput))), "OK", 3, 0))
                    { _ = MsgBox.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                case "FVF":
                    using (Form MsgBox1 = new MmsgBox("Primary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputPrimary))) + Environment.NewLine + "Output: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayOutput))), "OK", 3, 0))
                    { _ = MsgBox1.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                case "VVF":
                    using (Form MsgBox2 = new MmsgBox("Output: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayOutput))), "OK", 3, 0))
                    { _ = MsgBox2.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                case "FFF":
                    using (Form MsgBox3 = new MmsgBox("Primary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputPrimary))) + Environment.NewLine + "Secondary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputSecondary))) + Environment.NewLine + "Output: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayOutput))), "OK", 3, 0))
                    { _ = MsgBox3.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                case "FFV":
                    using (Form MsgBox4 = new MmsgBox("Primary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputPrimary))) + Environment.NewLine + "Secondary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputSecondary))), "OK", 3, 0))
                    { _ = MsgBox4.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                case "FVV":
                    using (Form MsgBox5 = new MmsgBox("Primary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputPrimary))), "OK", 3, 0))
                    { _ = MsgBox5.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                case "VFV":
                    using (Form MsgBox6 = new MmsgBox("Secondary: " + MensagemErroSomemte1Chg0(Funcoes.DiscretagemInt(Somente1Chg0Vetor2ret(frmC.ArrayInputSecondary))), "OK", 3, 0))
                    { _ = MsgBox6.ShowDialog(); AtualizaSettingsnoSistem(); return; }
                default: break;
            }
        }

        /// <summary>
        /// Função auxiliar para as mensagens de verificação de somente 1 CHG0   .
        /// </summary>
        /// <param name="num">int representando tipo de erro</param>
        /// <returns>string com mensagem de erro</returns>
        public string MensagemErroSomemte1Chg0(int num)
        {
            if (num == 0) { return "No occurrences, you need only have one position titled 'CHG0   '! "; }
            if (num == 1) { return ""; }
            if (num == 2) { return "More than one occurrence, you must have only one position titled 'CHG0   '! "; }
            else { return "Failure! "; }
        }

        #endregion

        #region ClickCampText
        private void CmpTitIN01_Click(object sender, EventArgs e) { CmpTIN01.SelectAll(); }
        private void CmpTitIN02_Click(object sender, EventArgs e) { CmpTIN02.SelectAll(); }
        private void CmpTitIN03_Click(object sender, EventArgs e) { CmpTIN03.SelectAll(); }
        private void CmpTitIN04_Click(object sender, EventArgs e) { CmpTIN04.SelectAll(); }
        private void CmpTitIN05_Click(object sender, EventArgs e) { CmpTIN05.SelectAll(); }
        private void CmpTitIN06_Click(object sender, EventArgs e) { CmpTIN06.SelectAll(); }
        private void CmpTitIN07_Click(object sender, EventArgs e) { CmpTIN07.SelectAll(); }
        private void CmpTitIN08_Click(object sender, EventArgs e) { CmpTIN08.SelectAll(); }
        private void CmpIN09_Click(object sender, EventArgs e) { CmpTIN09.SelectAll(); }
        private void CmpIN010_Click(object sender, EventArgs e) { CmpTIN010.SelectAll(); }
        private void CmpIN011_Click(object sender, EventArgs e) { CmpTIN011.SelectAll(); }
        private void CmpIN012_Click(object sender, EventArgs e) { CmpTIN012.SelectAll(); }
        private void CmpIN013_Click(object sender, EventArgs e) { CmpTIN013.SelectAll(); }
        private void CmpIN014_Click(object sender, EventArgs e) { CmpTIN014.SelectAll(); }
        private void CmpIN015_Click(object sender, EventArgs e) { CmpTIN015.SelectAll(); }
        private void CmpIN016_Click(object sender, EventArgs e) { CmpTIN016.SelectAll(); }
        private void CmpOut1_Click(object sender, EventArgs e) { CmpOut1.SelectAll(); }
        private void CmpOut2_Click(object sender, EventArgs e) { CmpOut2.SelectAll(); }
        private void CmpOut3_Click(object sender, EventArgs e) { CmpOut3.SelectAll(); }
        private void CmpOut4_Click(object sender, EventArgs e) { CmpOut4.SelectAll(); }
        private void CmpOut5_Click(object sender, EventArgs e) { CmpOut5.SelectAll(); }
        private void CmpOut6_Click(object sender, EventArgs e) { CmpOut6.SelectAll(); }
        private void CmpOut7_Click(object sender, EventArgs e) { CmpOut7.SelectAll(); }
        private void CmpOut8_Click(object sender, EventArgs e) { CmpOut8.SelectAll(); }
        private void CmpOut9_Click(object sender, EventArgs e) { CmpOut9.SelectAll(); }
        private void CmpOut10_Click(object sender, EventArgs e) { CmpOut10.SelectAll(); }
        private void CmpOut11_Click(object sender, EventArgs e) { CmpOut11.SelectAll(); }
        private void CmpOut12_Click(object sender, EventArgs e) { CmpOut12.SelectAll(); }
        private void CmpOut13_Click(object sender, EventArgs e) { CmpOut13.SelectAll(); }
        private void CmpOut14_Click(object sender, EventArgs e) { CmpOut14.SelectAll(); }
        private void CmpOut15_Click(object sender, EventArgs e) { CmpOut15.SelectAll(); }
        private void CmpOut16_Click(object sender, EventArgs e) { CmpOut16.SelectAll(); }
        private void CmpRIN1_Click(object sender, EventArgs e) { CmpRIN1.SelectAll(); }
        private void CmpRIN2_Click(object sender, EventArgs e) { CmpRIN2.SelectAll(); }
        private void CmpRIN3_Click(object sender, EventArgs e) { CmpRIN3.SelectAll(); }
        private void CmpRIN4_Click(object sender, EventArgs e) { CmpRIN4.SelectAll(); }
        private void CmpRIN5_Click(object sender, EventArgs e) { CmpRIN5.SelectAll(); }
        private void CmpRIN6_Click(object sender, EventArgs e) { CmpRIN6.SelectAll(); }
        private void CmpRIN7_Click(object sender, EventArgs e) { CmpRIN7.SelectAll(); }
        private void CmpRIN8_Click(object sender, EventArgs e) { CmpRIN8.SelectAll(); }
        private void CmpRIN9_Click(object sender, EventArgs e) { CmpRIN9.SelectAll(); }
        private void CmpRIN10_Click(object sender, EventArgs e) { CmpRIN10.SelectAll(); }
        private void CmpRIN11_Click(object sender, EventArgs e) { CmpRIN11.SelectAll(); }
        private void CmpRIN12_Click(object sender, EventArgs e) { CmpRIN12.SelectAll(); }
        private void CmpRIN13_Click(object sender, EventArgs e) { CmpRIN13.SelectAll(); }
        private void CmpRIN14_Click(object sender, EventArgs e) { CmpRIN14.SelectAll(); }
        private void CmpRIN15_Click(object sender, EventArgs e) { CmpRIN15.SelectAll(); }
        private void CmpRIN16_Click(object sender, EventArgs e) { CmpRIN16.SelectAll(); }
        #endregion

        #region Flat buttons

        #region Hardware
        private void BtnSearch_MouseEnter(object sender, EventArgs e) { BtnSearch.ForeColor = SystemColors.WindowText; }
        private void BtnSearch_MouseLeave(object sender, EventArgs e) { BtnSearch.ForeColor = SystemColors.ButtonFace; }
        private void BtnOpen_MouseEnter(object sender, EventArgs e) { BtnOpen.ForeColor = SystemColors.WindowText; }
        private void BtnOpen_MouseLeave(object sender, EventArgs e) { BtnOpen.ForeColor = SystemColors.ButtonFace; }
        private void BtnWrite_MouseEnter(object sender, EventArgs e) { BtnWrite.ForeColor = SystemColors.WindowText; }
        private void BtnWrite_MouseLeave(object sender, EventArgs e) { BtnWrite.ForeColor = SystemColors.ButtonFace; }
        private void BtnClear_MouseEnter(object sender, EventArgs e) { BtnClear.ForeColor = SystemColors.WindowText; }
        private void BtnClear_MouseLeave(object sender, EventArgs e) { BtnClear.ForeColor = SystemColors.ButtonFace; }
        private void BtnSave_MouseEnter(object sender, EventArgs e) { BtnSave_system.ForeColor = SystemColors.WindowText; }
        private void BtnSave_MouseLeave(object sender, EventArgs e) { BtnSave_system.ForeColor = SystemColors.ButtonFace; }
        private void BtnRead_MouseEnter(object sender, EventArgs e) { BtnRead.ForeColor = SystemColors.WindowText; }
        private void BtnRead_MouseLeave(object sender, EventArgs e) { BtnRead.ForeColor = SystemColors.ButtonFace; }
        private void IpAddressControl1_MouseLeave(object sender, EventArgs e)
        {
            if (ValidaIp(ipAddressControl1.Text) == false)
            { ControleDevice.Enabled = false; picSealevel.Image = CHOV.Properties.Resources.Empty_device; }
            else { ControleDevice.Enabled = true; picSealevel.Image = CHOV.Properties.Resources._410E; }
        }
        #endregion

        #region System
        private void BtnCancelSystem_MouseEnter(object sender, EventArgs e) { BtnCancelSystem.ForeColor = SystemColors.WindowText; }
        private void BtnCancelSystem_MouseLeave(object sender, EventArgs e) { BtnCancelSystem.ForeColor = SystemColors.ButtonFace; }
        private void IpPrimaryControl_MouseLeave(object sender, EventArgs e) { if (ValidaIp(IpPrimaryControl.Text) == false) { IpPrimaryControl.Focus(); } }
        private void IpSecondaryControl_MouseLeave(object sender, EventArgs e) { if (ValidaIp(IpSecondaryControl.Text) == false) { IpSecondaryControl.Focus(); } }
        private void IpOutputControl_MouseLeave(object sender, EventArgs e) { if (ValidaIp(IpOutputControl.Text) == false) { IpOutputControl.Focus(); } }
        private void BtnSave_MouseEnter_1(object sender, EventArgs e) { BtnSave_system.ForeColor = SystemColors.WindowText; }
        private void BtnSave_MouseLeave_1(object sender, EventArgs e) { BtnSave_system.ForeColor = SystemColors.ButtonFace; }
        private void Browser_MouseLeave(object sender, EventArgs e) { Browser.ForeColor = SystemColors.ButtonFace; }
        private void Browser_MouseEnter(object sender, EventArgs e) { Browser.ForeColor = SystemColors.WindowText; }
        #endregion

        #region Inputs&Otputs
        private void BtnDefault_MouseEnter(object sender, EventArgs e) { btnDefault.ForeColor = SystemColors.WindowText; }
        private void BtnDefault_MouseLeave(object sender, EventArgs e) { btnDefault.ForeColor = SystemColors.ButtonFace; }
        private void BtnReply_MouseEnter(object sender, EventArgs e) { btnReply.ForeColor = SystemColors.WindowText; }
        private void BtnReply_MouseLeave(object sender, EventArgs e) { btnReply.ForeColor = SystemColors.ButtonFace; }
        private void BtCancel_MouseEnter(object sender, EventArgs e) { btnCancelIputOutput.ForeColor = SystemColors.WindowText; }
        private void BtCancel_MouseLeave(object sender, EventArgs e) { btnCancelIputOutput.ForeColor = SystemColors.ButtonFace; }
        private void BtAplicar_MouseEnter(object sender, EventArgs e) { BtnApply.ForeColor = SystemColors.WindowText; }
        private void BtAplicar_MouseLeave(object sender, EventArgs e) { BtnApply.ForeColor = SystemColors.ButtonFace; }
        #endregion

        #region Matrix
        private void BtnCancelmtX_MouseEnter(object sender, EventArgs e) { btnCancel.ForeColor = SystemColors.WindowText; }
        private void BtnCancelmtX_MouseLeave(object sender, EventArgs e) { btnCancel.ForeColor = SystemColors.ButtonFace; }
        private void BtnDeletemtX_MouseEnter(object sender, EventArgs e) { btnDel.ForeColor = SystemColors.WindowText; }
        private void BtnDeletemtX_MouseLeave(object sender, EventArgs e) { btnDel.ForeColor = SystemColors.ButtonFace; }
        private void BtnAddmtX_MouseEnter(object sender, EventArgs e) { btnAdd.ForeColor = SystemColors.WindowText; }
        private void BtnAddmtX_MouseLeave(object sender, EventArgs e) { btnAdd.ForeColor = SystemColors.ButtonFace; }
        private void BtnSavee_MouseEnter(object sender, EventArgs e) { btnSave_.ForeColor = SystemColors.WindowText; }
        private void BtnSavee_MouseLeave(object sender, EventArgs e) { btnSave_.ForeColor = SystemColors.ButtonFace; }
        private void BtnEditmtX_MouseEnter(object sender, EventArgs e) { btnEdit.ForeColor = SystemColors.WindowText; }
        private void BtnEditmtX_MouseLeave(object sender, EventArgs e) { btnEdit.ForeColor = SystemColors.ButtonFace; }
        #endregion

        #endregion

        #region IP
        /// <summary>
        /// Valida IP
        /// </summary>
        /// <param name="strIP">string com o endereço IP</param>
        /// <returns>Retorna TRUE se for um IP válido</returns>
        public static bool ValidaIp(String strIP)
        {
            char chrFullStop = '.';
            string[] arrOctets = strIP.Split(chrFullStop);
            string chk = arrOctets[3] + arrOctets[2] + arrOctets[1] + arrOctets[0];
            if ((arrOctets[0] == "0" || arrOctets[0] == "" || arrOctets[1] == "" || arrOctets[2] == "" || arrOctets[3] == "") && (chk != ""))
            {
                using (MmsgBox mmsgBox = new MmsgBox("Invalid IP!", "OK", 5, 0))
                { _ = mmsgBox.ShowDialog(); return false; }
            }
            else
            { if (chk == "") { return false; } return true; }
        }
        #endregion

        #region Teclas de atalho
        /// <summary>
        /// Monitora asteclas de atalho
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmConfiguracoes_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Shift && e.KeyCode == Keys.H) { TbcConfiguration.SelectTab(0); log.Debug("Atalho para Hardware"); }
            if (e.Shift && e.KeyCode == Keys.S) { TbcConfiguration.SelectTab(1); log.Debug("Atalho para System"); }
            if (e.Shift && e.KeyCode == Keys.I) { TbcConfiguration.SelectTab(2); log.Debug("Atalho para InOut"); }
            if (e.Shift && e.KeyCode == Keys.M) { TbcConfiguration.SelectTab(3); log.Debug("Atalho para Matrix"); }
            if (e.Shift && e.KeyCode == Keys.P) { log.Debug("Atalho para Painel"); Funcoes.ReturnPanel(); }
            //atalho para preencher names 'WWWWWWW' usada somente com sistema em 'Mtx'
            if (e.Control && e.KeyCode == Keys.W) { RetornaDebug(); }
            //atalho para preencher names com valores simulando valores de um sistema real, usada somente com sistema em 'Mtx'
            if (e.Control && e.KeyCode == Keys.Q) { RetornaDebugTest(); }
        }

        /// <summary>
        /// Função para Salvar um condição "Default" no vetor de combinações
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Informações_DoubleClick(object sender, EventArgs e)
        {
            string[] sasuke = new string[1]; sasuke[0] = "[ Primary  :     IN 11 ] 'IN 11  ' [ AND ] [ Secondary:     IN 12 ] 'IN 12  ' ->[ OUT 13 ] 'OUT 13 '";
            //Limpa Vetor do sistema
            Properties.Settings.Default.Combinations.Clear();
            //Insere vetor de pgm no vetor de sistema
            Properties.Settings.Default.Combinations.AddRange((sasuke));
            //Save
            Properties.Settings.Default.Save(); this.Close();
        }

        private void PnlSystemChg0_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            { PMenu.Show(this, new Point(e.X, e.Y)); }
        }

        // Eventos dos itens do menu
        private void Item1_clicked(object sender, EventArgs e)
        {
            log.Debug("Botão Import Congigurações acionado");
            //Preenche os campos com os dados importado e decriptografado
            if (WritevarSetting(SetConfig(ImportCripto())) == 1)
            {
                //Passa dados dos campos p settings
                WritevarSetting();
                SaveSettings();
                TabPageIndex();
                //verificando tipo de sistema e habilitando o q necessario.
                EnableDisable(Properties.Settings.Default.System);
                //Função verifica em qual posição está "CHG0   ".
                PosicaoChg0noVetor(Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput);
                //Função que insere o CHG0   .
                InsertChg0(Properties.Settings.Default.System, Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput, Properties.Settings.Default.InputDeviceChg0);
                Properties.Settings.Default.Save();
                AtualizaSettingsnoSistem();
                frmC.GetInfoSettings();
                frmC.AtivaPnl(Properties.Settings.Default.System);

                using (MmsgBox mmsgBox = new MmsgBox("Import Complete!", "OK", 1, 0))
                { _ = mmsgBox.ShowDialog(); }
            }
            else
            {
                using (MmsgBox mmsgBox = new MmsgBox("Import Canceled!", "OK", 1, 0))
                { _ = mmsgBox.ShowDialog(); }
            }
        }

        private void Item2_clicked(object sender, EventArgs e)
        {
            log.Debug("Botão Export Congigurações acionado");
            //Exporta configurações com criptografia
            ExportConfig(GetCfgCripto());
            //Exporta configurações sem criptografia
        }

        #endregion

    }
}
