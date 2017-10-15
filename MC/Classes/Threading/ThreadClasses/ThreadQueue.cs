using System;
using System.Collections.Generic;
using System.Threading;

namespace MC.Classes.Threading.ThreadClasses
{
    public class ThreadQueue
    {
        public static event EventHandler<EventArgs> ThreadingComplite;
        private readonly Queue<string> _queueOfFiles;
        private readonly ActionWithThread _func;
        public System.Threading.Thread TheThread { get; }

        public ThreadQueue(Queue<string> queueOfFiles, ActionWithThread func)
        {            
            this._func = func;
            this._queueOfFiles = queueOfFiles;
            TheThread = new System.Threading.Thread(new ThreadStart(this.ThreadFunc));
        }

        private void ThreadFunc()
        {
            System.Threading.Thread.Sleep(50);
            var count = _queueOfFiles.Count;
            for (int i = 0; i < count && _queueOfFiles.Count > 0; i++)
            {
                _func(_queueOfFiles.Dequeue());
            }
            
            ThreadingComplite?.Invoke(this, new EventArgs());
        }

        public void BeginProcessData()
        {
            TheThread.Start();
        }

        public void IterruptProcessData()
        {
            TheThread.Interrupt();
        }

        public void EndProcessData()
        {
            TheThread.Join();
        }

    }
}
