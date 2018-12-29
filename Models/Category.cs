using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.CompilerServices;
using System.Text;
using Models.Enums;

namespace Models
{
    public class Category
    {
        public Category()
        {
            this.Products = new List<Product>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public CategoryType Type { get; set; }

        [Required]
        public string Image { get; set; }

        public string Views { get; set; }

        [NotMapped]
        public int ViewsCount => this.Views?.Split(new[] {", "}, StringSplitOptions.RemoveEmptyEntries).Length ?? 0;

        public ICollection<Product> Products { get; set; }
    }
}
