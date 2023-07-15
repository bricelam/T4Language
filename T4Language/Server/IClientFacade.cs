using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server;

interface IClientFacade
{
    Task ExecuteAsync<TParams>(LspNotification<TParams> method, TParams argument);
    Task<TResult> ExecuteAsync<TParams, TResult>(LspRequest<TParams, TResult> method, TParams argument, CancellationToken cancellationToken = default);
}
