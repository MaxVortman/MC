using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using Microsoft.Win32;

namespace MC
{   

    static class LogicForUI
    {

        //List for ListViewSource
        //it is really faster
        static List<List_sElement> DataList;
        static private void FillInList(string path)
        {
                                   //must be faster
            DataList = new List<List_sElement>(500);
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
                    graphics.DataSource = new ObservableCollection<List_sElement>(DataList);
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

        static Buffer buffer;

        static private List_sElement DataToCopy;

        internal static void CopyElem(object elem)
        {
            DeleteTemp(buffer);

            List_sElement item = elem as List_sElement;
            try
            {
                buffer = item.Copy();
                DataToCopy = item;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void ArchiveOrUnachiveElem(object elem)
        {
            List_sElement item = elem as List_sElement;

            try
            {
                if (Path.GetExtension(item.Path) == ".rar")
                {
                    Unarchive(item);
                }
                else
                {
                    Archive(item);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void Unarchive(List_sElement item)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.Windows.Forms.DialogResult.OK == folderDialog.ShowDialog())
            {
                item.Unarchive(folderDialog.SelectedPath);
            }
        }

        private static void Archive(List_sElement item)
        {

            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "All Files | *.* ";
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "rar";
            //getting full file name, where we'll save the archive
            if (fileDialog.ShowDialog() == true)
            {
                item.Archive(fileDialog.FileName);
            }
        }

        private static void DeleteTemp(Buffer buffer)
        {
            if (buffer != null)
            {
                if (buffer is FileBuffer)
                {
                    FileBuffer fileBuffer = buffer as FileBuffer;
                    System.IO.File.Delete(fileBuffer.tempPath);
                }
                else
                {
                    FolderBuffer folderBuffer = buffer as FolderBuffer;
                    foreach (Buffer item in folderBuffer.FoldersBuffer)
                    {
                        DeleteTemp(item);
                    }
                }
            }
        }

        internal static void CutElem(object elem, GraphicalApp graphics)
        {
            try
            {
                CopyElem(elem);
                DeleteElem(elem, graphics);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void PasteElem(GraphicalApp graphics)
        {
            if (buffer != null)
            {
                //For the dynamics
                ObservableCollection<List_sElement> Data = graphics.DataSource;
                string path = System.IO.Path.Combine(graphics.Path, DataToCopy.Name);

                try
                {
                    DataToCopy.Paste(path, buffer);
                    //that's it
                    if (DataToCopy is Folder)
                    {
                        Data.Add(new Folder(path));
                    }
                    else
                        Data.Add(new File(path));
                    graphics.DataSource = Data;
                    //
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }                
            }
        }

        internal static void DeleteElem(object elem, GraphicalApp graphics)
        {
            try
            {
                //For the dynamics
                ObservableCollection<List_sElement> Data = graphics.DataSource;


                List_sElement item = elem as List_sElement;
                string path = item.Path;
                if (item is File)
                {
                    System.IO.File.Delete(path);
                }
                else
                {
                    System.IO.Directory.Delete(path, true);
                }
                //that's it
                Data.Remove(item);
                graphics.DataSource = Data;
                //
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

