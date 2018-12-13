using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Models;
using Models.Enums;
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
                new ClothesSize {Sex = Sex.Men},
                new ClothesSize(){Sex = Sex.Men},
                new ClothesSize(){Sex = Sex.Men}
            }.AsQueryable());

            var service = new SizesService(sizeRepo.Object, null);
            var sizes = service.GetSizes(CategoryType.Clothes, Sex.Men);
            Assert.Equal(3, sizes.Count);
            sizeRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetSizesShouldReturnSizesInGivenType()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            sizeRepo.Setup(r => r.All()).Returns(new List<Size>
            {
                new ClothesSize(),
                new ClothesSize(),
                new ShoesSize {Sex = Sex.Men}
            }.AsQueryable());

            var service = new SizesService(sizeRepo.Object, null);
            var sizes = service.GetSizes(CategoryType.Clothes, Sex.Men);
            Assert.Equal(2, sizes.Count);
            sizeRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetSizesShouldReturnSizesShouldFilterBySexIfNotTypeClothes()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            sizeRepo.Setup(r => r.All()).Returns(new List<Size>
            {
                new ShoesSize {Sex = Sex.Kids},
                new ClothesSize {Sex = null},
                new ShoesSize {Sex = Sex.Men, Name = "32"},
                new ShoesSize {Sex = Sex.Women},
            }.AsQueryable());

            var service = new SizesService(sizeRepo.Object, null);
            var sizes = service.GetSizes(CategoryType.Shoes, Sex.Men);
            Assert.Equal(1, sizes.Count);
            sizeRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetSizesShouldReturnSizesShouldFilterBySexIfNotTypeClothesAndOrderByName()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            sizeRepo.Setup(r => r.All()).Returns(new List<Size>
            {
                new ShoesSize {Sex = Sex.Men, Name = "34"},
                new ClothesSize {Sex = null},
                new ShoesSize {Sex = Sex.Men, Name = "32"},
                new ShoesSize {Sex = Sex.Men,  Name = "33"},
            }.AsQueryable());

            var service = new SizesService(sizeRepo.Object, null);
            var sizes = service.GetSizes(CategoryType.Shoes, Sex.Men).ToList();
            Assert.Equal(3, sizes.Count);
            Assert.Equal("32", sizes.First().Name);
            Assert.Equal("33", sizes[1].Name);
            Assert.Equal("34", sizes.Last().Name);
            sizeRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task AddSizeShouldBeCalledOnce()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            var productSizeRepo = new Mock<IRepository<ProductSize>>();
            var sizes = new List<ProductSize>
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
            };

            productSizeRepo.Setup(r => r.All()).Returns(sizes.AsQueryable());
           
            var size = new ProductSize
            {
                ProductId = Guid.NewGuid(),
                SizeId = Guid.NewGuid(),
                Quantity = 1
            };

            productSizeRepo.Setup(r => r.AddAsync(It.IsAny<ProductSize>())).Returns<ProductSize>(Task.FromResult)
                .Callback<ProductSize>(s =>
                {
                    sizes.Add(s);
                });

            var service = new SizesService(sizeRepo.Object, productSizeRepo.Object);
            await service.Create(size);

            Assert.Equal(3, sizes.Count);
            Assert.Contains(sizes, s => s.ProductId == size.ProductId && s.SizeId == size.SizeId && s.Quantity == size.Quantity);
            Assert.DoesNotContain(sizes, s => s.Quantity > 1);

            productSizeRepo.Verify(r => r.AddAsync(size), Times.Once);
            productSizeRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddSizeShouldUpdateQuantityIfSizeExists()
        {
            var sizeRepo = new Mock<IRepository<Size>>();
            var productSizeRepo = new Mock<IRepository<ProductSize>>();
            var sizes = new List<ProductSize>
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
            };

            productSizeRepo.Setup(r => r.All()).Returns(sizes.AsQueryable());

            var service = new SizesService(sizeRepo.Object, productSizeRepo.Object);
            var size = sizes[0];

            await service.Create(size);

            Assert.Equal(2, sizes.Count);
            Assert.Contains(sizes, s => s.Quantity == 2);
            productSizeRepo.Verify(r => r.AddAsync(size), Times.Never);
            productSizeRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }
    }
}
