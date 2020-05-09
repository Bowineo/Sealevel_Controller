using System;
using System.Windows.Forms;
using CHOV___MODEL;

namespace CHOV
{
    public partial class FrmHardware : Form
    {

        FrmPainel frmH;
        public FrmHardware(FrmPainel frm2)
        {
            InitializeComponent();
            frmH = frm2;
        }

        private void btCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void BtClear_Click(object sender, EventArgs e)
        {
           
        }

        private void BtOpen_Click(object sender, EventArgs e)
        {
            
        }

  
    }
}
