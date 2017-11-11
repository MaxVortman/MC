using MC.Abstract_and_Parent_Classes;
using MC.Classes.Threading.AsyncClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes.Visitors
{
    class AsyncVisitor : IVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverAsync(file.Path);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverAsync(directory.Path);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcherBase = new SearchByPattern(file.Path, new SearchByPatternAsync());
            searcherBase.DoThread();
        }

        public void Search(Directory directory)
        {
            var searcherBase = new SearchByPattern(directory.Path, new SearchByPatternAsync());
            searcherBase.DoThread();
        }
    }
}
