using Microsoft.AspNetCore.Mvc;

namespace HMS.API
{
    public static class ControllerExtensions
    {
        public static IActionResult ToActionResult(this ControllerBase controller,
            CommonResponse response)
        {
            return controller.StatusCode(response.HttpStatusCode, response);
        }
    }
}
