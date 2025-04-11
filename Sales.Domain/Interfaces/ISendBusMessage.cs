namespace Sales.Domain.Interfaces;

public interface ISendBusMessage
{
    Task SendAsync<TMessage>(TMessage message, CancellationToken cancellationToken = default) where TMessage : class;
}