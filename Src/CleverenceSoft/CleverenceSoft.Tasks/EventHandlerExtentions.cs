using System;
using System.Threading.Tasks;

namespace CleverenceSoft.Tasks
{
    /// <summary>
    /// Класс расширений для EventHandler
    /// </summary>
    public static class EventHandlerExtentions
    {
        /// <summary>
        /// Асинхронный вызов делегата EventHandler с TPL
        /// </summary>
        /// <param name="eventHandler">EventHandler</param>
        /// <param name="sender">Sender</param>
        /// <param name="e">EventArgs</param>
        /// <returns>Task</returns>
        public static Task InvokeAsync(this EventHandler eventHandler, object sender, EventArgs e)
        {
            return Task.Factory.FromAsync(eventHandler.BeginInvoke(sender, e, null, null), eventHandler.EndInvoke);
        }
    }
}
