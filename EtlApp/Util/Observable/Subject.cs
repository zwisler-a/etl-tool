namespace EtlApp.Util.Observable;

public class Subject<T> : Observable<T>
{
    public new void Next(T data)
    {
        base.Next(data);
    }

    public new void Complete()
    {
        base.Complete();
    }

    public new void Error(Exception error)
    {
        base.Error(error);
    }
}