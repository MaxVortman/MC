using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using MahApps.Metro.Controls;
using Microsoft.Win32;
using Theme = MC.Source.Graphics.Themes.Theme;

namespace MC.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddTheme.xaml
    /// </summary>
    public partial class AddTheme : MetroWindow
    {
        public AddTheme()
        {
            InitializeComponent();
        }

        private void OnComboboxTextChanged(object sender, RoutedEventArgs e)
        {
            TextChanged(e, sender as ComboBox);
        }

        private void TextChanged(RoutedEventArgs e, ComboBox comboBox)
        {
            comboBox.IsDropDownOpen = true;
            // убрать selection, если dropdown только открылся
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            var cv = (CollectionView)CollectionViewSource.GetDefaultView(comboBox.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(comboBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        private const string Defaulttextontextbox = "Enter a name of new theme";
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Fill color comboboxes
            var SourceCB = new ObservableCollection<string>();
            foreach (PropertyInfo prop in typeof(Brushes).GetProperties())
                SourceCB.Add(prop.Name);
            //Такой говнокод из-за "присвоения" по ссылке.
            //Может change it soon.
            var source = new string[SourceCB.Count];
            var source1 = new string[SourceCB.Count];
            SourceCB.CopyTo(source, 0);
            SourceCB.CopyTo(source1, 0);
            BC.ItemsSource = SourceCB;
            LVC1.ItemsSource = source;
            LVC2.ItemsSource = source1;
            ThemeNameBox.SelectedText = Defaulttextontextbox;
        }

        private string _folderIconPath = null;
        private string _driveIconPath = null;
        private string _usbIconPath = null;
        private string _cdRomIconPath = null;
        private void PickButton_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog
            {
                Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.ICO;*.SVG)|*.BMP;*.JPG;*.GIF;*.PNG;*.ICO;*.SVG"
            };
            //getting full file name, where we'll save the txt
            if (fileDialog.ShowDialog() == true)
            {
                var button = sender as Button;
                var buttonName = button?.Name;
                switch (buttonName)
                {
                    case "PickFolder":
                        _folderIconPath = fileDialog.FileName;
                        break;
                    case "PickDrive":
                        _driveIconPath = fileDialog.FileName;
                        break;
                    case "PickUSB":
                        _usbIconPath = fileDialog.FileName;
                        break;
                    case "PickCDRom":
                        _cdRomIconPath = fileDialog.FileName;
                        break;
                    default:
                        throw new ArgumentException();
                }
                button.Background = Brushes.Gray;
            }
        }

        private System.IO.DirectoryInfo _themeDirectory;
        private string _datPath;
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {            
            var nameOfTheme = ThemeNameBox.Text;
            var iconsPath = new string[] { _folderIconPath, _driveIconPath, _usbIconPath, _cdRomIconPath };
            try
            {
                if (nameOfTheme != Defaulttextontextbox)
                {
                    //Create theme folder
                    _themeDirectory = Source.Entries.Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MC", "Themes", nameOfTheme));
                    var nameOfTheme_ = nameOfTheme.Replace(" ", "_");
                    _datPath = System.IO.Path.Combine(_themeDirectory.FullName, $"{nameOfTheme_}.dat");
                }
                else
                    throw new ArgumentException("Incorrect theme name!");
                if (_folderIconPath == null || _driveIconPath == null || _usbIconPath == null || _cdRomIconPath == null)
                {
                    throw new ArgumentException("You forgot icon!");
                }
                if (BC.Text == default(string) || LVC1.Text == default(string) || LVC2.Text == default(string))
                {
                    throw new ArgumentException("You forgot color!");
                }
           
            //move icons to theme directory
            var iconsDirectory = Source.Entries.Directory.CreateDirectory(System.IO.Path.Combine(_themeDirectory.FullName, "Icons")).FullName;
            for (int i = 0; i < iconsPath.Length; i++)
            {
                var newPath = System.IO.Path.Combine(iconsDirectory, System.IO.Path.GetFileName(iconsPath[i]));
                System.IO.File.Copy(iconsPath[i], newPath);
                iconsPath[i] = newPath;
            }
                //    

            //serialize theme
            var binFormat = new BinaryFormatter();
            using (var fStream = Source.Entries.File.Open(_datPath,
                System.IO.FileMode.Create, System.IO.FileAccess.Write, System.IO.FileShare.None))
            {
                binFormat.Serialize(fStream, new Theme()
                {
                    Name = nameOfTheme,
                    BackColorString = BC.Text,
                    CdRomIconPath = iconsPath[3],
                    FolderIconPath = iconsPath[0],
                    DriveIconPath = iconsPath[1],
                    UsbIconPath = iconsPath[2],
                    LvColorString = new string[]
                  {
                      LVC1.Text, LVC2.Text
                  }                  
                });
            }
            var settings = new Settings();
            settings.Show();
            Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
