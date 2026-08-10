using HMS.Application.Contracts.Service;
using HMS.Application.Models.GuestDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using static HMS.API.Examples;

namespace HMS.API.Controllers
{
    [Route("api/guest")]
    [ApiController]
    public class GuestController : Controller
    {
        private readonly IGuestService _guestService;
        private readonly CommonResponse _commonResponse;
        public GuestController(IGuestService guestService, CommonResponse resp)
        {
            _guestService = guestService;
            _commonResponse = resp;
            
        }
        
        [HttpPut]
        [Authorize(Roles = "Admin,Guest")]
        
        [SwaggerRequestExample(typeof(GuestForUpdatingDto),typeof(GuestForUpdatingDtoExample))]
        public async Task<IActionResult> UpdateGuest([FromBody] GuestForUpdatingDto model)
        {
            var guest = await _guestService.UpdateGuestAsync(model);
            var resp = _commonResponse.Success(guest);
            return StatusCode(resp.HttpStatusCode, resp);
                
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> DeleteGuest([FromRoute] int id)
        {
            var guest = await _guestService.DeleteGuestAsync(id);
            var resp = _commonResponse.Success(guest);
            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}
