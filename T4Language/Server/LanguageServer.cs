using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.CommonLanguageServerProtocol.Framework.Handlers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using StreamJsonRpc;
using T4Language.Server.Handlers;

namespace T4Language.Server;

class LanguageServer : AbstractLanguageServer<RequestContext>
{
    readonly JsonRpc _jsonRpc;

    public LanguageServer(JsonRpc jsonRpc, ILspLogger logger)
        : base(jsonRpc, logger)
    {
        _jsonRpc = jsonRpc;
        Initialize();
    }

    protected override ILspServices ConstructLspServices()
        => new LspServices(
            new ServiceCollection()
                .AddSingleton<IMethodHandler, InitializeHandler<InitializeParams, InitializeResult, RequestContext>>()
                .AddSingleton<IMethodHandler, InitializedHandler<InitializedParams, RequestContext>>()
                .AddSingleton<IMethodHandler, ShutdownHandler<RequestContext>>()
                .AddSingleton<IMethodHandler, ExitHandler<RequestContext>>()
                .AddSingleton<IMethodHandler, DidOpenTextDocumentHandler>()
                .AddSingleton<IMethodHandler, DidChangeTextDocumentHandler>()
                .AddSingleton<IMethodHandler, DidCloseTextDocumentHandler>()
                .AddSingleton<IMethodHandler, CompletionHandler>()
                .AddSingleton(_logger)
                .AddSingleton<IRequestContextFactory<RequestContext>, RequestContextFactory>()
                .AddSingleton<IInitializeManager<InitializeParams, InitializeResult>, CapabilitiesManager>()
                .AddSingleton<ILifeCycleManager>(this)
                .AddSingleton(this)
                .AddSingleton<TextDocumentManager>());

    public Task ExecuteAsync<TParams>(LspNotification<TParams> method, TParams argument)
        => _jsonRpc.NotifyWithParameterObjectAsync(method.Name, argument);

    public Task<TResult> ExecuteAsync<TParams, TResult>(LspRequest<TParams, TResult> method, TParams argument, CancellationToken cancellationToken = default)
        => _jsonRpc.InvokeWithParameterObjectAsync<TResult>(method.Name, argument, cancellationToken);
}
