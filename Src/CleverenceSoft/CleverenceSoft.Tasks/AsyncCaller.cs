using System;
using System.Threading;
using System.Threading.Tasks;

namespace CleverenceSoft.Tasks
{
    /// <summary>
    /// Обёртка для "полусинхронного" вызова делегатов EventHandler
    /// </summary>
    public class AsyncCaller
    {
        /// <summary>
        /// EventHandler
        /// </summary>
        private readonly EventHandler _EventHandler;

        /// <summary>
        /// Контсруктор класса
        /// </summary>
        /// <param name="eventHandler">EventHandler</param>
        public AsyncCaller(EventHandler eventHandler)
        {
            _EventHandler = eventHandler ?? throw new ArgumentNullException(nameof(eventHandler));
        }

        /// <summary>
        /// Вызов методов делегата с принудительным завершением через заданное время
        /// </summary>
        /// <param name="millisecondsDelay">Время до завершения метода</param>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        /// <returns>True - методы делегата выполнились, False - время истекло</returns>
        public bool Invoke(int millisecondsDelay, object sender, EventArgs e)
        {
            var cts = new CancellationTokenSource(millisecondsDelay);
            var cancellationToken = cts.Token;

            var taskCompletionSource = new TaskCompletionSource<object>();

            cancellationToken.Register(() =>
            {
                taskCompletionSource.TrySetCanceled();
            });
            var eventHandlerTask = _EventHandler.InvokeAsync(sender, e);

            var firstFinishedTask = Task.WhenAny(eventHandlerTask, taskCompletionSource.Task);
            firstFinishedTask.Wait();

            return firstFinishedTask.Result == eventHandlerTask;
        }
    }
}
