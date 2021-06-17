using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers.Common
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        protected readonly IMediator mediator;

        protected BaseController(IMediator mediator)
        {
            this.mediator = mediator;
        }
    }
}
