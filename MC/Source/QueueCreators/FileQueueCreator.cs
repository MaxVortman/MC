using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace MC.Source.QueueCreators
{
    class FileQueueCreator : QueueCreator<string>
    {

        public FileQueueCreator(string soursePath)
        {
            _listOfPath = new List<string>();
            if (Directory.Exists(soursePath))                
                _listOfPath.AddRange(Entries.Directory.GetAllFiles(soursePath));
            else
                _listOfPath.Add(soursePath);
        }

        public List<string> ListOfFilesInDirectory
        {
            get { return _listOfPath; }
        }
    }
}