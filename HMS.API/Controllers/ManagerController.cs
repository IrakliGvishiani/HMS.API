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
        private readonly CommonResponse _commonResponse;

        public ManagerController(IManagerService managerService,CommonResponse commonResponse)
        {
            _managerService = managerService;
            _commonResponse = commonResponse;
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> UpdateManager([FromBody] ManagerForUpdatingDto model)
        {
            var result = await _managerService.UpdateManagerAsync(model);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpDelete]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteManager([FromRoute] int id)
        {
            var result = await _managerService.DeleteManagerAsync(id);
            return this.ToActionResult(_commonResponse.NoContent());
        }

        [HttpGet]

        public async Task<IActionResult> GetAllManagers()
        {
            var result = await _managerService.GetManagersAsync();
            return this.ToActionResult(_commonResponse.Success(result));
        }
    }
}
