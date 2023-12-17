using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Events;
using Domain.Extensions;
using Domain.Repositories;
using Domain.Services.Generic;

namespace Domain.Services
{
    internal class PersonService : BaseInterface<Person>, IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly ILookupService _lookupService;
        private readonly Person _user;

        public PersonService(IPersonRepository repository, Person user, ILookupService lookupService) : base(repository)
        {
            _repository = repository;
            _user = user;
            _lookupService = lookupService;
        }

        public async Task<ServiceExecutionResponse> InviteModerators(string bbqId, DateTime bbqDate, string bbqReason)
        {
            try
            {
                var moderatorsId = await _lookupService.GetModeratorsId();

                if (moderatorsId?.Any() != true)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "There are no Moderators in the System.", httpStatusCode: HttpStatusCode.NotFound);
                }

                foreach (var id in moderatorsId)
                {
                    var person = new Person();
                    person.Id = id;

                    person.Apply(new PersonHasBeenInvitedToBbq(bbqId, bbqDate, bbqReason));

                    await SaveAsync(person, new { CreatedBy = _user.Id }, id);
                }

                return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceExecutionResponse> GetAllInvites()
        {
            try
            {
                var person = await GetAsync(_user.Id);

                if (person == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                return new ServiceExecutionResponse(isSuccess: true, data: person.TakeSnapshot(), httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg)
        {
            try
            {
                var person = await GetAsync(_user.Id);

                if (person == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                if (!person.Invites.Any(i => i.Id == inviteId))
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Invite not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                person.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = isVeg, PersonId = person.Id });

                await SaveAsync(person, null, person.Id);

                return new ServiceExecutionResponse(isSuccess: true, data: person.TakeSnapshot(), httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceExecutionResponse> DeclineInvite(string inviteId)
        {
            try
            {
                var person = await GetAsync(_user.Id);

                if (person == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                if (!person.Invites.Any(i => i.Id == inviteId))
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Invite not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                person.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

                await SaveAsync(person, null, person.Id);

                return new ServiceExecutionResponse(isSuccess: true, data: person.TakeSnapshot(), httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }
    }
}
