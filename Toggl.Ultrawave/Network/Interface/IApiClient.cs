using System;
using System.Threading.Tasks;

namespace Toggl.Ultrawave.Network
{
    internal interface IApiClient : IDisposable
    {
        Task<IResponse> Send(IRequest request);
    }
}
