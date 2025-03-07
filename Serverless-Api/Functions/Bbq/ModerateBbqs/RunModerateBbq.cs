using System.Net;

using Domain.Services;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace Serverless_Api
{
    public partial class RunModerateBbq
    {
        private readonly IBbqService _service;
        private readonly IPersonService _personService;

        public RunModerateBbq(IBbqService service, IPersonService personService)
        {
            _service = service;
            _personService = personService;
        }

        [Function(nameof(RunModerateBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "put", Route = "churras/{id}/moderar")] HttpRequestData req, string id)
        {
            var moderationRequest = await req.Body<ModerateBbqRequest>();

            if (moderationRequest == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "Body is required.");
            }

            var serviceResponse = await _service.UpdateBbqStatus(id, moderationRequest.GonnaHappen, moderationRequest.TrincaWillPay);

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(serviceResponse.HttpStatusCode, serviceResponse.Message);
            }

            var personServiceResponse = await _personService.UpdatePeopleInviteBasedOnBbqStatus(id);

            if (!personServiceResponse.IsSuccess)
            {
                return await req.CreateResponse(personServiceResponse.HttpStatusCode, personServiceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.OK, serviceResponse.Data);
        }
    }
}
