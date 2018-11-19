using System;
using System.Collections.Generic;
using System.Text;

namespace Models
{
    public class ProductSize
    {
        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public Guid SizeId { get; set; }

        public Size Size { get; set; }

        public int Quantity { get; set; }
    }
}
