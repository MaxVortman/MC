using MahApps.Metro.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
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
    /// Логика взаимодействия для AddTheme.xaml
    /// </summary>
    public partial class AddTheme : MetroWindow
    {
        public AddTheme()
        {
            InitializeComponent();
        }

        void OnComboboxTextChanged(object sender, RoutedEventArgs e)
        {
            TextChanged(e, sender as ComboBox);
        }

        private void TextChanged(RoutedEventArgs e, ComboBox comboBox)
        {
            comboBox.IsDropDownOpen = true;
            // убрать selection, если dropdown только открылся
            var tb = (TextBox)e.OriginalSource;
            tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
            CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(comboBox.ItemsSource);
            cv.Filter = s =>
                ((string)s).IndexOf(comboBox.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        }

        const string DEFAULTTEXTONTEXTBOX = "Enter a name of new theme";
        private void MetroWindow_Loaded(object sender, RoutedEventArgs e)
        {
            //Fill color comboboxes
            ObservableCollection<string> SourceCB = new ObservableCollection<string>();
            foreach (PropertyInfo prop in typeof(Brushes).GetProperties())
                SourceCB.Add(prop.Name);
            //Такой говнокод из-за "присвоения" по ссылке.
            //Может change it soon.
            string[] source = new string[SourceCB.Count];
            string[] source1 = new string[SourceCB.Count];
            SourceCB.CopyTo(source, 0);
            SourceCB.CopyTo(source1, 0);
            BC.ItemsSource = SourceCB;
            LVC1.ItemsSource = source;
            LVC2.ItemsSource = source1;
            ThemeNameBox.SelectedText = DEFAULTTEXTONTEXTBOX;
        }

        private string FolderIconPath = null;
        private string DriveIconPath = null;
        private string USBIconPath = null;
        private string CDRomIconPath = null;
        private void PickButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog();
            fileDialog.Filter = "Image Files(*.BMP;*.JPG;*.GIF;*.PNG;*.ICO;*.SVG)|*.BMP;*.JPG;*.GIF;*.PNG;*.ICO;*.SVG";
            //getting full file name, where we'll save the txt
            if (fileDialog.ShowDialog() == true)
            {
                Button button = sender as Button;
                string buttonName = button.Name;
                switch (buttonName)
                {
                    case "PickFolder":
                        FolderIconPath = fileDialog.FileName;
                        break;
                    case "PickDrive":
                        DriveIconPath = fileDialog.FileName;
                        break;
                    case "PickUSB":
                        USBIconPath = fileDialog.FileName;
                        break;
                    case "PickCDRom":
                        CDRomIconPath = fileDialog.FileName;
                        break;
                    default:
                        throw new ArgumentException();
                }
                button.Background = Brushes.Gray;
            }
        }
        DirectoryInfo themeDirectory;
        string datPath;
        private void OkButton_Click(object sender, RoutedEventArgs e)
        {            
            string nameOfTheme = ThemeNameBox.Text;
            string[] iconsPath = new string[] { FolderIconPath, DriveIconPath, USBIconPath, CDRomIconPath };
            try
            {
                if (nameOfTheme != DEFAULTTEXTONTEXTBOX)
                {
                    //Create theme folder
                    themeDirectory = Directory.CreateDirectory(System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "MC", "Themes", nameOfTheme));
                    string nameOfTheme_ = nameOfTheme.Replace(" ", "_");
                    datPath = System.IO.Path.Combine(themeDirectory.FullName, nameOfTheme_ + ".dat");
                }
                else
                    throw new ArgumentException("Incorrect theme name!");
                if (FolderIconPath == null || DriveIconPath == null || USBIconPath == null || CDRomIconPath == null)
                {
                    throw new ArgumentException("You forgot icon!");
                }
                if (BC.Text == default(string) || LVC1.Text == default(string) || LVC2.Text == default(string))
                {
                    throw new ArgumentException("You forgot color!");
                }
           
            //move icons to theme directory
            string iconsDirectory = Directory.CreateDirectory(System.IO.Path.Combine(themeDirectory.FullName, "Icons")).FullName;
            for (int i = 0; i < iconsPath.Length; i++)
            {
                string newPath = System.IO.Path.Combine(iconsDirectory, System.IO.Path.GetFileName(iconsPath[i]));
                System.IO.File.Copy(iconsPath[i], newPath);
                iconsPath[i] = newPath;
            }
                //    

            //serialize theme
            BinaryFormatter binFormat = new BinaryFormatter();
            using (FileStream fStream = System.IO.File.Open(datPath,
                FileMode.Create, FileAccess.Write, FileShare.None))
            {
                binFormat.Serialize(fStream, new Theme()
                {
                    Name = nameOfTheme,
                    BackColorString = BC.Text,
                    CDRomIconPath = iconsPath[3],
                    FolderIconPath = iconsPath[0],
                    DriveIconPath = iconsPath[1],
                    USBIconPath = iconsPath[2],
                    LVColorString = new string[]
                  {
                      LVC1.Text, LVC2.Text
                  }                  
                });
            }
            Settings settings = new Settings();
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
