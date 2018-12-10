using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Models;
using Moq;
using Services;
using Xunit;

namespace ServicesTests
{
    public class SizesServiceTests
    {
        [Fact]
        public void GetSizesShouldReturnAllSizes()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            sizeRepo.Setup(r => r.All()).Returns(new List<Size>
            {
                new Size(),
                new Size(),
                new Size()
            }.AsQueryable());

            var service = new SizesService(sizeRepo.Object, null);
            var sizes = service.GetSizes();
            Assert.Equal(3, sizes.Count);
            sizeRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task AddSizeShouldBeCalledOnce()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            var productSizeRepo = new Mock<IRepository<ProductSize>>();

            productSizeRepo.Setup(r => r.All()).Returns(new List<ProductSize>
            {
                new ProductSize
                {
                    ProductId = Guid.NewGuid(),
                    SizeId = Guid.NewGuid(),
                    Quantity = 1
                },
                new ProductSize
                {
                    ProductId = Guid.NewGuid(),
                    SizeId = Guid.NewGuid(),
                    Quantity = 1
                }
            }.AsQueryable());

            var size = new ProductSize
            {
                ProductId = Guid.NewGuid(),
                SizeId = Guid.NewGuid(),
                Quantity = 1
            };

            var service = new SizesService(sizeRepo.Object, productSizeRepo.Object);
            await service.Create(size);

            productSizeRepo.Verify(r => r.AddAsync(size), Times.Once);
        }

        [Fact]
        public async Task AddSizeShouldNotCallAddAsyncIfSizeExists()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            var productSizeRepo = new Mock<IRepository<ProductSize>>();
            var productId = Guid.NewGuid();
            var sizeId = Guid.NewGuid();

            productSizeRepo.Setup(r => r.All()).Returns(new List<ProductSize>
            {
                new ProductSize
                {
                    ProductId = productId,
                    SizeId = sizeId,
                    Quantity = 1
                },
                new ProductSize
                {
                    ProductId = Guid.NewGuid(),
                    SizeId = Guid.NewGuid(),
                    Quantity = 1
                }
            }.AsQueryable());

            var size = new ProductSize
            {
                ProductId = productId,
                SizeId = sizeId,
                Quantity = 1
            };

            var service = new SizesService(sizeRepo.Object, productSizeRepo.Object);
            await service.Create(size);
 
            productSizeRepo.Verify(r => r.AddAsync(size), Times.Never);
            productSizeRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
