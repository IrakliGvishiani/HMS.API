using HMS.Application.Contracts.Service;
using HMS.Application.Models.ReservationDtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace HMS.API.Controllers
{
    [Route("api/reservations")]
    [ApiController]
    public class ReservationController : Controller
    {
        private readonly IReservationService _reservationService;
        private readonly CommonResponse _commonResponse;

        public ReservationController(
            IReservationService reservationService,
            CommonResponse resp)
        {
            _reservationService = reservationService;
            _commonResponse = resp;
        }


        [HttpPost]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> CreateReservation(
            [FromBody] ReservationForCreatingDto model)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var userRole = User.FindFirstValue(
                ClaimTypes.Role);

            var result =
                await _reservationService.CreateReservationAsync(
                    model,
                    userId,
                    userRole);

            return this.ToActionResult(
                _commonResponse.Created(result));
        }


        [HttpPut]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> UpdateReservation(
            [FromBody] ReservationForUpdatingDto model)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var userRole = User.FindFirstValue(
                ClaimTypes.Role);

            var result =
                await _reservationService.UpdateReservationAsync(
                    model,
                    userId,
                    userRole);

            return this.ToActionResult(
                _commonResponse.Success(result));
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> DeleteReservation(
            [FromRoute] int id)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var userRole = User.FindFirstValue(
                ClaimTypes.Role);

            var result =
                await _reservationService.DeleteReservationAsync(
                    id,
                    userId,
                    userRole);

            return this.ToActionResult(
                _commonResponse.Success(result));
        }


        [HttpGet("search")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> SearchReservations(
            [FromQuery] ReservationSearchDto model)
        {
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            var userRole = User.FindFirstValue(
                ClaimTypes.Role);

            var result =
                await _reservationService.SearchReservationsAsync(
                    model,
                    userId,
                    userRole);

            return this.ToActionResult(
                _commonResponse.Success(result));
        }

        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Manager,Guest")]
        public async Task<IActionResult> GetReservation([FromRoute] int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var userRole = User.FindFirstValue(ClaimTypes.Role);

            var result = await _reservationService.GetReservationByIdAsync(id, userId, userRole);
            return this.ToActionResult(_commonResponse.Success(result));
        }
    }
}