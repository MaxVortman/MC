using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace MC
{
    static class LogicForUI
    {

        //List for ListViewSource
        //it is really faster
        static ObservableCollection<List_sElement> DataList = new ObservableCollection<List_sElement>();
        static private void FillInList(string path)
        {
            DataList = new ObservableCollection<List_sElement>();
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
        }

        static public void OpenElem(object elem, GraphicalApp graphics)
        {
            //test for folder
            if (((List_sElement)elem).Open())
            {
                try
                {
                    graphics.SetCaptionOfPath(((List_sElement)elem).Path);
                    graphics.ClearList();
                    FillInList(((List_sElement)elem).Path);

                }
                catch (UnauthorizedAccessException e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                finally
                {
                    graphics.DataSource = DataList;
                }
            }
        }

        static public ObservableCollection<List_sElement> FillTheListBoxWithDrives()
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            ObservableCollection<List_sElement> drivesElem = new ObservableCollection<List_sElement>(); 
            foreach (DriveInfo info in drives)
            {
                drivesElem.Add(new Drive(info));
            }
            return drivesElem;
        }
    }
}

