using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using MC.Abstract_and_Parent_Classes;
using MC.Classes.Drives;
using MC.Classes.Graphics;
using Microsoft.Win32;
using System.Threading.Tasks;

namespace MC.Classes
{
    public delegate void ActionWithThread(string pFile);
    public delegate void PackFiles(string sPath);
    public delegate void ThreadProcess(InThreadProcess process);
    public delegate void InThreadProcess();

    internal static class LogicForUi
    {


        public static async Task<string> ReadStatisticAsync(object selectedItem)
        {
            var path = (selectedItem as ListSElement).Path;
            Statistics stat = new NonParallelStatistics(path);
            var result = await stat.GetStatisticsAsync();
            stat = new ParallelStatistics(path);
            result += await stat.GetStatisticsAsync();
            return result;
        }


        private static void SearchInThread(string fromPath, ThreadProcess StartThread)
        {
            var fileDialog = new SaveFileDialog
            {
                Filter = "All Files | *.* ",
                AddExtension = true,
                DefaultExt = "txt"
            };
            //getting full file name, where we'll save the txt
            if (fileDialog.ShowDialog() == true)
            {
                _passport = new List<Group>();
                _number = new List<Group>();
                _tin = new List<Group>();
                _pin = new List<Group>();
                _email = new List<Group>();
                _ftp = new List<Group>();
                _vk = new List<Group>();
                _exeptions = new StringBuilder();
                var messageBoxResult = System.Windows.MessageBox.Show("Would you like to open the file after the search is complete?",
                        "Are you sure?", System.Windows.MessageBoxButton.YesNo);
                var fileName = fileDialog.FileName;
                StartThread(() =>
                {
                    using (var writer = new StreamWriter(fileName))
                    {
                        writer.Write(WriteInStream().ToString());
                    }
                    MessageBox.Show(_exeptions.ToString(),
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


        public static bool IsFree = false;
        internal static void Closing()
        {
            if (Threads != null && !IsFree)
            {
                throw new Exception("The action is not finished. Close the program is impossible.");
            }
        }

        private static StringBuilder WriteInStream()
        {
            var sb = new StringBuilder();
            sb.Append("Passport \r\n \r\n");
            for (int i = 0; i < _passport.Count; i++)
                sb.Append($"{_passport[i]} \r\n");
            sb.Append("Number \r\n \r\n");
            for (int i = 0; i < _number.Count; i++)
                sb.Append($"{_number[i]} \r\n");
            sb.Append("TIN \r\n \r\n");
            for (int i = 0; i < _tin.Count; i++)
                sb.Append($"{_tin[i]} \r\n");
            sb.Append("PIN \r\n \r\n");
            for (int i = 0; i < _pin.Count; i++)
                sb.Append($"{_pin[i]} \r\n");
            sb.Append("Email \r\n \r\n");
            for (int i = 0; i < _email.Count; i++)
                sb.Append($"{_email[i]} \r\n");
            sb.Append("FTP \r\n \r\n");
            for (int i = 0; i < _ftp.Count; i++)
                sb.Append($"{_ftp[i]} \r\n");
            sb.Append("VK \r\n \r\n");
            for (int i = 0; i < _vk.Count; i++)
                sb.Append($"{_vk[i]} \r\n");
            return sb;
        }

        private const string Filter = @"(?x)(?i)(?m)(?'passport'\b\d{2}\s\d{2}\s\d{4}\b)|
                                    (?'number'(?<=\s)\+?[78]\s ?\d{3}\s?\d{3}[\s-]?\d{2}[\s-]?\d{2}\b)|
                                    (?'TIN'\b\d{12}\b)|
                                    (?'PIN'\b\d{3}\-\d{3}\-\d{3}\s\d{2}\b)|
                                    (?'email'\b(\w|\p{P})+(?=@)(\w|\p{P})\w+\.\w+\b)|
                                    (?'ftp'\bftps?:\/\/?(\w|\p{P})*\b)|
                                    (?'vk'\b(https?:\/\/)?vk\.com\/?(\w|\p{P})*\b)";
        private static readonly Regex Regex = new Regex(Filter);
        private static List<Group> _passport = new List<Group>();
        private static List<Group> _number = new List<Group>();
        private static List<Group> _tin = new List<Group>();
        private static List<Group> _pin = new List<Group>();
        private static List<Group> _email = new List<Group>();
        private static List<Group> _ftp = new List<Group>();
        private static List<Group> _vk = new List<Group>();
        private static StringBuilder _exeptions = new StringBuilder();
        private static void SearchAndSave(string filePath)
        {
            try
            {
                var text = "";
                try
                {
                    using (var reader = new StreamReader(filePath))
                    {
                        text = reader.ReadToEnd();
                    }
                }
                catch (Exception e)
                {
                    _exeptions.Append($"{filePath} is not read, because: {e.Message}\n");
                }
                foreach (Match match in Regex.Matches(text))
                {
                    var group = match.Groups["passport"];
                    if (@group.ToString() != "")
                    {
                        _passport.Add(@group);
                    }
                    @group = match.Groups["number"];
                    if (@group.ToString() != "")
                    {
                        _number.Add(@group);
                    }
                    @group = match.Groups["TIN"];
                    if (@group.ToString() != "")
                    {
                        _tin.Add(@group);
                    }
                    @group = match.Groups["PIN"];
                    if (@group.ToString() != "")
                    {
                        _pin.Add(@group);
                    }
                    @group = match.Groups["email"];
                    if (@group.ToString() != "")
                    {
                        _email.Add(@group);
                    }
                    @group = match.Groups["ftp"];
                    if (@group.ToString() != "")
                    {
                        _ftp.Add(@group);
                    }
                    @group = match.Groups["vk"];
                    if (@group.ToString() != "")
                    {
                        _vk.Add(@group);
                    }
                }
            }

            catch (UnauthorizedAccessException e)
            {
                _exeptions.Append($"{filePath} is not read, because: {e.Message}\n");
            }
        }

       
        public static ObservableCollection<ListSElement> FillTheListBoxWithDrives(DriveInfo[] drives)
        {
            var drivesElem = new ObservableCollection<ListSElement>();
            foreach (var info in drives)
            {
                switch (info.DriveType.ToString())
                {
                    case "Fixed":
                        drivesElem.Add(new Fixed(info));
                        break;
                    case "CDRom":
                        drivesElem.Add(new CdRom(info));
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

        static Buffer _buffer;

        private static ListSElement _dataToCopy;

        internal static void CopyElem(object elem)
        {
            DeleteTemp(_buffer);

            var item = elem as ListSElement;
            try
            {
                _buffer = item.Copy();
                _dataToCopy = item;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void UnarchiveElemInThread(ListSElement item)
        {
            System.Windows.Forms.FolderBrowserDialog folderDialog = new System.Windows.Forms.FolderBrowserDialog();
            if (System.Windows.Forms.DialogResult.OK == folderDialog.ShowDialog())
            {
                item.Unarchive(folderDialog.SelectedPath);
            }
        }

        private static void DeleteTemp(Buffer buffer)
        {
            if (buffer == null) return;
            if (buffer is FileBuffer)
            {
                var fileBuffer = buffer as FileBuffer;
                System.IO.File.Delete(fileBuffer.TempPath);
            }
            else
            {
                var folderBuffer = buffer as FolderBuffer;
                foreach (var item in folderBuffer.FoldersBuffer)
                {
                    DeleteTemp(item);
                }
            }
        }

        internal static void RenameFile(object v, string text)
        {
            var elem = v as ListSElement;
            var sourcePath = elem.Path;
            var destinationPath = Path.Combine(sourcePath.Remove(sourcePath.LastIndexOf(@"\", StringComparison.Ordinal)), text);

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
            if (_buffer == null) return;
            var path = System.IO.Path.Combine(graphics.Path, _dataToCopy.Name);
            try
            {
                _dataToCopy.Paste(path, _buffer);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        internal static void DeleteElem(object elem)
        {
            try
            {
                var item = elem as ListSElement;
                var path = item.Path;
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
                SearchInThread(path, (process) =>
                {
                    var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
                    var threads = new Classes.Threading.ThreadQueue[filesQueue.Length];
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        threads[i] = new Classes.Threading.ThreadQueue(filesQueue[i], SearchAndSave);
                        threads[i].BeginProcessData();
                    }

                    var waitingThread = new Thread(() =>
                    {
                        for (; ; Thread.Sleep(3000))
                        {
                            var count = 0;
                            for (var i = 0; i < threads.Length; i++)
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

                        var processThread = new Thread(new ThreadStart(process));
                        processThread.Start();
                    });
                    waitingThread.Start();
                });
            }
            else
            {
                ArchiveOrUnarchiveElemInThread(item as ListSElement);
            }
        }
        public static int CountOfCompliteThread = 0;
        public static Classes.Threading.ThreadQueue[] Threads { get; private set; }
        private static ZipArchive _archive;
        private static void ArchiveOrUnarchiveElemInThread(ListSElement elem)
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
        private static readonly object LockToken = new object();
        private static void ArchiveElemInThread(string pElem)
        {
            LogicForUi.pElem = pElem;
            IsFree = false;
            var filesQueue = CreateAndFillQueue(pElem, new PackFiles(GetFilesPathFromFolder));
            var pZip = GetPathOnDialog();
            var zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
            _archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            Threads = new Classes.Threading.ThreadQueue[filesQueue.Length];
            Classes.Threading.ThreadQueue.ThreadingComplite += CompressComplite;
            for (int i = 0; i < filesQueue.Length; i++)
            {
                Threads[i] = new Classes.Threading.ThreadQueue(filesQueue[i], new ActionWithThread(Archive));
                Threads[i].BeginProcessData();
            }
        }
        private static string pElem;
        private static void Archive(string pFile)
        {
            var BufferSize = 16384;
            var byteBuffer = new byte[BufferSize];
            ZipArchiveEntry fileEntry;
            lock (LockToken)
            {
                fileEntry = _archive.CreateEntry(pFile.Substring(pElem.Length + 1));
            }
            using (var inFileStream = System.IO.File.Open(pFile, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                using (var writer = fileEntry.Open())
                {
                    var bytesRead = 0;
                    do
                    {
                        bytesRead = inFileStream.Read(byteBuffer, 0, BufferSize);
                        writer.Write(byteBuffer, 0, bytesRead);
                    } while (bytesRead > 0);
                }
            }
        }

        private static void CompressComplite(object sender, EventArgs e)
        {
            _archive.Dispose();
            Threads = null;
            CountOfCompliteThread = 0;
            GC.Collect(2);
            GC.WaitForPendingFinalizers();
        }

        private const string Archiveextension = "RAR";
        private static string GetPathOnDialog()
        {
            var fileDialog = new SaveFileDialog
            {
                Filter = "All Files | *.* ",
                AddExtension = true,
                DefaultExt = Archiveextension
            };
            //getting full file name, where we'll save the archive
            if (fileDialog.ShowDialog() == true)
            {
                return fileDialog.FileName;
            }
            throw new ArgumentException("Pick folder pls.");
        }

        private static Queue<string>[] CreateAndFillQueue(string path, PackFiles GetFiles)
        {
            var countOfProcessor = Environment.ProcessorCount;
            var filesQueue = new Queue<string>[countOfProcessor];
            for (int i = 0; i < countOfProcessor; i++)
            {
                filesQueue[i] = new Queue<string>();
            }
            _listOfPath = new List<string>();
            GetFiles(path);
            var k = 0;
            while (k < _listOfPath.Count)
            {
                for (int j = 0; j < countOfProcessor && k < _listOfPath.Count; j++)
                {
                    filesQueue[j].Enqueue(_listOfPath[k++]);
                }
            }
            return filesQueue;
        }

        private static List<string> _listOfPath;
        private static void GetFilesPathFromFolder(string path)
        {
            try
            {
                Parallel.ForEach(Directory.GetFiles(path), item =>
                {
                    _listOfPath.Add(item);
                });
                Parallel.ForEach(Directory.GetDirectories(path), item =>
                {
                    GetFilesPathFromFolder(item);
                });
            }
            catch (UnauthorizedAccessException e)
            {
                _exeptions.Append($"{path} is not read, because: {e.Message}\n");
            }
        }

        internal static void ParallelOperation(object item)
        {
            if (item is string)
            {
                var path = item as string;
                SearchInThread(path, (process) =>
                {
                    Task.Factory.StartNew(() =>
                    {
                        var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
                        ParallelLoopResult result = Parallel.ForEach(filesQueue, currentQueue =>
                        {
                            for (int i = 0; i < currentQueue.Count; i++)
                            {
                                SearchAndSave(currentQueue.Dequeue());
                            }
                        });
                        if (result.IsCompleted)
                        {
                            process();
                        }
                    });
                });
            }
            else
            {
                var elem = item as ListSElement;
                try
                {
                    if (Regex.IsMatch(System.IO.Path.GetExtension(elem.Path), @"\w*\.(RAR|ZIP|GZ|TAR)"))
                    {
                        UnarchiveElemInThread(elem);
                    }
                    else
                    {
                        ArchiveElemInParallel(elem.Path);
                    }
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private static void ArchiveElemInParallel(string path)
        {
            pElem = path;
            var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
            var pZip = GetPathOnDialog();
            var zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
            _archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            Task.Factory.StartNew(() =>
            {
                var result = Parallel.ForEach(filesQueue, currentQueue =>
                {
                    for (int i = 0; i < currentQueue.Count; i++)
                    {
                        Archive(currentQueue.Dequeue());
                    }
                });
                if (result.IsCompleted)
                {
                    _archive.Dispose();
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            });
        }

        internal static void TasksOperation(object item)
        {
            if (item is string)
            {
                var path = item as string;
                SearchInThread(path, process =>
                {
                    var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
                    var tasks = new Task[filesQueue.Length];
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        var queue = filesQueue[i];
                        tasks[i] = Task.Run(() =>
                        {
                            for (int j = 0; j < queue.Count; j++)
                            {
                                SearchAndSave(queue.Dequeue());
                            }
                        });
                    }
                    Task.Run(() =>
                    {
                        if (tasks.IsComplite())
                        {
                            process();
                        }
                    });
                });
            }
            else
            {
                var elem = item as ListSElement;
                try
                {
                    if (Regex.IsMatch(System.IO.Path.GetExtension(elem.Path), @"\w*\.(RAR|ZIP|GZ|TAR)"))
                    {
                        UnarchiveElemInThread(elem);
                    }
                    else
                    {
                        ArchiveElemInTasks(elem.Path);
                    }
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private static void ArchiveElemInTasks(string path)
        {
            pElem = path;
            var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
            var pZip = GetPathOnDialog();
            var zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
            _archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
            var tasks = new Task[filesQueue.Length];
            for (int i = 0; i < filesQueue.Length; i++)
            {
                var queue = filesQueue[i];
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < queue.Count; j++)
                    {
                        Archive(queue.Dequeue());
                    }
                });
            }
            Task.Run(() =>
            {
                if (tasks.IsComplite())
                {
                    _archive.Dispose();
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            });
        }

        private static bool IsComplite(this Task[] tasks)
        {
            for (int i = 0; i < tasks.Length; i++)
            {
                var awaiter = tasks[i].GetAwaiter();
                awaiter.GetResult();
            }
            return true;
        }

        private static object _searchLock = new object();
        private static CancellationTokenSource seachCTS;
        internal static void AsyncOperation(object item)
        {
            if (item is string)
            {
                var path = item as string;
                SearchInThread(path, async (process) =>
                {
                    seachCTS = new CancellationTokenSource();
                    var seachTC = seachCTS.Token;
                    var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
                    var totalCount = _listOfPath.Count;
                    var ProgressLayout = new Windows.ProgressWindow("Search") { CTS = seachCTS };
                    var progress = new Progress<double>(ProgressLayout.ReportProgress);
                    var realProgres = progress as IProgress<double>;
                    ProgressLayout.Show();
                    await Task.Run(() =>
                                {

                                    var tasks = new Task[filesQueue.Length];
                                    var tempCount = 0;
                                    for (int i = 0; i < filesQueue.Length; i++)
                                    {
                                        var queue = filesQueue[i];
                                        tasks[i] = Task.Run(() =>
                                        {
                                            try
                                            {
                                                for (int j = 0; j < queue.Count; j++)
                                                {
                                                    seachTC.ThrowIfCancellationRequested();
                                                    SearchAndSave(queue.Dequeue());
                                                    //Interlocked.Increment(ref tempCount);
                                                    lock (_searchLock)
                                                        realProgres.Report(++tempCount * 100 / (double)totalCount);
                                                }
                                            }
                                            catch (OperationCanceledException)
                                            {
                                                GC.Collect(2);
                                                GC.WaitForPendingFinalizers();
                                            }
                                        }, seachTC);
                                    }
                                    tasks.IsComplite();
                                });
                    ProgressLayout.Close();
                    process();
                });
            }
            else
            {
                var elem = item as ListSElement;
                try
                {
                    if (Regex.IsMatch(System.IO.Path.GetExtension(elem.Path), @"\w*\.(RAR|ZIP|GZ|TAR)"))
                    {
                        UnarchiveElemInThread(elem);
                    }
                    else
                    {
                        ArchiveElemAsync(elem.Path);
                    }
                }
                catch (ArgumentException e)
                {
                    MessageBox.Show(e.Message, "Error!", MessageBoxButton.OK, MessageBoxImage.Error);
                }

            }
        }

        private static object _archiveLock = new object();
        private static CancellationTokenSource archiveCTS;
        private static int tempCount;
        private static void ArchiveElemAsync(string path)
        {
            archiveCTS = new CancellationTokenSource();
            var archiveCT = archiveCTS.Token;
            var ProgressLayout = new Windows.ProgressWindow("Archive") { CTS = archiveCTS };
            var progress = new Progress<double>(ProgressLayout.ReportProgress);
            var realProgres = progress as IProgress<double>;
            ProgressLayout.Show();
            Task.Run(async () =>
            {
                pElem = path;
                var filesQueue = CreateAndFillQueue(path, new PackFiles(GetFilesPathFromFolder));
                var totalCount = _listOfPath.Count;
                tempCount = 0;
                var pZip = GetPathOnDialog();
                var zipToOpen = new FileStream(pZip, FileMode.Create, FileAccess.ReadWrite, FileShare.Inheritable);
                _archive = new ZipArchive(zipToOpen, ZipArchiveMode.Update);
                var tasks = new Task[filesQueue.Length];                
                await Task.Run(() =>
                {
                    for (int i = 0; i < filesQueue.Length; i++)
                    {
                        var queue = filesQueue[i];
                        tasks[i] = Task.Run(() =>
                        {
                            try
                            {
                                for (int j = 0; j < queue.Count; j++)
                                {
                                    archiveCT.ThrowIfCancellationRequested();
                                    Archive(queue.Dequeue());
                                    lock (_archiveLock)
                                        realProgres.Report(++tempCount * 100 / (double)totalCount);
                                }
                            }
                            catch (OperationCanceledException)
                            {
                                MessageBox.Show("Archive canceled.");                                
                                GC.Collect(2);
                                GC.WaitForPendingFinalizers();
                            }
                        }, archiveCT);
                    }
                    tasks.IsComplite();
                });
                ProgressLayout.Close();
                _archive.Dispose();
                _archive.Dispose();
                GC.Collect(2);
                GC.WaitForPendingFinalizers();
            });            
        }
    }
}


