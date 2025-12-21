using FileManager.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;
using System.Threading.Tasks;
using System.Windows;
using System.Xml;

namespace FileManager.HelperClasses
{
    internal static class SnapshotService
    {
        public static void Save(IEnumerable<FileData> fileData)
        {
            SaveFileDialog saveDialog = new();
            saveDialog.Filter = "Файлы xml|*.xml|Файлы json|*.json";
            if (saveDialog.ShowDialog() == true)
            {
                string filename = saveDialog.FileName;

                if (filename.EndsWith(".xml"))
                {
                    SaveToXML(filename, fileData);
                }
                else if (filename.EndsWith(".json"))
                {
                    SaveToJSON(filename, fileData);
                }
                else MessageBox.Show("Сохранение в файл данного типа не поддерживаается");
            }
        }

        public static List<FileData>? Load()
        {
            List<FileData>? fileList = null;

            OpenFileDialog openDialog = new();
            if (openDialog.ShowDialog() == true)
            {
                string filePath = openDialog.FileName;

                if (filePath.EndsWith(".xml"))
                {
                    fileList = LoadFromXML(filePath);
                }
                else if (filePath.EndsWith(".json"))
                {
                    fileList = LoadFromJSON(filePath);
                }
                else MessageBox.Show("Загрузка файла данного типа не поддерживаается");
            }
            return fileList;
        }



        private static void SaveToXML(string filename, IEnumerable<FileData> files)
        {
            XmlDocument xml = new();
            XmlDeclaration decl = xml.CreateXmlDeclaration("1.0", "utf-8", null);
            xml.AppendChild(decl);
            
            XmlElement root = xml.CreateElement("snapshot");
            xml.AppendChild(root);

            foreach (FileData file in files)
            {
                XmlElement fileElement = xml.CreateElement("FileData");
                fileElement.SetAttribute("name", file.Name);
                fileElement.SetAttribute("type", file.Type);
                fileElement.SetAttribute("size", file.Size);

                string? changedTimeText = file.ChangedTime?.ToString("dd.MM.yy HH:mm");
                fileElement.SetAttribute("changedTime", changedTimeText);

                root.AppendChild(fileElement);
            }

            xml.Save(filename);
        }
        private static void SaveToJSON(string filename, IEnumerable<FileData> fileData)
        {
            using (FileStream fs = new(filename, FileMode.OpenOrCreate))
            {
                JsonSerializerOptions options1 = new()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                };
                JsonSerializer.Serialize(fs, fileData, options1);
            }
        }

        private static List<FileData>? LoadFromXML(string filename)
        {
            List<FileData> fileList = new(10);

            XmlDocument xml = new();
            xml.Load(filename);

            if (xml.DocumentElement == null) return null;

            XmlElement snapshot = xml.DocumentElement;
            if (snapshot.Name != "snapshot") return null;

            try
            {
                foreach (XmlElement file in snapshot)
                    fileList.Add(new FileData()
                    {
                        Name = file.GetAttribute("name"),
                        Type = file.GetAttribute("type"),
                        Size = file.GetAttribute("size"),
                        ChangedTime = DateTime.ParseExact(file.GetAttribute("changedTime"), "dd.MM.yy HH:mm", CultureInfo.InvariantCulture),
                    });
            }
            catch (Exception e) { Debug.Print("Ошибка загрузки файла: {0}", e.Message); return null; }

            return fileList;
        }
        private static List<FileData>? LoadFromJSON(string filename)
        {
            List<FileData>? fileList = new(10);

            using (FileStream fs = new FileStream(filename, FileMode.Open))
            {
                JsonSerializerOptions options1 = new()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true
                };
                IEnumerable<FileData>? data = JsonSerializer.Deserialize<IEnumerable<FileData>>(fs);
                if (data == null) return null;
                foreach (var file in data) fileList.Add(file);
            }

            return fileList;
        }
    }
}
