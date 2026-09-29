using HMS.Application.Contracts.Service;
using HMS.Application.Models.ManagerDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;
using static CommonResponse;

namespace HMS.API.Controllers
{
    [Route("api/manager")]
    [ApiController]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;
        private readonly CommonResponse _commonResponse;

        public ManagerController(IManagerService managerService,CommonResponse commonResponse)
        {
            _managerService = managerService;
            _commonResponse = commonResponse;
        }

        [HttpPut]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> UpdateManager([FromBody] ManagerForUpdatingDto model)
        {
            var result = await _managerService.UpdateManagerAsync(model);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteManager([FromRoute] int id)
        {
            var result = await _managerService.DeleteManagerAsync(id);
            return this.ToActionResult(_commonResponse.NoContent());
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllManagers()
        {
            var result = await _managerService.GetManagersAsync();
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpGet("me")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetOwnProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _managerService.GetOwnProfileAsync(userId);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpGet("analytics")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAnalytics()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var result = await _managerService.GetManagerAnalyticsAsync(
                userId,
                userRole);

            var resp = _commonResponse.Success(result);

            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}
