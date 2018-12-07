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

        public string BuyerId { get; set; }

        public User Buyer { get; set; }

        public string SellerId { get; set; }

        public User Seller { get; set; }

        public bool IsAccepted { get; set; }

        public bool IsDelivered { get; set; }

        public Guid? ProductId { get; set; }

        public virtual Product Product { get; set; }

        public string ProductImage { get; set; }

        public string ProductName { get; set; }

        public decimal ProductPrice { get; set; }

        public string Size { get; set; }

        public int Quantity { get; set; }
    }
}
