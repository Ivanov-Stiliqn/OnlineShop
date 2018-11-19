using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Size
    {
        public Size()
        {
            this.Products = new List<ProductSize>();
            this.Orders = new List<Order>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public ICollection<ProductSize> Products { get; set; }

        public ICollection<Order> Orders { get; set; }
    }
}
