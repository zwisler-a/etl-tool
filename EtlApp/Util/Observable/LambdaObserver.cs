namespace EtlApp.Util.Observable;

public class LambdaObserver<T> : IObserver<T>
{
    private readonly Action<T> _onNext;
    private readonly Action<Exception> _onError;
    private readonly Action _onCompleted;

    public LambdaObserver(
        Action<T>? onNext = null, 
        Action<Exception>? onError = null, 
        Action? onCompleted = null)
    {
        _onNext = onNext ?? (_ => { });
        _onError = onError ?? (_ => { });
        _onCompleted = onCompleted ?? (() => { });
    }

    public void OnNext(T value) => _onNext(value);
    public void OnError(Exception error) => _onError(error);
    public void OnCompleted() => _onCompleted();
}