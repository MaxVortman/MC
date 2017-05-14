using MahApps.Metro.Controls;
using MahAppsMetroThemesSample;
using Microsoft.Win32;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MC
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
            graphics1 = new GraphicalApp(this.ListView1, this.PathOfListView1);
            graphics2 = new GraphicalApp(this.ListView2, this.PathOfListView2);
        }

        GraphicalApp graphics1;
        GraphicalApp graphics2;
        public static UserPrefs userPrefs { get; private set; }
        public MainWindow(UserPrefs userPrefs)
        {
            MainWindow.userPrefs = userPrefs;
            InitializeComponent();
            graphics1 = new GraphicalApp(this.ListView1, this.PathOfListView1);
            graphics2 = new GraphicalApp(this.ListView2, this.PathOfListView2);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            HelloBtn.Content = String.Format("Hello, {0}!", userPrefs.Login);
            FontFamily = userPrefs.FontFamily;
            Background = userPrefs.Theme.BackColor;

            DriveInfo[] drives = DriveInfo.GetDrives();
            Places.ItemsSource = LogicForUI.FillTheListBoxWithDrives(drives);
            LogicForUI.CreateWatchersAndSetGraphics(graphics1, graphics2);
            LogicForUI.OpenElem(new Folder(drives[0].Name), graphics1, Dispatcher);
            LogicForUI.OpenElem(new Folder(drives[1].Name), graphics2, Dispatcher);
        }

        private void ListView1_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogicForUI.OpenElem(ListView1.SelectedItem, graphics1, Dispatcher);
        }

        private void ListView2_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            LogicForUI.OpenElem(ListView2.SelectedItem, graphics2, Dispatcher);
        }

        private void Places_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            bool focus = e.ButtonState == e.LeftButton;
            ListBoxItem item = sender as ListBoxItem;
            Drive drive = item.DataContext as Drive;
            if (drive.isReady)
            {
                LogicForUI.OpenElem(Places.SelectedItem, focus ? graphics1 : graphics2, Dispatcher);
            }
            else
            {
                MessageBox.Show("Can not open because disk is not ready.", "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ContextMenu1_Click(object sender, RoutedEventArgs e)
        {
            MenuItem item = sender as MenuItem;
            ContextMenu menu = item.Parent as ContextMenu;
            switch (item.Header.ToString())
            {
                case "Copy":
                    LogicForUI.CopyElem(selectedItem);
                    break;
                case "Paste":
                    LogicForUI.PasteElem(ListView1.IsFocused ? graphics1 : graphics2);
                    break;
                case "Cut":
                    LogicForUI.CutElem(selectedItem);
                    break;
                case "Delete":
                    LogicForUI.DeleteElem(selectedItem);
                    break;
                case "Archive/Unarchive":
                    DialogThreadWindow dialog = new DialogThreadWindow(selectedItem);
                    dialog.Show();
                    break;
                case "Rename":
                    // Getting the currently selected ListBoxItem
                    // Note that the ListBox must have
                    // IsSynchronizedWithCurrentItem set to True for this to work
                    ListViewCustom myListBox = menu.Name == "ContextMenu1" ? ListView1 : ListView2;
                    ListViewItem myListBoxItem =
                        (ListViewItem)(myListBox.ItemContainerGenerator.ContainerFromItem(selectedItem));

                    // Getting the ContentPresenter of myListBoxItem
                    ContentPresenter myContentPresenter = FindVisualChild<ContentPresenter>(myListBoxItem);

                    // Finding textBlock from the DataTemplate that is set on that ContentPresenter
                    DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                    myTextBox = (TextBox)myDataTemplate.FindName("ItemNameBox", myContentPresenter);

                    // Do something to the DataTemplate-generated TextBlock
                    myTextBox.IsReadOnly = false;
                    myTextBox.SelectionStart = 0;
                    myTextBox.SelectionLength = myTextBox.Text.Length;
                    KeyDown += Rename_KeyDown;                    
                    break;
            }
        }

        TextBox myTextBox;        
        private void Rename_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && myTextBox != null && selectedItem != null)
            {
                myTextBox.IsReadOnly = true;
                LogicForUI.RenameFile(selectedItem, myTextBox.Text);
                KeyDown -= Rename_KeyDown;
            }
        }




        ListViewItem pastItem;
        private void TextBox_MouseDown(object sender, MouseEventArgs e)
        {
            ListViewItem item = sender as ListViewItem;
            if (pastItem != null)
            {
                pastItem.IsSelected = false;
            }
            item.IsSelected = true;
            pastItem = item;
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
    where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }
        object selectedItem;
        private void ContextMenu1_Opened(object sender, RoutedEventArgs e)
        {
            ContextMenu menu = sender as ContextMenu;
            selectedItem = menu.Name == "ContextMenu1" ? ListView1.SelectedItem : ListView2.SelectedItem;
        }


        //private MetroWindow accentThemeTestWindow;
        private void Settings_Click(object sender, RoutedEventArgs e)
        {
            Settings settings = new Settings();
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
            DialogThreadWindow dialog = new DialogThreadWindow(item.Header.ToString());
            dialog.Show();
        }

        private void HelloBtn_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Do you want to change profile?",
                "Are you sure?", System.Windows.MessageBoxButton.OKCancel);
            if (messageBoxResult == MessageBoxResult.OK)
            {
                WelcomeScreen welcomeScreen = new WelcomeScreen();
                welcomeScreen.Show();
                Close();
            }
            else return;            
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            try
            {
                LogicForUI.Closing();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
