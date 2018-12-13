using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories.Contracts;
using Models;
using Models.Enums;
using Services.Contracts;

namespace Services
{
    public class SizesService: ISizesService
    {
        private readonly IRepository<Size> sizesRepository;
        private readonly IRepository<ProductSize> productSizeRepository;

        public SizesService(
            IRepository<Size> sizesRepository,
            IRepository<ProductSize> productSizeRepository)
        {
            this.sizesRepository = sizesRepository;
            this.productSizeRepository = productSizeRepository;  
        }

        public ICollection<Size> GetSizes(CategoryType type, Sex sex)
        {
            return type == CategoryType.Clothes
                ? sizesRepository.All().Where(s => s is ClothesSize).ToList()
                : sizesRepository.All()
                    .Where(s => s is ShoesSize && s.Sex != null && s.Sex == sex)
                    .OrderBy(s => int.Parse(s.Name))
                    .ToList();
        }

        public async Task Create(ProductSize size)
        {
            var existingSize =
                this.productSizeRepository.All().FirstOrDefault(ps => ps.ProductId == size.ProductId && ps.SizeId == size.SizeId);
            if (existingSize != null)
            {
                existingSize.Quantity += size.Quantity;
            }
            else
            {
                await this.productSizeRepository.AddAsync(size);
            }

            await this.productSizeRepository.SaveChangesAsync();
        }
    }
}
