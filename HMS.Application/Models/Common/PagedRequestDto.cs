using System;
using System.Collections.Generic;
using System.Text;

namespace HMS.Application.Models.Common
{
    public class PagedRequestDto
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 20;

        public string? SortBy { get; set; }

        public string? FilterBy { get; set; }   
        public bool Ascending { get; set; } = true;
    }
}
