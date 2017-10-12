using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class FileArchiverParallel : FileArchiver
    {
        public FileArchiverParallel(string sourcePathOfFile) : base(sourcePathOfFile)
        {
        }

        public override void Archive()
        {            
            Task.Factory.StartNew(() =>
            {
                var result = Parallel.ForEach(filesQueue, currentQueue =>
                {
                    for (int i = 0; i < currentQueue.Count; i++)
                    {
                        ArchiveFileInEntry(currentQueue.Dequeue());
                    }
                });
                if (result.IsCompleted)
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
