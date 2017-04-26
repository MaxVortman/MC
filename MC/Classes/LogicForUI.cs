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
using System.Windows.Threading;
using System.Threading;

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
        private static GraphicalApp currentGraphics;
        private static Dispatcher dispatcher;
        static public void OpenElem(object o, GraphicalApp graphics, Dispatcher disp)
        {
            dispatcher = disp;
            currentGraphics = graphics;
            List_sElement elem = o as List_sElement;
            if (elem is Folder || elem is Drive)
            {
                //create watcher
                CreateWatcher(elem);
                //test for folder                
                try
                {
                    graphics.SetCaptionOfPath(elem.Path);
                    graphics.ClearList();
                    FillInList(elem.Path);

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
            else
            {
                elem.Open();
            }
        }               

        private static void CreateWatcher(List_sElement elem)
        {
            System.IO.FileSystemWatcher watcher = new System.IO.FileSystemWatcher();
            watcher.Path = elem.Path;
            watcher.NotifyFilter = System.IO.NotifyFilters.Size | System.IO.NotifyFilters.FileName |
                System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.CreationTime;
            watcher.Filter = "*.*";
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
            watcher.EnableRaisingEvents = true;
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
            //For the dynamics
            ObservableCollection<List_sElement> Data = currentGraphics.DataSource;
            List_sElement elem = null;
            foreach (var item in Data)
            {
                if (item.Path == e.OldFullPath.Replace("/", ""))
                {
                    elem = item;
                    break;
                }
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                //that's it
                elem?.UpdateName(e.FullPath);
                //
            });
        }

        private static void Watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //For the dynamics
            ObservableCollection<List_sElement> Data = currentGraphics.DataSource;
            List_sElement elem = null;
            foreach (var item in Data)
            {
                if (item.Path == e.FullPath)
                {
                    elem = item;
                    break;
                }
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                elem?.UpdateSize();
                
            });
        }

        private static void Watcher_Created(object sender, FileSystemEventArgs e)
        {
            //For the dynamics
            ObservableCollection<List_sElement> Data = currentGraphics.DataSource;
            List_sElement elem = null;
            if (Directory.Exists(e.FullPath))
            {
                elem = new Folder(e.FullPath);
            }
            else if (System.IO.File.Exists(e.FullPath))
            {
                elem = new File(e.FullPath);
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                //that's it
                if (elem != null)
                {
                    Data.Add(elem);
                }
                //
            });
        }

        private static void Watcher_Deleted(object sender, FileSystemEventArgs e)
        {
            //For the dynamics
            ObservableCollection<List_sElement> Data = currentGraphics.DataSource;
            //that's it
            List_sElement elem = null;            
            foreach (var item in Data)
            {
                if (item.Path == e.FullPath)
                {
                    elem = item;
                    break;
                }
            }
            dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (ThreadStart)delegate ()
            {
                Data.Remove(elem);
            });
            //
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

        internal static void RenameFile(object v, string text)
        {
            List_sElement elem = v as List_sElement;
            string sourcePath = elem.Path;
            string destinationPath = Path.Combine(sourcePath.Remove(sourcePath.LastIndexOf(@"\")), text);

            try
            {
                if (elem is File)
                {
                    System.IO.File.Move(sourcePath, destinationPath);
                }
                else
                       if (elem is Folder)
                {
                    System.IO.Directory.Move(sourcePath, destinationPath);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void CutElem(object elem)
        {
            try
            {
                CopyElem(elem);
                DeleteElem(elem);
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
                string path = System.IO.Path.Combine(graphics.Path, DataToCopy.Name);
                try
                {
                    DataToCopy.Paste(path, buffer);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }                
            }
        }

        internal static void DeleteElem(object elem)
        {
            try
            {
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
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

