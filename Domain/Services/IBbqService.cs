using Domain.Application;
using System.Threading.Tasks;

using Domain.Entities;
using Domain.Services.Generic;
using System;

namespace Domain.Services
{
    public interface IBbqService : IBaseInterface<Bbq>
    {
        Task<ServiceExecutionResponse> CreateBbq(DateTime date, string reason, bool isTrincaPaying, Guid? id = null);

        Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg);
        Task<ServiceExecutionResponse> DeclineInvite(string inviteId);
    }
}
