using log4net;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace CHOV
{
    public partial class FrmPainel : Form
    {
        #region Declaration of variables
        //Variaveis Globais
        private readonly Color cor1 = ColorTranslator.FromHtml("#67809f");
        private readonly Color cor2 = SystemColors.ControlDarkDark;
        //private readonly Color cor3 = SystemColors.AppWorkspace;
        //public Color def = ColorTranslator.FromHtml("#00e640");
        public Color def = ColorTranslator.FromHtml("#5333ed");
        public Color chg0v = ColorTranslator.FromHtml("#db0a5b");
        //Arrays do pgm  [INOUTS]
        public string[] ArrayInputPrimary = new string[16];
        public string[] ArrayInputSecondary = new string[16];
        public string[] ArrayOutput = new string[16];
        //Array do pgm [MATRIX-SLOTS]
        public string[] Combinas = new string[17];
        public string indxCmbP1 = "";
        public string indxCmbP2 = "";
        public string indxCmbOut = "";
        //Array pgm usado para receber a opção de escolha dos dados dos logs
        public string[] ArrayDataLog = new string[8];
        //Variaveis setting do pgm
        //Dispositivo para change over
        public string DeviceChg0_pgm = Properties.Settings.Default.DeviceChg0;
        //Entrada do dispositivo usado como change over
        public string InputDeviceChg0_pgm = Properties.Settings.Default.InputDeviceChg0;
        //Tipo de sistema configurado
        public string System_pgm = Properties.Settings.Default.System;
        //IP do input primary
        public string IP_Primary_pgm = Properties.Settings.Default.IP_Primary;
        //IP do input secondary
        public string IP_Secondary_pgm = Properties.Settings.Default.IP_Secondary;
        //IP do output 
        public string IP_OutputIP_pgm = Properties.Settings.Default.IP_Output;
        //Dispositivo que está selecionado no sistema
        public string CurrentSelection_pgm = Properties.Settings.Default.CurrentSelection;
        //Variavel para habilitar os logs
        public bool EnableLog_pgm = Properties.Settings.Default.EnableLog;
        //Path para salvar os logs do sistema
        public string SystemLog_pgm = Properties.Settings.Default.SystemLog;
        //Tamanho maximo do arquivo de logs
        public int MaxSizeLog_pgm = Properties.Settings.Default.MaxSizeLog;
        //Mostrar combinações salvas no log de operação
        public bool EnableCombinations_pgm;
        //Variavel para sisema do relogio
        public string Relogio_pgm = " ";
        //Variavel para sisema do relogio
        public string Data_pgm = " ";
        //Variavel para alternar o LedHeart do sistema;
        int n = 0;
        //Variavel que recebe o caminho para salvar os logs
        private string PathLog = Properties.Settings.Default.SystemLog + @"\LOGs.log";
        //var para o histórico
        public List<string> ArrayHistoric = new List<string>();
        //Variavel para expansão dos forms
        int exp = 0;
        public int exp1 = 0;
        //vetor transitorio sistema.
        bool[] vt = new bool[16];
        //vetor transitorio sistema.
        bool[] CtT = new bool[16];
        //vetor transitorio sistema.
        bool[] CtR = new bool[16];
        string primary1a8 = "0";
        string primary9a16 = "0";
        string secondary1a8 = "0";
        string secondary9a16 = "0";
        //variavel para logs
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public string Primary1a8 { get => primary1a8; set => primary1a8 = value; }
        public string Primary9a16 { get => primary9a16; set => primary9a16 = value; }
        public string Secondary1a8 { get => secondary1a8; set => secondary1a8 = value; }
        public string Secondary9a16 { get => secondary9a16; set => secondary9a16 = value; }
        #endregion

        public FrmPainel()
        {
            InitializeComponent();
            //Parametros iniciais
            PainelStart();
            //Insere o Ch0v initial
            Funcoes.InitialInsertChg0(Properties.Settings.Default.System, Properties.Settings.Default.NamesInputPrimary, Properties.Settings.Default.NamesInputSecondary, Properties.Settings.Default.NamesOutput, Properties.Settings.Default.InputDeviceChg0);
            Properties.Settings.Default.Save();
            //Lendo vetores de nomes
            RecebeNomeSettings();
            //Atribui os nomes para os labels
            AtribuindoNomesPainel();
            GetInfoSettings();
            //Zera todos In/Out
            ZeraAllInOut();
            CHOV.Logger.ConfigureFileAppender(PathLog, MaxSizeLog_pgm, ArrayDataLog, EnableLog_pgm);
        }

        #region Settings
        /// <summary>
        /// Configurações iniciais do projeto.
        /// </summary>
        public void PainelStart()
        { AtivaPnl(Properties.Settings.Default.System); status.Text = CurrentSelection_pgm; }

        /// <summary>
        /// Vetores pgm recebem os nomes dos vetores do settings.
        /// </summary>
        public void RecebeNomeSettings()
        {   //Lendo vetores de nomes
            Properties.Settings.Default.NamesInputPrimary.CopyTo(ArrayInputPrimary, 0);
            Properties.Settings.Default.NamesInputSecondary.CopyTo(ArrayInputSecondary, 0);
            Properties.Settings.Default.NamesOutput.CopyTo(ArrayOutput, 0);
            Properties.Settings.Default.Combinations.CopyTo(Combinas, 0);
            Properties.Settings.Default.DatawrittenLog.CopyTo(ArrayDataLog, 0);
            //Condição parq quando o sistema executa pela primeira vez
            if (InputDeviceChg0_pgm.Length > 5)
            { Properties.Settings.Default.InputDeviceChg0 = InputDeviceChg0_pgm.Substring(0, 5); Properties.Settings.Default.Save(); InputDeviceChg0_pgm = Properties.Settings.Default.InputDeviceChg0; }
        }

        /// <summary>
        /// Atribui os nomes do vetor pgm para o painel principal.
        /// </summary>
        public void AtribuindoNomesPainel()
        {
            LblT_in1.Text = ArrayInputPrimary[0];
            LblT_in2.Text = ArrayInputPrimary[1];
            LblT_in3.Text = ArrayInputPrimary[2];
            LblT_in4.Text = ArrayInputPrimary[3];
            LblT_in5.Text = ArrayInputPrimary[4];
            LblT_in6.Text = ArrayInputPrimary[5];
            LblT_in7.Text = ArrayInputPrimary[6];
            LblT_in8.Text = ArrayInputPrimary[7];
            LblT_in9.Text = ArrayInputPrimary[8];
            LblT_in10.Text = ArrayInputPrimary[9];
            LblT_in11.Text = ArrayInputPrimary[10];
            LblT_in12.Text = ArrayInputPrimary[11];
            LblT_in13.Text = ArrayInputPrimary[12];
            LblT_in14.Text = ArrayInputPrimary[13];
            LblT_in15.Text = ArrayInputPrimary[14];
            LblT_in16.Text = ArrayInputPrimary[15];
            LblR_in1.Text = ArrayInputSecondary[0];
            LblR_in2.Text = ArrayInputSecondary[1];
            LblR_in3.Text = ArrayInputSecondary[2];
            LblR_in4.Text = ArrayInputSecondary[3];
            LblR_in5.Text = ArrayInputSecondary[4];
            LblR_in6.Text = ArrayInputSecondary[5];
            LblR_in7.Text = ArrayInputSecondary[6];
            LblR_in8.Text = ArrayInputSecondary[7];
            LblR_in9.Text = ArrayInputSecondary[8];
            LblR_in10.Text = ArrayInputSecondary[9];
            LblR_in11.Text = ArrayInputSecondary[10];
            LblR_in12.Text = ArrayInputSecondary[11];
            LblR_in13.Text = ArrayInputSecondary[12];
            LblR_in14.Text = ArrayInputSecondary[13];
            LblR_in15.Text = ArrayInputSecondary[14];
            LblR_in16.Text = ArrayInputSecondary[15];
            Lbl_out1.Text = ArrayOutput[0];
            Lbl_out2.Text = ArrayOutput[1];
            Lbl_out3.Text = ArrayOutput[2];
            Lbl_out4.Text = ArrayOutput[3];
            Lbl_out5.Text = ArrayOutput[4];
            Lbl_out6.Text = ArrayOutput[5];
            Lbl_out7.Text = ArrayOutput[6];
            Lbl_out8.Text = ArrayOutput[7];
            Lbl_out9.Text = ArrayOutput[8];
            Lbl_out10.Text = ArrayOutput[9];
            Lbl_out11.Text = ArrayOutput[10];
            Lbl_out12.Text = ArrayOutput[11];
            Lbl_out13.Text = ArrayOutput[12];
            Lbl_out14.Text = ArrayOutput[13];
            Lbl_out15.Text = ArrayOutput[14];
            Lbl_out16.Text = ArrayOutput[15];
        }

        /// <summary>
        /// Pega as informções atuais salvas no settings.
        /// </summary>
        public void GetInfoSettings()
        {
            if (Properties.Settings.Default.System == "Change Over")
            { LblRodape.Text = "System: " + Properties.Settings.Default.System + ";    " + " " + "Current Selection: " + Properties.Settings.Default.CurrentSelection + ";    " + " " + "Trigger device: " + Properties.Settings.Default.DeviceChg0 + ";    " + " Input trigger: " + Properties.Settings.Default.InputDeviceChg0 + ";    " + " " + "IP Primary: " + Properties.Settings.Default.IP_Primary + ";    " + " " + "IP Secondary: " + Properties.Settings.Default.IP_Secondary + ";    " + " " + "IP Output: " + Properties.Settings.Default.IP_Output; }
            else
            { LblRodape.Text = "System: " + Properties.Settings.Default.System + ";    " + "IP Primary: " + Properties.Settings.Default.IP_Primary + ";    " + " " + "IP Secondary: " + Properties.Settings.Default.IP_Secondary + ";    " + " " + "IP Output: " + Properties.Settings.Default.IP_Output; }
        }

        /// <summary>
        /// Top most
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPainel_Click(object sender, EventArgs e)
        { this.TopMost = true; }

        /// <summary>
        /// Ativa painel de acordo com o tipo de sistema configurado
        /// </summary>
        /// <param name="system">string representando o tipo de sistema configurado no sistema</param>
        public void AtivaPnl(string system)
        {
            if (system == "Change Over") { AtivaPnlChg0(); }
            else { AtivaPnlMtx(); }
        }

        /// <summary>
        /// Ativa painel Matrix
        /// </summary>
        public void AtivaPnlMtx()
        {
            PnlTitular_1a16.BorderStyle = BorderStyle.Fixed3D; PnlTitular_1a16.BackColor = cor1; PnlReserva_1a16.BorderStyle = BorderStyle.Fixed3D;
            PnlReserva_1a16.BackColor = cor1; Pnl_out1a16.BorderStyle = BorderStyle.Fixed3D;
            Pnl_out1a16.BackColor = cor1;
            ZeraAllInT(); ZeraAllInR(); ZeraAllInOut();
            status.Visible = false; LblChangeover.Visible = false; PicChang0ver.Visible = false;
            CorChg0v(VetorColor((Posicao(Properties.Settings.Default.NamesInputPrimary))));
        }

        /// <summary>
        /// Ativa painel Change Over
        /// </summary>
        public void AtivaPnlChg0()
        {   //decisão a ser tomada: de acordo cmom settings
            if (CurrentSelection_pgm == "Primary") { TitularAtivado(); }
            if (CurrentSelection_pgm == "Secondary") { ReservaAtivado(); };
            Pnl_out1a16.BorderStyle = BorderStyle.Fixed3D; Pnl_out1a16.BackColor = cor1;
            ZeraAllInT(); ZeraAllInR(); ZeraAllInOut();
            status.Visible = true; LblChangeover.Visible = true; PicChang0ver.Visible = true;
            CorChg0v(VetorColor((Posicao(Properties.Settings.Default.NamesInputPrimary))));
        }

        /// <summary>
        /// Ativa a indicação de Titular ativado.
        /// </summary>
        public void TitularAtivado()
        {   //Habilita/desabilita os paineis acionados
            PnlTitular_1a16.BorderStyle = BorderStyle.Fixed3D; PnlTitular_1a16.BackColor = cor1;
            PnlReserva_1a16.BackColor = cor2; PnlReserva_1a16.BorderStyle = BorderStyle.None;
            //Altera a variavel de seleção das entradas
            CurrentSelection_pgm = "Primary"; status.Text = CurrentSelection_pgm;
            ZeraInR1a8(); ZeraInR9a16(); ZeraOut1a16();
        }

        /// <summary>
        /// Ativa a indicação de Reserva ativado.
        /// </summary>
        public void ReservaAtivado()
        {   //Habilita/desabilita os paineis acionados
            PnlReserva_1a16.BorderStyle = BorderStyle.Fixed3D; PnlReserva_1a16.BackColor = cor1;
            PnlTitular_1a16.BorderStyle = BorderStyle.None; PnlTitular_1a16.BackColor = cor2;
            //Altera a variavel de seleção das entradas
            CurrentSelection_pgm = "Secondary"; status.Text = CurrentSelection_pgm;
            ZeraInT1a8(); ZeraInT9a16(); ZeraOut1a16();
        }

        /// <summary>
        /// Função que recebe info atualizadas do settings.
        /// </summary>
        public void RefreshSettings()
        {
            DeviceChg0_pgm = Properties.Settings.Default.DeviceChg0;
            InputDeviceChg0_pgm = Properties.Settings.Default.InputDeviceChg0;
            System_pgm = Properties.Settings.Default.System;
            IP_Primary_pgm = Properties.Settings.Default.IP_Primary;
            IP_Secondary_pgm = Properties.Settings.Default.IP_Secondary;
            IP_OutputIP_pgm = Properties.Settings.Default.IP_Output;
            EnableLog_pgm = Properties.Settings.Default.EnableLog;
            SystemLog_pgm = Properties.Settings.Default.SystemLog;
            MaxSizeLog_pgm = Convert.ToInt32(Properties.Settings.Default.MaxSizeLog);
            EnableCombinations_pgm = Properties.Settings.Default.EnableCombinationsLog;
            log.Logger.Repository.Shutdown();
            PathLog = Properties.Settings.Default.SystemLog + @"\LOGs.log";
            CHOV.Logger.ConfigureFileAppender(PathLog, MaxSizeLog_pgm, ArrayDataLog, EnableLog_pgm);
        }

        /// <summary>
        /// Função para realizar o Chaveamento direto com pulso da entrada configurada. Só verifica se o chg0 foi ativado
        /// </summary>
        /// <param name="trigger">string reprensentando qual entrada está configurada como trigger do sistema</param>
        /// <param name="position">string representando qual posição está ativada</param>
        /// <param name="systemType">string representando o tipo de sitema está configurado</param>
        /// <returns>Retorna True se o chg0 foi ativado</returns>
        public bool Chg0ver(string trigger, int position, string systemType)
        {
            bool chg = false;
            switch (systemType)
            {
                case "Change Over":
                    if (position.ToString() == trigger.Substring(3)) { chg = true; }
                    else { chg = false; }
                    return chg;
                case "Matrix of Signals": return chg;
            }
            return chg;
        }

        /// <summary>
        /// Função para realizar o Chaveamento direto com pulso da entrada configurada
        /// </summary>
        /// <param name="trigger">string reprensentando qual entrada está configurada como trigger do sistema</param>
        /// <param name="position">string representando qual posição está ativada</param>
        /// <param name="systemType">string representando o tipo de sitema está configurado</param>
        /// <returns>Retorna True se o chg0 foi ativado</returns>
        public bool Chg0verON(string trigger, int position, string systemType)
        {
            bool chg = false;
            string posicao = position.ToString();
            switch (systemType)
            {
                case "Change Over":
                    if (posicao.Length == 1) { posicao = "0" + posicao; }
                    if (posicao == trigger.Substring(3, 2))
                    {
                        switch (CurrentSelection_pgm)
                        {
                            case "Primary":
                                //Registro Logs
                                log.Info("Primary:" + Funcoes.InOnlyOnLog(Funcoes.FormatVetorStatus(vt)));
                                ArrayHistoric.Add(DateTime.Now.ToString() + " - #Chg0!" + " - Secondary is selected now!" + Environment.NewLine);
                                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                                PicChang0ver.Image = CHOV.Properties.Resources.Active_Secondary; CurrentSelection_pgm = "Secondary";
                                ReservaAtivado();
                                Properties.Settings.Default.CurrentSelection = CurrentSelection_pgm; Properties.Settings.Default.Save();
                                GetInfoSettings();
                                log.Warn(DateTime.Now.ToString() + " - CHANGE OVER - Secondary is selected now!");
                                break;

                            case "Secondary":
                                //Registro Logs
                                log.Info("Secondary:" + Funcoes.InOnlyOnLog(Funcoes.FormatVetorStatus(vt)));
                                ArrayHistoric.Add(DateTime.Now.ToString() + " - #Chg0!" + " - Primary is selected now!" + Environment.NewLine);
                                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                                PicChang0ver.Image = CHOV.Properties.Resources.Active_Primary; CurrentSelection_pgm = "Primary";
                                TitularAtivado();
                                Properties.Settings.Default.CurrentSelection = CurrentSelection_pgm; Properties.Settings.Default.Save();
                                //Atualiza as informções atuais salvas no settings.
                                GetInfoSettings();
                                log.Warn(DateTime.Now.ToString() + " - CHANGE OVER - Primary is selected now!");
                                break;

                            default: break;
                        }
                        chg = true;
                    }
                    else { chg = false; }
                    return chg;
                case "Matrix of Signals": return chg;
            }
            return chg;
        }

        /// <summary>
        /// Realiza a alteração de Change Over pelo botão de manutenção.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PicChang0ver_Click(object sender, EventArgs e)
        {
            if (CurrentSelection_pgm == "Primary")
            {
                PicChang0ver.Image = CHOV.Properties.Resources.Active_Secondary; CurrentSelection_pgm = "Secondary";
                ReservaAtivado();
                Properties.Settings.Default.CurrentSelection = CurrentSelection_pgm; Properties.Settings.Default.Save();
                //Atualiza as informções atuais salvas no settings.
                GetInfoSettings();
                ArrayHistoric.Add(DateTime.Now.ToString() + " - #Chg0!" + " - Secondary is selected now!" + Environment.NewLine);
                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                log.Warn(DateTime.Now.ToString() + "CHANGE OVER - Secondary is selected now!");
            }
            else
            {
                PicChang0ver.Image = CHOV.Properties.Resources.Active_Primary; CurrentSelection_pgm = "Primary";
                TitularAtivado();
                Properties.Settings.Default.CurrentSelection = CurrentSelection_pgm; Properties.Settings.Default.Save();
                //Atualiza as informções atuais salvas no settings.
                GetInfoSettings();
                ArrayHistoric.Add(DateTime.Now.ToString() + " - #Chg0!" + " - Primary is selected now!" + Environment.NewLine);
                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                log.Warn(DateTime.Now.ToString() + "CHANGE OVER - Primary is selected now!");
            }
        }

        #endregion

        #region Functions for ON/OFF Leds
        /// <summary>
        /// Retorna a posição do Chg0v, caso a stringCollection possua "CHG0"
        /// </summary>
        /// <param name="vt">StringCollection para verificar se possui nessa coleção "CHG0"</param>
        /// <returns>Int com a posição do "CHG0"</returns>
        public int Posicao(System.Collections.Specialized.StringCollection vt)
        {
            if (vt.Contains("CHG0") == true) { return vt.IndexOf("CHG0"); }
            else { return 99; }
        }

        //Botão write test
        private void BtnWriteTest_Click(object sender, EventArgs e)
        {
            log.Debug("Botão Write Mtx acionado");
            //frmC.Saida1a16(Funcoes.Slotnot(frmC.ArraySlotsP, frmC.ArraySlotsOp, frmC.ArraySlotsS, frmC.ArraySlotsO, cBp1a8.Text, cBp9a16.Text, cBs1a8.Text, cBs9a16.Text));
            Saida1a16(Funcoes.ExeCombinacoes(Properties.Settings.Default.Combinations, cBp1a8.Text, cBp9a16.Text, cBs1a8.Text, cBs9a16.Text));
        }

        /// <summary>
        /// Função Para alterar LedHeart
        /// </summary>
        public void LedSystem()
        {
            if (n > 3600) { n = 0; }
            if ((n % 2) == 0) { LedHeart.On = true; }
            else { LedHeart.On = false; }
            n++;
        }

        /// <summary>
        /// Retorna um vetor de cores substituindo a posição de entrada
        /// </summary>
        /// <param name="entrada">vetor de cores no qual será alterado a cor da posição de entrada</param>
        /// <returns>vetor com a cor do Chg0 alterada</returns>
        public Color[] VetorColor(int entrada)
        {
            Color[] saida = new Color[16];
            for (int i = 0; i <= 15; i++)
            { saida[i] = def; }
            if (entrada == 99) { return saida; }
            else { saida[entrada] = chg0v; }
            return saida;
        }

        /// <summary>
        /// Funçao para alterar a cor do Chg0v
        /// </summary>
        /// <param name="inputs">>vetor de cores no qual será alterado a cor da posição de entrada</param>
        public void CorChg0v(params Color[] inputs)
        {
            LedT_in1.Color = inputs[0];
            LedT_in2.Color = inputs[1];
            LedT_in3.Color = inputs[2];
            LedT_in4.Color = inputs[3];
            LedT_in5.Color = inputs[4];
            LedT_in6.Color = inputs[5];
            LedT_in7.Color = inputs[6];
            LedT_in8.Color = inputs[7];
            LedT_in9.Color = inputs[8];
            LedT_in10.Color = inputs[9];
            LedT_in11.Color = inputs[10];
            LedT_in12.Color = inputs[11];
            LedT_in13.Color = inputs[12];
            LedT_in14.Color = inputs[13];
            LedT_in15.Color = inputs[14];
            LedT_in16.Color = inputs[15];
            LedR_in1.Color = inputs[0];
            LedR_in2.Color = inputs[1];
            LedR_in3.Color = inputs[2];
            LedR_in4.Color = inputs[3];
            LedR_in5.Color = inputs[4];
            LedR_in6.Color = inputs[5];
            LedR_in7.Color = inputs[6];
            LedR_in8.Color = inputs[7];
            LedR_in9.Color = inputs[8];
            LedR_in10.Color = inputs[9];
            LedR_in11.Color = inputs[10];
            LedR_in12.Color = inputs[11];
            LedR_in13.Color = inputs[12];
            LedR_in14.Color = inputs[13];
            LedR_in15.Color = inputs[14];
            LedR_in16.Color = inputs[15];
            Led_out1.Color = inputs[0];
            Led_out2.Color = inputs[1];
            Led_out3.Color = inputs[2];
            Led_out4.Color = inputs[3];
            Led_out5.Color = inputs[4];
            Led_out6.Color = inputs[5];
            Led_out7.Color = inputs[6];
            Led_out8.Color = inputs[7];
            Led_out9.Color = inputs[8];
            Led_out10.Color = inputs[9];
            Led_out11.Color = inputs[10];
            Led_out12.Color = inputs[11];
            Led_out13.Color = inputs[12];
            Led_out14.Color = inputs[13];
            Led_out15.Color = inputs[14];
            Led_out16.Color = inputs[15];
        }

        /// <summary>
        /// Zera entradas titulares 1 a 8.
        /// </summary>
        public void ZeraInT1a8()
        {
            LedT_in1.On = false;
            LedT_in2.On = false;
            LedT_in3.On = false;
            LedT_in4.On = false;
            LedT_in5.On = false;
            LedT_in6.On = false;
            LedT_in7.On = false;
            LedT_in8.On = false;
        }

        /// <summary>
        /// Zera entradas titulares 9 a 16.
        /// </summary>
        public void ZeraInT9a16()
        {
            LedT_in9.On = false;
            LedT_in10.On = false;
            LedT_in11.On = false;
            LedT_in12.On = false;
            LedT_in13.On = false;
            LedT_in14.On = false;
            LedT_in15.On = false;
            LedT_in16.On = false;
        }

        /// <summary>
        /// Zera entradas titulares 1 a 16.
        /// </summary>
        public void ZeraAllInT()
        { ZeraInT1a8(); ZeraInT9a16(); }

        /// <summary>
        /// Zera entradas reservas 1 a 8.
        /// </summary>
        /// 
        public void ZeraInR1a8()
        {
            LedR_in1.On = false;
            LedR_in2.On = false;
            LedR_in3.On = false;
            LedR_in4.On = false;
            LedR_in5.On = false;
            LedR_in6.On = false;
            LedR_in7.On = false;
            LedR_in8.On = false;
        }

        /// <summary>
        /// Zera entradas reservas 9 a 16.
        /// </summary>
        public void ZeraInR9a16()
        {
            LedR_in9.On = false;
            LedR_in10.On = false;
            LedR_in11.On = false;
            LedR_in12.On = false;
            LedR_in13.On = false;
            LedR_in14.On = false;
            LedR_in15.On = false;
            LedR_in16.On = false;
        }

        /// <summary>
        /// Zera entradas resercas 1 a 16.
        /// </summary>
        public void ZeraAllInR()
        { ZeraInR1a8(); ZeraInR9a16(); }

        /// <summary>
        /// Zera saidas 1 a 16.
        /// </summary>
        public void ZeraOut1a16()
        {
            Led_out1.On = false;
            Led_out2.On = false;
            Led_out3.On = false;
            Led_out4.On = false;
            Led_out5.On = false;
            Led_out6.On = false;
            Led_out7.On = false;
            Led_out8.On = false;
            Led_out9.On = false;
            Led_out10.On = false;
            Led_out11.On = false;
            Led_out12.On = false;
            Led_out13.On = false;
            Led_out14.On = false;
            Led_out15.On = false;
            Led_out16.On = false;
        }

        /// <summary>
        /// Zera todas entradas e saidas.
        /// </summary>
        public void ZeraAllInOut()
        { ZeraAllInT(); ZeraAllInR(); ZeraOut1a16(); }

        /// <summary>
        /// Ativa as entradas titulares 1 a 8.
        /// </summary>
        public void AtivaInT1a8()
        {
            LedT_in1.On = true;
            LedT_in2.On = true;
            LedT_in3.On = true;
            LedT_in4.On = true;
            LedT_in5.On = true;
            LedT_in6.On = true;
            LedT_in7.On = true;
            LedT_in8.On = true;
        }

        /// <summary>
        /// Ativa as entradas titulares 9 a 16.
        /// </summary>
        public void AtivaInT9a16()
        {
            LedT_in9.On = true;
            LedT_in10.On = true;
            LedT_in11.On = true;
            LedT_in12.On = true;
            LedT_in13.On = true;
            LedT_in14.On = true;
            LedT_in15.On = true;
            LedT_in16.On = true;
        }

        /// <summary>
        /// Ativa todas as entradas titulares.
        /// </summary>
        public void AtivaAllInT()
        { AtivaInT1a8(); AtivaInT9a16(); }

        /// <summary>
        /// Ativa as entradas reservas 1 a 8.
        /// </summary>
        public void AtivaInR1a8()
        {
            LedR_in1.On = true;
            LedR_in2.On = true;
            LedR_in3.On = true;
            LedR_in4.On = true;
            LedR_in5.On = true;
            LedR_in6.On = true;
            LedR_in7.On = true;
            LedR_in8.On = true;
        }

        /// <summary>
        /// Ativa as entradas reservas 9 a 16.
        /// </summary>
        public void AtivaInR9a16()
        {
            LedR_in9.On = true;
            LedR_in10.On = true;
            LedR_in11.On = true;
            LedR_in12.On = true;
            LedR_in13.On = true;
            LedR_in14.On = true;
            LedR_in15.On = true;
            LedR_in16.On = true;
        }

        /// <summary>
        /// Ativa todas as entradas reservas.
        /// </summary>
        public void AtivaAllInR()
        { AtivaInR1a8(); AtivaInR9a16(); }

        /// <summary>
        /// Ativas todas saida e entradas.
        /// </summary>
        public void AtivaAllInOut()
        { AtivaAllInT(); AtivaAllInR(); AtivaAllOut(); }

        /// <summary>
        /// Ativa saidas 1 a 16.
        /// </summary>
        public void AtivaAllOut()
        {
            Led_out1.On = true;
            Led_out2.On = true;
            Led_out3.On = true;
            Led_out4.On = true;
            Led_out5.On = true;
            Led_out6.On = true;
            Led_out7.On = true;
            Led_out8.On = true;
            Led_out9.On = true;
            Led_out10.On = true;
            Led_out11.On = true;
            Led_out12.On = true;
            Led_out13.On = true;
            Led_out14.On = true;
            Led_out15.On = true;
            Led_out16.On = true;
        }
        #endregion

        #region Functions for indication inputs and outpus

        /// <summary>
        /// Indicação entradas titulares 1 a 16.
        /// </summary>
        /// <param name="inputs">vetor booleano de entradas</param>
        public void EntradasT1a16(params bool[] inputs)
        {
            LedT_in1.On = inputs[0];
            LedT_in2.On = inputs[1];
            LedT_in3.On = inputs[2];
            LedT_in4.On = inputs[3];
            LedT_in5.On = inputs[4];
            LedT_in6.On = inputs[5];
            LedT_in7.On = inputs[6];
            LedT_in8.On = inputs[7];
            LedT_in9.On = inputs[8];
            LedT_in10.On = inputs[9];
            LedT_in11.On = inputs[10];
            LedT_in12.On = inputs[11];
            LedT_in13.On = inputs[12];
            LedT_in14.On = inputs[13];
            LedT_in15.On = inputs[14];
            LedT_in16.On = inputs[15];
        }

        /// <summary>
        /// Indicação entradas reservas 1 a 16.
        /// </summary>
        /// <param name="inputs">vetor booleano de entradas</param>
        public void EntradasR1a16(params bool[] inputs)
        {
            LedR_in1.On = inputs[0];
            LedR_in2.On = inputs[1];
            LedR_in3.On = inputs[2];
            LedR_in4.On = inputs[3];
            LedR_in5.On = inputs[4];
            LedR_in6.On = inputs[5];
            LedR_in7.On = inputs[6];
            LedR_in8.On = inputs[7];
            LedR_in9.On = inputs[8];
            LedR_in10.On = inputs[9];
            LedR_in11.On = inputs[10];
            LedR_in12.On = inputs[11];
            LedR_in13.On = inputs[12];
            LedR_in14.On = inputs[13];
            LedR_in15.On = inputs[14];
            LedR_in16.On = inputs[15];
        }

        /// <summary>
        /// Indicação saidas 1 a 16.
        /// </summary>
        /// <param name="inputs">vetor booleano de saidas</param>
        public void Saida1a16(params bool[] inputs)
        {
            Led_out1.On = inputs[0];
            Led_out2.On = inputs[1];
            Led_out3.On = inputs[2];
            Led_out4.On = inputs[3];
            Led_out5.On = inputs[4];
            Led_out6.On = inputs[5];
            Led_out7.On = inputs[6];
            Led_out8.On = inputs[7];
            Led_out9.On = inputs[8];
            Led_out10.On = inputs[9];
            Led_out11.On = inputs[10];
            Led_out12.On = inputs[11];
            Led_out13.On = inputs[12];
            Led_out14.On = inputs[13];
            Led_out15.On = inputs[14];
            Led_out16.On = inputs[15];
        }

        /// <summary>
        /// Verifica num vetor bool qual posição recebeu o disparo.
        /// </summary>
        /// <param name="vetorPrimary">vetor bool com a leitura do primary</param>
        /// <param name="vetorSecondary">vetor bool com a leitura do secondary</param>
        /// <param name="device">string represetntando o device configurado</param>
        /// <returns>Int com a posição do pulso</returns>
        public int PositionPulse(bool[] vetorPrimary, bool[] vetorSecondary, string device)
        {
            if (device == "Primary") { return 1 + Array.IndexOf(vetorPrimary, true); }
            if (device == "Secondary") { return 1 + Array.IndexOf(vetorSecondary, true); }
            return 0;
        }

        /// <summary>
        /// Controle manual de testes
        /// </summary>
        /// <returns>vetor bool com a leitura do controle manual Titular</returns>
        public bool[] ControleManualTitular()
        {
            bool[] vetorT = new bool[16];
            vetorT[0] = checkBox1T.Checked;
            vetorT[1] = checkBox2T.Checked;
            vetorT[2] = checkBox3T.Checked;
            vetorT[3] = checkBox4T.Checked;
            vetorT[4] = checkBox5T.Checked;
            vetorT[5] = checkBox6T.Checked;
            vetorT[6] = checkBox7T.Checked;
            vetorT[7] = checkBox8T.Checked;
            vetorT[8] = checkBox9T.Checked;
            vetorT[9] = checkBox10T.Checked;
            vetorT[10] = checkBox11T.Checked;
            vetorT[11] = checkBox12T.Checked;
            vetorT[12] = checkBox13T.Checked;
            vetorT[13] = checkBox14T.Checked;
            vetorT[14] = checkBox15T.Checked;
            vetorT[15] = checkBox16T.Checked;
            return vetorT;
        }

        /// <summary>
        /// Controle manual de testes
        /// </summary>
        /// <returns>vetor bool com a leitura do controle manual Reserva</returns>
        public bool[] ControleManualReserva()
        {
            bool[] vetorR = new bool[16];
            vetorR[0] = checkBox1R.Checked;
            vetorR[1] = checkBox2R.Checked;
            vetorR[2] = checkBox3R.Checked;
            vetorR[3] = checkBox4R.Checked;
            vetorR[4] = checkBox5R.Checked;
            vetorR[5] = checkBox6R.Checked;
            vetorR[6] = checkBox7R.Checked;
            vetorR[7] = checkBox8R.Checked;
            vetorR[8] = checkBox9R.Checked;
            vetorR[9] = checkBox10R.Checked;
            vetorR[10] = checkBox11R.Checked;
            vetorR[11] = checkBox12R.Checked;
            vetorR[12] = checkBox13R.Checked;
            vetorR[13] = checkBox14R.Checked;
            vetorR[14] = checkBox15R.Checked;
            vetorR[15] = checkBox16R.Checked;
            return vetorR;
        }

        /// <summary>
        /// recebe vetor bool do controle Mtx1a8 e tranforma em decimal(string)
        /// </summary>
        /// <param name="entrada">vetor de entrada bool a ser convertido em string</param>
        /// <returns>vetor string do controle MTx1a8</returns>
        public string ControleMtx1a8(bool[] entrada)
        {
            int n = 0;
            if (entrada[0] == true) { n = +1; }
            if (entrada[1] == true) { n = +2; }
            if (entrada[2] == true) { n = +4; }
            if (entrada[3] == true) { n = +8; }
            if (entrada[4] == true) { n = +16; }
            if (entrada[5] == true) { n = +32; }
            if (entrada[6] == true) { n = +64; }
            if (entrada[7] == true) { n = +128; }
            return _ = n.ToString();
        }

        /// <summary>
        /// recebe vetor bool do controle Mtx9a16 e tranforma em decimal(string)
        /// </summary>
        /// <param name="entrada">vetor de entrada bool a ser convertido em string</param>
        /// <returns>vetor string do controle MTx9a16</returns>
        public string ControleMtx9a16(bool[] entrada)
        {
            int n = 0;
            if (entrada[8] == true) { n = +1; }
            if (entrada[9] == true) { n = +2; }
            if (entrada[10] == true) { n = +4; }
            if (entrada[11] == true) { n = +8; }
            if (entrada[12] == true) { n = +16; }
            if (entrada[13] == true) { n = +32; }
            if (entrada[14] == true) { n = +64; }
            if (entrada[15] == true) { n = +128; }
            return _ = n.ToString();
        }

        /// <summary>
        /// libera Check do controle manual
        /// </summary>
        public void LiberaControleManual()
        {
            checkBox1T.Checked = false;
            checkBox2T.Checked = false;
            checkBox3T.Checked = false;
            checkBox4T.Checked = false;
            checkBox5T.Checked = false;
            checkBox6T.Checked = false;
            checkBox7T.Checked = false;
            checkBox8T.Checked = false;
            checkBox9T.Checked = false;
            checkBox10T.Checked = false;
            checkBox11T.Checked = false;
            checkBox12T.Checked = false;
            checkBox13T.Checked = false;
            checkBox14T.Checked = false;
            checkBox15T.Checked = false;
            checkBox16T.Checked = false;
            checkBox1R.Checked = false;
            checkBox2R.Checked = false;
            checkBox3R.Checked = false;
            checkBox4R.Checked = false;
            checkBox5R.Checked = false;
            checkBox6R.Checked = false;
            checkBox7R.Checked = false;
            checkBox8R.Checked = false;
            checkBox9R.Checked = false;
            checkBox10R.Checked = false;
            checkBox11R.Checked = false;
            checkBox12R.Checked = false;
            checkBox13R.Checked = false;
            checkBox14R.Checked = false;
            checkBox15R.Checked = false;
            checkBox16R.Checked = false;
        }

        /// <summary>
        /// Desabilita secondary controle
        /// </summary>
        public void DesabilitaPrimary()
        {
            checkBox1T.Visible = false;
            checkBox2T.Visible = false;
            checkBox3T.Visible = false;
            checkBox4T.Visible = false;
            checkBox5T.Visible = false;
            checkBox6T.Visible = false;
            checkBox7T.Visible = false;
            checkBox8T.Visible = false;
            checkBox9T.Visible = false;
            checkBox10T.Visible = false;
            checkBox11T.Visible = false;
            checkBox12T.Visible = false;
            checkBox13T.Visible = false;
            checkBox14T.Visible = false;
            checkBox15T.Visible = false;
            checkBox16T.Visible = false;
            PrimaryInputs.Visible = false;
        }

        /// <summary>
        /// Habilita secondary controle manual
        /// </summary>
        public void HabilitaPrimary()
        {
            checkBox1T.Visible = true;
            checkBox2T.Visible = true;
            checkBox3T.Visible = true;
            checkBox4T.Visible = true;
            checkBox5T.Visible = true;
            checkBox6T.Visible = true;
            checkBox7T.Visible = true;
            checkBox8T.Visible = true;
            checkBox9T.Visible = true;
            checkBox10T.Visible = true;
            checkBox11T.Visible = true;
            checkBox12T.Visible = true;
            checkBox13T.Visible = true;
            checkBox14T.Visible = true;
            checkBox15T.Visible = true;
            checkBox16T.Visible = true;
            PrimaryInputs.Visible = true;
        }

        /// <summary>
        /// Desabilita secondary controle manual
        /// </summary>
        public void DesabilitaSecondary()
        {
            checkBox1R.Visible = false;
            checkBox2R.Visible = false;
            checkBox3R.Visible = false;
            checkBox4R.Visible = false;
            checkBox5R.Visible = false;
            checkBox6R.Visible = false;
            checkBox7R.Visible = false;
            checkBox8R.Visible = false;
            checkBox9R.Visible = false;
            checkBox10R.Visible = false;
            checkBox11R.Visible = false;
            checkBox12R.Visible = false;
            checkBox13R.Visible = false;
            checkBox14R.Visible = false;
            checkBox15R.Visible = false;
            checkBox16R.Visible = false;
            SecondaryInputs.Visible = false;
        }

        /// <summary>
        /// Habilita secondary controle manual
        /// </summary>
        public void HabilitaSecondary()
        {
            checkBox1R.Visible = true;
            checkBox2R.Visible = true;
            checkBox3R.Visible = true;
            checkBox4R.Visible = true;
            checkBox5R.Visible = true;
            checkBox6R.Visible = true;
            checkBox7R.Visible = true;
            checkBox8R.Visible = true;
            checkBox9R.Visible = true;
            checkBox10R.Visible = true;
            checkBox11R.Visible = true;
            checkBox12R.Visible = true;
            checkBox13R.Visible = true;
            checkBox14R.Visible = true;
            checkBox15R.Visible = true;
            checkBox16R.Visible = true;
            SecondaryInputs.Visible = true;
        }

        /// <summary>
        /// Função para gravar nos logs as combinações salvas
        /// </summary>
        /// <param name="combinacoes">String collection com as combinações salvas</param>
        public static void CombinacoesSalvas(System.Collections.Specialized.StringCollection combinacoes, bool enablelog)
        {
            if (enablelog == true)
            {
                log.Info("The saved matrix combinations are: ");
                for (int i = 0; i < combinacoes.Count; i++) { log.Info(combinacoes[i]); }
            }
            else { log.Info("The system is configured to not show matrix combinations saved in the log"); }

        }

        #endregion

        #region Call de Forms

        /// <summary>
        /// Abre form Sobre.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SobreToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Funcoes.CheckFormAberto() == 0)
            {
                log.Debug("Load Sobre");
                using (Form Sobre = new FrmSobre()) { Sobre.ShowDialog(); }
            }
        }

        /// <summary>
        /// Abre form Configuração.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ConfiguraçõesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Funcoes.CheckFormAberto() == 0)
            {
                log.Debug("Load Configurações");
                using (Form Config = new FrmConfiguracoes(this)) { Config.ShowDialog(); }
            }
        }

        /// <summary>
        /// Abre form Logs.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void LogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Funcoes.CheckFormAberto() == 0)
            {
                log.Debug("Load Logs");
                Form logs = new FrmLogs(this); logs.Show();
            }
        }

        /// <summary>
        /// Retorna para painel principal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PainelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            log.Debug("Retorno painel");
            //Fecha os demais forms e da topMost no Panel principal.
            Funcoes.ReturnPanel();
        }

        /// <summary>
        /// Load do Form principal, que ativa o timer do sistema.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPainel_Load(object sender, EventArgs e)
        {
            log.Debug("Load painel");
            //obtem as inforções atuais do settings.
            GetInfoSettings();
            //Inicializa o timer principal
            TimerPanel.Start();
        }

        /// <summary>
        /// Expandindo form p habilitar controle manual.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PictureBox2_Click(object sender, EventArgs e)
        { Expand(); }

        /// <summary>
        /// Função para expandir form painel
        /// </summary>
        private void Expand()
        {
            if (exp == 0)
            {
                LblRodape.Location = new Point(34, 460);
                PnlPulses.Location = new Point(182, 375);
                pictureBox2.Image = CHOV.Properties.Resources.icon_Up;
                this.Height = 525;
                exp = 1;
            }
            else
            {
                LblRodape.Location = new Point(34, 371);
                PnlPulses.Location = new Point(182, 403);
                pictureBox2.Image = CHOV.Properties.Resources.icon_Down;
                this.Height = 410;
                exp = 0;
            }
        }

        /// <summary>
        /// Fechamento Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPainel_FormClosed(object sender, FormClosedEventArgs e)
        { log.Debug("Aplicação encerrada"); }

        /// <summary>
        /// Teclas de atalho 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmPainel_KeyDown(object sender, KeyEventArgs e)
        {
            //Configurações
            if (e.Shift && e.KeyCode == Keys.C) { log.Debug("Atalho Configurações"); using (Form Config = new FrmConfiguracoes(this)) { Config.Show(); } }
            //Logs
            if (e.Shift && e.KeyCode == Keys.L) { log.Debug("Atalho Logs"); using (Form logs = new FrmLogs(this)) { logs.Show(); } }
            //Sobre
            if (e.Shift && e.KeyCode == Keys.H) { log.Debug("Atalho Sobre"); using (Form Sobre = new FrmSobre()) { Sobre.Show(); } }
            //Expandir painel
            if (e.KeyCode == Keys.Space) { log.Debug("Expandido painel"); Expand(); }
        }

        private void BtnWriteTest_MouseEnter(object sender, EventArgs e) { BtnWriteTest.ForeColor = SystemColors.WindowText; }

        private void BtnWriteTest_MouseLeave(object sender, EventArgs e) { BtnWriteTest.ForeColor = SystemColors.ButtonFace; }
        #endregion

        #region Main Timer Routine for Read / Write

        private void Timer1_Tick(object sender, EventArgs e)
        {
            #region old
            //Abre a conexão
            //  listBox1.Items.Insert(0, Funcoes.AbreConexao());

            //Data e hora atual
            //listBox1.Items.Insert(0, DateTime.Now.ToString());

            //Atribui o valor da para variavel "pass"
            //  pass = Funcoes.LeituraEntradas(Selecao);

            //Change
            //  Change(pass);

            //Escreve no histórico a variavel "pass"
            //   Historico.Items.Insert(0, pass);

            //Escreve nas saídas
            //    Historico.Items.Insert(0, Funcoes.EscreveSaidas(pass));

            //Fecha conexão
            //listBox1.Items.Insert(0, Funcoes.FechaConexao());

            /*
            string vtr = Funcoes.FormatByte("255");
            Saida1a16(Funcoes.VetorBoolean(vtr, vtr));
            EntradasT1a16(Funcoes.VetorBoolean(vtr, vtr));
            EntradasR1a16(Funcoes.VetorBoolean(vtr, vtr));
            Saida1a16(Funcoes.VetorBoolean(vtr, vtr));
            Historico.Items.Insert(0, Funcoes.StatusIN1a8(Funcoes.VetorBoolean(vtr, vtr)));
            Historico.Items.Insert(0, Funcoes.StatusIN9a16(Funcoes.VetorBoolean(vtr, vtr)));
            */
            /*

            if (Selecao == "Primary")
            {
                vt = ControleManualTitular();
                EntradasT1a16(vt);
                Historico.Items.Insert(0, Funcoes.StatusIN1a8(vt));
                Historico.Items.Insert(0, Funcoes.StatusOUT1a8(Selecao, vt));

                pictureBox1.Image = CHOV.Properties.Resources.Tit;

                TitularAtivado();
                HabilitaPrimary();
                DesabilitaSecondary();
                PnlPulses.Visible = true;

            }
            if (Selecao == "Secondary")
            {
                vt = ControleManualReserva();
                EntradasR1a16(vt);
                Historico.Items.Insert(0, Funcoes.StatusIN1a8(vt));
                Historico.Items.Insert(0, Funcoes.StatusOUT1a8(Selecao, vt));

                pictureBox1.Image = CHOV.Properties.Resources.Reser;

                ReservaAtivado();
                HabilitaSecondary();
                DesabilitaPrimary();

                PnlPulses.Visible = true;
               }
         */
            #endregion

            #region HERE
            /*
            //Abre a conexão
            // Funcoes.AbreConexao("10.8.201.76");

            //ALTERADO AQUI<<
            listBox1.Items.Insert(0, Funcoes.AbreConexao());

            listBox1.Items.AddRange(Funcoes.Inicial());

            string vtrr = Funcoes.LeituraEntradas(CurrentSelection_pgm);
            string vtr = Funcoes.FormatByte(vtrr);
            string zero = Funcoes.FormatByte("0");

            if ((Funcoes.LeituraEntradas(CurrentSelection_pgm) != "0"))
            {
                listBox1.Items.Insert(0, "Valor decimal da leitura: " + Funcoes.LeituraEntradas(CurrentSelection_pgm));
            }

            CtT = Funcoes.VetorBoolean(vtr, zero);
            CtR = Funcoes.VetorBoolean(vtr, zero);

            //Escreve nas saídas
            listBox1.Items.Insert(0, Funcoes.EscreveSaidas(vtrr));
            
            //Fecha conexão
            //listBox1.Items.Insert(0, Funcoes.FechaConexao());


            Teste();
            */
            #endregion

            LedSystem();
            CtT = ControleManualTitular();
            CtR = ControleManualReserva();
            primary1a8 = ControleMtx1a8(CtT);
            primary9a16 = ControleMtx9a16(CtT);
            secondary1a8 = ControleMtx1a8(CtR);
            secondary9a16 = ControleMtx9a16(CtR);
            int p = PositionPulse(CtT, CtR, CHOV.Properties.Settings.Default.DeviceChg0);
            //Relógio & Data
            LblRelogio.Text = Relogio_pgm = DateTime.Now.ToLongTimeString();
            LblData.Text = Data_pgm = Funcoes.GetDateSystem();
            LblRelogio.Visible = true;
            LblData.Visible = true;
            ZeraAllInOut();
            switch (System_pgm)
            {
                case "Change Over":
                    if (Funcoes.TemTrue(CtT) || Funcoes.TemTrue(CtR))
                    {

                        if (CurrentSelection_pgm == "Primary")
                        {
                            if ((DeviceChg0_pgm == "Secondary" && (Chg0ver(CHOV.Properties.Settings.Default.InputDeviceChg0, p, CHOV.Properties.Settings.Default.System))))
                            {
                                vt = CtR;
                                EntradasR1a16(vt);
                                ArrayHistoric.Add(Funcoes.StatusIn1a16ChgT(vt));
                                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                                Saida1a16(vt);
                                log.Info("Output:" + Funcoes.OutOnlyOnLog(Funcoes.FormatVetorStatus(vt)));
                            }
                            if (Funcoes.TemTrue(CtT) && CurrentSelection_pgm == "Primary")
                            {
                                vt = CtT;
                                EntradasT1a16(vt);
                                ArrayHistoric.Add(Funcoes.StatusIn1a16ChgR(vt));
                                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                                log.Info("Output:" + Funcoes.OutOnlyOnLog(Funcoes.FormatVetorStatus(vt)));
                                Saida1a16(vt);
                            }
                        }
                        if (CurrentSelection_pgm == "Secondary")
                        {
                            if ((DeviceChg0_pgm == "Primary" && (Chg0ver(CHOV.Properties.Settings.Default.InputDeviceChg0, p, CHOV.Properties.Settings.Default.System))))
                            {
                                vt = CtT;
                                EntradasT1a16(vt);
                                ArrayHistoric.Add(Funcoes.StatusIn1a16ChgR(vt));
                                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                                Saida1a16(vt);
                                log.Info("Output:" + Funcoes.OutOnlyOnLog(Funcoes.FormatVetorStatus(vt)));
                            }
                            if (Funcoes.TemTrue(CtR) && CurrentSelection_pgm == "Secondary")
                            {
                                vt = CtR;
                                EntradasR1a16(vt);
                                ArrayHistoric.Add(Funcoes.StatusIn1a16ChgT(vt));
                                Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                                Saida1a16(vt);
                                log.Info("Output:" + Funcoes.OutOnlyOnLog(Funcoes.FormatVetorStatus(vt)));
                            }
                        }
                        Chg0verON(CHOV.Properties.Settings.Default.InputDeviceChg0, p, CHOV.Properties.Settings.Default.System);
                        //Desflega o controle manual.
                        LiberaControleManual();
                    }
                    return;
                case "Matrix of Signals":
                    bool[] Exeslots = Funcoes.ExeCombinacoes(Properties.Settings.Default.Combinations, primary1a8, primary9a16, secondary1a8, secondary9a16);
                    Saida1a16(Exeslots);
                    if (Funcoes.TemTrue(Exeslots) == true)
                    {
                        string inflog = Funcoes.SlotsLogs(Exeslots);
                        ArrayHistoric.Add(inflog);
                        Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                        log.Info("Output:" + Funcoes.OutOnlyOnLog(Funcoes.FormatVetorStatus(Exeslots)));
                        CombinacoesSalvas(Properties.Settings.Default.Combinations, Properties.Settings.Default.EnableCombinationsLog);
                    }
                    //Desflega o controle manual.
                    if (Funcoes.TemTrue(CtT))
                    {
                        ArrayHistoric.Add(Funcoes.StatusIn1a16ChgR(CtT));
                        Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                        log.Info("Input:" + Funcoes.InOnlyOnLog(Funcoes.FormatVetorStatus(CtT)));
                    }
                    if (Funcoes.TemTrue(CtR))
                    {
                        ArrayHistoric.Add(Funcoes.StatusIn1a16ChgT(CtR));
                        Historico.Items.Insert(0, ArrayHistoric[ArrayHistoric.Count - 1]);
                        log.Info("Input:" + Funcoes.InOnlyOnLog(Funcoes.FormatVetorStatus(CtR)));
                    }
                    LiberaControleManual();
                    return;
            }
        }

        #endregion

    }
}



