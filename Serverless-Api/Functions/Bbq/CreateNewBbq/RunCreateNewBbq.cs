using Eveneum;
using System.Net;
using CrossCutting;
using Domain.Events;
using Domain.Entities;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Repositories;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly Person _user;
        private readonly SnapshotStore _snapshots;
        private readonly IBbqRepository _repository;
        private readonly IPersonRepository _persons;
        public RunCreateNewBbq(IBbqRepository repository, IPersonRepository persons, SnapshotStore snapshots, Person user)
        {
            _user = user;
            _snapshots = snapshots;
            _repository = repository;
            _persons = persons;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>();

            if (input == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "input is required.");
            }

            var churras = new Bbq();

            churras.Apply(new ThereIsSomeoneElseInTheMood(Guid.NewGuid(), input.Date, input.Reason, input.IsTrincasPaying));

            await _repository.SaveAsync(churras);

            var churrasSnapshot = churras.TakeSnapshot();

            var Lookups = await _snapshots.AsQueryable<Lookups>("Lookups").SingleOrDefaultAsync();

            foreach (var personId in Lookups.ModeratorIds)
            {
                var person = await _persons.GetAsync(personId);
                if (person != null)
                {
                    var @event = new PersonHasBeenInvitedToBbq(churras.Id, churras.Date, churras.Reason);
                    person.Apply(@event);
                    await _persons.SaveAsync(person);
                }
            }

            return await req.CreateResponse(HttpStatusCode.Created, churrasSnapshot);
        }
    }
}
