using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Services;
using System.Net;

namespace Serverless_Api
{
    public partial class RunDeclineInvite
    {
        private readonly IPersonService _service;
        private readonly IBbqService _bbqService;

        public RunDeclineInvite(IPersonService service, IBbqService bbqService)
        {
            _service = service;
            _bbqService = bbqService;
        }

        [Function(nameof(RunDeclineInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/decline")] HttpRequestData req, string inviteId)
        {
            var serviceResponse = await _service.DeclineInvite(inviteId);

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(HttpStatusCode.InternalServerError, serviceResponse.Message);
            }

            var bbqServiceResponse = await _bbqService.DeclineInvite(inviteId);

            if (!bbqServiceResponse.IsSuccess)
            {
                return await req.CreateResponse(HttpStatusCode.InternalServerError, bbqServiceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.OK, serviceResponse.Data);
        }
    }
}
