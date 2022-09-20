using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentDidChangeName)]
class DidChangeTextDocumentHandler : INotificationHandler<DidChangeTextDocumentParams, RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public DidChangeTextDocumentHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => true;

    public Task HandleNotificationAsync(
        DidChangeTextDocumentParams request,
        RequestContext requestContext,
        CancellationToken cancellationToken)
        => _textDocumentManager.OpenOrChangeAsync(request.TextDocument.Uri, request.ContentChanges[0].Text);
}
