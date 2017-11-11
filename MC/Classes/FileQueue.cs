using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes
{
    class FileQueueCreator
    {

        public FileQueueCreator(string soursePath)
        {
            this.soursePath = soursePath;
            _listOfPath = new List<string>();
            if (Directory.Exists(soursePath))
                GetFilesPathFromFolder(soursePath);
            else
                _listOfPath.Add(soursePath);
        }

        public Queue<string>[] GetFilledQueueOfFilesPath()
        {
            var countOfProcessor = Environment.ProcessorCount;
            var filesQueue = new Queue<string>[countOfProcessor];
            for (int i = 0; i < countOfProcessor; i++)
            {
                filesQueue[i] = new Queue<string>();
            }
                        
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

        private List<string> _listOfPath;
        private readonly string soursePath;

        public void GetFilesPathFromFolder(string path)
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
    }
}
