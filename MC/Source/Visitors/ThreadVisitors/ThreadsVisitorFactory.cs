using System;

namespace MC.Source.Visitors
{
    static class ThreadsVisitorFactory
    {
        public static IThreadsVisitor CreateVisitor(string typeOfThread)
        {
            switch (typeOfThread)
            {
                case "Thread":
                    return new ThreadVisitor();
                case "Parralel":
                    return new ParallelVisitor();
                case "Tasks":
                    return new TaskVisitor();
                case "Async":
                    return new AsyncVisitor();
                default:
                    throw new ArgumentException();
            }
        }
    }
}
