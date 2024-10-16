﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using MaxDomainEventCore.Net.DomainEvents;
using MaxDomainEventCore.Net.Interceptor;
using Microsoft.AspNetCore.Builder;

namespace MaxDomainEventCore.Net.ApplicationBuilderExtend;

public static class ApplicationBuilderInitializationInterceptor
{
    public static void InitializeMaxDomainEventInterceptor(this IApplicationBuilder app)
    {
        var interceptorPreserver = app.ApplicationServices.GetAutofacRoot()
            .Resolve<MaxDomainEventInterceptorPreserver<IMaxDomainEventInterceptorContext<IDomainEvent, IDomainResponse>>>();
        
        interceptorPreserver.InitializeInterceptor();
    }
}