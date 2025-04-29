using MassTransit;
using Microsoft.Extensions.Configuration;
using Sales.Domain.Interfaces;

namespace Sales.Infrastructure.MassTransit;

public class SendBusMessage : ISendBusMessage
{
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public SendBusMessage(ISendEndpointProvider sendEndpointProvider)
    {
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task SendAsync<TMessage>(TMessage message,CancellationToken cancellationToken = default)
        where TMessage : class
    {
        
        var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri("exchange:report-generator?type=direct"));
        await endpoint.Send(message, ctx =>
        {
            ctx.SetRoutingKey("report");
        }, cancellationToken);
    }
}