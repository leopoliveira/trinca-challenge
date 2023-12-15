using System;
using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Services.Generic;

namespace Domain.Services
{
    internal class BbqService : BaseInterface<Bbq>, IBbqService
    {
        private readonly IBbqRepository _repository;
        private readonly ILookupService _lookupService;
        private readonly Person _user;

        public BbqService(IBbqRepository repository, ILookupService lookupService, Person user) : base(repository)
        {
            _repository = repository;
            _lookupService = lookupService;
            _user = user;
        }

        public async Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg)
        {
            try
            {
                var bbq = await GetAsync(inviteId);

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found.");
                }

                bbq.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = isVeg, PersonId = _user.Id });

                await SaveAsync(bbq);

                return new ServiceExecutionResponse(isSuccess: true);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex);
            }
        }

        public async Task<ServiceExecutionResponse> DeclineInvite(string inviteId)
        {
            try
            {
                var bbq = await GetAsync(inviteId);

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found.");
                }

                bbq.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = _user.Id });

                await SaveAsync(bbq);

                return new ServiceExecutionResponse(isSuccess: true);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex);
            }
        }
    }
}
