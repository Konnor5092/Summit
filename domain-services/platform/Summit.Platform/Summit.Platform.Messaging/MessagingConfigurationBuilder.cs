using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summit.Platform.MassTransit;

public class MessagingConfigurationBuilder : IMessagingConfigurationBuilder
{
    public void AddSend<TSend>() where TSend : class
    {
        throw new NotImplementedException();
    }

    public void AddPublish<TPublish>() where TPublish : class
    {
        throw new NotImplementedException();
    }

    public void AddSendConsumer<TConsumer>() where TConsumer : class, IConsumer
    {
        throw new NotImplementedException();
    }

    public void AddPublishConsumer<TConsumer>() where TConsumer : class, IConsumer
    {
        throw new NotImplementedException();
    }
}
