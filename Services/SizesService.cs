using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Data;
using Models;
using Services.Contracts;

namespace Services
{
    public class SizesService: ISizesService
    {
        private readonly ApplicationContext db;

        public SizesService(ApplicationContext db)
        {
            this.db = db;
        }

        public IQueryable<Size> GetSizes()
        {
            return this.db.Sizes;
        }

        public void Create(ProductSize size)
        {
            var existingSize =
                this.db.ProductSizes.FirstOrDefault(ps => ps.ProductId == size.ProductId && ps.SizeId == size.SizeId);
            if (existingSize != null)
            {
                existingSize.Quantity += size.Quantity;
            }
            else
            {
                this.db.ProductSizes.Add(size);
            }

            this.db.SaveChanges();
        }
    }
}
