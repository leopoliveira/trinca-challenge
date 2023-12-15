using System.Net;

using Domain.Services;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunGetInvites
    {
        private readonly IPersonService _service;

        public RunGetInvites(IPersonService service)
        {
            _service = service;
        }

        [Function(nameof(RunGetInvites))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "person/invites")] HttpRequestData req)
        {
            var serviceResponse = await _service.GetAllInvites();

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(serviceResponse.HttpStatusCode, serviceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.OK, serviceResponse.Data);
        }
    }
}
