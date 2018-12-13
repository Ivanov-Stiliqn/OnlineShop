using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;
using Models.Enums;

namespace Models
{
    public class Size
    {
        public Size()
        {
            this.Products = new List<ProductSize>();
        }

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public Guid Id { get; set; }

        [Required]
        public string Name { get; set; }

        public Sex? Sex { get; set; }

        public ICollection<ProductSize> Products { get; set; }
    }
}
