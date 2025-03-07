﻿using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;

namespace Domain.Services
{
    internal class PersonService : IPersonService
    {
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqs;
        private readonly ILookupService _lookupService;
        private readonly LoggedUser _user;

        public PersonService(IPersonRepository repository, IBbqRepository bbqs, ILookupService lookupService, LoggedUser user)
        {
            _repository = repository;
            _bbqs = bbqs;
            _lookupService = lookupService;
            _user = user;
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

        public async Task<ServiceExecutionResponse> AcceptInvite(string inviteId, bool isVeg, string? personId = null)
        {
            try
            {
                var person = await GetAsync(personId ?? _user.Id);

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

        public async Task<ServiceExecutionResponse> DeclineInvite(string inviteId, string? personId = null)
        {
            try
            {
                var person = await GetAsync(personId ?? _user.Id);

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

        public async Task<ServiceExecutionResponse> UpdatePeopleInviteBasedOnBbqStatus(string bbqId)
        {
            try
            {
                var bbq = await _bbqs.GetAsync(bbqId);

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found with the given id.", HttpStatusCode.NoContent);
                }

                var lookups = await _lookupService.GetLookups();

                if (lookups == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "There are no person recorded in the system.", httpStatusCode: HttpStatusCode.NotFound);
                }

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                if (bbq.Status == BbqStatus.ItsNotGonnaHappen)
                {
                    foreach (var personId in lookups.ModeratorIds)
                    {
                        var declineInvite = await DeclineInvite(bbq.Id, personId);

                        if (!declineInvite.IsSuccess)
                        {
                            return new ServiceExecutionResponse(isSuccess: declineInvite.IsSuccess, httpStatusCode: declineInvite.HttpStatusCode);
                        }
                    }

                    return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
                }

                if (bbq.Status == BbqStatus.Confirmed || bbq.Status == BbqStatus.New)
                {
                    // Future implementation.

                    return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
                }

                foreach (var personId in lookups.PeopleIds)
                {
                    if (lookups.ModeratorIds.Contains(personId))
                    {
                        continue;
                    }

                    var person = await GetAsync(personId);

                    if (person == null)
                    {
                        continue;
                    }

                    person.Apply(new PersonHasBeenInvitedToBbq(bbq.Id, bbq.Date, bbq.Reason));

                    await SaveAsync(person, null, personId);
                }

                return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Person> GetAsync(string id)
        {
            return await _repository.GetAsync(id);
        }

        private async Task SaveAsync(Person person, object? metadata = null, string? streamId = null)
        {
            await _repository.SaveAsync(person, metadata, streamId);
        }
    }
}
