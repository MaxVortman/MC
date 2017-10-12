using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class FileArchiverInThread : FileArchiver
    {
        public FileArchiverInThread(string pathOfFile) : base(pathOfFile)
        {
        }

        public int CountOfCompliteThread = 0;
        public Classes.Threading.ThreadQueue[] Threads { get; private set; }

        public override void Archive()
        {
            IsFree = false;
            Threads = new Threading.ThreadQueue[filesQueue.Length];
            Threading.ThreadQueue.ThreadingComplite += CompressComplite;
            for (int i = 0; i < filesQueue.Length; i++)
            {
                Threads[i] = new Classes.Threading.ThreadQueue(filesQueue[i], new ActionWithThread(ArchiveFileInEntry));
                Threads[i].BeginProcessData();
            }
        }

        private static object lockToken = new object();
        private void CompressComplite(object sender, EventArgs e)
        {
            lock (lockToken)
            {
                if (++CountOfCompliteThread == Environment.ProcessorCount)
                {
                    archive.Dispose();
                    Threads = null;
                    CountOfCompliteThread = 0;
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            }            
        }

        public static bool IsFree = false;

        internal override void Closing()
        {
            if (Threads != null && !IsFree)
            {
                throw new Exception("The action is not finished. Close the program is impossible.");
            }
        }
    }
}
