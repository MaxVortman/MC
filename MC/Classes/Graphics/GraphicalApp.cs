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

        public ObservableCollection<List_sElement> DataSource
        {
            set
            {
                list.ItemsSource = value;
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
            //if (window.PathOfListView1.Text.Length > 3)
            //{
            //    string parentPath = Directory.GetParent(window.PathOfListView1.Text).FullName;
            //    window.ListView1.Items.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            //}
        }

        public void SetCaptionOfPath(string path)
        {
            text.Text = path;
        }

    }

    
}
