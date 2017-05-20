using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MC.Classes;
using MC.Classes.Graphics.Themes;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : MetroWindow
    {
        public WelcomeScreen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "")
            {
                var messageBoxResult = System.Windows.MessageBox.Show("You did not specify a login. Click OK if you want to start with the default settings.",
                "Are you sure?", System.Windows.MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    var main = new MainWindow(new UserPrefs() { FontFamily = new FontFamily("Arial"), Login = "default", Password = "", Theme = new BlueTheme() });
                    main.Show();
                    Close();
                }
            }
            else
            {
                var datPath = System.IO.Path.Combine(_appDirectory.FullName, LoginBox.Text + ".dat");
                //if user registered
                if (System.IO.File.Exists(datPath))
                {
                    var binFormat = new BinaryFormatter();
                    UserPrefs userPrefs = null;
                    using (var fStream = System.IO.File.OpenRead(datPath))
                    {
                        userPrefs = binFormat.Deserialize(fStream) as UserPrefs;
                    }

                    if (userPrefs != null && userPrefs.Password == PassBox.Password)
                    {
                        var main = new MainWindow(userPrefs);
                        main.Show();
                        Close();
                    }
                    else
                        MessageBox.Show("Incorrect password.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //if user didn't register
                else
                {
                    var messageBoxResult = System.Windows.MessageBox.Show("You are not registred. Do you want to be one?",
                "MC", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        var settings = new Settings(LoginBox.Text, PassBox.Password);
                        settings.Show();
                        Close();
                    }
                }
            }

        }

        private DirectoryInfo _appDirectory;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Create app folder
            _appDirectory = Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"MC"));
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                Button_Click(sender, e);
            }
        }
    }
}
