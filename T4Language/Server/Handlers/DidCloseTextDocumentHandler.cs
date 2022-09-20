using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentDidCloseName)]
class DidCloseTextDocumentHandler : INotificationHandler<DidCloseTextDocumentParams, RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public DidCloseTextDocumentHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => true;

    public Task HandleNotificationAsync(
        DidCloseTextDocumentParams request,
        RequestContext requestContext,
        CancellationToken cancellationToken)
    {
        _textDocumentManager.Close(request.TextDocument.Uri);

        return Task.CompletedTask;
    }
}
