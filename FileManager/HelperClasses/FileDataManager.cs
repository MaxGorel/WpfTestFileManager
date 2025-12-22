using FileManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using static System.Net.WebRequestMethods;

namespace FileManager.HelperClasses
{
    internal static class FileDataManager
    {
        public static readonly string STRING_TYPE_FOLDER = "Папка";
        public static readonly string STRING_TYPE_DRIVE = "Диск";
        public static bool DirectoryExists(string directory) => Directory.Exists(directory);

        public static bool GetFileDataFromDirectory(string directory, ObservableCollection<FileData> files)
        {
            files.Clear();
            try
            {
                DirectoryInfo di = new DirectoryInfo(directory);

                foreach (var dir in di.GetDirectories()) //Поиск папок
                    files.Add(new FileData()
                    {
                        Name = dir.Name,
                        Size = FormatFileSize(GetDirectorySize(ChangeDirectoryPathString(directory, dir.Name))),
                        Type = STRING_TYPE_FOLDER,
                        ChangedTime = dir.LastWriteTime,

                    });

                foreach (var file in di.GetFiles()) //Поиск файлов
                    files.Add(new FileData()
                    {
                        Name = file.Name,
                        Size = FormatFileSize(file.Length),
                        Type = file.Extension,
                        ChangedTime = file.LastWriteTime,
                    });
            }
            catch (Exception e) { Debug.Print("Ошибка: {0}", e.Message); return false; }

            return true;
        }

        public static bool GetFileDataFromDisks(ObservableCollection<FileData> files)
        {
            try
            {
                foreach (var drive in DriveInfo.GetDrives()) //Поиск дисков
                    files.Add(new FileData()
                    {
                        Name = drive.Name,
                        Size = FormatFileSize(drive.TotalSize, false),
                        Type = STRING_TYPE_DRIVE,
                        ChangedTime = null,

                    });
            }
            catch (Exception e) { Debug.Print("Ошибка: {0}", e.Message); return false; }

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
            if (oldDirectory == string.Empty) return string.Empty;

            string res = oldDirectory;
            if (addition != null) // Движение вперед
            {
                if (oldDirectory[^1] != '/' && oldDirectory[^1] != '\\')
                    res += '\\';

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

        private static long? GetDirectorySize(string directory)
        {
            DirectoryInfo di = new DirectoryInfo(directory);
            long? size;
            try
            {
                size = di.EnumerateFiles("*", SearchOption.TopDirectoryOnly).Sum(f => f.Length);
            }
            catch (Exception) { return null; }
            return size;
        }


        private static string FormatFileSize(long? fileSize, bool ceiling = true)
        {
            if (fileSize == null) return "-";

            string[] suffixes = { "байт", "KB", "MB", "GB", "TB", "PB" };
            int i;
            for (i = 0; i < suffixes.Length; i++) // Подбираем "суффикс" в зависимости от деления на 1024
                if (fileSize <= Math.Pow(1024, i + 1)) break;

            double value = (double)fileSize / Math.Pow(1024, i);
            
            if (ceiling) // Округление до большего числа (как в "проводнике")
                return double.Ceiling(value).ToString("0") + " " + suffixes[i];
            else        // Округление - для дисков
                return double.Floor(value).ToString("0") + " " + suffixes[i];
        }

    }
}
