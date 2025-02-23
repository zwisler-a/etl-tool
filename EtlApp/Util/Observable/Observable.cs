using System.ComponentModel.DataAnnotations;
using EtlApp.Domain.Dto;

namespace EtlApp.Util.Observable;

public interface IPipeable<in T, out TO> : IObservable<TO>, IObserver<T>;

public abstract class Observable<T> : IObservable<T>
{
    private readonly List<IObserver<T>> _observers = [];

    public IDisposable Subscribe(IObserver<T> observer)
    {
        if (!_observers.Contains(observer))
            _observers.Add(observer);

        return new Unsubscriber(_observers, observer);
    }

    protected void Next(T data)
    {
        foreach (var observer in _observers)
            observer.OnNext(data);
    }

    protected void Complete()
    {
        foreach (var observer in _observers)
            observer.OnCompleted();
    }

    protected void Error(Exception error)
    {
        foreach (var observer in _observers)
            observer.OnError(error);
    }

    private class Unsubscriber(List<IObserver<T>> observers, IObserver<T> observer)
        : IDisposable
    {
        public void Dispose()
        {
            observers.Remove(observer);
        }
    }
}