using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MC
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        GraphicalApp graphics;
        LogicForUI logic;
        const string defaultPath = "C:/";

        public MainWindow()
        {
            InitializeComponent();
            graphics = new GraphicalApp(this);
            logic = new LogicForUI(graphics);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            logic.FillInList(defaultPath);
        }
    }
}
