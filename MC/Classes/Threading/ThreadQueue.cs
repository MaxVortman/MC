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
        private readonly Queue<string> _queueOfFiles;
        private readonly ActionWithThread _func;
        public Thread TheThread { get; }

        public ThreadQueue(Queue<string> queueOfFiles, ActionWithThread func)
        {            
            this._func = func;
            this._queueOfFiles = queueOfFiles;
            TheThread = new Thread(new ThreadStart(this.ThreadFunc));
        }

        private void ThreadFunc()
        {
            Thread.Sleep(50);
            var count = _queueOfFiles.Count;
            for (int i = 0; i < count && _queueOfFiles.Count > 0; i++)
            {
                _func(_queueOfFiles.Dequeue());
            }

            if (++LogicForUi.CountOfCompliteThread != Environment.ProcessorCount || LogicForUi.Threads == null) return;
            LogicForUi.IsFree = true;
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
