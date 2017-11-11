using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MC.Abstract_and_Parent_Classes;
using MC.Classes.Threading.TaskClasses;

namespace MC.Classes.Visitors
{
    class TaskVisitor : IVisitor
    {
        public void Archive(File file)
        {
            var archiver = new FileArchiverInTask(file.Path);
            archiver.DoThread();
        }

        public void Archive(Directory directory)
        {
            var archiver = new FileArchiverInTask(directory.Path);
            archiver.DoThread();
        }

        public void Search(File file)
        {
            var searcherBase = new SearchByPattern(file.Path, new SearchByPatternInTask());
            searcherBase.DoThread();
        }

        public void Search(Directory directory)
        {
            var searcherBase = new SearchByPattern(directory.Path, new SearchByPatternInTask());
            searcherBase.DoThread();
        }
    }
}
