using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MC.Source.QueueCreators
{
    public class QueueCreator <T> where T : class
    {
        protected List<T> _listOfPath;

        public Queue<T>[] GetFilledQueueOfFilesPath()
        {
            var countOfProcessor = Environment.ProcessorCount;
            var filesQueue = new Queue<T>[countOfProcessor];
            for (int i = 0; i < countOfProcessor; i++)
            {
                filesQueue[i] = new Queue<T>();
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
    }
}
