using log4net;
using System;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace CHOV
{
    partial class FrmSobre : Form
    {
        //Variável Log
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));

        public FrmSobre()
        {
            InitializeComponent();
            this.Text = string.Format("Sobre {0}", AssemblyTitle);
            this.labelProductName.Text = AssemblyProduct;
            this.labelVersion.Text = string.Format("Versão {0}", AssemblyVersion);
            this.labelCopyright.Text = AssemblyCopyright;
            this.labelCompanyName.Text = AssemblyCompany;
            this.textBoxDescription.Text = AssemblyDescription;
        }

        #region Assembly Attribute Attachment
        public string AssemblyTitle
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
                if (attributes.Length > 0)
                {
                    AssemblyTitleAttribute titleAttribute = (AssemblyTitleAttribute)attributes[0];
                    if (titleAttribute.Title != "") { return titleAttribute.Title; }
                }
                return System.IO.Path.GetFileNameWithoutExtension(Assembly.GetExecutingAssembly().CodeBase);
            }
        }

        public string AssemblyVersion
        { get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); } }

        public string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0) { return ""; }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        public string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                { return ""; }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        public string AssemblyCopyright
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
                if (attributes.Length == 0)
                { return ""; }
                return ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        public string AssemblyCompany
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyCompanyAttribute), false);
                if (attributes.Length == 0) { return ""; }
                return ((AssemblyCompanyAttribute)attributes[0]).Company;
            }
        }

        #endregion

        #region Actions Buttons
        private void OkButton_Click(object sender, EventArgs e)
        { log.Debug("Botão Ok acionado"); this.Close(); }

        private void OkButton_MouseEnter(object sender, EventArgs e)
        { okButton.ForeColor = SystemColors.WindowText; }

        private void OkButton_MouseLeave(object sender, EventArgs e)
        { okButton.ForeColor = SystemColors.ButtonFace; }

        #endregion

        /// <summary>
        /// Teclas de atalho
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSobre_KeyDown(object sender, KeyEventArgs e)
        { if (e.Shift && e.KeyCode == Keys.P) { log.Debug("Atalho para painel acionado"); Funcoes.ReturnPanel(); } }

        /// <summary>
        /// Load Form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmSobre_Load(object sender, EventArgs e)
        { log.Debug("Form Sobre carregado"); }
    }
}
