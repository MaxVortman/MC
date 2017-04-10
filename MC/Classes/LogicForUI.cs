using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;

namespace MC
{
    class LogicForUI
    {
        private GraphicalApp graphics;


        public LogicForUI(GraphicalApp graphics)
        {
            this.graphics = graphics;
        }

        private void FillInList(string path)
        {

            //List for ListViewSource
            //it is really faster
            ObservableCollection<List_sElement> DataList = new ObservableCollection<List_sElement>();


            graphics.ClearList();

            // ... folder
            if (path.Length > 3)
            {
                string parentPath = Directory.GetParent(path).FullName;
                DataList.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
            //enumerate folder's path
            foreach (var item in Directory.EnumerateDirectories(path))
            {
                DataList.Add(new Folder(item));
                //graphics.AddLine(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in Directory.EnumerateFiles(path))
            {
                DataList.Add(new File(item));
                //graphics.AddLine(new File(item));
            }
            graphics.DataSource = DataList;
        }

        public void OpenElem(object elem)
        {
            //test for folder
            if (((List_sElement)elem).Open())
            {
                try
                {
                    graphics.SetCaptionOfPath(((List_sElement)elem).Path);
                    FillInList(((List_sElement)elem).Path);

                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

