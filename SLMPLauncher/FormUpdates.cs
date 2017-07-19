using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormUpdates : Form
    {
        string updateFolder = FormMain.launcherFolder + @"Updates\";
        string nameUpdateInfo = "UpdateInfo.ini";
        string nameControlPanel = "SLMPLauncher.exe";
        string nameHostName = "http://www.slmp.ru";
        string nameDLFolderHost = "/_SLMP-GR/";
        string downloadFileType = null;
        string downloadFileName = null;
        string buttonUpdateCP_TE = "Обновить";
        string buttonUpdateCP_TN = "Нет обновления";
        string buttonCvsU_TI = "Установлено";
        string buttonCvsU_TU = "Обновить";
        string buttonCvsU_TS = "Стоп";
        string buttonCvsU_TC = "Проверить";
        string label4_T = "нет обновлений";
        string label5_T = "Размер: ";
        string wrongPing = "Нет доступа к: ";
        string installedUpdate = "Установлено / ";
        string installedUpdateN = "Обновление / ";
        string notSyncWithUI = "Скачанный файл не соответствует UpdateInfo. Повторите попытку.";
        string noTools = "Нет компонентов для установки обновления (файла обновления, UnRAR или UpdateInfo).";
        bool updatesFound = false;
        bool stopDownload = false;
        bool updatesCPFound = false;
        bool updateInstall = false;
        int numberSelectFile = -1;
        List<int> realIndexI = new List<int>();
        List<int> realIndex = new List<int>();
        List<string> installPreLoad = new List<string>();
        WebClient client = new WebClient();

        public FormUpdates()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(FormMain.launcherFolder);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            if (FormMain.numberStyle > 1)
            {
                imageBackgroundImage();
            }
            if (FormMain.langTranslate != "RU")
            {
                LangTranslateEN();
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void imageBackgroundImage()
        {
            BackgroundImage = Properties.Resources.FormBackground;
            FuncMisc.LabelsTextColor(this, System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(30, 30, 30), false);
        }
        private void LangTranslateEN()
        {
            label1.Text = "Game and control panel updates";
            label2.Text = "Panel update";
            label3.Text = "Available files";
            label4.Text = "Not check";
            buttonAboutU.Text = "About update";
            buttonUpdateCP.Text = "Not check";
            buttonUpdateCP_TE = "Update";
            buttonUpdateCP_TN = "No updates";
            buttonCvsU.Text = "Check";
            buttonCvsU_TI = "Installed";
            buttonCvsU_TU = "Update";
            buttonCvsU_TS = "Stop";
            buttonCvsU_TC = "Check";
            label4_T = "no updates";
            label5_T = "Size: ";
            wrongPing = "No access to: ";
            installedUpdate = "Installed / ";
            installedUpdateN = "Update / ";
            notSyncWithUI = "The downloaded file does not correspond to UpdateInfo. Try again.";
            noTools = "No components to install the update (update file, UnRAR or UpdateInfo).";
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonCheckU_Click(object sender, EventArgs e)
        {
            if (stopDownload)
            {
                client.CancelAsync();
                stopDownload = false;
                EnableDisableButtons();
            }
            else
            {
                stopDownload = true;
                EnableDisableButtons();
                if (updatesFound)
                {
                    string line = FuncParser.stringRead(updateFolder + nameUpdateInfo, "Update_" + numberSelectFile, "update_file_warning");
                    if (line != null)
                    {
                        MessageBox.Show(line);
                        line = null;
                    }
                    if (checkUpdateFile(false))
                    {
                        unpackUpdates();
                    }
                    else
                    {
                        FuncFiles.Delete(updateFolder + "file" + numberSelectFile + ".rar");
                        downloadFileName = "file" + numberSelectFile + ".rar";
                        downloadFileType = "UpdateG";
                        client_DownloadProgressStart();
                    }
                }
                else
                {
                    FuncFiles.Delete(updateFolder + nameUpdateInfo);
                    downloadFileName = nameUpdateInfo;
                    downloadFileType = "CheckU";
                    realIndexI.Clear();
                    realIndex.Clear();
                    installPreLoad.Clear();
                    client_DownloadProgressStart();
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void aboutU_Click(object sender, EventArgs e)
        {
            MessageBox.Show(FuncParser.stringRead(updateFolder + nameUpdateInfo, "Update_" + numberSelectFile, "update_file_discription"));
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonUpdateCP_Click(object sender, EventArgs e)
        {
            stopDownload = true;
            EnableDisableButtons();
            FuncFiles.Delete(updateFolder + nameControlPanel);
            downloadFileName = nameControlPanel;
            downloadFileType = "UpdateCP";
            client_DownloadProgressStart();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void EnableDisableButtons()
        {
            if (updatesFound)
            {
                if (stopDownload)
                {
                    comboBox1.Enabled = false;
                    buttonAboutU.Enabled = false;
                    buttonCvsU.Text = buttonCvsU_TS;
                }
                else
                {
                    comboBox1.Enabled = true;
                    buttonAboutU.Enabled = true;
                    label5.Text = label5_T + FuncParser.convertFileSize(FuncParser.doubleRead(updateFolder + nameUpdateInfo, "Update_" + numberSelectFile, "update_file_filesize"));
                    if (updateInstall)
                    {
                        buttonCvsU.Enabled = false;
                        buttonCvsU.Text = buttonCvsU_TI;
                    }
                    else
                    {
                        buttonCvsU.Enabled = true;
                        buttonCvsU.Text = buttonCvsU_TU;
                    }
                }
            }
            else
            {
                comboBox1.Enabled = false;
                buttonAboutU.Enabled = false;
                label5.Text = "";
                if (stopDownload)
                {
                    buttonCvsU.Text = buttonCvsU_TS;
                }
                else
                {
                    buttonCvsU.Text = buttonCvsU_TC;
                }
            }
            if (updatesCPFound)
            {
                if (stopDownload)
                {
                    buttonUpdateCP.Enabled = false;
                }
                else
                {
                    buttonUpdateCP.Enabled = true;
                    buttonUpdateCP.Text = buttonUpdateCP_TE;
                }
            }
            else
            {
                buttonUpdateCP.Enabled = false;
                buttonUpdateCP.Text = buttonUpdateCP_TN;
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void client_DownloadProgressStart()
        {
            if (FuncMisc.Ping(nameHostName + nameDLFolderHost + downloadFileName))
            {
                FuncFiles.CreatDirectory(updateFolder);
                client.DownloadFileAsync(new Uri(nameHostName + nameDLFolderHost + downloadFileName), updateFolder + downloadFileName);
            }
            else
            {
                stopDownload = false;
                EnableDisableButtons();
                MessageBox.Show(wrongPing + nameHostName + nameDLFolderHost + downloadFileName);
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            progressBar1.Value = e.ProgressPercentage;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            if (stopDownload)
            {
                if (File.Exists(updateFolder + nameUpdateInfo))
                {
                    if (downloadFileType == "CheckU")
                    {
                        int CountComboBox = FuncParser.intRead(updateFolder + nameUpdateInfo, "General", "numbers_files_update");
                        if (CountComboBox > 0)
                        {
                            for (int i = 1; i <= CountComboBox; i++)
                            {
                                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                                if (checkUpdateVersion(i))
                                {
                                    realIndexI.Add(i);
                                    installPreLoad.Add(installedUpdate + FuncParser.stringRead(updateFolder + nameUpdateInfo, "Update_" + i, "update_file"));
                                }
                                else
                                {
                                    realIndex.Add(i);
                                    comboBox1.Items.Add(installedUpdateN + FuncParser.stringRead(updateFolder + nameUpdateInfo, "Update_" + i, "update_file"));
                                }
                                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
                            }
                            for (int i = 0; i < realIndexI.Count; i++)
                            {
                                realIndex.Add(realIndexI[i]);
                                comboBox1.Items.Add(installPreLoad[i]);
                            }
                            if (comboBox1.Items.Count > 0)
                            {
                                comboBox1.SelectedIndex = 0;
                            }
                            updatesFound = true;
                            label4.Text = CountComboBox.ToString();
                        }
                        else
                        {
                            updatesFound = false;
                            label4.Text = label4_T;
                        }
                        string line = FuncParser.stringRead(updateFolder + nameUpdateInfo, "General", "version_control_panel");
                        if (line != null)
                        {
                            var result = new Version(FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).ProductVersion).CompareTo(new Version(line));
                            if (result < 0)
                            {
                                updatesCPFound = true;
                            }
                            else
                            {
                                updatesCPFound = false;
                            }
                        }
                    }
                    if (downloadFileType == "UpdateG")
                    {
                        if (checkUpdateFile(true))
                        {
                            unpackUpdates();
                        }
                    }
                    if (downloadFileType == "UpdateCP")
                    {
                        string version1 = FileVersionInfo.GetVersionInfo(updateFolder + nameControlPanel).ProductVersion;
                        if (version1 == FuncParser.stringRead(updateFolder + nameUpdateInfo, "General", "version_control_panel"))
                        {
                            StreamWriter writer = new StreamWriter(FormMain.launcherFolder + "Update.bat");
                            writer.WriteLine("@Echo off");
                            writer.WriteLine("mode con:cols=50 lines=10");
                            writer.WriteLine("color 0E");
                            writer.WriteLine("cd %~dp0 >nul 2>nul");
                            writer.WriteLine("SET CP_S=" + FormMain.launcherFolder + nameControlPanel);
                            writer.WriteLine("SET CP_U=" + updateFolder + nameControlPanel);
                            writer.WriteLine("Echo Please Wait 5 second before start update.");
                            writer.WriteLine("TIMEOUT /T 2 /NOBREAK > nul");
                            writer.WriteLine("IF EXIST \"%CP_U%\" (");
                            writer.WriteLine("Echo -Update file found.");
                            writer.WriteLine("TIMEOUT /T 1 /NOBREAK > nul");
                            writer.WriteLine("Echo -Deleted old file control panel.");
                            writer.WriteLine("del \"%CP_S%\" /Q >nul 2>nul");
                            writer.WriteLine("TIMEOUT /T 1 /NOBREAK > nul");
                            writer.WriteLine("Echo -Trying move new file control panel.");
                            writer.WriteLine("move /Y \"%CP_U%\" \"%CP_S%\" >nul 2>nul");
                            writer.WriteLine("TIMEOUT /T 1 /NOBREAK > nul");
                            writer.WriteLine("Echo -Expectation launching new control panel.");
                            writer.WriteLine("start \"Run new file\" \"%CP_S%\" >nul 2>nul");
                            writer.WriteLine(") else (");
                            writer.WriteLine("Echo -Update file not found...");
                            writer.WriteLine("TIMEOUT /T 5 /NOBREAK > nul");
                            writer.WriteLine(")");
                            writer.WriteLine("Echo -Ready. Closing.");
                            writer.WriteLine("TIMEOUT /T 2 /NOBREAK > nul");
                            writer.WriteLine("del \"" + FormMain.launcherFolder + "Update.bat\" /Q >nul 2>nul");
                            writer.Close();
                            Process.Start(FormMain.launcherFolder + "Update.bat");
                            Application.Exit();
                        }
                        else
                        {
                            MessageBox.Show(notSyncWithUI);
                            FuncFiles.Delete(updateFolder + nameControlPanel);
                        }
                    }
                }
                else
                {
                    updatesFound = false;
                    updatesCPFound = false;
                }
            }
            stopDownload = false;
            progressBar1.Value = 0;
            EnableDisableButtons();
        }

        private bool checkUpdateFile(bool fromDL)
        {
            if (File.Exists(updateFolder + "file" + numberSelectFile + ".rar") && File.Exists(updateFolder + nameUpdateInfo) && File.Exists(FormMain.launcherFolder + "UnRAR.exe"))
            {
                if (new FileInfo(updateFolder + "file" + numberSelectFile + ".rar").Length == FuncParser.doubleRead(updateFolder + nameUpdateInfo, "Update_" + numberSelectFile, "update_file_filesize"))
                {
                    return true;
                }
                else
                {
                    if (fromDL)
                    {
                        MessageBox.Show(notSyncWithUI);
                    }
                    FuncFiles.Delete(updateFolder + "file" + numberSelectFile + ".rar");
                }
            }
            else
            {
                if (fromDL)
                {
                    MessageBox.Show(noTools);
                }
            }
            return false;
        }
        private void unpackUpdates()
        {
            FuncMisc.UnPackRAR(updateFolder + "file" + numberSelectFile + ".rar");
            if (File.Exists(updateFolder + "Update_" + numberSelectFile + ".bat"))
            {
                Process.Start(updateFolder + "Update_" + numberSelectFile + ".bat");
            }
            FuncParser.iniWrite(FormMain.iniLauncher, "Updates", "Update_" + numberSelectFile + "_Version", FuncParser.stringRead(updateFolder + nameUpdateInfo, "Update_" + numberSelectFile, "update_file_version"));
            comboBox1_SelectedIndexChanged(this, new EventArgs());
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            numberSelectFile = realIndex[comboBox1.SelectedIndex];
            if (checkUpdateVersion(numberSelectFile))
            {
                updateInstall = true;
                comboBox1.SelectedIndexChanged -= comboBox1_SelectedIndexChanged;
                comboBox1.Items[comboBox1.SelectedIndex] = installedUpdate + FuncParser.stringRead(updateFolder + nameUpdateInfo, "Update_" + numberSelectFile, "update_file");
                comboBox1.SelectedIndexChanged += comboBox1_SelectedIndexChanged;
            }
            else
            {
                updateInstall = false;
            }
            EnableDisableButtons();
        }
        private bool checkUpdateVersion(int index)
        {
            double UpdateVersion1 = FuncParser.doubleRead(updateFolder + nameUpdateInfo, "Update_" + index, "update_file_version");
            if (UpdateVersion1 != -1)
            {
                double UpdateVersion2 = FuncParser.doubleRead(FormMain.iniLauncher, "Updates", "Update_" + index + "_Version");
                if (UpdateVersion1 != -1)
                {
                    if (UpdateVersion1 <= UpdateVersion2)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonClose_Click(object sender, EventArgs e)
        {
            client.DownloadProgressChanged -= new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
            client.DownloadFileCompleted -= new AsyncCompletedEventHandler(client_DownloadFileCompleted);
            client.CancelAsync();
            FuncFiles.Delete(updateFolder + nameControlPanel);
            FuncFiles.Delete(updateFolder + nameUpdateInfo);
            Dispose();
        }
    }
}