using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.CommonLanguageServerProtocol.Framework.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using StreamJsonRpc;

namespace T4Language;

class LanguageServer : AbstractLanguageServer<RequestContext>
{
    public LanguageServer(JsonRpc jsonRpc, ILspLogger logger)
        : base(jsonRpc, logger)
        => Initialize();

    protected override ILspServices ConstructLspServices()
        => new LspServices(
            new ServiceCollection()
                .AddSingleton<IMethodHandler, InitializeHandler<InitializeParams, InitializeResult, RequestContext>>()
                .AddSingleton<IMethodHandler, InitializedHandler<InitializedParams, RequestContext>>()
                .AddSingleton<IMethodHandler, ShutdownHandler<RequestContext>>()
                .AddSingleton<IMethodHandler, ExitHandler<RequestContext>>()
                .AddSingleton(_logger)
                .AddSingleton<IRequestContextFactory<RequestContext>, RequestContextFactory>()
                .AddSingleton<IInitializeManager<InitializeParams, InitializeResult>, CapabilitiesManager>()
                .AddSingleton<ILifeCycleManager>(this)
                .AddSingleton(this));
}
