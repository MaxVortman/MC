using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MC.Source;
using MC.Source.Graphics.Themes;
using Theme = MC.Source.Graphics.Themes.Theme;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для Settings.xaml
    /// </summary>
    public partial class Settings : MetroWindow
    {
        private readonly string _login;
        private readonly string _password;

        public Settings()
        {
            InitializeComponent();
        }

        public Settings(string login, string password)
        {
            this._login = login;
            this._password = password;
            InitializeComponent();
        }

        private void OnComboboxTextChanged(object sender, RoutedEventArgs e)
        {
            CB.IsDropDownOpen = true;
            // убрать selection, если dropdown только открылся
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            var cv = (CollectionView)CollectionViewSource.GetDefaultView(CB.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(CB.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (MainWindow.UserPrefs != null)
            {
                Background = MainWindow.UserPrefs.Theme.BackColor;
                FontFamily = MainWindow.UserPrefs.FontFamily; 
            }


            //Fill font combobox
            ObservableCollection<string> SourceCB = new ObservableCollection<string>();
            foreach (var font in new System.Drawing.Text.InstalledFontCollection().Families)
            {
                SourceCB.Add(font.Name);
            }
            CB.ItemsSource = SourceCB;

            //Fill theme combobox
            var binFormat = new BinaryFormatter();
            var customThemes = new List<Theme> {new BlueTheme(), new DarkTheme()};
            var pathOfThemes = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MC", "Themes");
            if (Directory.Exists(pathOfThemes))
                foreach (var directory in Directory.EnumerateDirectories(pathOfThemes))
                {
                    //can fix?
                    foreach (var file in Directory.GetFiles(directory))
                    {
                        using (var fStream = System.IO.File.OpenRead(file))
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
            if (_login == default(string))
            {
                userPrefs = MainWindow.UserPrefs;
            }
            else
            {
                userPrefs = new UserPrefs
                {
                    Login = _login,
                    Password = _password
                };
            }
            userPrefs.FontFamily = new FontFamily(CB.Text);
            userPrefs.Theme = TS.SelectedItem as Theme;     
            //Serializing userPrefs
            var binFormat = new BinaryFormatter();
            using (var fStream = System.IO.File.Open(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal),@"MC\" + userPrefs.Login + ".dat"),
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, userPrefs);
            }
            var messageBoxResult = System.Windows.MessageBox.Show("A reboot is required for the changes to take effect.",
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
            var addTheme = new AddTheme();
            addTheme.Show();
            Close();
        }
    }
}
