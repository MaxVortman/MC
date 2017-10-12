using System;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;
using MC.Classes;
using MC.Abstract_and_Parent_Classes;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для DilogThreadWindow.xaml
    /// </summary>
    public partial class DialogThreadWindow : MetroWindow
    {
        private string pathOfFile;

        public DialogThreadWindow(string pathOfFile)
        {
            this.pathOfFile = pathOfFile;
            InitializeComponent();
        }
        private FileArchiver fileArchiver;

        private void TypeButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            
            switch (btn.Content.ToString())
            {
                case "Thread":
                    fileArchiver = new FileArchiverInThread(pathOfFile);                    
                    break;
                case "Parralel":
                    fileArchiver = new FileArchiverParallel(pathOfFile);
                    break;
                case "Tasks":
                    fileArchiver = new FileArchiverInTask(pathOfFile);
                    break;
                case "Async":
                    fileArchiver = new FileArchiverAsync(pathOfFile);
                    break;
                default:
                    throw new ArgumentException();
            }
            fileArchiver.Archive();
        }

        internal void ClosingThread()
        {
            fileArchiver?.Closing();
        }
    }
}
