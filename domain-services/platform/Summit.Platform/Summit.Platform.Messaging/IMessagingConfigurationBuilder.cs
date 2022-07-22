using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Summit.Platform.MassTransit;

public interface IMessagingConfigurationBuilder
{
    void AddSendConsumer<TConsumer>() where TConsumer : class, IConsumer;
    void AddPublishConsumer<TConsumer>() where TConsumer : class, IConsumer;
    void AddSend<TSend>() where TSend : class;
    void AddPublish<TPublish>() where TPublish : class;
}

