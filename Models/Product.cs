using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using Models.Enums;

namespace Models
{
    public class Product
    {
        public Product()
        {
            this.Sizes = new List<ProductSize>();
            this.Reviews = new List<Review>();
            this.Orders = new List<Order>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Sex Sex { get; set; }

        public decimal Price { get; set; }

        public DateTime DateOfCreation { get; set; } = DateTime.Now;

        public int Views { get; set; }

        [Required]
        public string Color { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public string Details { get; set; }

        [Required]
        public string ImageUrls { get; set; }

        [NotMapped]
        public ICollection<string> Images => this.ImageUrls.Split(new []{", "}, StringSplitOptions.RemoveEmptyEntries).ToList(); 

        public Guid CategoryId { get; set; }

        public Category Category { get; set; }

        public string CreatorId { get; set; }

        public User Creator { get; set; }

        public ICollection<ProductSize> Sizes { get; set; }

        public ICollection<Review> Reviews { get; set; }

        public ICollection<Order> Orders { get; set; }

    }
}
