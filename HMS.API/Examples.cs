using HMS.Application.Models.HotelDtos;
using HMS.Application.Models.RoomDtos;
using Swashbuckle.AspNetCore.Filters;

namespace HMS.API
{
    public class Examples
    {
        public sealed record HotelForCreatingDtoExample : IExamplesProvider<HotelForCreatingDto>
        {
            public HotelForCreatingDto GetExamples()
            {
                return new HotelForCreatingDto
                {
                    Name = "Grand Hotel",
                    Address = "123 Main St, Cityville",
                    City = "Cityville",
                    Country = "Countryland",
                    Rating = 3
                };
            }

            public sealed record HotelForUpdatingDtoExample : IExamplesProvider<HotelForUpdatingDto>
            {
                public HotelForUpdatingDto GetExamples()
                {
                    return new HotelForUpdatingDto
                    {
                        Id = 1002,
                        Name = "Updated Grand Hotel",
                        Address = "456 Elm St, Cityville",
                        Rating = 4
                    };
                }


            };

            public sealed record RoomForCreatingDtoExample : IExamplesProvider<RoomForCreatingDto>
            {
                public RoomForCreatingDto GetExamples()
                {
                    return new RoomForCreatingDto
                    {
                        Name = "Deluxe Suite",
                        Price = 250.00,
                        HotelId = 2002
                    };
                }
            }
        }
    }
}
