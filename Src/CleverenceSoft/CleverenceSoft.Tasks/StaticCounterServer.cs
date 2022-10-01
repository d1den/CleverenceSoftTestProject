using System.Threading;

namespace CleverenceSoft.Tasks
{
    /// <summary>
    /// Статический класс "Сервер", предоставляющий сервис счётчика с синхронизацией потоков
    /// </summary>
    public static class StaticCounterServer
    {
        /// <summary>
        /// Примитив синхронизации потоков
        /// </summary>
        private static readonly ReaderWriterLockSlim _Lock = new ReaderWriterLockSlim();

        /// <summary>
        /// Поле значения счётчика
        /// </summary>
        private static int _Count = 0;

        /// <summary>
        /// Получить значение счётчика
        /// </summary>
        /// <returns>Значение счётчика</returns>
        public static int GetCount()
        {
            _Lock.EnterReadLock();
            try
            {
                return _Count;
            }
            finally
            {
                _Lock.ExitReadLock();
            }
        }

        /// <summary>
        /// Добавить значение к счётчику с монопольной блокировкой доступа
        /// </summary>
        /// <param name="value">Добавляемое значение</param>
        public static void AddToCount(int value)
        {
            _Lock.EnterWriteLock();
            try
            {
                _Count += value;
            }
            finally
            {
                _Lock.ExitWriteLock();
            }
        }
    }
}
