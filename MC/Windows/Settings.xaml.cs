using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : MetroWindow
    {
        private string login;
        private string password;

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(string login, string password)
        {
            this.login = login;
            this.password = password;
            InitializeComponent();
        }

        void OnComboboxTextChanged(object sender, RoutedEventArgs e)
        {
            CB.IsDropDownOpen = true;
            // убрать selection, если dropdown только открылся
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(CB.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(CB.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.userPrefs != null)
            {
                Background = MainWindow.userPrefs.Theme.BackColor;
                FontFamily = MainWindow.userPrefs.FontFamily; 
            }


            //Fill font combobox
            ObservableCollection<string> SourceCB = new ObservableCollection<string>();
            foreach (var font in new System.Drawing.Text.InstalledFontCollection().Families)
            {
                SourceCB.Add(font.Name);
            }
            CB.ItemsSource = SourceCB;

            //Fill theme combobox
            BinaryFormatter binFormat = new BinaryFormatter();
            List<Theme> customThemes = new List<Theme>();
            customThemes.Add(new BlueTheme());
            customThemes.Add(new DarkTheme());
            string pathOfThemes = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MC", "Themes");
            if (Directory.Exists(pathOfThemes))
                foreach (string directory in Directory.EnumerateDirectories(pathOfThemes))
                {
                    //can fix?
                    foreach (string file in Directory.GetFiles(directory))
                    {
                        using (FileStream fStream = System.IO.File.OpenRead(file))
                        {
                            customThemes.Add(binFormat.Deserialize(fStream) as Theme);
                        }
                    }
                }
            TS.ItemsSource = customThemes;

        }

        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            UserPrefs userPrefs = null;
            //If the calling window is not welcome screen
            if (login == default(string))
            {
                userPrefs = MainWindow.userPrefs;
            }
            else
            {
                userPrefs = new UserPrefs();
                userPrefs.Login = login;
                userPrefs.Password = password;
            }
            userPrefs.FontFamily = new FontFamily(CB.Text);
            userPrefs.Theme = TS.SelectedItem as Theme;     
            //Serializing userPrefs
            BinaryFormatter binFormat = new BinaryFormatter();
            using (FileStream fStream = System.IO.File.Open(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),@"MC\" + userPrefs.Login + ".dat"),
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, userPrefs);
            }
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("A reboot is required for the changes to take effect.",
                "Are you sure?", System.Windows.MessageBoxButton.OKCancel);
            if (messageBoxResult == MessageBoxResult.OK)
            {
                System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
                Application.Current.Shutdown();
            }
                this.Close();
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddThemeButton_Click(object sender, RoutedEventArgs e)
        {
            AddTheme addTheme = new AddTheme();
            addTheme.Show();
            Close();
        }
    }
}
