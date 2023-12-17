using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Domain.Services;

namespace Serverless_Api
{
    public partial class RunCreateNewBbq
    {
        private readonly IBbqService _service;
        private readonly IPersonService _personService;

        public RunCreateNewBbq(IBbqService service, IPersonService personService)
        {
            _service = service;
            _personService = personService;
        }

        [Function(nameof(RunCreateNewBbq))]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "post", Route = "churras")] HttpRequestData req)
        {
            var input = await req.Body<NewBbqRequest>();

            if (input == null)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "Input is required.");
            }

            if (input.Date < DateTime.Now)
            {
                return await req.CreateResponse(HttpStatusCode.BadRequest, "The date must be later than the current date");
            }

            var churrasId = Guid.NewGuid();

            var serviceResponse = await _service.CreateBbq(input.Date, input.Reason, input.IsTrincasPaying, churrasId);

            if (!serviceResponse.IsSuccess)
            {
                return await req.CreateResponse(serviceResponse.HttpStatusCode, serviceResponse.Message);
            }

            var personServiceResponse = await _personService.InviteModerators(churrasId.ToString(), input.Date, input.Reason);

            if (!personServiceResponse.IsSuccess)
            {
                return await req.CreateResponse(personServiceResponse.HttpStatusCode, personServiceResponse.Message);
            }

            return await req.CreateResponse(HttpStatusCode.Created, serviceResponse.Data);
        }
    }
}
