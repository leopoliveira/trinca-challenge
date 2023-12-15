using Domain.Application;
using System.Threading.Tasks;

using Domain.Entities;
using Domain.Services.Generic;

namespace Domain.Services
{
    public interface IBbqService : IBaseInterface<Bbq>
    {
        Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg);
        Task<ServiceExecutionResponse> DeclineInvite(string inviteId);
    }
}
