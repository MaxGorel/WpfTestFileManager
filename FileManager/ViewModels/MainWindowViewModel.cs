using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using WpfTest.HelperClasses;
using WpfTest.Models;
using WpfTest.MVVM;

namespace WpfTest.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<FileData> Files { get; set; } = new();
        public ButtonCommand SearchCommand => new ButtonCommand(execute => SearchDirectory());
        public ButtonCommand DoubleClickTableCommand => new ButtonCommand(execute => DoubleClickOnTables());

        //
        // ----------- Текстовое поле ввода
        //
        private string searchString = string.Empty;
        private string lastSearchString = string.Empty;
        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; OnPropertyChanged(); }
        }
        // Эта мешанина нужна, чтобы текстовое поле в UI обновлялось, если мы изменили переменную searchString в коде 
        public event PropertyChangedEventHandler? PropertyChanged;  
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null) => 
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        //
        // ----------- Выбранная строка
        //
        private FileData selectedFileData = new() { Name = "", Type = "" };
        public FileData SelectedFileData
        {
            get { return selectedFileData; }
            set { selectedFileData = value; }
        }

        //
        // ----------- Функции
        //
        private void SearchDirectory() // Вызывается при нажатии кнопки
        {
            if (searchString == string.Empty || !FileDataManager.DirectoryExists(searchString))
            {
                MessageBox.Show("Введите корректный путь!");
                return;
            }
            else if (searchString == lastSearchString)
            {
                MessageBox.Show("Информация о файлах уже выгружена!");
                return;
            }

            Files.Clear(); //TODO: Эта строка учавствует в сортировке вместе со всеми, т.е. не всегда в начале списка
            Files.Add(new FileData() { Name = "...", Type = "Link" });

            if (!FileDataManager.GetFileDataFromDirectory(searchString, Files))
                MessageBox.Show("Ошибка!");

            lastSearchString = searchString;

        }

        private void DoubleClickOnTables()
        {
            if (selectedFileData == null) return;

            if (selectedFileData.Type == FileDataManager.FOLDER_TYPE_STRING)
                SearchString = FileDataManager.ChangeDirectoryPathString(searchString, selectedFileData.Name);
            else if (selectedFileData.Name == "...")
                SearchString = FileDataManager.ChangeDirectoryPathString(searchString, null);

                SearchDirectory();
        }

    }
}
