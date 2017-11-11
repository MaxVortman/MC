using System.Collections.ObjectModel;
using System.Windows.Controls;
using MC.Source.Entries;

namespace MC.Source.Graphics
{
    internal class GraphicalApp
    {

        private readonly ListViewCustom _list;
        private readonly TextBox _text;

        public string Path => _text.Text;

        public ObservableCollection<Entity> DataSource
        {
            set => _list.ItemsSource = value;
            get => (ObservableCollection<Entity>)_list.ItemsSource;
        }


        public GraphicalApp(ListViewCustom list, TextBox text)
        {
            _list = list;
            _text = text;
        }

        public void AddLine(Entity elem)
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
