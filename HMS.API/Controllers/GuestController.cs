using HMS.Application.Contracts.Service;
using HMS.Application.Models.GuestDtos;
using HMS.Application.Models.ReservationDtos;
using HMS.Application.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;
using static HMS.API.Examples;

namespace HMS.API.Controllers
{
    [Route("api/guest")]
    [ApiController]
    public class GuestController : Controller
    {
        private readonly IGuestService _guestService;
        private readonly CommonResponse _commonResponse;
        private readonly IReservationService _reservationService;
        public GuestController(IGuestService guestService, CommonResponse resp,IReservationService reservationService)
        {
            _guestService = guestService;
            _commonResponse = resp;
            _reservationService = reservationService;
        }
        
        [HttpPut]
        [Authorize(Roles = "Admin,Guest")]
        
        [SwaggerRequestExample(typeof(GuestForUpdatingDto),typeof(GuestForUpdatingDtoExample))]
        public async Task<IActionResult> UpdateGuest([FromBody] GuestForUpdatingDto model)
        {
            var guest = await _guestService.UpdateGuestAsync(model);
            return this.ToActionResult(_commonResponse.Success(guest));
                
        }

        [HttpGet]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllGuests()
        {
            var guests = await _guestService.GetAllGuestsAsync();
            return this.ToActionResult(_commonResponse.Success(guests));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> DeleteGuest([FromRoute] int id)
        {
            var guest = await _guestService.DeleteGuestAsync(id);
            return this.ToActionResult(_commonResponse.Success(guest));
        }

    }
}
