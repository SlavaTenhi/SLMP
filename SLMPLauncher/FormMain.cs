using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public partial class FormMain : Form
    {
        public static string launcherFolder = FuncFiles.PathAddSlash(AppDomain.CurrentDomain.BaseDirectory);
        public static string gameFolder = FuncFiles.PathAddSlash(Path.GetDirectoryName(Path.GetDirectoryName(launcherFolder)));
        public static string myDocPath = FuncFiles.PathAddSlash(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)) + @"My Games\Skyrim\";
        public static string iniSkyrim = myDocPath + @"Skyrim.ini";
        public static string iniSkyrimPrefs = myDocPath + @"SkyrimPrefs.ini";
        public static string appDataPath = FuncFiles.PathAddSlash(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)) + @"Skyrim\";
        public static string iniLauncher = launcherFolder + "SLMPLauncher.ini";
        public static string iniIgnoreFiles = launcherFolder + "SLMPIgnoreFiles.ini";
        public static string startWithGame = null;
        public static string waitBeforeStart = null;
        public static string langTranslate = "RU";
        public static int numberStyle = 1;
        string helpPath = gameFolder + @"_Programs\ProgramFiles\SLMP-GR Help.chm";
        string wryeBash = gameFolder + @"_Programs\ProgramFiles\Mopy\Wrye Bash.exe";
        string dsr = gameFolder + @"Data\SkyProc Patchers\Dual Sheath Redux Patch\Dual Sheath Redux Patch.jar";
        string fnis = gameFolder + @"Data\Tools\GenerateFNIS_for_Users\GenerateFNISforUsers.exe";
        string notFound = " не найден(а).";
        string confirmTitle = "Подтверждение";
        string settingsReset = "Сбросить настройки?";
        string notFoundTemplates = "Нет шаблонов конфигурационных файлов для сброса настроек.";
        string clearDirectory = "Очистить директорию?";
        string notInDirectory = "Выбран(ы) объект(ы) вне директории игры.";
        string noIniFound = "Файлы настроек Skyrim не сформированы. Сделайте сброс настроек.";
        string failWriteToRegistry = "Не удалось записать путь в реест.";
        bool moveWindow = false;
        bool windgetOpen = false;
        int mouseWindowX = 0;
        int mouseWindowY = 0;
        public Bitmap BMbuttonlogoGlow;
        public Bitmap BMbuttonlogo;
        public Bitmap BMbuttonlogoPressed;
        public Bitmap BMbuttonFullGlow;
        public Bitmap BMbuttonFull;
        public Bitmap BMbuttonFullPressed;
        public Bitmap BMbuttonClearGlow;
        public Bitmap BMbuttonClear;
        public Bitmap BMbuttonHalfGlow;
        public Bitmap BMbuttonHalf;
        public Bitmap BMbuttonHalfPressed;
        public Bitmap BMbuttonOneGlow;
        public Bitmap BMbuttonOne;
        public Bitmap BMBackgroundImage;
        FormWidget settingsWidget = null;

        public FormMain()
        {
            InitializeComponent();
            Directory.SetCurrentDirectory(FormMain.launcherFolder);
            if (File.Exists(iniLauncher))
            {
                if (FuncParser.keyExists(iniLauncher, "Position", "POS_WindowTop") && FuncParser.keyExists(iniLauncher, "Position", "POS_WindowLeft"))
                {
                    int Wleft = -1;
                    int Wtop = -1;
                    Wleft = FuncParser.intRead(iniLauncher, "Position", "POS_WindowLeft");
                    Wtop = FuncParser.intRead(iniLauncher, "Position", "POS_WindowTop");
                    if (Wleft > (Screen.PrimaryScreen.Bounds.Width - Size.Width))
                    {
                        Wleft = Screen.PrimaryScreen.Bounds.Width - Size.Width;
                    }
                    else if (Wleft < 0)
                    {
                        Wleft = 0;
                    }
                    if (Wtop > (Screen.PrimaryScreen.Bounds.Height - Size.Height))
                    {
                        Wtop = Screen.PrimaryScreen.Bounds.Height - Size.Height;
                    }
                    else if (Wtop < 0)
                    {
                        Wtop = 0;
                    }
                    StartPosition = FormStartPosition.Manual;
                    Location = new Point(Wleft, Wtop);
                }
                else
                {
                    StartPosition = FormStartPosition.CenterScreen;
                }
                if (FuncParser.keyExists(iniLauncher, "General", "Language"))
                {
                    if (FuncParser.stringRead(iniLauncher, "General", "Language") == "RU")
                    {
                        SetLangTranslateRU(true);
                    }
                    else
                    {
                        langTranslate = "EN";
                        SetLangTranslateEN();
                    }
                }
                string version = FuncParser.stringRead(iniLauncher, "General", "Version_CP");
                if (FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).ProductVersion != version)
                {
                    FuncParser.iniWrite(iniLauncher, "General", "Version_CP", FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).ProductVersion);
                }
                numberStyle = FuncParser.intRead(iniLauncher, "General", "NumberStyle");
                if (numberStyle < 1 && numberStyle > 2)
                {
                    numberStyle = 1;
                }
            }
            else
            {
                SetLangTranslateRU(true);
                StartPosition = FormStartPosition.CenterScreen;
                OnProcessExit(this, new EventArgs());
            }
            if (!File.Exists(iniSkyrimPrefs) || !File.Exists(iniSkyrim))
            {
                resetSettings();
            }
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnProcessExit);
            RefreshStyle();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void OnProcessExit(object sender, EventArgs e)
        {
            if (!File.Exists(iniLauncher))
            {
                StreamWriter sw = new StreamWriter(iniLauncher);
                sw.WriteLine("[General]");
                sw.WriteLine("Version_CP=" + FileVersionInfo.GetVersionInfo(Process.GetCurrentProcess().MainModule.FileName).ProductVersion);
                sw.WriteLine("HideWebButtons=false");
                sw.WriteLine("NumberStyle=" + numberStyle);
                sw.WriteLine("Language=" + langTranslate);
                sw.WriteLine("");
                sw.WriteLine("[Position]");
                sw.WriteLine("POS_WindowTop=" + Top);
                sw.WriteLine("POS_WindowLeft=" + Left);
                sw.WriteLine("");
                sw.WriteLine("[Updates]");
                sw.Close();
            }
            else
            {
                FuncParser.iniWrite(iniLauncher, "General", "NumberStyle", numberStyle.ToString());
                FuncParser.iniWrite(iniLauncher, "General", "Language", langTranslate);
                FuncParser.iniWrite(iniLauncher, "Position", "POS_WindowTop", Top.ToString());
                FuncParser.iniWrite(iniLauncher, "Position", "POS_WindowLeft", Left.ToString());
            }
            AppDomain.CurrentDomain.ProcessExit -= new EventHandler(OnProcessExit);
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonWryeBash_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (File.Exists(wryeBash))
            {
                buttonWryeBash.Enabled = false;
                buttonWryeBash.MouseEnter -= new EventHandler(buttonWryeBash_MouseEnter);
                buttonWryeBash.MouseLeave -= new EventHandler(buttonWryeBash_MouseLeave);
                buttonWryeBash.BackgroundImage = BMbuttonFullPressed;
                FuncMisc.RunProcess(wryeBash, null, WryeBashExited, this);
            }
            else
            {
                MessageBox.Show("Wrye Bash" + notFound);
            }
        }
        private void WryeBashExited(object sender, EventArgs e)
        {
            buttonWryeBash.Enabled = true;
            buttonWryeBash.MouseEnter += new EventHandler(buttonWryeBash_MouseEnter);
            buttonWryeBash.MouseLeave += new EventHandler(buttonWryeBash_MouseLeave);
            buttonWryeBash.BackgroundImage = BMbuttonFull;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonDSRStart_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (File.Exists(dsr))
            {
                buttonDSRStart.Enabled = false;
                buttonDSRStart.MouseEnter -= new EventHandler(buttonDSRStart_MouseEnter);
                buttonDSRStart.MouseLeave -= new EventHandler(buttonDSRStart_MouseLeave);
                buttonDSRStart.BackgroundImage = BMbuttonHalfPressed;
                FuncMisc.RunProcess(dsr, null, DSRExited, this);
            }
            else
            {
                MessageBox.Show("Dual Sheath Redux" + notFound);
            }
        }
        private void DSRExited(object sender, EventArgs e)
        {
            buttonDSRStart.Enabled = true;
            buttonDSRStart.MouseEnter += new EventHandler(buttonDSRStart_MouseEnter);
            buttonDSRStart.MouseLeave += new EventHandler(buttonDSRStart_MouseLeave);
            buttonDSRStart.BackgroundImage = BMbuttonHalf;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonFNIS_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (File.Exists(fnis))
            {
                FuncMisc.ToggleButtons(this, false);
                FuncMisc.UnPackRAR(launcherFolder + @"CPFiles\BackUp\FNIS.rar");
                FuncMisc.ToggleButtons(this, true);
                buttonFNISStart.Enabled = false;
                buttonFNISStart.MouseEnter -= new EventHandler(buttonFNISStart_MouseEnter);
                buttonFNISStart.MouseLeave -= new EventHandler(buttonFNISStart_MouseLeave);
                buttonFNISStart.BackgroundImage = BMbuttonHalfPressed;
                FuncMisc.RunProcess(fnis, null, FNISExited, this);
            }
            else
            {
                MessageBox.Show("FNIS" + notFound);
            }
        }
        private void FNISExited(object sender, EventArgs e)
        {
            buttonFNISStart.Enabled = true;
            buttonFNISStart.MouseEnter += new EventHandler(buttonFNISStart_MouseEnter);
            buttonFNISStart.MouseLeave += new EventHandler(buttonFNISStart_MouseLeave);
            buttonFNISStart.BackgroundImage = BMbuttonHalf;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonGameDirectory_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (Directory.Exists(gameFolder))
            {
                Process.Start(gameFolder);
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonProgrammsFolder_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (Directory.Exists(gameFolder + "_Programs"))
            {
                Process.Start(gameFolder + "_Programs");
            }
            else
            {
                MessageBox.Show("_Programs" + notFound);
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonResetSettings_Click(object sender, EventArgs e)
        {
            label1.Focus();
            DialogResult dialogResult = MessageBox.Show(settingsReset, confirmTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                resetSettings();
            }
        }
        public void resetSettings()
        {
            if (File.Exists(launcherFolder + "Skyrim.ini") && File.Exists(launcherFolder + "SkyrimPrefs.ini") && File.Exists(launcherFolder + @"MasterList\DLCList.txt") && File.Exists(launcherFolder + @"MasterList\plugins.txt"))
            {
                try
                {
                    RegistryKey key;
                    key = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Bethesda Softworks\Skyrim");
                    key.SetValue("Installed Path", gameFolder);
                    key.Close();
                }
                catch
                {
                    MessageBox.Show(failWriteToRegistry);
                }
                FuncFiles.Delete(myDocPath + "Skyrim.ini");
                FuncFiles.Delete(myDocPath + "SkyrimPrefs.ini");
                FuncFiles.Delete(myDocPath + "Logs");
                FuncFiles.Delete(myDocPath + "SKSE");
                FuncFiles.Delete(myDocPath + "SkyProc");
                FuncFiles.Delete(myDocPath + "BashSettings.dat");
                FuncFiles.Delete(myDocPath + "BashSettings.dat.bak");
                FuncFiles.Delete(myDocPath + "RendererInfo.txt");
                FuncFiles.Delete(myDocPath + @"Saves\Bash");
                FuncFiles.CreatDirectory(myDocPath);
                FuncFiles.CopyAnyFiles(launcherFolder + "Skyrim.ini", myDocPath + "Skyrim.ini");
                FuncFiles.CopyAnyFiles(launcherFolder + "SkyrimPrefs.ini", myDocPath + "SkyrimPrefs.ini");
                FuncFiles.CopyAnyFiles(launcherFolder + @"MasterList\BashSettings.dat", myDocPath + "BashSettings.dat");
                FuncFiles.Delete(appDataPath + @"DLCList.txt");
                FuncFiles.Delete(appDataPath + @"plugins.txt");
                FuncFiles.Delete(appDataPath + @"loadorder.txt");
                FuncFiles.CreatDirectory(appDataPath);
                FuncFiles.CopyAnyFiles(launcherFolder + @"MasterList\DLCList.txt", appDataPath + @"DLCList.txt");
                FuncFiles.CopyAnyFiles(launcherFolder + @"MasterList\plugins.txt", appDataPath + @"plugins.txt");
                FuncFiles.CopyAnyFiles(launcherFolder + @"MasterList\Plugins.tes5viewsettings", appDataPath + @"Plugins.tes5viewsettings");
                var form = new FormOptions();
                form.resetSettings();
                form.Dispose();
                form = null;
            }
            else
            {
                MessageBox.Show(notFoundTemplates);
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonClearDirectory_Click(object sender, EventArgs e)
        {
            label1.Focus();
            DialogResult dialogResult = MessageBox.Show(clearDirectory, confirmTitle, MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                FuncFiles.Delete(gameFolder + @"..\Skyrim Mods");
                FuncClear.Clear();
                FuncClear.EmptyFolder(gameFolder);
            }
        }
        private void buttonAddFileToIgnore_Click(object sender, EventArgs e)
        {
            label1.Focus();
            openFileDialog1.InitialDirectory = gameFolder;
            DialogResult result = openFileDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (openFileDialog1.FileName.Contains(gameFolder))
                {
                    foreach (string line in openFileDialog1.FileNames)
                    {
                        string FileName = line.Remove(0, gameFolder.Length);
                        File.AppendAllText(iniIgnoreFiles, FileName + Environment.NewLine);
                    }
                }
                else
                {
                    MessageBox.Show(notInDirectory);
                }
            }
        }
        private void buttonAddFolderToIgnore_Click(object sender, EventArgs e)
        {
            label1.Focus();
            folderBrowserDialog1.SelectedPath = gameFolder;
            DialogResult result = folderBrowserDialog1.ShowDialog();
            if (result == DialogResult.OK)
            {
                if (folderBrowserDialog1.SelectedPath.Contains(gameFolder))
                {
                    string DirName = folderBrowserDialog1.SelectedPath.Remove(0, gameFolder.Length);
                    File.AppendAllText(iniIgnoreFiles, DirName + Environment.NewLine);
                }
                else
                {
                    MessageBox.Show(notInDirectory);
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonInstRemPrograms_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (Directory.Exists(gameFolder + @"_Programs\ProgramFiles"))
            {
                var form = new FormPrograms();
                form.ShowDialog();
                form = null;
            }
            else
            {
                MessageBox.Show(@"_Programs\ProgramFiles" + notFound);
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonENBmenu_Click(object sender, EventArgs e)
        {
            label1.Focus();
            var form = new FormENB();
            form.ShowDialog();
            form = null;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonSkyrim_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (File.Exists(gameFolder + "TESV.exe"))
            {
                buttonSkyrim.Enabled = false;
                buttonSkyrim.MouseEnter -= new EventHandler(buttonSkyrim_MouseEnter);
                buttonSkyrim.MouseLeave -= new EventHandler(buttonSkyrim_MouseLeave);
                buttonSkyrim.BackgroundImage = BMbuttonlogoPressed;
                if (startWithGame != null)
                {
                    if (File.Exists(startWithGame))
                    {
                        Process.Start(startWithGame);
                    }
                }
                if (waitBeforeStart != null)
                {
                    int WaitTime = FuncParser.stringToInt(waitBeforeStart);
                    if (WaitTime > 0)
                    {
                        FuncMisc.ToggleButtons(this, false);
                        Thread.Sleep(WaitTime * 1000);
                        FuncMisc.ToggleButtons(this, true);
                        buttonSkyrim.Enabled = false;
                    }
                }
                FuncMisc.RunProcess(gameFolder + "TESV.exe", "-forcesteamloader", SKSEExited, this);
            }
            else
            {
                MessageBox.Show("TESV.exe" + notFound);
            }
        }
        private void SKSEExited(object sender, EventArgs e)
        {
            bool find = false;
            foreach (Process line in Process.GetProcessesByName("SKYRIM"))
            {
                line.EnableRaisingEvents = true;
                line.Exited += processGAMEExited;
                find = true;
            }
            if (!find)
            {
                processGAMEExited(this, new EventArgs());
            }
        }
        private void processGAMEExited(object sender, EventArgs e)
        {
            buttonSkyrim.Enabled = true;
            buttonSkyrim.MouseEnter += new EventHandler(buttonSkyrim_MouseEnter);
            buttonSkyrim.MouseLeave += new EventHandler(buttonSkyrim_MouseLeave);
            buttonSkyrim.BackgroundImage = BMbuttonlogo;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonMods_Click(object sender, EventArgs e)
        {
            label1.Focus();
            var form = new FormMods();
            form.ShowDialog();
            form = null;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonOptions_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (File.Exists(iniSkyrim) && File.Exists(iniSkyrimPrefs) && File.Exists(appDataPath + @"plugins.txt"))
            {
                var form = new FormOptions();
                form.ShowDialog();
                form = null;
            }
            else
            {
                MessageBox.Show(noIniFound);
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (File.Exists(helpPath))
            {
                Process.Start(helpPath);
            }
            else
            {
                MessageBox.Show("SLMP-GR Help.chm" + notFound);
            }
        }
        private void buttonWidget_Click(object sender, EventArgs e)
        {
            label1.Focus();
            if (!windgetOpen)
            {
                windgetOpen = true;
                settingsWidget = new FormWidget();
                settingsWidget.DesktopLocation = new Point(Left, Top - settingsWidget.Size.Height);
                settingsWidget.Show(this);
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidgetPressed;
            }
            else
            {
                windgetOpen = false;
                settingsWidget.Dispose();
                settingsWidget = null;
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidget;
            }
        }
        private void buttonMinimize_Click(object sender, EventArgs e)
        {
            label1.Focus();
            WindowState = FormWindowState.Minimized;
            if (windgetOpen)
            {
                windgetOpen = false;
                settingsWidget.Dispose();
                settingsWidget = null;
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidget;
            }
        }
        private void buttonClose_Click(object sender, EventArgs e)
        {
            label1.Focus();
            Application.Exit();
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void MainForm_MouseDown(object sender, MouseEventArgs e)
        {
            moveWindow = true;
            mouseWindowX = e.X;
            mouseWindowY = e.Y;
        }
        private void MainForm_MouseUp(object sender, MouseEventArgs e)
        {
            if (moveWindow)
            {
                moveWindow = false;
            }
        }
        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (moveWindow)
            {
                Location = new Point(Cursor.Position.X - mouseWindowX, Cursor.Position.Y - mouseWindowY);
                if (windgetOpen)
                {
                    settingsWidget.Location = new Point(Left, Top - settingsWidget.Size.Height);
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        public void SetLangTranslateRU(bool fromStart)
        {
            if (!fromStart)
            {
                buttonResetSettings.Text = "Сброс Настроек";
                buttonClearDirectory.Text = "Очистка";
                buttonOptions.Text = "Настройки Игры";
                buttonInstRemPrograms.Text = "Программы";
                buttonProgrammsFolder.Text = "Все Программы";
                buttonMods.Text = "Моды";
                buttonGameDirectory.Text = "Директория Игры";
                buttonENBmenu.Text = "ENB Меню";
                notFound = " не найден(а).";
                confirmTitle = "Подтверждение";
                settingsReset = "Сбросить настройки?";
                notFoundTemplates = "Нет шаблонов конфигурационных файлов для сброса настроек.";
                clearDirectory = "Очистить директорию?";
                notInDirectory = "Выбран(ы) объект(ы) вне директории игры.";
                noIniFound = "Файлы настроек Skyrim не сформированы. Сделайте сброс настроек.";
                failWriteToRegistry = "Не удалось записать путь в реест.";
            }
            toolTip1.SetToolTip(buttonMods, "Установка опциональных модов.");
            toolTip1.SetToolTip(buttonWryeBash, "Сортировщик модов. Моды имеющие конфликты будут красными.");
            toolTip1.SetToolTip(buttonDSRStart, "Патчер Dual Sheath Redux. Применять после изменения модов содержащих оружие.");
            toolTip1.SetToolTip(buttonFNISStart, "Патчер FNIS. Применять после изменения модов содержащих анимации.");
            toolTip1.SetToolTip(buttonGameDirectory, "Открывает папку-директорию игры.");
            toolTip1.SetToolTip(buttonResetSettings, "Полный сброс настроек игры и восстановление последовательности модов.");
            toolTip1.SetToolTip(buttonClearDirectory, "Удаляет \"чужие\" файлы. В т.ч. распакованные программы.");
            toolTip1.SetToolTip(buttonInstRemPrograms, "Распаковка различных программ для редактирования игры.");
            toolTip1.SetToolTip(buttonENBmenu, "Меню управления ENB с выбором различных пресетов.");
            toolTip1.SetToolTip(buttonProgrammsFolder, "Открывает папку с ярлыками программ для редактирования игры.");
            toolTip1.SetToolTip(buttonSkyrim, "Запустить игру.");
            toolTip1.SetToolTip(buttonAddFolderToIgnore, "Добавление папки в шаблон игнор листа.");
            toolTip1.SetToolTip(buttonAddFileToIgnore, "Добавление файла(ов) в шаблон игнор листа.");
            toolTip1.SetToolTip(buttonOptions, "Настройка конфигурации, параметров игры, управление подключаемыми файлами.");
        }
        public void SetLangTranslateEN()
        {
            buttonResetSettings.Text = "Reset Settings";
            buttonClearDirectory.Text = "Clear";
            buttonOptions.Text = "Game Settings";
            buttonInstRemPrograms.Text = "Programs";
            buttonProgrammsFolder.Text = "All Programs";
            buttonMods.Text = "Mods";
            buttonGameDirectory.Text = "Game Directory";
            buttonENBmenu.Text = "ENB Menu";
            notFound = " not found.";
            confirmTitle = "Confirm";
            settingsReset = "Reset settings?";
            notFoundTemplates = "No configuration file templates for resetting settings.";
            clearDirectory = "Clear directory?";
            notInDirectory = "Selected object(s) outside the game directory.";
            noIniFound = "Skyrim configuration files are not generated. Reset the settings.";
            failWriteToRegistry = "Could not write path to the registry.";
            toolTip1.SetToolTip(buttonMods, "Installing optional mods.");
            toolTip1.SetToolTip(buttonWryeBash, "Mods sorter. Mods having conflicts will be red.");
            toolTip1.SetToolTip(buttonDSRStart, "Patcher Dual Sheath Redux. Apply after the change in the mods containing the weapons.");
            toolTip1.SetToolTip(buttonFNISStart, "Patcher FNIS. Apply after the change in the mods containing the animation.");
            toolTip1.SetToolTip(buttonGameDirectory, "Opens folder-directory of the game.");
            toolTip1.SetToolTip(buttonResetSettings, "Full reset of game settings and recovery of a sequence of mods.");
            toolTip1.SetToolTip(buttonClearDirectory, "Delete \"strangers\" files. Including unpacked programs.");
            toolTip1.SetToolTip(buttonInstRemPrograms, "Unpacking various programs for editing games.");
            toolTip1.SetToolTip(buttonENBmenu, "The ENB control menu with a selection of different presets.");
            toolTip1.SetToolTip(buttonProgrammsFolder, "Opens a folder with shortcuts for editing games.");
            toolTip1.SetToolTip(buttonSkyrim, "Start the game.");
            toolTip1.SetToolTip(buttonAddFolderToIgnore, "Adding a folder to the ignore list template.");
            toolTip1.SetToolTip(buttonAddFileToIgnore, "Adding a file(s) to the ignore list template.");
            toolTip1.SetToolTip(buttonOptions, "Configuring the configuration, game settings, managing the connected files.");
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        public void RefreshStyle()
        {
            if (FormMain.numberStyle == 1)
            {
                BMbuttonlogoGlow = Properties.Resources._1_buttonlogoGlow;
                BMbuttonlogo = Properties.Resources._1_buttonlogo;
                BMbuttonlogoPressed = Properties.Resources._1_buttonlogoPressed;
                BMbuttonFullGlow = Properties.Resources._1_buttonFullGlow;
                BMbuttonFull = Properties.Resources._1_buttonFull;
                BMbuttonFullPressed = Properties.Resources._1_buttonFullPressed;
                BMbuttonClearGlow = Properties.Resources._1_buttonClearGlow;
                BMbuttonClear = Properties.Resources._1_buttonClear;
                BMbuttonHalfGlow = Properties.Resources._1_buttonHalfGlow;
                BMbuttonHalf = Properties.Resources._1_buttonHalf;
                BMbuttonHalfPressed = Properties.Resources._1_buttonHalfPressed;
                BMbuttonOneGlow = Properties.Resources._1_buttonOneGlow;
                BMbuttonOne = Properties.Resources._1_buttonOne;
                BMBackgroundImage = Properties.Resources._1_MainForm;
                FuncMisc.LabelsTextColor(this, System.Drawing.SystemColors.ControlText, System.Drawing.Color.FromArgb(232, 232, 232), true);
            }
            else
            {
                BMbuttonlogoGlow = Properties.Resources._2_buttonlogoGlow;
                BMbuttonlogo = Properties.Resources._2_buttonlogo;
                BMbuttonlogoPressed = Properties.Resources._2_buttonlogoPressed;
                BMbuttonFullGlow = Properties.Resources._2_buttonFullGlow;
                BMbuttonFull = Properties.Resources._2_buttonFull;
                BMbuttonFullPressed = Properties.Resources._2_buttonFullPressed;
                BMbuttonClearGlow = Properties.Resources._2_buttonClearGlow;
                BMbuttonClear = Properties.Resources._2_buttonClear;
                BMbuttonHalfGlow = Properties.Resources._2_buttonHalfGlow;
                BMbuttonHalf = Properties.Resources._2_buttonHalf;
                BMbuttonHalfPressed = Properties.Resources._2_buttonHalfPressed;
                BMbuttonOneGlow = Properties.Resources._2_buttonOneGlow;
                BMbuttonOne = Properties.Resources._2_buttonOne;
                BMBackgroundImage = Properties.Resources._2_MainForm;
                FuncMisc.LabelsTextColor(this, System.Drawing.SystemColors.ControlLight, System.Drawing.Color.FromArgb(30, 30, 30), true);
            }
            if (buttonSkyrim.Enabled == true)
            {
                buttonSkyrim.BackgroundImage = BMbuttonlogo;
            }
            else
            {
                buttonSkyrim.BackgroundImage = BMbuttonlogoPressed;
            }
            if (buttonWryeBash.Enabled == true)
            {
                buttonWryeBash.BackgroundImage = BMbuttonFull;
            }
            else
            {
                buttonWryeBash.BackgroundImage = BMbuttonFullPressed;
            }
            if (buttonDSRStart.Enabled == true)
            {
                buttonDSRStart.BackgroundImage = BMbuttonHalf;
            }
            else
            {
                buttonDSRStart.BackgroundImage = BMbuttonHalfPressed;
            }
            if (buttonFNISStart.Enabled == true)
            {
                buttonFNISStart.BackgroundImage = BMbuttonHalf;
            }
            else
            {
                buttonFNISStart.BackgroundImage = BMbuttonHalfPressed;
            }
            buttonProgrammsFolder.BackgroundImage = BMbuttonFull;
            buttonGameDirectory.BackgroundImage = BMbuttonFull;
            buttonResetSettings.BackgroundImage = BMbuttonFull;
            buttonClearDirectory.BackgroundImage = BMbuttonClear;
            buttonAddFolderToIgnore.BackgroundImage = BMbuttonOne;
            buttonAddFileToIgnore.BackgroundImage = BMbuttonOne;
            buttonMods.BackgroundImage = BMbuttonFull;
            buttonInstRemPrograms.BackgroundImage = BMbuttonFull;
            buttonENBmenu.BackgroundImage = BMbuttonFull;
            buttonOptions.BackgroundImage = BMbuttonFull;
            BackgroundImage = BMBackgroundImage;
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        private void buttonSkyrim_MouseEnter(object sender, EventArgs e)
        {
            buttonSkyrim.BackgroundImage = BMbuttonlogoGlow;
        }
        private void buttonSkyrim_MouseLeave(object sender, EventArgs e)
        {
            buttonSkyrim.BackgroundImage = BMbuttonlogo;
        }

        private void buttonWryeBash_MouseEnter(object sender, EventArgs e)
        {
            buttonWryeBash.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonWryeBash_MouseLeave(object sender, EventArgs e)
        {
            buttonWryeBash.BackgroundImage = BMbuttonFull;
        }

        private void buttonDSRStart_MouseEnter(object sender, EventArgs e)
        {
            buttonDSRStart.BackgroundImage = BMbuttonHalfGlow;
        }
        private void buttonDSRStart_MouseLeave(object sender, EventArgs e)
        {
            buttonDSRStart.BackgroundImage = BMbuttonHalf;
        }

        private void buttonFNISStart_MouseEnter(object sender, EventArgs e)
        {
            buttonFNISStart.BackgroundImage = BMbuttonHalfGlow;
        }
        private void buttonFNISStart_MouseLeave(object sender, EventArgs e)
        {
            buttonFNISStart.BackgroundImage = BMbuttonHalf;
        }

        private void buttonProgrammsFolder_MouseEnter(object sender, EventArgs e)
        {
            buttonProgrammsFolder.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonProgrammsFolder_MouseLeave(object sender, EventArgs e)
        {
            buttonProgrammsFolder.BackgroundImage = BMbuttonFull;
        }

        private void buttonGameDirectory_MouseEnter(object sender, EventArgs e)
        {
            buttonGameDirectory.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonGameDirectory_MouseLeave(object sender, EventArgs e)
        {
            buttonGameDirectory.BackgroundImage = BMbuttonFull;
        }

        private void buttonResetSettings_MouseEnter(object sender, EventArgs e)
        {
            buttonResetSettings.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonResetSettings_MouseLeave(object sender, EventArgs e)
        {
            buttonResetSettings.BackgroundImage = BMbuttonFull;
        }

        private void buttonClearDirectory_MouseEnter(object sender, EventArgs e)
        {
            buttonClearDirectory.BackgroundImage = BMbuttonClearGlow;
        }
        private void buttonClearDirectory_MouseLeave(object sender, EventArgs e)
        {
            buttonClearDirectory.BackgroundImage = BMbuttonClear;
        }

        private void buttonAddFolderToIgnore_MouseEnter(object sender, EventArgs e)
        {
            buttonAddFolderToIgnore.BackgroundImage = BMbuttonOneGlow;
        }
        private void buttonAddFolderToIgnore_MouseLeave(object sender, EventArgs e)
        {
            buttonAddFolderToIgnore.BackgroundImage = BMbuttonOne;
        }

        private void buttonAddFileToIgnore_MouseEnter(object sender, EventArgs e)
        {
            buttonAddFileToIgnore.BackgroundImage = BMbuttonOneGlow;
        }
        private void buttonAddFileToIgnore_MouseLeave(object sender, EventArgs e)
        {
            buttonAddFileToIgnore.BackgroundImage = BMbuttonOne;
        }

        private void buttonMods_MouseEnter(object sender, EventArgs e)
        {
            buttonMods.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonMods_MouseLeave(object sender, EventArgs e)
        {
            buttonMods.BackgroundImage = BMbuttonFull;
        }

        private void buttonInstRemPrograms_MouseEnter(object sender, EventArgs e)
        {
            buttonInstRemPrograms.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonInstRemPrograms_MouseLeave(object sender, EventArgs e)
        {
            buttonInstRemPrograms.BackgroundImage = BMbuttonFull;
        }

        private void buttonENBmenu_MouseEnter(object sender, EventArgs e)
        {
            buttonENBmenu.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonENBmenu_MouseLeave(object sender, EventArgs e)
        {
            buttonENBmenu.BackgroundImage = BMbuttonFull;
        }

        private void buttonOptions_MouseEnter(object sender, EventArgs e)
        {
            buttonOptions.BackgroundImage = BMbuttonFullGlow;
        }
        private void buttonOptions_MouseLeave(object sender, EventArgs e)
        {
            buttonOptions.BackgroundImage = BMbuttonFull;
        }

        private void buttonHelp_MouseEnter(object sender, EventArgs e)
        {
            buttonHelp.BackgroundImage = Properties.Resources.buttonHelpGlow;
        }
        private void buttonHelp_MouseLeave(object sender, EventArgs e)
        {
            buttonHelp.BackgroundImage = Properties.Resources.buttonHelp;
        }

        private void buttonWidget_MouseEnter(object sender, EventArgs e)
        {
            if (windgetOpen)
            {
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidgetPressed;
            }
            else
            {
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidgetGlow;
            }
        }
        private void buttonWidget_MouseLeave(object sender, EventArgs e)
        {
            if (windgetOpen)
            {
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidgetPressed;
            }
            else
            {
                buttonWidget.BackgroundImage = Properties.Resources.buttonWidget;
            }
        }

        private void buttonMinimize_MouseEnter(object sender, EventArgs e)
        {
            buttonMinimize.BackgroundImage = Properties.Resources.buttonMinimizeGlow;
        }
        private void buttonMinimize_MouseLeave(object sender, EventArgs e)
        {
            buttonMinimize.BackgroundImage = Properties.Resources.buttonMinimize;
        }

        private void buttonClose_MouseEnter(object sender, EventArgs e)
        {
            buttonClose.BackgroundImage = Properties.Resources.buttonCloseGlow;
        }
        private void buttonClose_MouseLeave(object sender, EventArgs e)
        {
            buttonClose.BackgroundImage = Properties.Resources.buttonClose;
        }
    }
}