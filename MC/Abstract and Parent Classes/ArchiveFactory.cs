using MC.Classes.Threading.AsyncClasses;
using MC.Classes.Threading.ParallelClasses;
using MC.Classes.Threading.TaskClasses;
using MC.Classes.Threading.ThreadClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Abstract_and_Parent_Classes
{
    public class ArchiveFactory : IThreadFactory
    {
        public IThreder CreateObject(string type, string filePath)
        {
            IThreder fileArchiver;
            switch (type)
            {
                case "Thread":
                    fileArchiver = new FileArchiverInThread(filePath);
                    break;
                case "Parralel":
                    fileArchiver = new FileArchiverParallel(filePath);
                    break;
                case "Tasks":
                    fileArchiver = new FileArchiverInTask(filePath);
                    break;
                case "Async":
                    fileArchiver = new FileArchiverAsync(filePath);
                    break;
                default:
                    throw new ArgumentException();
            }

            return fileArchiver;
        }
    }
}
