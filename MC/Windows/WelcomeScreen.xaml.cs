using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
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
    /// Логика взаимодействия для WelcomeScreen.xaml
    /// </summary>
    public partial class WelcomeScreen : Window
    {
        public WelcomeScreen()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (LoginBox.Text == "")
            {
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("You did not specify a login. Click OK if you want to start with the default settings.",
                "Are you sure?", System.Windows.MessageBoxButton.OKCancel);
                if (messageBoxResult == MessageBoxResult.OK)
                {
                    MainWindow main = new MainWindow(null);
                    main.Show();
                    Close();
                }
            }
            else
            {
                string datPath = System.IO.Path.Combine(appDirectory.FullName, LoginBox.Text + ".dat");
                //if user registered
                if (System.IO.File.Exists(datPath))
                {
                    BinaryFormatter binFormat = new BinaryFormatter();
                    UserPrefs userPrefs = null;
                    using (FileStream fStream = System.IO.File.OpenRead(datPath))
                    {
                        userPrefs = binFormat.Deserialize(fStream) as UserPrefs;
                    }

                    if (userPrefs.Password == PassBox.Password)
                    {
                        MainWindow main = new MainWindow(userPrefs);
                        main.Show();
                        Close();
                    }
                    else
                        MessageBox.Show("Incorrect password.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                //if user didn't register
                else
                {
                    MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("You are not registred. Do you want to be one?",
                "MC", System.Windows.MessageBoxButton.YesNo);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        Settings settings = new Settings(LoginBox.Text, PassBox.Password);
                        settings.Show();
                        Close();
                    }
                }
            }

        }
        DirectoryInfo appDirectory;
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            //Create app folder
            appDirectory = Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), @"MC"));
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
