using System;
using System.Threading;

namespace SteamIDScanner
{
    public class ThreadController
    {
        public static readonly int MaximumThreads = Environment.ProcessorCount - 4;

        public ThreadController(Action action)
        {
            if (AvailableThreads > 1)
            {
                ActiveThread = new Thread(action.Invoke);
                ExecuteType = ExecuteMethod.Thread;
                TotalExecutedThreads++;
            }
            else
            {
                action.Invoke();
                ExecuteType = ExecuteMethod.Action;
                TotalExecutedActions++;
            }
        }

        public static int TotalExecutedThreads { get; set; }
        public static int TotalExecutedActions { get; set; }

        private static int AvailableThreads { get; set; } = MaximumThreads;

        private Thread ActiveThread { get; }

        private ExecuteMethod ExecuteType { get; }

        public void Start()
        {
            if (ExecuteType == ExecuteMethod.Action) return;
            ActiveThread.Start();
            AvailableThreads--;
            CheckForAlive();
        }

        private void CheckForAlive()
        {
            while (true)
            {
                if (ActiveThread.IsAlive is false)
                {
                    AvailableThreads++;
                    return;
                }

                Console.Title =
                    $"Количество свободных потоков: {AvailableThreads} | Проанализировано папок: {IDFinder.TotalScannedFolders} | Просканировано файлов: {IDFinder.TotalScannedFiles} | {IDFinder.ActivePath}";
                Thread.Sleep(1);
            }
        }

        private enum ExecuteMethod
        {
            Thread = 1,
            Action = 2
        }
    }
}