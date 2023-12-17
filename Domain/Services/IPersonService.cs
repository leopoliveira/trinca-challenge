using System;
using System.Threading.Tasks;

using Domain.Application;

namespace Domain.Services
{
    public interface IPersonService
    {
        Task<ServiceExecutionResponse> GetAllInvites();

        Task<ServiceExecutionResponse> InviteModerators(string bbqId, DateTime bbqDate, string bbqReason);

        Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg, string? personId = null);
        Task<ServiceExecutionResponse> DeclineInvite(string inviteId, string? personId = null);

        Task<ServiceExecutionResponse> UpdatePeopleInviteBasedOnBbqStatus(string bbqId);
    }
}
