using MC.Source.Entries.Zipped;
using MC.Source.Searchers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.QueueCreators
{
    public class SearchbleQueueCreator : QueueCreator<ISearchble>
    {

        public SearchbleQueueCreator(string sourcePath)
        {
            _listOfPath = new List<ISearchble>();
            if (Entries.Directory.Exists(sourcePath))
                GetSearchbleFromFolder(sourcePath);
            else
                _listOfPath.Add(new Entries.File(sourcePath));
        }

        private void GetSearchbleFromFolder(string sourcePath)
        {
            var filesPath = Entries.Directory.GetAllFiles(sourcePath);
            foreach (var fPath in filesPath)
            {
                if (Entries.File.IsArchive(fPath))
                {
                    _listOfPath.AddRange(ZipFactory.GetZipEntries(fPath));
                }
                else
                    _listOfPath.Add(new Entries.File(fPath));
            }
        }
    }
}
