using System.Collections.ObjectModel;
using System.Windows.Controls;
using MC.Abstract_and_Parent_Classes;

namespace MC.Classes.Graphics
{
    internal class GraphicalApp
    {

        private readonly ListViewCustom _list;
        private readonly TextBlock _text;

        public string Path => _text.Text;

        public ObservableCollection<ListSElement> DataSource
        {
            set => _list.ItemsSource = value;
            get => (ObservableCollection<ListSElement>)_list.ItemsSource;
        }


        public GraphicalApp(ListViewCustom list, TextBlock text)
        {
            _list = list;
            _text = text;
        }

        public void AddLine(ListSElement elem)
        {
            _list.Items.Add(elem);
        }

        public void ClearList()
        {
            _list.ClearValue(ItemsControl.ItemsSourceProperty);
        }

        public void SetCaptionOfPath(string path)
        {
            _text.Text = path;
        }

        public void Refresh()
        {
            _list.Items.Refresh();
        }
    }

    
}
