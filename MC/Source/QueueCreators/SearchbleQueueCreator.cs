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
            Zip zip;
            foreach (var fPath in filesPath)
            {
                if (Zip.IsArchive(fPath))
                {
                    zip = new Zip(fPath, File.Open(fPath, FileMode.Open));
                    foreach (var entry in zip.Entries)
                    {
                        _listOfPath.Add(new Entries.Zipped.ZippedFile(zip, new Entries.Zipped.Entry(entry, zip.Path)));
                    }
                }
                else
                    _listOfPath.Add(new Entries.File(fPath));
            }
        }
    }
}
