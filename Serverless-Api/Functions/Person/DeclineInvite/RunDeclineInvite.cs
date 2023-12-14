using Domain;
using Eveneum;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Domain.Repositories;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using static Domain.ServiceCollectionExtensions;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly Person _user;
        private readonly IPersonRepository _repository;
        private readonly IBbqRepository _bbqs;

        public RunDeclineInvite(Person user, IPersonRepository repository, IBbqRepository bbqs)
        {
            _user = user;
            _repository = repository;
            _bbqs = bbqs;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var person = await _repository.GetAsync(_user.Id);

            if (person == null)
                return req.CreateResponse(System.Net.HttpStatusCode.NoContent);

            person.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

            await _repository.SaveAsync(person);

            var bbq = await _bbqs.GetAsync(inviteId);

            bbq.Apply(new InviteWasDeclined { InviteId = inviteId, PersonId = person.Id });

            await _bbqs.SaveAsync(bbq);

            return await req.CreateResponse(System.Net.HttpStatusCode.OK, person.TakeSnapshot());
        }
    }
}
