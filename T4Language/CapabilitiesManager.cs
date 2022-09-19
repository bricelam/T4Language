using Microsoft.CommonLanguageServerProtocol.Framework;
using Microsoft.VisualStudio.LanguageServer.Protocol;

namespace T4Language;

internal class CapabilitiesManager : IInitializeManager<InitializeParams, InitializeResult>
{
    InitializeParams _initializeParams;

    public InitializeParams GetInitializeParams()
        => _initializeParams;

    public void SetInitializeParams(InitializeParams request)
        => _initializeParams = request;

    public InitializeResult GetInitializeResult()
        => new InitializeResult
        {
            Capabilities = new ServerCapabilities
            {
                // TODO
            }
        };
}
