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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Collections;
using System.IO.Compression;

namespace MC
{
    public delegate void ActionWithThread(string pFile);
    public delegate void PackFiles(string sPath);
    public delegate void ThreadProcess(InThreadProcess process);
    public delegate void InThreadProcess(object threads);
    static class LogicForUI
    {        
        //List for ListViewSource
        //it is really faster
        static List<List_sElement> DataList;
        static private void FillInList(string path)
        {
            //must be faster
            CreateDataList(path);
            //enumerate folder's path
            foreach (var item in Directory.EnumerateDirectories(path))
            {
                DataList.Add(new Folder(item));
            }
            //enumerate file's path
            foreach (var item in Directory.EnumerateFiles(path))
            {
                DataList.Add(new File(item));
            }
        }

        private static void CreateDataList(string path)
        {
            DataList = new List<List_sElement>(500);
            // ... folder
            if (path.Length > 3)
            {
                string parentPath = Directory.GetParent(path).FullName;
                DataList.Add(new Folder(parentPath) { Name = "...", Date = "", Size = "" });
            }
        }       

        private static Dispatcher dispatcher;
        static public void OpenElem(object o, GraphicalApp graphics, Dispatcher disp)
        {
            currentGraphics = graphics;
            dispatcher = disp;
            List_sElement elem = o as List_sElement;
            try
            {
                if (elem is Folder || elem is Drive)
                {
                    //Assign a path of watcher
                    ChangePathOfWatcher(elem.Path, graphics.Path);
                    //start fill            

                    graphics.SetCaptionOfPath(elem.Path);
                    //test for two list on the same path
                    if (graphics1.Path == graphics2.Path)
                    {
                        if (graphics1 == graphics)
                        {
                            graphics1.DataSource = graphics2.DataSource;
                        }
                        else
                            graphics2.DataSource = graphics1.DataSource;
                    }
                    else
                        FillInList(elem.Path);
                }
                else
                {
                    elem.Open();
                }
            }
            catch (UnauthorizedAccessException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (FileNotFoundException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                CreateDataList(elem.Path);
            }
            finally
            {
                //test for two list on the same path
                if (graphics1.Path != graphics2.Path)
                    graphics.DataSource = new ObservableCollection<List_sElement>(DataList);
            }

        }


        private static void ChangePathOfWatcher(string path, string oldPath)
        {
            if (watcher1.Path == oldPath)
            {
                watcher1.Path = path;
                watcher1.EnableRaisingEvents = true;
            }
            else
            {
                watcher2.Path = path;
                watcher2.EnableRaisingEvents = true;
            }
            //test for two list on the same path
            if (graphics1.Path == graphics2.Path)
            {
                watcher1 = watcher2;
            }
        }

        private static FileSystemWatcher watcher1;
        private static FileSystemWatcher watcher2;
        private static GraphicalApp graphics1;
        private static GraphicalApp graphics2;
        private static GraphicalApp currentGraphics;
        public static void CreateWatchersAndSetGraphics(GraphicalApp graphics1, GraphicalApp graphics2)
        {
            LogicForUI.graphics1 = graphics1;
            LogicForUI.graphics2 = graphics2;
            watcher1 = CreateWatcher();
            watcher2 = CreateWatcher();
        }

        private static FileSystemWatcher CreateWatcher()
        {
            FileSystemWatcher watcher = new FileSystemWatcher();
            watcher.NotifyFilter = System.IO.NotifyFilters.Size | System.IO.NotifyFilters.FileName |
                System.IO.NotifyFilters.DirectoryName | System.IO.NotifyFilters.CreationTime;
            watcher.Filter = "*.*";
            watcher.Changed += Watcher_Changed;
            watcher.Created += Watcher_Created;
            watcher.Deleted += Watcher_Deleted;
            watcher.Renamed += new RenamedEventHandler(Watcher_Renamed);
            return watcher;
        }

        private static void Watcher_Renamed(object sender, RenamedEventArgs e)
        {
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
                if (elem != null)
                {
                    elem.UpdateName(e.FullPath);
                    //test for two list on the same path
                    if (graphics1.Path == graphics2.Path)
                    {
                        graphics1.Refresh();
                        graphics2.Refresh();
                    }
                    else
                        currentGraphics.Refresh();
                }
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
                if (elem != null)
                {
                    elem.UpdateSize();
                    //test for two list on the same path
                    if (graphics1.Path == graphics2.Path)
                    {
                        graphics1.Refresh();
                        graphics2.Refresh();
                    }
                    else
                        currentGraphics.Refresh();
                }

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

        internal static void SearchInThread(string fromPath, ThreadProcess StartThread)
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "All Files | *.* ";
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = "txt";
            //getting full file name, where we'll save the txt
            if (fileDialog.ShowDialog() == true)
            {
                passport = new List<Group>();
                number = new List<Group>();
                TIN = new List<Group>();
                PIN = new List<Group>();
                email = new List<Group>();
                ftp = new List<Group>();
                vk = new List<Group>();
                exeptions = new StringBuilder();
                MessageBoxResult messageBoxResult = System.Windows.MessageBox.Show("Would you like to open the file after the search is complete?",
                        "Are you sure?", System.Windows.MessageBoxButton.YesNo);
                string fileName = fileDialog.FileName;
                StartThread((item)=>
                {
                    var threads = item as Classes.Threading.ThreadQueue[];

                    for (; ; Thread.Sleep(3000))
                    {
                        int count = 0;
                        for (int i = 0; i < threads.Length; i++)
                        {
                            if (threads[i].TheThread.ThreadState == System.Threading.ThreadState.Stopped)
                            {
                                count++;
                            }
                        }
                        if (count == threads.Length)
                        {
                            break;
                        }
                    }
                    using (StreamWriter writer = new StreamWriter(fileName))
                    {
                        writer.Write(WriteInStream().ToString());
                    }
                    MessageBox.Show(exeptions.ToString(),
                            "Attention!", MessageBoxButton.OK);
                    if (messageBoxResult == MessageBoxResult.Yes)
                    {
                        try
                        {
                            Process.Start(fileName);
                        }
                        catch (Exception e)
                        {
                            MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                        }
                    }
                });                
            }
        }

        
        public static bool isFree = false;
        internal static void Closing()
        {
            if (threads != null && !isFree)
            {
                throw new Exception("The action is not finished. Close the program is impossible.");
            }
        }

        private static StringBuilder WriteInStream()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Passport \r\n \r\n");
            for (int i = 0; i < passport.Count; i++)
            {
                sb.Append(passport[i].ToString() + " \r\n");
            }
            sb.Append("Number \r\n \r\n");
            for (int i = 0; i < number.Count; i++)
            {
                sb.Append(number[i].ToString() + " \r\n");
            }
            sb.Append("TIN \r\n \r\n");
            for (int i = 0; i < TIN.Count; i++)
            {
                sb.Append(TIN[i].ToString() + " \r\n");
            }
            sb.Append("PIN \r\n \r\n");
            for (int i = 0; i < PIN.Count; i++)
            {
                sb.Append(PIN[i].ToString() + " \r\n");
            }
            sb.Append("Email \r\n \r\n");
            for (int i = 0; i < email.Count; i++)
            {
                sb.Append(email[i].ToString() + " \r\n");
            }
            sb.Append("FTP \r\n \r\n");
            for (int i = 0; i < ftp.Count; i++)
            {
                sb.Append(ftp[i].ToString() + " \r\n");
            }
            sb.Append("VK \r\n \r\n");
            for (int i = 0; i < vk.Count; i++)
            {
                sb.Append(vk[i].ToString() + " \r\n");
            }
            return sb;
        }

        const string filter = @"(?x)(?i)(?m)(?'passport'\b\d{2}\s\d{2}\s\d{4}\b)|
                                    (?'number'(?<=\s)\+?[78]\s ?\d{3}\s?\d{3}[\s-]?\d{2}[\s-]?\d{2}\b)|
                                    (?'TIN'\b\d{12}\b)|
                                    (?'PIN'\b\d{3}\-\d{3}\-\d{3}\s\d{2}\b)|
                                    (?'email'\b(\w|\p{P})+(?=@)(\w|\p{P})\w+\.\w+\b)|
                                    (?'ftp'\bftps?:\/\/?(\w|\p{P})*\b)|
                                    (?'vk'\b(https?:\/\/)?vk\.com\/?(\w|\p{P})*\b)";
        private static Regex regex = new Regex(filter);
        private static List<Group> passport = new List<Group>();
        private static List<Group> number = new List<Group>();
        private static List<Group> TIN = new List<Group>();
        private static List<Group> PIN = new List<Group>();
        private static List<Group> email = new List<Group>();
        private static List<Group> ftp = new List<Group>();
        private static List<Group> vk = new List<Group>();
        private static StringBuilder exeptions = new StringBuilder();
        private static void SearchAndSave(string filePath)
        {
            try
            {
                string text = "";
                try
                {
                    using (StreamReader reader = new StreamReader(filePath))
                    {
                        text = reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    exeptions.Append(String.Format("{0} is not read, because: {1}\n", filePath, e.Message));
                }
                if (text != default(string))
                {
                    foreach (Match match in regex.Matches(text))
                    {
                        Group group = match.Groups["passport"];
                        if (group.ToString() != "")
                        {
                            passport.Add(group);
                        }
                        group = match.Groups["number"];
                        if (group.ToString() != "")
                        {
                            number.Add(group);
                        }
                        group = match.Groups["TIN"];
                        if (group.ToString() != "")
                        {
                            TIN.Add(group);
                        }
                        group = match.Groups["PIN"];
                        if (group.ToString() != "")
                        {
                            PIN.Add(group);
                        }
                        group = match.Groups["email"];
                        if (group.ToString() != "")
                        {
                            email.Add(group);
                        }
                        group = match.Groups["ftp"];
                        if (group.ToString() != "")
                        {
                            ftp.Add(group);
                        }
                        group = match.Groups["vk"];
                        if (group.ToString() != "")
                        {
                            vk.Add(group);
                        }
                    }

                }
            }

            catch (UnauthorizedAccessException e)
            {
                exeptions.Append(String.Format("{0} is not read, because: {1}\n", filePath, e.Message));
            }
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

        static public ObservableCollection<List_sElement> FillTheListBoxWithDrives(DriveInfo[] drives)
        {
            ObservableCollection<List_sElement> drivesElem = new ObservableCollection<List_sElement>();
            foreach (DriveInfo info in drives)
            {
                switch (info.DriveType.ToString())
                {
                    case "Fixed":
                        drivesElem.Add(new Fixed(info));
                        break;
                    case "CDRom":
                        drivesElem.Add(new CDRom(info));
                        break;
                    case "Network":
                        throw new NotImplementedException();
                    case "NoRootDirectory":
                        throw new NotImplementedException();
                    case "Ram":
                        throw new NotImplementedException();
                    case "Removable":
                        drivesElem.Add(new Removable(info));
                        break;
                    case "Unknown":
                        throw new NotImplementedException();
                }
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

        private static void UnarchiveElemInThread(List_sElement item)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.Windows.Forms.DialogResult.OK == folderDialog.ShowDialog())
            {
                item.Unarchive(folderDialog.SelectedPath);
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

        internal static void ThreadOperation(object item)
        {
            if (item is string)
            {                    
                var path = item as string;
                SearchInThread(path, (process)=>
                {
                    Queue<string>[] filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
                    Classes.Threading.ThreadQueue[] threads = new Classes.Threading.ThreadQueue[filesQueue.Length];
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        threads[i] = new Classes.Threading.ThreadQueue(filesQueue[i], SearchAndSave);
                        threads[i].BeginProcessData();
                    }
                    Thread processThread = new Thread(new ParameterizedThreadStart(process));
                    processThread.Start(threads);
                });
            }
            else
            {
                ArchiveOrUnarchiveElemInThread(item as List_sElement);
            }
        }
        public static int countOfCompliteThread = 0;
        public static Classes.Threading.ThreadQueue[] threads { get; private set; }
        private static ZipArchive archive;        
        private static void ArchiveOrUnarchiveElemInThread(List_sElement elem)
        {
            try
            {
                if (Regex.IsMatch(System.IO.Path.GetExtension(elem.Path), @"\w*\.(RAR|ZIP|GZ|TAR)"))
                {
                    UnarchiveElemInThread(elem);
                }
                else
                {
                    ArchiveElemInThread(elem.Path);
                }
            }
            catch (ArgumentException e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private static object lockToken = new object();
        private static void ArchiveElemInThread(string pElem)
        {
            isFree = false;
            Queue<string>[] filesQueue = CreateAndFillQueue(pElem, new PackFiles(GetFilesPathFromFolder));
            var pZip = GetPathOnDialog();
            var zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
            archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            threads = new Classes.Threading.ThreadQueue[filesQueue.Length];
            Classes.Threading.ThreadQueue.ThreadingComplite += CompressComplite;
            for (int i = 0; i < filesQueue.Length; i++)
            {
                threads[i] = new Classes.Threading.ThreadQueue(filesQueue[i], (pFile) =>
                {
                    int BufferSize = 16384;
                    byte[] byteBuffer = new byte[BufferSize];
                    ZipArchiveEntry fileEntry;
                    lock (lockToken)
                    {
                        fileEntry = archive.CreateEntry(pFile.Substring(pElem.Length + 1)); 
                    }
                    using (Stream inFileStream = System.IO.File.Open(pFile, FileMode.Open, FileAccess.Read, FileShare.Read))
                    {
                        using (Stream writer = fileEntry.Open())
                        {
                            int bytesRead = 0;
                            do
                            {
                                bytesRead = inFileStream.Read(byteBuffer, 0, BufferSize);
                                writer.Write(byteBuffer, 0, bytesRead);
                            } while (bytesRead > 0);
                        }
                    }
                });
                threads[i].BeginProcessData();
            }
        }

        private static void CompressComplite(object sender, EventArgs e)
        {
            archive.Dispose();
            threads = null;
            countOfCompliteThread = 0;
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
        }

        private const string  ARCHIVEEXTENSION = "RAR";
        private static string GetPathOnDialog()
        {
            SaveFileDialog fileDialog = new SaveFileDialog();
            fileDialog.Filter = "All Files | *.* ";
            fileDialog.AddExtension = true;
            fileDialog.DefaultExt = ARCHIVEEXTENSION;
            //getting full file name, where we'll save the archive
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            throw new ArgumentException("Pick folder pls.");
        }

        private static Queue<string>[] CreateAndFillQueue(string path, PackFiles GetFiles)
        {
            int countOfProcessor = Environment.ProcessorCount;
            Queue<string>[] filesQueue = new Queue<string>[countOfProcessor];
            for (int i = 0; i < countOfProcessor; i++)
            {
                filesQueue[i] = new Queue<string>();
            }
            listOfPath = new List<string>();
            GetFiles(path);
            int k = 0;
            while (k < listOfPath.Count)
            {
                for (int j = 0; j < countOfProcessor && k < listOfPath.Count; j++)
                {
                    filesQueue[j].Enqueue(listOfPath[k++]);
                }
            }
            return filesQueue;
        }        
                
        private static List<string> listOfPath;
        private static void GetFilesPathFromFolder(string path)
        {
            try
            {
                foreach (string item in Directory.GetFiles(path))
                {
                    listOfPath.Add(item);
                }
                foreach (string item in Directory.GetDirectories(path))
                {
                    GetFilesPathFromFolder(item);
                }
            }
            catch (UnauthorizedAccessException e)
            {
                exeptions.Append(String.Format("{0} is not read, because: {1}\n", path, e.Message));
            }
        }        
    }
}

