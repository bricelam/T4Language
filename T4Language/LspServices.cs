using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.Extensions.DependencyInjection;

namespace T4Language;

class LspServices : ILspServices
{
    readonly IServiceProvider _serviceProvider;

    public LspServices(IServiceCollection serviceCollection)
    {
        serviceCollection.AddSingleton<ILspServices>(this);

        _serviceProvider = serviceCollection.BuildServiceProvider();
    }

    public T GetRequiredService<T>()
        where T : notnull
        => _serviceProvider.GetRequiredService<T>();

    public object TryGetService(Type type)
        => _serviceProvider.GetService(type);

    public IEnumerable<T> GetRequiredServices<T>()
        => _serviceProvider.GetServices<T>();

    public ImmutableArray<Type> GetRegisteredServices()
        => throw new NotImplementedException();

    public bool SupportsGetRegisteredServices()
        => false;

    public void Dispose()
        => ((IDisposable)_serviceProvider).Dispose();
}
