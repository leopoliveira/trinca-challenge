using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using Domain.Application;
using Domain.Entities;
using Domain.Events;
using Domain.Repositories;

namespace Domain.Services
{
    internal class BbqService : IBbqService
    {
        private readonly IBbqRepository _repository;
        private readonly IPersonRepository _persons;
        private readonly ILookupService _lookupService;
        private readonly LoggedUser _user;

        public BbqService(IBbqRepository repository, IPersonRepository persons, ILookupService lookupService, LoggedUser user)
        {
            _repository = repository;
            _persons = persons;
            _lookupService = lookupService;
            _user = user;
        }

        public async Task<ServiceExecutionResponse> GetBbqsByLoggedPerson()
        {
            var person = await _persons.GetAsync(_user.Id);

            if (person == null)
            {
                return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.", httpStatusCode: HttpStatusCode.NotFound);
            }

            if (person.Invites?.Any() != true)
            {
                return new ServiceExecutionResponse(isSuccess: true, data: new object[] { },httpStatusCode: HttpStatusCode.OK);
            }

            var invites = new List<object>();

            foreach (var bbqId in person.Invites.Where(x => x.Date > DateTime.Now).Select(i => i.Id).ToList())
            {
                var bbq = await GetAsync(bbqId);

                if (bbq == null)
                {
                    continue;
                }

                if (bbq.Status != BbqStatus.ItsNotGonnaHappen)
                {
                    invites.Add(bbq.TakeSnapshot());
                }
            }

            return new ServiceExecutionResponse(isSuccess: true, data: invites, httpStatusCode: HttpStatusCode.OK);
        }

        public async Task<ServiceExecutionResponse> GetBbqShoppingList(string bbqId)
        {
            try
            {
                var person = await _persons.GetAsync(_user.Id);

                if (person == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Person not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                if (!person.IsCoOwner)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Allowed only for Co-Owners.", HttpStatusCode.Unauthorized);
                }

                var bbq = await _repository.GetAsync(bbqId);

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found with the given id.", HttpStatusCode.NoContent);
                }

                return new ServiceExecutionResponse(isSuccess: true, data: bbq.ShoppingList.TakeSnapshot(), HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceExecutionResponse> CreateBbq(DateTime date, string reason, bool isTrincaPaying, Guid? id = null)
        {
            try
            {
                Guid bbqId = id ?? Guid.NewGuid();

                var bbq = new Bbq();

                bbq.Apply(new ThereIsSomeoneElseInTheMood(bbqId, date, reason, isTrincaPaying));

                await SaveAsync(bbq, new { CreatedBy = _user.Id });

                return new ServiceExecutionResponse(isSuccess: true, data: bbq.TakeSnapshot(), httpStatusCode: HttpStatusCode.Created);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        public async Task<ServiceExecutionResponse> UpdateBbqStatus(string churrasId, bool bbqGonnaHappen, bool trincaWillPay)
        {
            try
            {
                var moderatorsId = await _lookupService.GetModeratorsId();

                if (moderatorsId == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "There are no Moderators in the System.", httpStatusCode: HttpStatusCode.NotFound);
                }

                if (!moderatorsId.Contains(_user.Id))
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Allowed only for moderators", httpStatusCode: HttpStatusCode.Unauthorized);
                }

                var bbq = await _repository.GetAsync(churrasId);

                var bbqValidation = BbqUpdateValidation(bbq);

                if (!bbqValidation.IsSuccess)
                {
                    return new ServiceExecutionResponse(isSuccess: bbqValidation.IsSuccess, message: bbqValidation.Message, httpStatusCode: bbqValidation.HttpStatusCode);
                }

                bbq.Apply(new BbqStatusUpdated(bbqGonnaHappen, trincaWillPay));

                await _repository.SaveAsync(bbq);

                return new ServiceExecutionResponse(isSuccess: true, data: bbq.TakeSnapshot() ,httpStatusCode: HttpStatusCode.OK);
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
                var bbq = await GetAsync(inviteId);

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                bbq.Apply(new InviteWasAccepted { InviteId = inviteId, IsVeg = isVeg, PersonId = _user.Id });

                await SaveAsync(bbq, null, inviteId);

                return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
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
                var bbq = await GetAsync(inviteId);

                if (bbq == null)
                {
                    return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found.", httpStatusCode: HttpStatusCode.NotFound);
                }

                bbq.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = _user.Id });

                await SaveAsync(bbq, null, inviteId);

                return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new ServiceExecutionResponse(error: ex.InnerException ?? ex, httpStatusCode: HttpStatusCode.InternalServerError);
            }
        }

        private async Task<Bbq> GetAsync(string id)
        {
            return await _repository.GetAsync(id);
        }

        private async Task SaveAsync(Bbq bbq, object? metadata = null, string? streamId = null)
        {
            await _repository.SaveAsync(bbq, metadata, streamId);
        }

        private ServiceExecutionResponse BbqUpdateValidation(Bbq bbq)
        {
            if (bbq == null)
            {
                return new ServiceExecutionResponse(isSuccess: false, message: "Churras not found.", httpStatusCode: HttpStatusCode.NotFound);
            }

            if (bbq.Date < DateTime.Now)
            {
                return new ServiceExecutionResponse(isSuccess: false, message: "Churras already happened.", httpStatusCode: HttpStatusCode.BadRequest);
            }

            if (bbq.Status == BbqStatus.ItsNotGonnaHappen)
            {
                return new ServiceExecutionResponse(isSuccess: false, message: "Churras already cancelled.", httpStatusCode: HttpStatusCode.BadRequest);
            }

            if (bbq.Status == BbqStatus.PendingConfirmations)
            {
                return new ServiceExecutionResponse(isSuccess: false, message: "Churras already confirmed.", httpStatusCode: HttpStatusCode.BadRequest);
            }

            if (bbq.Status == BbqStatus.Confirmed)
            {
                return new ServiceExecutionResponse(isSuccess: false, message: "Churras already confirmed.", httpStatusCode: HttpStatusCode.BadRequest);
            }

            return new ServiceExecutionResponse(isSuccess: true, httpStatusCode: HttpStatusCode.OK);
        }
    }
}
