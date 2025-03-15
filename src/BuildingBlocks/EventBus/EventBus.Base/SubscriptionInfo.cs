using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EventBus.Base;

public class SubscriptionInfo
{
    public Type HandlerType { get; }

    public SubscriptionInfo(Type handlerType)
    {
        this.HandlerType = handlerType ?? throw new ArgumentNullException(nameof(handlerType));
    }

    public static SubscriptionInfo Typed(Type type)
    {
        return new SubscriptionInfo(type);
    }
}
