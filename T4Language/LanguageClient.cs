using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.LanguageServer.Protocol;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Nerdbank.Streams;
using StreamJsonRpc;

namespace T4Language;

[ContentType("T4")]
[Export(typeof(ILanguageClient))]
class LanguageClient : ILanguageClient
{
    LanguageServer _server;

    public string Name
        => "T4";

    public IEnumerable<string> ConfigurationSections
        => null;

    public object InitializationOptions
        => null;

    public IEnumerable<string> FilesToWatch
        => null;

    public bool ShowNotificationOnInitializeFailed
        => true;

    public event AsyncEventHandler<EventArgs> StartAsync;
    public event AsyncEventHandler<EventArgs> StopAsync;

    public async Task<Connection> ActivateAsync(CancellationToken token)
    {
        await Task.Yield();

        var (clientStream, serverStream) = FullDuplexStream.CreatePair();

        var formatter = new JsonMessageFormatter();
        formatter.JsonSerializer.AddVSExtensionConverters();
        var jsonRpc = new JsonRpc(new HeaderDelimitedMessageHandler(serverStream, serverStream, formatter));

        _server = new LanguageServer(jsonRpc, new LspLogger());
        jsonRpc.StartListening();

        return new Connection(clientStream, clientStream);
    }

    public Task OnLoadedAsync()
        => StartAsync.InvokeAsync(this, EventArgs.Empty);

    public Task OnServerInitializedAsync()
        => Task.CompletedTask;

    public Task<InitializationFailureContext> OnServerInitializeFailedAsync(
        ILanguageClientInitializationInfo initializationState)
        => Task.FromResult(
            new InitializationFailureContext
            {
                FailureMessage = initializationState.InitializationException?.ToString()
            });
}
