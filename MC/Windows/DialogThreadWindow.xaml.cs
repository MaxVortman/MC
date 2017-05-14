using MahApps.Metro.Controls;
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
using System.Windows.Shapes;

namespace MC
{
    /// <summary>
    /// Логика взаимодействия для DilogThreadWindow.xaml
    /// </summary>
    public partial class DialogThreadWindow : MetroWindow
    {
        private object item;

        public DialogThreadWindow(object item)
        { 
            this.item = item;
            InitializeComponent();
        }
        
        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Content.ToString())
            {
                case "Thread":
                    LogicForUI.ThreadOperation(item);
                    break;
                case "Parralel":
                    break;
                case "Tasks":
                    break;
                case "Async":
                    break;
                default:
                    throw new ArgumentException();
            }
        }
    }
}
