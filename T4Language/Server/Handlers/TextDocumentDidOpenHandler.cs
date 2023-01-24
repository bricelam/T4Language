using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentDidOpenName)]
class TextDocumentDidOpenHandler : INotificationHandler<DidOpenTextDocumentParams, RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public TextDocumentDidOpenHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => true;

    public Task HandleNotificationAsync(
        DidOpenTextDocumentParams request,
        RequestContext context,
        CancellationToken cancellationToken = default)
        => _textDocumentManager.OpenOrChangeAsync(request.TextDocument.Uri, request.TextDocument.Text);
}
