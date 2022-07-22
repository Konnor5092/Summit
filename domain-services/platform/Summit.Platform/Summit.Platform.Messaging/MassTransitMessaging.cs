using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summit.Platform.MassTransit;

public class MassTransitMessaging : IMessaging
{
    private readonly IPublishEndpoint _publishEndpoint;
    private readonly ISendEndpointProvider _sendEndpointProvider;

    public MassTransitMessaging(IPublishEndpoint publishEndpoint, ISendEndpointProvider sendEndpointProvider)
    {
        _publishEndpoint = publishEndpoint;
        _sendEndpointProvider = sendEndpointProvider;
    }

    public async Task Publish<T>(T toPublish, CancellationToken cancellationToken) where T : class
    {
        await _publishEndpoint.Publish(toPublish, cancellationToken);
    }

    public async Task Send<T>(T toSend, CancellationToken cancellationToken) where T : class
    {
        await _sendEndpointProvider.Send(toSend, cancellationToken);
    }
}

