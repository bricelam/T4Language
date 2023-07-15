using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Client;
using Microsoft.VisualStudio.Threading;
using Microsoft.VisualStudio.Utilities;
using Nerdbank.Streams;
using T4Language.Server;

namespace T4Language;

[ContentType("T4")]
[Export(typeof(ILanguageClient))]
class LanguageClient : ILanguageClient
{
    LanguageServer _server;

    static LanguageClient()
    {
        // HACK: Don't try this at home
        var allowListField = Type.GetType("Microsoft.VisualStudio.LanguageServer.Client.ExperimentalSnippetSupport, Microsoft.VisualStudio.LanguageServer.Client.Implementation, Version=17.4.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")
            ?.GetField("AllowList", BindingFlags.Static | BindingFlags.NonPublic);
        if (allowListField is not null)
        {
            var allowList = ((string[])allowListField.GetValue(null)).ToList();
            allowList.Add("T4 Language Server Client");
            allowListField.SetValue(null, allowList.ToArray());
        }
    }

    public string Name
        => "T4 Language Server Client";

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

        _server = LanguageServer.Create(serverStream, serverStream, new LspLogger());

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
