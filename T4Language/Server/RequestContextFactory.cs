using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server;

class RequestContextFactory : IRequestContextFactory<RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public RequestContextFactory(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public Task<RequestContext> CreateRequestContextAsync<TRequestParam>(
        IQueueItem<RequestContext> queueItem,
        TRequestParam requestParam,
        CancellationToken cancellationToken)
    {
        var context = new RequestContext();

        if (requestParam is TextDocumentPositionParams textDocumentPositionParams)
        {
            context.TextDocument = _textDocumentManager.Get(textDocumentPositionParams.TextDocument.Uri);
        }

        return Task.FromResult(context);
    }
}
