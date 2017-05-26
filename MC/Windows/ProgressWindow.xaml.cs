using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для ProgressWindow.xaml
    /// </summary>
    /// 
    public partial class ProgressWindow : Window
    {
        private event EventHandler CloseWindow;
        public ProgressWindow(string ProcessCapture)
        {
            this.ProcessCapture = ProcessCapture;
            InitializeComponent();
            //PB.Maximum = maxValue;
            ProcessName.Text = $"{ProcessCapture} in progress...";

            CloseWindow += ProgressWindow_CloseWindow;
        }

        private void ProgressWindow_CloseWindow(object sender, EventArgs e)
        {
            Thread.Sleep(1000);
            Close();
        }

        public string ProcessCapture { get; private set; }
        public CancellationTokenSource CTS { private get; set; }

        public void ReportProgress(double value)
        {
            PB.Value = value;
            if (value == 100)
            {                
                CloseWindow.Invoke(this, new EventArgs());
            }
        }
        
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (CTS != null)
            {
                CTS.Cancel();
                CloseWindow.Invoke(this, new EventArgs());
            }
        }
    }
}
