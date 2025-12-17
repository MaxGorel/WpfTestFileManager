using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using WpfTest.HelperClasses;
using WpfTest.Models;
using WpfTest.MVVM;

namespace WpfTest.ViewModels
{
    class MainWindowViewModel
    {
        public ObservableCollection<FileData> Files { get; set; } = new();
        public ButtonCommand SearchCommand => new ButtonCommand(execute => SearchDirectory());

        private string searchString = string.Empty;
        private string lastSearchString = string.Empty;
        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; }
        }

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
            
            if (!FileDataManager.GetFileDataFromDirectory(searchString, Files))
                MessageBox.Show("Ошибка!");

            lastSearchString = searchString;
        }

    }
}
