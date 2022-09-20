﻿using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language.Server.Handlers;

[LanguageServerEndpoint(Methods.TextDocumentDidOpenName)]
class DidOpenTextDocumentHandler : INotificationHandler<DidOpenTextDocumentParams, RequestContext>
{
    readonly TextDocumentManager _textDocumentManager;

    public DidOpenTextDocumentHandler(TextDocumentManager textDocumentManager)
        => _textDocumentManager = textDocumentManager;

    public bool MutatesSolutionState => true;

    public Task HandleNotificationAsync(
        DidOpenTextDocumentParams request,
        RequestContext requestContext,
        CancellationToken cancellationToken)
        => _textDocumentManager.OpenOrChangeAsync(request.TextDocument.Uri, request.TextDocument.Text);
}
