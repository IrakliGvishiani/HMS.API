using HMS.Application.Contracts.Service;
using HMS.Application.Exceptions;
using HMS.Application.Models.HotelDtos;
using HMS.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using static CommonResponse;
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


        [HttpPost]

        public async Task<IActionResult> CreateNewHotel([FromBody] HotelForCreatingDto model)
        {
            var result = await _hotelService.CreateNewHotelAsync(model);
            var resp = new CommonResponse(CommonResponseMessage.SuccessMessage, result, true, Convert.ToInt32(HttpStatusCode.Created));
            return StatusCode(resp.HttpStatusCode, resp);
        }

    }
}
