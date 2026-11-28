using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Pcf.Common.Web.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
public abstract class BaseController : ControllerBase
{
}
