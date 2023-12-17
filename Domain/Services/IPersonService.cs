using System;
using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Services.Generic;

namespace Domain.Services
{
    public interface IPersonService : IBaseInterface<Person>
    {
        Task<Person> GetLoggedPersonAsync();

        Task<ServiceExecutionResponse> GetAllInvites();

        Task<ServiceExecutionResponse> InviteModerators(string bbqId, DateTime bbqDate, string bbqReason);

        Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg, string? personId = null);
        Task<ServiceExecutionResponse> DeclineInvite(string inviteId, string? personId = null);

        Task<ServiceExecutionResponse> UpdatePeopleInviteBasedOnBbqStatus(Bbq bbq);
    }
}
