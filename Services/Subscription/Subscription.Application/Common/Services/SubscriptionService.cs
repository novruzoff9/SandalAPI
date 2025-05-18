using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subscription.Application.Common.Services;

public interface ISubscriptionService
{
    Task<bool> SubscribeAsync(string companyId, string packageId);
}
public class SubscriptionService
{
}
