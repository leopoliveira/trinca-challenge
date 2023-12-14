using System.Net;

using Domain.Entities;
using Domain.Repositories;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public class RunGetBbqShoppingList
    {
        private readonly Person _user;
        private readonly IBbqRepository _repository;
        private readonly IPersonRepository _persons;

        public RunGetBbqShoppingList(Person user, IBbqRepository repository, IPersonRepository persons)
        {
            _user = user;
            _repository = repository;
            _persons = persons;
        }

        [Function(nameof(RunGetBbqShoppingList))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/shoppingList")] HttpRequestData req, string id)
        {
            var person = await _persons.GetAsync(_user.Id);

            if (person == null)
            {
                return await req.CreateResponse(HttpStatusCode.NotFound, "Person not found with the personId sent on Header.");
            }

            if (!person.IsCoOwner)
            {
                return await req.CreateResponse(HttpStatusCode.Forbidden, "Allowed only for Co-Owners.");
            }

            var bbq = await _repository.GetAsync(id);

            if (bbq == null)
            {
                return await req.CreateResponse(HttpStatusCode.NoContent, "Churras not found with the given id.");
            }

            return await req.CreateResponse(HttpStatusCode.OK, bbq.ShoppingList.TakeSnapshot());
        }
    }
}
