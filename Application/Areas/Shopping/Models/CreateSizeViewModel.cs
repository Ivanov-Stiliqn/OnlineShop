using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Application.Areas.Shopping.Controllers;
using Application.Infrastructure.Mapping;
using Models;

namespace Application.Areas.Shopping.Models
{
    public class CreateSizeViewModel: IMapTo<ProductSize>
    {
        [Required]
        [Display(Name = "Size")]
        public Guid SizeId { get; set; }

        public ICollection<SizeListItemViewModel> AllSizes { get; set; }
        
        [Range(1, int.MaxValue, ErrorMessage = "Quantity should be a positive number greater than 1.")]
        public int Quantity { get; set; }
    }
}
