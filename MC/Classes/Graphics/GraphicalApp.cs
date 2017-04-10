using System;
using System.Collections.Generic;
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
            window.ListView1.Items.Clear();
            if (window.PathOfListView1.Text.Length > 3)
            {
                string parentPath = Directory.GetParent(window.PathOfListView1.Text).FullName;
                window.ListView1.Items.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
        }

        public void SetCaptionOfPath(string path)
        {
            window.PathOfListView1.Text = path;
        }
    }

    
}
