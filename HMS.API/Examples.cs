using HMS.Application.Models.HotelDtos;
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
        }
    }
}
