using System.IO;
using System.Windows.Forms;

namespace SLMPLauncher
{
    public static class FuncFiles
    {
        public static void Delete(string path)
        {
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    if (FormMain.langTranslate != "RU")
                    {
                        MessageBox.Show("Could not delete file: " + path);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить файл: " + path);
                    }
                }
            }
            else if (Directory.Exists(path))
            {
                try
                {
                    Directory.Delete(path, true);
                }
                catch
                {
                    if (FormMain.langTranslate != "RU")
                    {
                        MessageBox.Show("Could not delete folder: " + path);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось удалить папку: " + path);
                    }
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        public static void MoveAnyFiles(string from, string to)
        {
            if (File.Exists(from))
            {
                if (!File.Exists(to))
                {
                    CreatDirectory(Path.GetDirectoryName(to));
                    try
                    {
                        File.Move(from, to);
                    }
                    catch
                    {
                        if (FormMain.langTranslate != "RU")
                        {
                            MessageBox.Show("Failed to move: " + from);
                        }
                        else
                        {
                            MessageBox.Show("Не удалось переместить: " + from);
                        }
                    }
                }
                else
                {
                    if (FormMain.langTranslate != "RU")
                    {
                        MessageBox.Show("File already exists: " + to);
                    }
                    else
                    {
                        MessageBox.Show("Файл уже существует: " + to);
                    }
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        public static void CopyAnyFiles(string from, string to)
        {
            if (File.Exists(from))
            {
                try
                {
                    File.Copy(from, to, true);
                }
                catch
                {
                    if (FormMain.langTranslate != "RU")
                    {
                        MessageBox.Show("Could not copy: " + from);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось скопировать: " + from);
                    }
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        public static void CreatDirectory(string dir)
        {
            if (!Directory.Exists(dir))
            {
                try
                {
                    Directory.CreateDirectory(dir);
                }
                catch
                {
                    if (FormMain.langTranslate != "RU")
                    {
                        MessageBox.Show("Could not create folder: " + dir);
                    }
                    else
                    {
                        MessageBox.Show("Не удалось создать папку: " + dir);
                    }
                }
            }
        }
        //////////////////////////////////////////////////////ГРАНИЦА ФУНКЦИИ//////////////////////////////////////////////////////////////
        public static string PathAddSlash(string path)
        {
            if (path.EndsWith(@"/") || path.EndsWith(@"\"))
            {
                return path;
            }
            if (path.Contains(@"/"))
            {
                return path + @"/";
            }
            if (path.Contains(@"\"))
            {
                return path + @"\";
            }
            return path;
        }
    }
}