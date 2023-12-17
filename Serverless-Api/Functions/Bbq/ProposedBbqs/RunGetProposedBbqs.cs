using System.Net;

using Domain.Services;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetProposedBbqs
    {
        private readonly IBbqService _service;
        private readonly IPersonService _personService;

        public RunGetProposedBbqs(IBbqService service, IPersonService personService)
        {
            _service = service;
            _personService = personService;
        }

        [Function(nameof(RunGetProposedBbqs))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "churras")] HttpRequestData req)
        {
            var serviceResponse = await _service.GetBbqsByPerson(await _personService.GetLoggedPersonAsync());

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(serviceResponse.HttpStatusCode, serviceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.OK, serviceResponse.Data);
        }
    }
}
