using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.Common;
using HMS.Application.Models.HotelDtos;
using HMS.Domain.Entities;
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

        public HotelController(IHotelService hotelService)
        {
            _hotelService = hotelService;
        }

        [HttpGet]
        public async Task<IActionResult> GetHotelList([FromQuery] PagedRequestDto parameters)
        {
            var hotels = await _hotelService.GetHotelListAsync(parameters);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, hotels, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpPost]
        [SwaggerRequestExample(typeof(HotelForCreatingDto), typeof(HotelForCreatingDtoExample))]
        public async Task<IActionResult> CreateNewHotel([FromBody] HotelForCreatingDto model)
        {
            var result = await _hotelService.CreateNewHotelAsync(model);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetHotel(int id)
        {
            var hotel = await _hotelService.GetHotelAsync(id);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, hotel, true, Convert.ToInt32(HttpStatusCode.OK));

            return StatusCode(resp.HttpStatusCode, resp);
        }


        [HttpPut]
        [SwaggerRequestExample(typeof(HotelForUpdatingDto), typeof(HotelForUpdatingDtoExample))]

        public async Task<IActionResult> UpdateHotel([FromBody] HotelForUpdatingDto model)
        {
            var result = await _hotelService.UpdateHotelAsync(model);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHotel(int id)
        {
            var result = await _hotelService.DeleteHotelAsync(id);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.OK));
            return StatusCode(resp.HttpStatusCode, resp);
        }
    }
}
