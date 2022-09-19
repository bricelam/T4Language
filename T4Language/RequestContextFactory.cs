using System.Threading;
using System.Threading.Tasks;
using Microsoft.CommonLanguageServerProtocol.Framework;

namespace T4Language;

class RequestContextFactory : IRequestContextFactory<RequestContext>
{
    //readonly ILspServices _lspServices;

    //public RequestContextFactory(ILspServices lspServices)
    //    => _lspServices = lspServices;

    public Task<RequestContext> CreateRequestContextAsync<TRequestParam>(
        IQueueItem<RequestContext> queueItem,
        TRequestParam requestParam,
        CancellationToken cancellationToken)
        => Task.FromResult(new RequestContext());
}
