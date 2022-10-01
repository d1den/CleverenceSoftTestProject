using CleverenceSoft.Tasks;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleverenceSoft.TestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            RunLoopToExitFromApplication();

            // Раскомментировать для демонстрации работы первого задания
            RunCounterServerExample();

            // Раскомментировать для демонстрации работы второго задания
            //RunAcyncCallerExample(1000, 500);

            Console.ReadKey();
        }

        /// <summary>
        /// Метод, запускающий цикл проверки Нажатия Esc для выхода из приложения
        /// </summary>
        private static void RunLoopToExitFromApplication()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    var button = Console.ReadKey();
                    if (button.Key == ConsoleKey.Escape)
                    {
                        Environment.Exit(0);
                    }
                }
            });
        }

        /// <summary>
        /// Запустить пример работы "Сервера" счётчика с синхронизацией
        /// </summary>
        private static void RunCounterServerExample()
        {
            var writeDelays = new[] { 500, 750, 1000, 1250 };
            for (int i = 0; i < 100; i++)
            {
                var addToCountValue = i + 1;
                RunTaskActionWithRandomDelay(() => StaticCounterServer.AddToCount(addToCountValue), writeDelays);
            }

            var lastCount = 0;
            while (true)
            {
                var currentCount = StaticCounterServer.GetCount();
                if (lastCount != currentCount)
                {
                    lastCount = currentCount;
                    Console.WriteLine(lastCount);
                }
            }
        }

        /// <summary>
        /// Запустить пример работы Обёртки для "полусинхронного" вызова делегатов EventHandler
        /// </summary>
        /// <param name="cancellationTime">Время до отмены выполнения делегата</param>
        /// <param name="eventHandlerExecutionTime">Время выполнения делегата</param>
        private static void RunAcyncCallerExample(int eventHandlerExecutionTime, int cancellationTime)
        {
            var eventHandler = new EventHandler((sender, e) =>
            {
                Console.WriteLine("Big work in EventHandler started...");
                Thread.Sleep(eventHandlerExecutionTime);
            });

            var asyncCaller = new AsyncCaller(eventHandler);
            if (!asyncCaller.Invoke(cancellationTime, null, EventArgs.Empty))
            {
                Console.WriteLine("Time out!");
            }
            else
            {
                Console.WriteLine("Delegate completed successfully!");
            }
        }


        /// <summary>
        /// Выполнить действие в задаче с случайной задержкой потока
        /// </summary>
        /// <param name="delays">Массив возможных задержек в мс</param>
        /// <param name="action">Действие</param>
        private static void RunTaskActionWithRandomDelay(Action action, params int[] delays)
        {
            var random = new Random();
            var randomIndex = delays != null ? random.Next(0, delays.Length) : 0;
            var delay = delays?.Length != 0 ? delays[randomIndex] : 0;

            Task.Run(() =>
            {
                Thread.Sleep(delay);
                action?.Invoke();
            });
        }
    }
}
