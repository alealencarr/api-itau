using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Shared.Itau.Result
{
    public static class CommandResultExtensions
    {
        public static IActionResult ToResult(this ICommandResult result)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(result),
                HttpStatusCode.NoContent => new NoContentResult(),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(result),
                HttpStatusCode.NotFound => new NotFoundObjectResult(result),
                _ => new ObjectResult(result) { StatusCode = (int)result.StatusCode }
            };
        }

        public static IActionResult ToResult<T>(this ICommandResult<T> result)
        {
            return result.StatusCode switch
            {
                HttpStatusCode.OK => new OkObjectResult(result),
                HttpStatusCode.Created => new CreatedResult($"/{result.Data!.ToString()}", result),
                HttpStatusCode.NoContent => new NoContentResult(),
                HttpStatusCode.BadRequest => new BadRequestObjectResult(result),
                HttpStatusCode.NotFound => new NotFoundObjectResult(result),
                _ => new ObjectResult(result) { StatusCode = (int)result.StatusCode }
            };
        }
    }
}
