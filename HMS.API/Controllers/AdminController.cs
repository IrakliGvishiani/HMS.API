using HMS.Application.Contracts.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers
{
    [Route("api/admin")]
    public class AdminController : Controller
    {
       private readonly IAdminService _adminService;
        private readonly CommonResponse _commonResponse;

        public AdminController(IAdminService adminService,CommonResponse commonResponse) {
            
            _adminService = adminService;
            _commonResponse = commonResponse;
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAdmin([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var admin = await _adminService.DeleteAdminAsync(id, userId);
            return this.ToActionResult(_commonResponse.NoContent());
        }
    }
}
