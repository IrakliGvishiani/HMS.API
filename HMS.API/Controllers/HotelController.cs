using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.Common;
using HMS.Application.Models.HotelDtos;
using HMS.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;
using System.Net;
using static CommonResponse;
using static HMS.API.Examples;
using static HMS.API.Examples.HotelForCreatingDtoExample;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace HMS.API.Controllers
{
    [Route("api/hotel")]

    [ApiController]
    public class HotelController : Controller
    {
        private readonly IHotelService _hotelService;
        private readonly CommonResponse _commonResponse;

        public HotelController(IHotelService hotelService,CommonResponse commonResponse)
        {
            _hotelService = hotelService;
            _commonResponse = commonResponse;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotelList([FromQuery] PagedRequestDto parameters)
        {
            var hotels = await _hotelService.GetHotelListAsync(parameters);
            return this.ToActionResult(_commonResponse.Success(hotels));
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(HotelForCreatingDto), typeof(HotelForCreatingDtoExample))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateNewHotel([FromBody] HotelForCreatingDto model)
        {
            var result = await _hotelService.CreateNewHotelAsync(model);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotel = await _hotelService.GetHotelAsync(id);
            return this.ToActionResult(_commonResponse.Success(hotel));
        }


        [HttpPut]
        [SwaggerRequestExample(typeof(HotelForUpdatingDto), typeof(HotelForUpdatingDtoExample))]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> UpdateHotel([FromBody] HotelForUpdatingDto model)
        {
            var result = await _hotelService.UpdateHotelAsync(model);
            return this.ToActionResult(_commonResponse.Success(result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            return this.ToActionResult(_commonResponse.NoContent());
        }
    }
}
