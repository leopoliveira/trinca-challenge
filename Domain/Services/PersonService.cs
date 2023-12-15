using System;
using System.Linq;
using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;
using Domain.Services.Generic;

namespace Domain.Services
{
    internal class PersonService : BaseInterface<Person>, IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly Person _user;

        public PersonService(IPersonRepository repository, Person user) : base(repository)
        {
            _repository = repository;
            _user = user;
        }

        public async Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg)
        {
            try
            {
                var person = await GetAsync(_user.Id);

                if (person == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.");
                }

                if (!person.Invites.Any(i => i.Id == inviteId))
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Invite not found.");
                }

                person.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = isVeg, PersonId = person.Id });

                await SaveAsync(person);

                return new ServiceExecutionResponse(isSuccess: true, data: person.TakeSnapshot());
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
                var person = await GetAsync(_user.Id);

                if (person == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.");
                }

                if (!person.Invites.Any(i => i.Id == inviteId))
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Invite not found.");
                }

                person.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

                await SaveAsync(person);

                return new ServiceExecutionResponse(isSuccess: true, data: person.TakeSnapshot());
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex);
            }
        }
    }
}
