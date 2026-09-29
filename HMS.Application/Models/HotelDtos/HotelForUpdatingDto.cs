using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.HotelDtos
{
    public class HotelForUpdatingDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public byte Rating { get; set; }

        public string Address { get; set; }

        public List<int> ImageIdsToDelete { get; set; } = new();
        public List<IFormFile> ImagesToAdd { get; set; } = new();

        public int? PrimaryImageId { get; set; }

    }
}
