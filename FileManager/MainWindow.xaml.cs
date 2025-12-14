using System.Text;
using System.Windows;
using WpfTest.ViewModels;

namespace FileManager
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainWindowViewModel vm = new MainWindowViewModel();
            DataContext = vm;
        }
    }
}