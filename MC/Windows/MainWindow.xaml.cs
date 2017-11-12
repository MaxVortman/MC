using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Text.RegularExpressions;
using System.Windows.Navigation;
using MC.Source;
using MC.Source.Entries;
using MC.Source.Entries.Drives;
using MC.Source.Fillers;
using MC.Source.Graphics;
using Directory = MC.Source.Entries.Directory;
using File = MC.Source.Entries.File;
using MC.Source.Visitors.EncryptVisitors;

namespace MC.Windows
{
    /*TO DO:
     * Нужно: изобрести watcher на изменение состояния дисков,
     *        добавить настройку цвета окна.
     * Можно: добавить визуализацию настроек,
     *        загрузку изображений по URL для иконок,
     *        drag and drop lvitem.   
    */

    public enum Action {Archive, Search }

    public partial class MainWindow : MetroWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            _graphics1 = new GraphicalApp(this.ListView1, this.PathOfListView1);
            _graphics2 = new GraphicalApp(this.ListView2, this.PathOfListView2);
        }

        private readonly GraphicalApp _graphics1;
        private readonly GraphicalApp _graphics2;
        public static UserPrefs UserPrefs { get; private set; }
        public MainWindow(UserPrefs userPrefs)
        {
            UserPrefs = userPrefs;
            InitializeComponent();
            _graphics1 = new GraphicalApp(this.ListView1, this.PathOfListView1);
            _graphics2 = new GraphicalApp(this.ListView2, this.PathOfListView2);
        }

        
        private FileFiller fileFiller1;
        private FileFiller fileFiller2;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            HelloBtn.Content = $"Hello, {UserPrefs.Login}!";
            FontFamily = UserPrefs.FontFamily;
            Background = UserPrefs.Theme.BackColor;

            var drives = DriveInfo.GetDrives();
            Places.ItemsSource = DriveFiller.FillTheListBoxWithDrives(drives);
            var watcher1 = new WatcherCreator(_graphics1, Dispatcher).CreateWatcher();
            var watcher2 = new WatcherCreator(_graphics2, Dispatcher).CreateWatcher();
            fileFiller1 = new FileFiller(_graphics1, watcher1);
            fileFiller2 = new FileFiller(_graphics2, watcher2);
            fileFiller1.OpenEntry(new Folder(drives[0].Name));
            fileFiller2.OpenEntry(new Folder(drives[1].Name));
        }

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            fileFiller1.OpenEntry(ListView1.SelectedItem as Entity);
        }

        private void ListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            fileFiller2.OpenEntry(ListView2.SelectedItem as Entity);
        }

        private void Places_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var focus = e.ButtonState == e.LeftButton;
            var item = sender as ListBoxItem;
            if (item == null) return;
            var drive = item.DataContext as Drive;
            if (drive != null && drive.IsReady)
            {
                if (focus)
                {
                    fileFiller1.OpenEntry(Places.SelectedItem as Entity);
                }
                else
                {
                    fileFiller2.OpenEntry(Places.SelectedItem as Entity);
                }
            }
            else
            {
                MessageBox.Show("Can not open because disk is not ready.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        
        private async void ContextMenu1_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null) return;
            var menu = item.Parent as ContextMenu;
            switch (item.Header.ToString())
            {
                case "Copy":
                    FileManipulator.CopyFile(selectedListItem);
                    break;
                case "Paste":
                    FileManipulator.PasteFileBy(ListView1.IsFocused ? _graphics1.Path : _graphics2.Path);
                    break;
                case "Cut":
                    FileManipulator.CutFile(selectedListItem);
                    break;
                case "Delete":
                    FileManipulator.DeleteFile(selectedListItem);
                    break;
                case "Archive":
                    ShowChooseDialog(selectedListItem);
                    break;
                case "Unarchive":
                    UnziperArchives.UnarchiveElemInThread(selectedListItem);
                    break;
                case "Rename":
                    // Getting the currently selected ListBoxItem
                    // Note that the ListBox must have
                    // IsSynchronizedWithCurrentItem set to True for this to work
                    ListViewCustom myListBox = menu.Name == "ContextMenu1" ? ListView1 : ListView2;
                    ListViewItem myListBoxItem =
                        (ListViewItem)(myListBox.ItemContainerGenerator.ContainerFromItem(selectedListItem));

                    // Getting the ContentPresenter of myListBoxItem
                    ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

                    // Finding textBlock from the DataTemplate that is set on that ContentPresenter
                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                    _myTextBox = (TextBox)myDataTemplate.FindName("ItemNameBox", myContentPresenter);

                    // Do something to the DataTemplate-generated TextBlock
                    _myTextBox.IsReadOnly = false;
                    _myTextBox.SelectionStart = 0;
                    _myTextBox.SelectionLength = _myTextBox.Text.Length;
                    KeyDown += Rename_KeyDown;                    
                    break;
                case "Statistic":
                    MessageBox.Show(await LogicForUi.ReadStatisticAsync(selectedListItem));
                    break;
                case "Decode":
                    selectedListItem.AcceptDecode(new AesVisitor());
                    break;
                case "Encode":
                    selectedListItem.AcceptEncode(new AesVisitor());
                    break;
            }
        }

        private void ShowChooseDialog(Entity selectedItem)
        {
            var dialog = new DialogThreadPage(Action.Archive, selectedItem);
            var win = new NavigationWindow() { Content = dialog, Width = 300, Height = 200 };
            win.Show();
        }

        private TextBox _myTextBox;        
        private void Rename_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || _myTextBox == null || selectedListItem == null) return;
            _myTextBox.IsReadOnly = true;
            FileManipulator.RenameFile(selectedListItem, _myTextBox.Text);
            KeyDown -= Rename_KeyDown;
        }


        private ListViewItem _pastItem;
        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            var item = sender as ListViewItem;
            if (_pastItem != null)
            {
                _pastItem.IsSelected = false;
            }
            if (item == null) return;
            item.IsSelected = true;
            _pastItem = item;
        }

        private static childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    var childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private Entity selectedListItem;
        private void ContextMenu1_Opened(object sender, RoutedEventArgs e)
        {
            var menu = sender as ContextMenu;            
            selectedListItem = (menu.Name == "ContextMenu1" ? ListView1.SelectedItem : ListView2.SelectedItem) as Entity;

            //TO DO: 6 - is bad
            var statisticItem = menu.Items[7] as MenuItem;
            if (selectedListItem.Name.EndsWith(".txt"))
                statisticItem.IsEnabled = true;
            else
                statisticItem.IsEnabled = false;

            var unarchiveItem = menu.Items[6] as MenuItem;
            var archiveItem = menu.Items[5] as MenuItem;
            bool isZip = Regex.Match(selectedListItem.Path, @"\w*\.(RAR|ZIP|GZ|TAR)$").Success;
            if (isZip)
            {
                unarchiveItem.IsEnabled = true;
                archiveItem.IsEnabled = false;
            }
            else
            {
                unarchiveItem.IsEnabled = false;
                archiveItem.IsEnabled = true;
            }
        }


        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.Show();
        }

        private void Go_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItemLV1.Header = PathOfListView1.Text;
            MenuItemLV2.Header = PathOfListView2.Text;
        }

        private void MenuItemLV_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            ShowChooseDialog(new Folder(item.Header.ToString()));
        }

        private void HelloBtn_Click(object sender, RoutedEventArgs e)
        {
            var messageBoxResult = System.Windows.MessageBox.Show("Do you want to change profile?",
                "Are you sure?", System.Windows.MessageBoxButton.OKCancel);
            if (messageBoxResult == MessageBoxResult.OK)
            {
                var welcomeScreen = new WelcomeScreen();
                welcomeScreen.Show();
                Close();
            }
            else return;            
        }
        
        private void Download_Click(object sender, RoutedEventArgs e)
        {
            var Download = new DownloadLink((sender as MenuItem).Header.ToString(), PathOfListView2.Text);
            Download.Show();
        }

        private void PathOfListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var textbox = sender as TextBox;
            if (!_searchFlag)
            {
                if (Directory.Exists(textbox.Text))
                {
                    if (textbox.Name.EndsWith("1"))
                        fileFiller1.OpenEntry(new Folder(textbox.Text));
                    else
                        fileFiller2.OpenEntry(new Folder(textbox.Text));
                }                   
                else
                    MessageBox.Show("Folder not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error); 
            }            
        }

        private bool _searchFlag = false;
        private string _directory;
        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & (ModifierKeys.Control | ModifierKeys.Shift)) == (ModifierKeys.Control | ModifierKeys.Shift))
            {
                var textbox = (sender as ListView).Name.EndsWith("1") ? PathOfListView1 : PathOfListView2;
                _directory = textbox.Text;
                textbox.Focus();
                textbox.SelectionStart = 0;
                textbox.SelectionLength = textbox.Text.Length;
                _searchFlag = true;
            }
        }

        private CancellationTokenSource _tokenSource;
        private void PathOfListView_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!_searchFlag) return;
            if (_tokenSource != null)
                _tokenSource.Cancel();
            var textbox = sender as TextBox;
            var currentGraphics = textbox.Name.EndsWith("1") ? _graphics1 : _graphics2;
            var searchText = textbox.Text;
            _tokenSource = new CancellationTokenSource();
            var ct = _tokenSource.Token;
            Task.Run(() =>
            {
                try
                {
                    var result = SearchEngineFiles.GetPathOfFilesBy(_directory, searchText, ct);

                    var dataList = new List<Entity>(result.Count);
                    foreach (var item in result)
                    {
                        dataList.Add(new File(item));
                    }
                    Dispatcher.Invoke(() => currentGraphics.DataSource = new System.Collections.ObjectModel.ObservableCollection<Entity>(dataList));
                    _tokenSource = null;
                }
                catch (OperationCanceledException)
                {
                    _tokenSource = null;
                }
            }, ct);

        }

        private void PathOfListView_LostFocus(object sender, RoutedEventArgs e)
        {
            _searchFlag = false;
            if (_tokenSource != null) _tokenSource.Cancel();
        }
    }
}
