using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summit.Platform.MassTransit;

public interface IMessaging
{
    Task Send<T>(T toSend, CancellationToken cancellationToken) where T : class;

    Task Publish<T>(T toPublish, CancellationToken cancellationToken) where T : class;
}
