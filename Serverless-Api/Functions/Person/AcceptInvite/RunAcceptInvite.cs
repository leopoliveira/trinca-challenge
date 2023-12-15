using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Services;
using System.Net;

namespace Serverless_Api
{
    public partial class RunAcceptInvite
    {
        private readonly IPersonService _service;
        private readonly IBbqService _bbqService;

        public RunAcceptInvite(IPersonService service, IBbqService bbqService)
        {
            _service = service;
            _bbqService = bbqService;
        }

        [Function(nameof(RunAcceptInvite))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "person/invites/{inviteId}/accept")] HttpRequestData req, string inviteId)
        {
            var answer = await req.Body<InviteAnswer>();

            if (answer == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "'isVeg' property must be sent in the request body.");
            }

           var serviceResponse = await _service.AcceptInvite(inviteId, answer.IsVeg);

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(HttpStatusCode.InternalServerError, serviceResponse.Message);
            }

            var bbqServiceResponse = await _bbqService.AcceptInvite(inviteId, answer.IsVeg);

            if (!bbqServiceResponse.IsSuccess)
            {
                return await req.CreateResponse(HttpStatusCode.InternalServerError, bbqServiceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.OK, serviceResponse.Data);
        }
    }
}
