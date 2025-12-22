using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using FileManager.HelperClasses;
using FileManager.Models;
using FileManager.MVVM;

namespace FileManager.ViewModels
{
    class MainWindowViewModel : INotifyPropertyChanged
    {
        private const string SEARCH_STRING_DRIVES = "drives";
        public ObservableCollection<FileData> Files { get; set; } = new();
        public ObservableCollection<FileData> BackLink { get; set; } = new();
        public RelayCommand SearchCommand => new(execute => SearchDirectory());
        public RelayCommand SaveCommand => new(execute => SaveSnapshot());
        public RelayCommand LoadCommand => new(execute => LoadSnapshot());
        public RelayCommand DoubleClickTableCommand => new(execute => DoubleClickOnTables());
        public RelayCommand DoubleClickBackLinkCommand => new(execute => DoubleClickOnBackLink());

        public MainWindowViewModel()
        {
            BackLink.Add(FileData.LinkToBackFileData);
        }

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
        private void SearchDirectory()
        {
            if (searchString == string.Empty || 
                (searchString != SEARCH_STRING_DRIVES && !FileDataManager.DirectoryExists(searchString)))
            {
                MessageBox.Show("Введите корректный путь!");
                return;
            }
            else if (searchString == lastSearchString)
            {
                MessageBox.Show("Информация о файлах уже выгружена!");
                return;
            }

            Files.Clear();
            if (searchString == SEARCH_STRING_DRIVES)
                FileDataManager.GetFileDataFromDisks(Files);
            else if (!FileDataManager.GetFileDataFromDirectory(searchString, Files))
                MessageBox.Show("Ошибка!");

            lastSearchString = searchString;
        }

        private void SaveSnapshot()
        {
            SnapshotService.Save(Files);
        }

        private void LoadSnapshot()
        {
            var fileList = SnapshotService.Load();
            Files.Clear();
            if (fileList != null) foreach (var file in fileList) Files.Add(file);
        }
        private void DoubleClickOnBackLink()
        {
            SearchString = FileDataManager.ChangeDirectoryPathString(searchString, null);
            
            if (SearchString == string.Empty)
                SearchString = SEARCH_STRING_DRIVES;

            SearchDirectory();
        }
        private void DoubleClickOnTables()
        { 
            if (selectedFileData == null || searchString == string.Empty) return;

            if (selectedFileData.Type == FileDataManager.STRING_TYPE_FOLDER)
                SearchString = FileDataManager.ChangeDirectoryPathString(searchString, selectedFileData.Name);
            else if (selectedFileData.Type == FileDataManager.STRING_TYPE_DRIVE)
                SearchString = selectedFileData.Name;

            SearchDirectory();
        }

    }
}
