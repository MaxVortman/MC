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

        GraphicalApp graphics1;
        GraphicalApp graphics2;
        //need to fix
        const string defaultPath1 = "C:/";
        const string defaultPath2 = "D:/";

        public MainWindow()
        {
            InitializeComponent();
            graphics1 = new GraphicalApp(this.ListView1, this.PathOfListView1);
            graphics2 = new GraphicalApp(this.ListView2, this.PathOfListView2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Places.ItemsSource = LogicForUI.FillTheListBoxWithDrives();
            LogicForUI.OpenElem(new Folder(defaultPath1), graphics1);
            LogicForUI.OpenElem(new Folder(defaultPath2), graphics2);
        }

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogicForUI.OpenElem(ListView1.SelectedItem, graphics1);
        }

        private void ListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogicForUI.OpenElem(ListView2.SelectedItem, graphics2);
        }

        private void Places_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool focus = e.ButtonState == e.LeftButton;
            LogicForUI.OpenElem(Places.SelectedItem, focus ? graphics1 : graphics2);
        }
    }
}
