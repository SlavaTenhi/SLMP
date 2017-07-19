using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormWidget : Form
    {
        FormMain mainFormStyle = null;

        public FormWidget()
        {
            InitializeComponent();
            if (FormMain.numberStyle > 1)
            {
                ImageBackgroundImage();
            }
            if (FormMain.langTranslate != "RU")
            {
                LangTranslateEN();
            }
            if (FuncParser.stringRead(FormMain.iniLauncher, "General", "HideWebButtons") == "true")
            {
                ClientSize = new System.Drawing.Size(221, 60);
                label2.Size = new System.Drawing.Size(221, 60);
                pictureBox4.Visible = false;
                buttonUpdates.Visible = false;
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void ImageBackgroundImage()
        {
            BackgroundImage = Properties.Resources.FormBackground;
            label1.ForeColor = System.Drawing.SystemColors.ControlLight;
        }
        private void LangTranslateEN()
        {
            label1.Text = "Styles";
            buttonUpdates.Text = "Updates";
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonStyle1_Click(object sender, System.EventArgs e)
        {
            FormMain.numberStyle = 1;
            mainFormStyle = Owner as FormMain;
            mainFormStyle.RefreshStyle();
            BackgroundImage = Properties.Resources.FormBackgroundNone;
            label1.ForeColor = System.Drawing.SystemColors.ControlText;
            this.Owner.Focus();
        }
        private void buttonStyle2_Click(object sender, System.EventArgs e)
        {
            FormMain.numberStyle = 2;
            mainFormStyle = Owner as FormMain;
            mainFormStyle.RefreshStyle();
            ImageBackgroundImage();
            this.Owner.Focus();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void pictureBox2_Click(object sender, System.EventArgs e)
        {
            FormMain.langTranslate = "RU";
            mainFormStyle = Owner as FormMain;
            mainFormStyle.SetLangTranslateRU(false);
            label1.Text = "Стили";
            buttonUpdates.Text = "Обновления";
            this.Owner.Focus();
        }
        private void pictureBox3_Click(object sender, System.EventArgs e)
        {
            FormMain.langTranslate = "EN";
            mainFormStyle = Owner as FormMain;
            mainFormStyle.SetLangTranslateEN();
            LangTranslateEN();
            this.Owner.Focus();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void label2_Click(object sender, System.EventArgs e)
        {
            this.Owner.Focus();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonUpdates_Click(object sender, System.EventArgs e)
        {
            var form = new FormUpdates();
            form.ShowDialog(this.Owner);
            form = null;
            this.Owner.Focus();
        }
    }
}