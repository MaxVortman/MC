using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MC.Classes.Threading
{
    public class ThreadQueue
    {
        public static event EventHandler<EventArgs> ThreadingComplite;
        private Queue<string> queueOfFiles;
        private ActionWithThread func;
        private Thread theThread;
        public Thread TheThread
        {
            get { return theThread; }
        }
        public ThreadQueue(Queue<string> queueOfFiles, ActionWithThread func)
        {            
            this.func = func;
            this.queueOfFiles = queueOfFiles;
            theThread = new Thread(new ThreadStart(this.ThreadFunc));
        }

        private void ThreadFunc()
        {
            Thread.Sleep(50);
            int count = queueOfFiles.Count;
            for (int i = 0; i < count && queueOfFiles.Count > 0; i++)
            {
                func(queueOfFiles.Dequeue());
            }

            if (++LogicForUI.countOfCompliteThread == Environment.ProcessorCount && LogicForUI.threads != null)
            {
                LogicForUI.isFree = true;
                ThreadingComplite.Invoke(this, new EventArgs());
            }
        }

        public void BeginProcessData()
        {
            theThread.Start();
        }

        public void IterruptProcessData()
        {
            theThread.Interrupt();
        }

        public void EndProcessData()
        {
            theThread.Join();
        }

    }
}
