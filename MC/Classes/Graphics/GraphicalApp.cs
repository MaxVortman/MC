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

        private MainWindow window;

        public ObservableCollection<List_sElement> DataSource
        {
            set
            {
                window.ListView1.ItemsSource = value;
            }
        }


        public GraphicalApp(MainWindow window)
        {
            this.window = window;
        }

        public void AddLine(List_sElement elem)
        {
            window.ListView1.Items.Add(elem);
        }

        public void ClearList()
        {
            window.ListView1.ClearValue(ListViewCustom.ItemsSourceProperty);
            //if (window.PathOfListView1.Text.Length > 3)
            //{
            //    string parentPath = Directory.GetParent(window.PathOfListView1.Text).FullName;
            //    window.ListView1.Items.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            //}
        }

        public void SetCaptionOfPath(string path)
        {
            window.PathOfListView1.Text = path;
        }

    }

    
}
