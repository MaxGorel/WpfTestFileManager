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
        public static bool DirectoryExists(string directory) => Directory.Exists(directory);


        public static bool GetFileDataFromDirectory(string directory, ObservableCollection<FileData> files)
        {
            files.Clear();

            try
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                foreach (var dir in di.GetDirectories()) //Поиск папок
                {
                    files.Add(new FileData()
                    {
                        Name = dir.Name,
                        Size = null,
                        Type = "Папка",
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
