using Shared.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Order.Benchmark;

public class FakeIdentityService : ISharedIdentityService
{
    public string GetUserId => "test-user-id";

    public string GetCompanyId => "1e5dc749-5d62-43dd-8760-a9b5e2afe427";

    public string GetWarehouseId => throw new NotImplementedException();

    public ClaimsPrincipal GetUser => throw new NotImplementedException();
}

