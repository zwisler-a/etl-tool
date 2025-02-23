using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EtlApp.Util.Observable
{
    public class ThreadPoolObservable<T> : IObservable<T>, IDisposable
    {
        private readonly ConcurrentBag<IObserver<T>> _observers = new();
        private readonly ConcurrentQueue<TaskData<T>> _taskQueue = new();
        private readonly Task[] _workers;
        private readonly int _maxThreads;
        private bool _isDisposed;
        private readonly object _syncLock = new(); // Lock for thread safety

        public ThreadPoolObservable()
        {
            _maxThreads = 5;
            _workers = new Task[_maxThreads];

            // Start worker threads
            for (int i = 0; i < _maxThreads; i++)
            {
                _workers[i] = Task.Run(Worker);
            }
        }

        public IDisposable Subscribe(IObserver<T> observer)
        {
            lock (_syncLock)  // Ensure thread-safe observer addition
            {
                if (!_observers.Contains(observer))
                    _observers.Add(observer);
            }

            return new Unsubscriber(_observers, observer, _syncLock);
        }

        protected void Next(T data)
        {
            if (_observers.Count == 0) return;

            // Enqueue data explicitly in the task queue
            foreach (var observer in _observers)
            {
                _taskQueue.Enqueue(new TaskData<T>(data, observer));
            }
        }

        protected void Complete()
        {
            if (_observers.Count == 0) return;

            Console.WriteLine("Queuing completion signal.");

            foreach (var observer in _observers)
            {
                _taskQueue.Enqueue(new TaskData<T>(default, observer, true, false));
            }
        }

        protected void Error(Exception error)
        {
            if (_observers.Count == 0) return;

            Console.WriteLine($"Queuing error: {error.Message}");

            foreach (var observer in _observers)
            {
                _taskQueue.Enqueue(new TaskData<T>(default, observer, false, true, error));
            }
        }

        private async Task Worker()
        {
            while (!_isDisposed)
            {
                if (_taskQueue.TryDequeue(out var task))
                {
                    // Extract stored data and observer explicitly
                    var observer = task.Observer;

                    if (task.IsError)
                    {
                        observer.OnError(task.Error);
                    }
                    else if (task.IsCompleted)
                    {
                        observer.OnCompleted();
                    }
                    else
                    {
                        Console.WriteLine($"Processing data: {task.Data}");
                        observer.OnNext(task.Data);
                    }
                }
                else
                {
                    await Task.Delay(50); // Delay if no tasks are in the queue
                }
            }
        }

        private class TaskData<T>
        {
            public T Data { get; }
            public IObserver<T> Observer { get; }
            public bool IsCompleted { get; }
            public bool IsError { get; }
            public Exception Error { get; }

            public TaskData(T data, IObserver<T> observer, bool isCompleted = false, bool isError = false, Exception error = null)
            {
                Data = data;
                Observer = observer;
                IsCompleted = isCompleted;
                IsError = isError;
                Error = error;
            }
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentBag<IObserver<T>> _observers;
            private readonly IObserver<T> _observer;
            private readonly object _syncLock;

            public Unsubscriber(ConcurrentBag<IObserver<T>> observers, IObserver<T> observer, object syncLock)
            {
                _observers = observers;
                _observer = observer;
                _syncLock = syncLock;
            }

            public void Dispose()
            {
                lock (_syncLock) // Ensure thread-safe observer removal
                {
                    _observers.TryTake(out _);
                }
            }
        }

        public void Dispose()
        {
            _isDisposed = true;
            Task.WhenAll(_workers).Wait(); // Wait for worker threads to finish
        }
    }
}
