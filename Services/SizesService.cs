using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories.Contracts;
using Models;
using Services.Contracts;

namespace Services
{
    public class SizesService: ISizesService
    {
        private readonly IRepository<Size> sizeRepository;
        private readonly IRepository<ProductSize> productSizeRepository;

        public SizesService(IRepository<Size> sizeRepository, IRepository<ProductSize> productSizeRepository)
        {
            this.sizeRepository = sizeRepository;
            this.productSizeRepository = productSizeRepository;
        }

        public ICollection<Size> GetSizes()
        {
            return this.sizeRepository.All().ToList();
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
