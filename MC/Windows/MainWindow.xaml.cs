using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using MahApps.Metro.Controls;
using MC.Abstract_and_Parent_Classes;
using MC.Classes;
using MC.Classes.Graphics;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;

namespace MC.Windows
{
    /*
     * Нужно: изобрести watcher на изменение состояния дисков,
     *        добавить настройку цвета окна.
     * Можно: добавить визуализацию настроек,
     *        загрузку изображений по URL для иконок,
     *        drag and drop lvitem.   
    */
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
            MainWindow.UserPrefs = userPrefs;
            InitializeComponent();
            _graphics1 = new GraphicalApp(this.ListView1, this.PathOfListView1);
            _graphics2 = new GraphicalApp(this.ListView2, this.PathOfListView2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            HelloBtn.Content = $"Hello, {UserPrefs.Login}!";
            FontFamily = UserPrefs.FontFamily;
            Background = UserPrefs.Theme.BackColor;

            var drives = DriveInfo.GetDrives();
            Places.ItemsSource = LogicForUi.FillTheListBoxWithDrives(drives);
            LogicForUi.CreateWatchersAndSetGraphics(_graphics1, _graphics2);
            LogicForUi.OpenElem(new Folder(drives[0].Name), _graphics1, Dispatcher);
            LogicForUi.OpenElem(new Folder(drives[1].Name), _graphics2, Dispatcher);
        }

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogicForUi.OpenElem(ListView1.SelectedItem, _graphics1, Dispatcher);
        }

        private void ListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogicForUi.OpenElem(ListView2.SelectedItem, _graphics2, Dispatcher);
        }

        private void Places_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var focus = e.ButtonState == e.LeftButton;
            var item = sender as ListBoxItem;
            if (item == null) return;
            var drive = item.DataContext as Drive;
            if (drive != null && drive.IsReady)
            {
                LogicForUi.OpenElem(Places.SelectedItem, focus ? _graphics1 : _graphics2, Dispatcher);
            }
            else
            {
                MessageBox.Show("Can not open because disk is not ready.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenu1_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            if (item == null) return;
            var menu = item.Parent as ContextMenu;
            switch (item.Header.ToString())
            {
                case "Copy":
                    LogicForUi.CopyElem(_selectedItem);
                    break;
                case "Paste":
                    LogicForUi.PasteElem(ListView1.IsFocused ? _graphics1 : _graphics2);
                    break;
                case "Cut":
                    LogicForUi.CutElem(_selectedItem);
                    break;
                case "Delete":
                    LogicForUi.DeleteElem(_selectedItem);
                    break;
                case "Archive/Unarchive":
                    var dialog = new DialogThreadWindow(_selectedItem);
                    dialog.Show();
                    break;
                case "Rename":
                    // Getting the currently selected ListBoxItem
                    // Note that the ListBox must have
                    // IsSynchronizedWithCurrentItem set to True for this to work
                    ListViewCustom myListBox = menu.Name == "ContextMenu1" ? ListView1 : ListView2;
                    ListViewItem myListBoxItem =
                        (ListViewItem)(myListBox.ItemContainerGenerator.ContainerFromItem(_selectedItem));

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
            }
        }

        private TextBox _myTextBox;        
        private void Rename_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter || _myTextBox == null || _selectedItem == null) return;
            _myTextBox.IsReadOnly = true;
            LogicForUi.RenameFile(_selectedItem, _myTextBox.Text);
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

        private object _selectedItem;
        private void ContextMenu1_Opened(object sender, RoutedEventArgs e)
        {
            var menu = sender as ContextMenu;
            _selectedItem = menu != null && menu.Name == "ContextMenu1" ? ListView1.SelectedItem : ListView2.SelectedItem;
        }


        //private MetroWindow accentThemeTestWindow;
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            var settings = new Settings();
            settings.Show();
            //if (accentThemeTestWindow != null)
            //{
            //    accentThemeTestWindow.Activate();
            //    return;
            //}

            //accentThemeTestWindow = new AccentStyleWindow();
            //accentThemeTestWindow.Owner = this;
            //accentThemeTestWindow.Closed += (o, args) => accentThemeTestWindow = null;
            //accentThemeTestWindow.Left = this.Left + this.ActualWidth / 2.0;
            //accentThemeTestWindow.Top = this.Top + this.ActualHeight / 2.0;
            //accentThemeTestWindow.Show();
        }

        private void Go_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            MenuItemLV1.Header = PathOfListView1.Text;
            MenuItemLV2.Header = PathOfListView2.Text;
        }

        private void MenuItemLV_Click(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;
            var dialog = new DialogThreadWindow(item.Header.ToString());
            dialog.Show();
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

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                LogicForUi.Closing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
                    LogicForUi.OpenElem(new Folder(textbox.Text), textbox.Name.EndsWith("1") ? _graphics1 : _graphics2, Dispatcher);
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
            if (_tokenSource != null) _tokenSource.Cancel();
            var textbox = sender as TextBox;
            var currentGraphics = textbox.Name.EndsWith("1") ? _graphics1 : _graphics2;
            var searchText = textbox.Text;
            _tokenSource = new CancellationTokenSource();
            var ct = _tokenSource.Token;
            Task.Run(() =>
            {

                // Were we already canceled?
                ct.ThrowIfCancellationRequested();

                var result = Search.Files(_directory, searchText);
                var dataList = new List<ListSElement>(result.Count);
                foreach (var item in result)
                {
                    dataList.Add(new Classes.File(item));
                }
                Dispatcher.Invoke(() => currentGraphics.DataSource = new System.Collections.ObjectModel.ObservableCollection<ListSElement>(dataList));
                _tokenSource = null;
            }, ct);
            
        }

        private void PathOfListView_LostFocus(object sender, RoutedEventArgs e)
        {
            _searchFlag = false;
            if (_tokenSource != null) _tokenSource.Cancel();
        }
    }
}
