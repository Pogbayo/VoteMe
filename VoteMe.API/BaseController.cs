using Microsoft.AspNetCore.Mvc;
using VoteMe.Application.Common;

namespace VoteMe.API
{
    public class BaseController : ControllerBase
    {
        protected ActionResult OkResponse<T>(T data, string message)
        {
            var response = ApiResponse<T>.SuccessResponse(data, message);
            return Ok(response);
        }

        protected ActionResult ErrorResponse(string error, List<string> message = null!)
        {
            var response = ApiResponse<object>.FailureResponse(error, message);
            return BadRequest(response);
        }

        protected ActionResult NotFoundResponse(string error = "Resource not found", List<string> message = null!)
        {
            var response = ApiResponse<object>.FailureResponse(error, message);
            return NotFound(response);
        }

        protected ActionResult UnauthorizedResponse(string error = "Unauthorized", List<string> message = null!)
        {
            var response = ApiResponse<object>.FailureResponse(error, message);
            return Unauthorized(response);
        }
    }
}