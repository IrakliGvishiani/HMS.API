using HMS.Application.Models.AuthDtos;
using HMS.Application.Models.GuestDtos;
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

            public sealed record ManagerRegistrationRequestDtoExample : IExamplesProvider<ManagerRegistrationRequestDto>
            {
                public ManagerRegistrationRequestDto GetExamples()
                {
                    return new ManagerRegistrationRequestDto
                    {
                        FirstName = "Ika",
                        LastName = "gvisho",
                        PersonalNumber = "12345678911",
                        Email = "irakligvidhiani@gmail.com",
                        Password = "Ika123456789!",
                        PhoneNumber = "555101010",
                        HotelId = 2002
                    };
    }
            } }


        public sealed record AdminRegistrationRequestDtoExample : IExamplesProvider<AdminRegistrationRequestDto>
        {
            public AdminRegistrationRequestDto GetExamples()
            {
                return new AdminRegistrationRequestDto
                {
                    FirstName = "Ika",
                    LastName = "gvisho",
                    PersonalNumber = "12345678911",
                    Email = "irakligvidhiani@gmail.com",
                    Password = "Ika123456789!",
                    PhoneNumber = "555101010"
                    
                };
            }
        }

        public sealed record GuestRegistrationRequestDtoExample : IExamplesProvider<GuestRegistrationRequestDto>
        {
            public GuestRegistrationRequestDto GetExamples()
            {
                return new GuestRegistrationRequestDto
                {
                    FirstName = "Ika",
                    LastName = "gvisho",
                    PersonalNumber = "12345678911",
                    Email = "irakligvidhiani@gmail.com",
                    Password = "Ika123456789!",
                    PhoneNumber = "555101010",
                    

                };
            }
        }

        public sealed record GuestForUpdatingDtoExample : IExamplesProvider<GuestForUpdatingDto>
        {
            public GuestForUpdatingDto GetExamples()
            {
                return new GuestForUpdatingDto
                {
                    Id = 3,
                    FirstName = "Updated Ika",
                    LastName = "Updated gvisho",
                    PersonalNumber = "12345678911",
                    PhoneNumber = "555101010",

                };
            }
        }

    } 
}
