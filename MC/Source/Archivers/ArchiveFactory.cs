using System;
using MC.Source.Threading;

namespace MC.Source.Archivers
{
    public class ArchiveFactory : IThreadFactory
    {
        public IThreder CreateObject(string type, string filePath)
        {
            FileArchiver fileArchiver;
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
