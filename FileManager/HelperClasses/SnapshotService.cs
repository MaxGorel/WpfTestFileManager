using FileManager.Models;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Unicode;
using System.Windows;
using System.Xml;

namespace FileManager.HelperClasses
{
    internal static class SnapshotService
    {
        // Все комментарии оставлены на случай, если понадобится сохранять время как dd.MM.yy HH:mm (для читаемости в сохраненном файле)

        //private class CustomDateTimeConverter : JsonConverter<DateTime>
        //{
        //    private readonly string Format;
        //    public CustomDateTimeConverter(string format) => Format = format;
        //    public override void Write(Utf8JsonWriter writer, DateTime date, JsonSerializerOptions options) =>
        //        writer.WriteStringValue(date.ToString(Format));
        //    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        //        DateTime.ParseExact(reader.GetString(), Format, null);
        //}
        //static CustomDateTimeConverter JSONdateTimeConverter = new("dd.MM.yy HH:mm");

        public static void Save(IEnumerable<FileData> fileData)
        {
            SaveFileDialog saveDialog = new();
            saveDialog.Filter = "Файлы xml|*.xml|Файлы json|*.json";
            if (saveDialog.ShowDialog() == true)
            {
                if (saveDialog.FileName.EndsWith(".xml"))
                {
                    SaveToXML(saveDialog.FileName, fileData);
                }
                else if (saveDialog.FileName.EndsWith(".json"))
                {
                    SaveToJSON(saveDialog.FileName, fileData);
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
                if (openDialog.FileName.EndsWith(".xml"))
                {
                    fileList = LoadFromXML(openDialog.FileName);
                }
                else if (openDialog.FileName.EndsWith(".json"))
                {
                    fileList = LoadFromJSON(openDialog.FileName);
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
                
                string? changedTimeText = file.ChangedTime.ToString();
                //string? changedTimeText = file.ChangedTime?.ToString("dd.MM.yy HH:mm");
                fileElement.SetAttribute("changedTime", changedTimeText);

                root.AppendChild(fileElement);
            }

            xml.Save(filename);
        }
        private static void SaveToJSON(string filename, IEnumerable<FileData> fileData)
        {
            using (FileStream fs = new(filename, FileMode.Truncate))
            {
                JsonSerializerOptions options = new()
                {
                    Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                    WriteIndented = true,
                };
                //options.Converters.Add(JSONdateTimeConverter);
                JsonSerializer.Serialize(fs, fileData, options);
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
                        //ChangedTime = DateTime.ParseExact(file.GetAttribute("changedTime"), "dd.MM.yy HH:mm", CultureInfo.InvariantCulture)
                        ChangedTime = DateTime.Parse(file.GetAttribute("changedTime"))
                    });
            }
            catch (Exception e) { Debug.Print("Ошибка загрузки файла: {0}", e.Message); return null; }

            return fileList;
        }
        private static List<FileData>? LoadFromJSON(string filename)
        {
            List<FileData>? fileList = new(10);

            try
            {
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    JsonSerializerOptions options = new()
                    {
                        Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic),
                        WriteIndented = true
                    };
                    //options.Converters.Add(JSONdateTimeConverter);
                    IEnumerable<FileData>? data = JsonSerializer.Deserialize<IEnumerable<FileData>>(fs);
                    if (data == null) return null;
                    foreach (var file in data) fileList.Add(file);
                }
            }
            catch (Exception e) { Debug.Print("Ошибка загрузки файла: {0}", e.Message); return null; }

            return fileList;
        }
    }
}
