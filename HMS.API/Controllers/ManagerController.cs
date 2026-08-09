using HMS.Application.Contracts.Service;
using HMS.Application.Models.ManagerDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CommonResponse;

namespace HMS.API.Controllers
{
    [Route("api/manager")]
    [ApiController]
    public class ManagerController : Controller
    {
        private readonly IManagerService _managerService;

        public ManagerController(IManagerService managerService)
        {
            _managerService = managerService;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateManager([FromBody] ManagerForUpdatingDto model)
        {
            var result = await _managerService.UpdateManagerAsync(model);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteManager([FromRoute] int id)
        {
            var result = await _managerService.DeleteManagerAsync(id);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpGet]

        public async Task<IActionResult> GetAllManagers()
        {
            var result = await _managerService.GetManagersAsync();
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}
