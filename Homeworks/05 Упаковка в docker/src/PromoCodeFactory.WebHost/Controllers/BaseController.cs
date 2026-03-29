using Microsoft.AspNetCore.Mvc;

namespace PromoCodeFactory.WebHost.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public abstract class BaseController : ControllerBase
{
}
