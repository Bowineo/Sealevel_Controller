using System;
using System.Drawing;
using System.Windows.Forms;

namespace CHOV
{
    public partial class MmsgBox : Form
    {
        /// <summary>
        /// Declarando construtores.
        /// </summary>
        public int Iccon { get; private set; }
        public int Res { get; private set; }
        public string TtypeMsg { get; private set; }
        public string[] Txt { get; private set; }
        public string[] Lista { get; private set; }

        /// <summary>
        /// Contrustor generico da mensagem
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="TypeMsg">string tipo de mensagem</param>
        /// <param name="Icon">string para selecionaro icone</param>
        /// <param name="res">string para cair no caso especial das combinações</param>
        public MmsgBox(string txt, string TypeMsg, int Icon, int res)
        {
            InitializeComponent();
            //Atribuição das var de entrada.
            Iccon = Icon;
            LblMsgBox.Text = txt;
            TtypeMsg = TypeMsg;
            Res = res;
        }


        /// <summary>
        /// Contrustor generico da mensagem com combinações de matrix
        /// </summary>
        /// <param name="vetor"></param>
        /// <param name="txt"></param>
        /// <param name="TypeMsg">string tipo de mensagem</param>
        /// <param name="Icon">string para selecionaro icone</param>
        /// <param name="res">string para cair no caso especial das combinações</param>
        public MmsgBox(string[] vetor, string txt, string TypeMsg, int Icon, int res)
        {
            InitializeComponent();
            //Atribuição das var de entrada.
            Iccon = Icon;
            LblMsgBox.Text = txt;
            Lista = vetor;
            TtypeMsg = TypeMsg;
            Res = res;
            Resume();
        }

        /// <summary>
        /// Ação Botão OK.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOkMsgBox_Click(object sender, EventArgs e)
        { this.DialogResult = DialogResult.OK; }

        /// <summary>
        /// Ação Botão Save.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnSaveMsgBox_Click(object sender, EventArgs e)
        { this.DialogResult = DialogResult.Yes; }

        /// <summary>
        /// Ação Botão Cancel.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCancelMsgBox_Click(object sender, EventArgs e)
        { this.DialogResult = DialogResult.Cancel; }

        /// <summary>
        /// Load Form MsgBox.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MsgBox_Load(object sender, EventArgs e)
        {
            //Seleciona o icone
            SeletorIcon(Iccon);
            //Seleciona o tipo de msg
            SeletorTypeMsg(TtypeMsg, Res);
        }

        /* Icones Disponiveis:
         * 1-Sucess 
         * 2-Info 
         * 3-Alert 
         * 4-Pergunta? 
         * 5-Delete 
         * 6-RecycleBin.
         * 7-Sem img
        */
        /// <summary>
        /// Determina o icone da mensagem.
        /// </summary>
        /// <param name="num">int para selecionar o icone da mensagem</param>
        public void SeletorIcon(int num)
        {
            switch (num)
            {
                case 1:
                    MmsgBox.ActiveForm.Text = "Sucess";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_selecionado_50_2;
                    break;
                case 2:
                    MmsgBox.ActiveForm.Text = "Information";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_informações_50_2;
                    break;
                case 3:
                    MmsgBox.ActiveForm.Text = "Alert";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_erro_50;
                    break;
                case 4:
                    MmsgBox.ActiveForm.Text = "Alert";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_ajuda_50;
                    break;
                case 5:
                    MmsgBox.ActiveForm.Text = "Error";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_respostas_50;
                    break;
                case 6:
                    MmsgBox.ActiveForm.Text = "Alert";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_excluir_50;
                    break;
                case 7:
                    MmsgBox.ActiveForm.Text = "Combinations";
                    pictureBoxMsg.Image = CHOV.Properties.Resources.icons8_respostas_50;
                    break;

            }
        }

        /* Tipo de Mensagem:
        OK 
        OK&CANCEL
        SAVE&CANCEL.
        */
        /// <summary>
        /// Determina o tipo da mensagem e os botões adequados. 
        /// </summary>
        /// <param name="txt">Texto da mensagem</param>
        /// <param name="n"> int para cair na condição especial</param>
        public void SeletorTypeMsg(string txt, int n)
        {
            if (n == 0)
            {
                switch (txt)
                {
                    case "OK":
                        BtnOkMsgBox.Visible = true; BtnSaveMsgBox.Visible = false; BtnCancelMsgBox.Visible = false; BtnOkMsgBox.Location = new Point(170, 187);
                        break;
                    case "OK&CANCEL":
                        BtnOkMsgBox.Visible = true; BtnSaveMsgBox.Visible = false; BtnCancelMsgBox.Visible = true; BtnOkMsgBox.Location = new Point(137, 187); BtnCancelMsgBox.Location = new Point(207, 187);
                        break;
                    case "SAVE&CANCEL":
                        BtnOkMsgBox.Visible = false; BtnSaveMsgBox.Visible = true; BtnCancelMsgBox.Visible = true; BtnSaveMsgBox.Location = new Point(137, 187); BtnCancelMsgBox.Location = new Point(207, 187);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                if (n == 13)
                {
                    switch (txt)
                    {
                        case "OK":
                            BtnOkMsgBox.Visible = true; BtnSaveMsgBox.Visible = false; BtnCancelMsgBox.Visible = false; BtnOkMsgBox.Location = new Point(170, 187);
                            break;
                        case "OK&CANCEL":
                            BtnOkMsgBox.Visible = true; BtnSaveMsgBox.Visible = false; BtnCancelMsgBox.Visible = true; BtnOkMsgBox.Location = new Point(137, 187); BtnCancelMsgBox.Location = new Point(207, 187);
                            break;
                        case "SAVE&CANCEL":
                            BtnOkMsgBox.Visible = false; BtnSaveMsgBox.Visible = true; BtnCancelMsgBox.Visible = true; BtnSaveMsgBox.Location = new Point(137, 187); BtnCancelMsgBox.Location = new Point(207, 187);
                            break;
                    }
                }
                else
                {
                    switch (txt)
                    {
                        case "OK":
                            BtnOkMsgBox.Visible = true; BtnSaveMsgBox.Visible = false; BtnCancelMsgBox.Visible = false; BtnOkMsgBox.Location = new Point(170, 320);
                            break;
                        case "OK&CANCEL":
                            BtnOkMsgBox.Visible = true; BtnSaveMsgBox.Visible = false; BtnCancelMsgBox.Visible = true; BtnOkMsgBox.Location = new Point(137, 320); BtnCancelMsgBox.Location = new Point(207, 320);
                            break;
                        case "SAVE&CANCEL":
                            BtnOkMsgBox.Visible = false; BtnSaveMsgBox.Visible = true; BtnCancelMsgBox.Visible = true; BtnSaveMsgBox.Location = new Point(137, 530); BtnCancelMsgBox.Location = new Point(207, 530);
                            break;
                        default:
                            break;
                    }
                }

            }
        }

        /// <summary>
        /// Função para preencher os labels com os dados das combinações
        /// </summary>
        /// <param name="saida"> string[] com as combinações</param>
        public void PreencheLbl(string[] saida)
        {
            int count = Lista.Length;
            if (count == 1) { comb_01.Text = saida[0]; }
            if (count == 2) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_02.Text = saida[1]; }
            if (count == 3) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; }
            if (count == 4) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; }
            if (count == 5) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; }
            if (count == 6) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; }
            if (count == 7) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; }
            if (count == 8) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; }
            if (count == 9) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; }
            if (count == 10) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; }
            if (count == 11) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; comb_11.Text = saida[10]; }
            if (count == 12) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; comb_11.Text = saida[10]; comb_12.Text = saida[11]; }
            if (count == 13) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; comb_11.Text = saida[10]; comb_12.Text = saida[11]; comb_13.Text = saida[12]; }
            if (count == 14) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; comb_11.Text = saida[10]; comb_12.Text = saida[11]; comb_13.Text = saida[12]; comb_14.Text = saida[13]; }
            if (count == 15) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; comb_11.Text = saida[10]; comb_12.Text = saida[11]; comb_13.Text = saida[12]; comb_14.Text = saida[13]; comb_15.Text = saida[14]; }
            if (count == 16) { comb_01.Text = saida[0]; comb_02.Text = saida[1]; comb_03.Text = saida[2]; comb_04.Text = saida[3]; comb_05.Text = saida[4]; comb_06.Text = saida[5]; comb_07.Text = saida[6]; comb_08.Text = saida[7]; comb_09.Text = saida[8]; comb_10.Text = saida[9]; comb_11.Text = saida[10]; comb_12.Text = saida[11]; comb_13.Text = saida[12]; comb_14.Text = saida[13]; comb_15.Text = saida[14]; comb_16.Text = saida[15]; }
        }

        /// <summary>
        /// Função para Resumir os slots de combinações
        /// </summary>
        public void Resume()
        {
            PreencheLbl(Lista);
            int count = Lista.Length;
            pnl_Comb.Visible = true;
            pnl_default_msg.Visible = true;
            switch (count)
            {
                case 01: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 030; pnl_Comb.Location = new Point(424, 109); pnl_default_msg.Location = new Point(7, 16); return;
                case 02: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 055; pnl_Comb.Location = new Point(424, 097); pnl_default_msg.Location = new Point(7, 16); return;
                case 03: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 075; pnl_Comb.Location = new Point(424, 085); pnl_default_msg.Location = new Point(7, 16); return;
                case 04: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 100; pnl_Comb.Location = new Point(424, 074); pnl_default_msg.Location = new Point(7, 16); return;
                case 05: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 120; pnl_Comb.Location = new Point(424, 064); pnl_default_msg.Location = new Point(7, 16); return;
                case 06: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 148; pnl_Comb.Location = new Point(424, 050); pnl_default_msg.Location = new Point(7, 16); return;
                case 07: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 170; pnl_Comb.Location = new Point(424, 039); pnl_default_msg.Location = new Point(7, 16); return;
                case 08: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 190; pnl_Comb.Location = new Point(424, 029); pnl_default_msg.Location = new Point(7, 16); return;
                case 09: this.Width = 995; this.Height = 293; pnl_Comb.Width = 540; pnl_Comb.Height = 217; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 16); return;
                case 10: this.Width = 995; this.Height = 313; pnl_Comb.Width = 540; pnl_Comb.Height = 240; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 28); return;
                case 11: this.Width = 995; this.Height = 336; pnl_Comb.Width = 540; pnl_Comb.Height = 263; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 39); return;
                case 12: this.Width = 995; this.Height = 358; pnl_Comb.Width = 540; pnl_Comb.Height = 284; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 50); return;
                case 13: this.Width = 995; this.Height = 381; pnl_Comb.Width = 540; pnl_Comb.Height = 309; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 62); return;
                case 14: this.Width = 995; this.Height = 407; pnl_Comb.Width = 540; pnl_Comb.Height = 332; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 74); return;
                case 15: this.Width = 995; this.Height = 429; pnl_Comb.Width = 540; pnl_Comb.Height = 354; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 85); return;
                case 16: this.Width = 995; this.Height = 451; pnl_Comb.Width = 540; pnl_Comb.Height = 378; pnl_Comb.Location = new Point(424, 016); pnl_default_msg.Location = new Point(7, 99); return;
                default: break;
            }
        }
    }
}
