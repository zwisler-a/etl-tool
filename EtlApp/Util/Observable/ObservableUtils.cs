namespace EtlApp.Util.Observable;

public static class ObservableUtils
{
    public class ObservablePipe<T>
    {
        public required IObserver<T> Observer { get; init; }
        public required IObservable<T> Observable { get; init; }
    }

    public static ObservablePipe<T> Concat<T>(List<IPipeable<T, T>> observables)
    {
        var current = observables.First();
        foreach (var observable in observables)
        {
            if (!observable.Equals(current))
            {
                current.Subscribe(observable);
            }

            current = observable;
        }

        return new ObservablePipe<T>
        {
            Observer = observables.First(),
            Observable = observables.Last()
        };
    }
}