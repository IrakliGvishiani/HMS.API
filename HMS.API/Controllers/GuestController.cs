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

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> DeleteGuest([FromRoute] int id)
        {
            var guest = await _guestService.DeleteGuestAsync(id);
            return this.ToActionResult(_commonResponse.Success(guest));
        }

        [HttpPost("create-reservation")]
        [Authorize(Roles = "Guest")]
        public async Task<IActionResult> CreateReservation([FromForm]
    ReservationForCreatingDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = await _reservationService
                .CreateReservationAsync(model, userId);

            return this.ToActionResult(_commonResponse.Created(result));
        }

        [HttpPut("reservation")]
        [Authorize(Roles = "Guest")]

        public async Task<IActionResult> UpdateReservation([FromBody] ReservationForUpdatingDto model)
        {
            var reservation = await _reservationService .UpdateReservationAsync(model);
            return this.ToActionResult(_commonResponse.Success(reservation));
        }

        [HttpDelete("reservation/{id}")]
        [Authorize(Roles = "Guest")]

        public async Task<IActionResult> DeleteReservation([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var result = await _reservationService .DeleteReservationAsync(id, userId);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpGet("search-reservations")]
        [Authorize(Roles = "Guest")]

        public async Task<IActionResult> SearchReservations([FromQuery] ReservationSearchDto model)
        {
            var result = await _reservationService.SearchReservationsAsync(model);
            return this.ToActionResult(_commonResponse.Success(result));
        }
    }
}
