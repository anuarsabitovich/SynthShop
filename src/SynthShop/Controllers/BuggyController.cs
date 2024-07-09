using Microsoft.AspNetCore.Mvc;

namespace SynthShop.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BuggyController : ControllerBase
{
    [HttpGet("not-found")]
    public ActionResult GetNotFound()
    {
        return NotFound();
    }

    [HttpGet("bad-request")]
    public ActionResult GetBadRequest()
    {
        return BadRequest(new ProblemDetails { Title = "This is a bad request " });
    }


    [HttpGet("unauthorised")]
    public ActionResult GetUnauthorised()
    {
        return Unauthorized(new ProblemDetails { Title = "Unauthorised" });
    }

    [HttpGet("validation-error")]
    public ActionResult GetValidationError()
    {
        ModelState.AddModelError("Problem1", "This is the first error");
        ModelState.AddModelError("Problem2", "This is the second error");
        return ValidationProblem();
    }

    [HttpGet("server-error")]
    public ActionResult GetServerError()
    {
        throw new Exception("This is a server error");
    }
}