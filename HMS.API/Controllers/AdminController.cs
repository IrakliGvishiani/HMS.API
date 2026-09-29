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

        [HttpDelete("account")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteAccount()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _adminService.DeleteAdminAsync(userId);

            return this.ToActionResult(
                _commonResponse.Success(result)
            );
        }

        [HttpGet]

        public async Task<IActionResult> GetAllAdmin()
        {
            var admins = await _adminService.GetAllAdminAsync();
            return this.ToActionResult(_commonResponse.Success(admins));
        }
    }
}
