using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Models
{
    public class Pagination
    {
        public int TotalPages { get; set; }

        public int First { get; set; }

        public int Last { get; set; }

        public int Previous { get; set; }

        public int Next { get; set; }

        public int Active { get; set; }

        public string Category { get; set; }

        public string Price { get; set; }

        public string SizeId { get; set; }

        public string Size { get; set; }

        public int Sex { get; set; }

        public string Gender { get; set; }
    }
}
