﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Polly.Caching;
using Polly.Caching.Memory;
using Polly.Contrib.AzureFunctions.CircuitBreaker;
using Polly.Registry;

[assembly: FunctionsStartup(typeof(CircuitBreaker.Startup))]

namespace CircuitBreaker
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddMemoryCache();
            builder.Services.AddSingleton<IAsyncCacheProvider, MemoryCacheProvider>();
            builder.Services.AddSingleton<IPolicyRegistry<string>>(new PolicyRegistry());
            builder.Services.AddSingleton<IDurableCircuitBreakerClient, DurableCircuitBreakerClient>();
        }
    }
}