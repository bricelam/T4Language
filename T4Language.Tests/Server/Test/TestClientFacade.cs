using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Test;

class TestClientFacade : IClientFacade
{
    public Task ExecuteAsync<TParams>(LspNotification<TParams> method, TParams argument)
        => Task.CompletedTask;

    public Task<TResult> ExecuteAsync<TParams, TResult>(LspRequest<TParams, TResult> method, TParams argument, CancellationToken cancellationToken = default)
        => Task.FromResult(default(TResult));
}