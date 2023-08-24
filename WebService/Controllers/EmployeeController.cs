using System.Net;
using Domain.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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

        [HttpPost]
        public async Task<IActionResult> CreateEmployee([FromBody] CreateEmployeeCommand command)
        {
            _logger.LogInformation($"CreateEmployee STARTED");
            command.CorrelationId = Guid.NewGuid().ToString();
            var response = await this._mediator.Send(command);
            if (response?.ValidationResult == null)
            {
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }

            if (!response.ValidationResult.IsValid)
            {
                return this.BadRequest(response);
            }

            return this.Ok(response);
        }
    }
}
