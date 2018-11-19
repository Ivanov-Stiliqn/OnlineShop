using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Models
{
    public class Order
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        public string UserId { get; set; }

        public User User { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsDelivered { get; set; }

        public Guid ProductId { get; set; }

        public Product Product { get; set; }

        public Guid SizeId { get; set; }

        public Size Size { get; set; }

        public int Quantity { get; set; }
    }
}
