using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MC.Source.Threading;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для DilogThreadWindow.xaml
    /// </summary>
    public partial class DialogThreadWindow : MetroWindow
    {
        private string pathOfFile;
        private readonly IThreadFactory threadFactory;

        public DialogThreadWindow(string pathOfFile, IThreadFactory threadFactory)
        {
            this.pathOfFile = pathOfFile;
            this.threadFactory = threadFactory;
            InitializeComponent();
        }

        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            threadFactory.CreateObject(btn.Content.ToString(), pathOfFile).DoThread();
        }
    }
}
