using System.Net;

using Domain.Entities;
using Domain.Repositories;
using Domain.Services;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public class RunGetBbqShoppingList
    {
        private readonly IBbqService _service;

        public RunGetBbqShoppingList(IBbqService service)
        {
            _service = service;
        }

        [Function(nameof(RunGetBbqShoppingList))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras/{id}/shoppingList")] HttpRequestData req, string id)
        {
            var serviceResponse = await _service.GetBbqShoppingList(id);

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(serviceResponse.HttpStatusCode, serviceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.OK, serviceResponse.Data);
        }
    }
}
