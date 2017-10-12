using MC.Abstract_and_Parent_Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MC.Classes
{
    class SearchByPatternInThread : SearchByPattern
    {
        public SearchByPatternInThread(string path) : base(path)
        {
        }

        public override void Search()
        {            
            SearchInThread((process) =>
            {
                var threads = new Classes.Threading.ThreadQueue[filesQueue.Length];
                for (int i = 0; i < filesQueue.Length; i++)
                {
                    threads[i] = new Classes.Threading.ThreadQueue(filesQueue[i], SearchAndSaveIn);
                    threads[i].BeginProcessData();
                }

                var waitingThread = new Thread(() =>
                {
                    for (; ; Thread.Sleep(3000))
                    {
                        var count = 0;
                        for (var i = 0; i < threads.Length; i++)
                        {
                            if (threads[i].TheThread.ThreadState == System.Threading.ThreadState.Stopped)
                            {
                                count++;
                            }
                        }
                        if (count == threads.Length)
                        {
                            break;
                        }
                    }

                    var processThread = new Thread(new ThreadStart(process));
                    processThread.Start();
                });
                waitingThread.Start();
            });
        }
    }
}
