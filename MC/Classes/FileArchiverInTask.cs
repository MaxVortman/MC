using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class FileArchiverInTask : FileArchiver
    {
        public FileArchiverInTask(string sourcePathOfFile) : base(sourcePathOfFile)
        {
        }

        public override void Archive()
        {
            var tasks = new Task[filesQueue.Length];
            for (int i = 0; i < filesQueue.Length; i++)
            {
                var queue = filesQueue[i];
                tasks[i] = Task.Run(() =>
                {
                    for (int j = 0; j < queue.Count; j++)
                    {
                        ArchiveFileInEntry(queue.Dequeue());
                    }
                });
            }
            Task.Run(() =>
            {
                if (tasks.IsComplite())
                {
                    archive.Dispose();
                    GC.Collect(2);
                    GC.WaitForPendingFinalizers();
                }
            });
        }

        internal override void Closing()
        {
            //TO DO: 
        }
    }
}
