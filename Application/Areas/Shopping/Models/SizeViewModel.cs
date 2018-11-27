using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.Areas.Shopping.Models
{
    public class SizeViewModel
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public int Quantity { get; set; }
    }
}
