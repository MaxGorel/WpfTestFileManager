using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using WpfTest.Models;

namespace WpfTest.HelperClasses
{
    internal static class FileDataManager
    {
        public static readonly string FOLDER_TYPE_STRING = "Папка";
        public static bool DirectoryExists(string directory) => Directory.Exists(directory);

        public static bool GetFileDataFromDirectory(string directory, ObservableCollection<FileData> files)
        {
            try
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                foreach (var dir in di.GetDirectories()) //Поиск папок
                {
                    files.Add(new FileData()
                    {
                        Name = dir.Name,
                        Size = null,
                        Type = FOLDER_TYPE_STRING,
                        ChangedTime = dir.LastWriteTime,

                    });
                }

                foreach (var file in di.GetFiles()) //Поиск файлов
                {
                    files.Add(new FileData()
                    {
                        Name = file.Name,
                        Size = FormatFileSize(file.Length),
                        Type = file.Extension,
                        ChangedTime = file.LastWriteTime,
                    });
                }
            }
            catch (Exception e) { Debug.Print("Ошибка: {}", e.Message); return false; }

            return true;
        }

        /// <summary>
        /// Добавляет к пути новую директорию.
        /// </summary>
        /// <param name="oldDirectory">Значение текущего положения</param>
        /// <param name="addition">Если null, то движение идет назад (закрываем текущую папку)</param>
        /// <returns></returns>
        public static string ChangeDirectoryPathString(string oldDirectory, string? addition)
        {
            string res = oldDirectory;
            if (addition != null) // Движение вперед
            {
                if (oldDirectory[oldDirectory.Length - 1] != '/')
                    res += '/';

                return res + addition;
            }

            // Движение назад

            if (oldDirectory[^1] == '/' || oldDirectory[^1] == '\\')
                res = res[..^1];
            
            int indexSlash = res.LastIndexOf('/');
            int indexReversedSlash = res.LastIndexOf('\\');
            int i = int.Max(indexSlash, indexReversedSlash); // Подбор последнего слеша (либо / либо \)
            
            return res[..(i + 1)];
        }

        private static string FormatFileSize(long fileSize)
        {
            string[] suffixes = { "байт", "KB", "MB", "GB", "TB", "PB" };
            int i;
            for (i = 0; i < suffixes.Length; i++) // Подбираем "суффикс" в зависимости от деления на 1024
            {
                if (fileSize <= Math.Pow(1024, i + 1)) break;
            }

            double value = fileSize / Math.Pow(1024, i);
            
            // Округление до большего числа (как в "проводнике")
            return double.Ceiling(value).ToString("0") + " " + suffixes[i];
        }

    }
}
