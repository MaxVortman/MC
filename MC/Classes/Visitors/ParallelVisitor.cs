using MC.Abstract_and_Parent_Classes;
using MC.Classes.Threading.ParallelClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Classes.Visitors
{
    class ParallelVisitor : IVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverParallel(file.Path);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverParallel(directory.Path);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcher = new SearchByPatternParallel();
            var searcherBase = new SearchByPattern(file.Path, searcher);
        }

        public void Search(Directory directory)
        {
            var searcher = new SearchByPatternParallel();
            var searcherBase = new SearchByPattern(directory.Path, searcher);
        }
    }
}
