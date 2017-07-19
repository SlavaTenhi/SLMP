using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormMods : Form
    {
        public static string CPFilesPath = FormMain.gameFolder + @"Skyrim\CPFiles\";
        string noFileSelect = "Не выбран файл.";
        string confirmDelete = "Удалить мод?";
        string confirmTitle = "Подтверждение";

        public FormMods()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(FormMain.launcherFolder);
            if (FormMain.numberStyle > 1)
            {
                imageBackgroundImage();
            }
            if (FormMain.langTranslate != "RU")
            {
                LangTranslateEN();
            }
            RefreshFileList();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void imageBackgroundImage()
        {
            BackgroundImage = Properties.Resources.FormBackground;
            FuncMisc.LabelsTextColor(this, System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(30, 30, 30), false);
        }
        private void LangTranslateEN()
        {
            label1.Text = "Files available " + Environment.NewLine + @"from the folder Skyrim\CPFiles";
            label2.Text = "Uninstalling " + Environment.NewLine + "standard mod";
            button_ModInstall.Text = "Install";
            noFileSelect = "No file select.";
            confirmDelete = "Delete mod?";
            confirmTitle = "Confirm";
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void RefreshFileList()
        {
            if (Directory.Exists(CPFilesPath))
            {
                foreach (string line in Directory.GetFiles(CPFilesPath, "*.rar").Select(f => f.Substring((CPFilesPath).Length)))
                {
                    listBox1.Items.Add(line);
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void button_ModInstall_Click(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex != -1)
            {
                fileUnpack(listBox1.GetItemText(listBox1.SelectedItem));
            }
            else
            {
                MessageBox.Show(noFileSelect);
            }
        }
        private void fileUnpack(string filename)
        {
            FuncMisc.ToggleButtons(this, false);
            listBox1.Enabled = false;
            FuncMisc.UnPackRAR(CPFilesPath + filename);
            FuncMisc.ToggleButtons(this, true);
            listBox1.Enabled = true;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonDeleteOSA_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.OSA);
        }
        private void buttonDeleteAS_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.AS);
        }
        private void buttonDeleteFFC_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.FFC);
        }
        private void buttonDeleteINEED_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.INEED);
        }
        private void buttonDeleteLAD_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.LAD);
        }
        private void buttonDeleteORD_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.ORD);
        }
        private void buttonDeleteTunic_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.TUN);
        }
        private void buttonDeleteUnPaused_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.UP);
        }
        private void buttonDeleteCamera_Click(object sender, EventArgs e)
        {
            deleteMod(FuncClear.CAM);
        }
        private void deleteMod(MethodInvoker method)
        {
            DialogResult dialogResult = MessageBox.Show(confirmDelete, confirmTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                method();
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonClose_Click(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}