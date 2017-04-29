using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace MC
{
    class GraphicalApp
    {

        private ListViewCustom list;
        private TextBlock text;

        public string Path
        {
            get
            {
                return text.Text;
            }
        }

        public ObservableCollection<List_sElement> DataSource
        {
            set
            {
                list.ItemsSource = value;
            }
            get
            {
                return (ObservableCollection<List_sElement>)list.ItemsSource;
            }
        }


        public GraphicalApp(ListViewCustom list, TextBlock text)
        {
            this.list = list;
            this.text = text;
        }

        public void AddLine(List_sElement elem)
        {
            list.Items.Add(elem);
        }

        public void ClearList()
        {
            list.ClearValue(ListViewCustom.ItemsSourceProperty);
        }

        public void SetCaptionOfPath(string path)
        {
            text.Text = path;
        }

        public void Refresh()
        {
            list.Items.Refresh();
        }
    }

    
}
