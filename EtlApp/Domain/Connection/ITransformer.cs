namespace EtlApp.Domain.Connection;

public interface ITransformer
{
    public object? Transform(object? value);
}