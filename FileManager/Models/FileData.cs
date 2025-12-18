using System;

namespace WpfTest.Models
{
    class FileData
    {
        public required string Name { get; set; }
        public DateTime? ChangedTime { get; set; }
        public string? Type { get; set; }
        public string? Size { get; set; }


        public readonly static FileData LinkToBackFileData = new() { Name = "..."};
    }
}
