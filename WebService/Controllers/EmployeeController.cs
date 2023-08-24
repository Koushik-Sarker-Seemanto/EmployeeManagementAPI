using System.Net;
using Domain.Commands;
using Dtos.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace WebService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<EmployeeController> _logger;
        public EmployeeController(ILogger<EmployeeController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("Create")]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand command)
        {
            command.CorrelationId = Guid.NewGuid().ToString();
            _logger.LogInformation($"CreateEmployee STARTED with CorrelationId: {command.CorrelationId}");
            CommandResponse? response = await this._mediator.Send(command);
            if (response?.ValidationResult == null)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (!response.ValidationResult.IsValid)
            {
                _logger.LogInformation($"{JsonConvert.SerializeObject(response)}");
                return this.StatusCode((int)response.StatusCode, response);
            }
            response.StatusCode = HttpStatusCode.OK;
            return this.Ok(response);
        }
        
        [HttpPost("Update")]
        public async Task<IActionResult> UpdateEmployee([FromBody] UpdateEmployeeCommand command)
        {
            command.CorrelationId = Guid.NewGuid().ToString();
            _logger.LogInformation($"UpdateEmployee STARTED with CorrelationId: {command.CorrelationId}");
            CommandResponse? response = await this._mediator.Send(command);
            if (response?.ValidationResult == null)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (!response.ValidationResult.IsValid)
            {
                _logger.LogInformation($"{JsonConvert.SerializeObject(response)}");
                return this.StatusCode((int)response.StatusCode, response);
            }
            response.StatusCode = HttpStatusCode.OK;
            return this.Ok(response);
        }
    }
}
