using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public string SearchString
        {
            get { return searchString; }
            set { searchString = value; }
        }

        private void SearchDirectory() // Вызывается при нажатии кнопки
        {
            FileDataManager.GetFileDataFromDirectory(searchString, Files);
        }

    }
}
