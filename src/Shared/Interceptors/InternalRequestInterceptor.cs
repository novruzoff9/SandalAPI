using Grpc.Core;
using Grpc.Core.Interceptors;
using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.Interceptors;

public class InternalRequestInterceptor : Interceptor
{
    public override AsyncUnaryCall<TResponse> AsyncUnaryCall<TRequest, TResponse>(TRequest request, ClientInterceptorContext<TRequest, TResponse> context, AsyncUnaryCallContinuation<TRequest, TResponse> continuation)
    {
        var headers = new Metadata
        {
            { "X-Internal-Request", "true" },
        };

        var newContext = new ClientInterceptorContext<TRequest, TResponse>
            (context.Method, context.Host,
            new CallOptions(headers, context.Options.Deadline, context.Options.CancellationToken));

        return base.AsyncUnaryCall(request, newContext, continuation);
    }
}
