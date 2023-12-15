using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Services.Generic;

namespace Domain.Services
{
    public interface IPersonService : IBaseInterface<Person>
    {
        Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg);
    }
}
