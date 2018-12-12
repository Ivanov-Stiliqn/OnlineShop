using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories.Contracts;
using Microsoft.AspNetCore.Identity;
using Models;
using Models.Enums;
using Moq;
using Services;
using Xunit;

namespace ServicesTests
{
    public class ProductsServiceTests
    {
        [Fact]
        public async Task CreateProductShouldWork()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product(),
                new Product()
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());
            productRepo.Setup(r => r.AddAsync(It.IsAny<Product>())).Returns<Product>(Task.FromResult)
                .Callback<Product>(p => products.Add(p));

            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User> {new User {UserName = "stamat"}}.AsQueryable());

            var service = new ProductsService(productRepo.Object, userRepo.Object, null);
            var newProduct = new Product { Id = Guid.NewGuid() };
            await service.CreateProduct(newProduct, "stamat");

            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Id == newProduct.Id);
            productRepo.Verify(r => r.AddAsync(newProduct), Times.Once);
        }

        [Fact]
        public void GetLatestProductsShouldOrderAscendingByDate()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {DateOfCreation = DateTime.Now.AddDays(1), Name = "Jacket"},
                new Product {DateOfCreation = DateTime.Now.AddDays(3), Name = "Suit"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());
            
            var service = new ProductsService(productRepo.Object, null, null);
            var checkProducts = service.GetLatestProducts().ToList();

            Assert.Equal(3, checkProducts.Count);
            Assert.Equal("Jacket", checkProducts.First().Name);
            Assert.Equal("T-shirt", checkProducts[1].Name);
            Assert.Equal("Suit", checkProducts.Last().Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetLatestProductsShouldOrderAscendingByDateAndTakeEight()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {DateOfCreation = DateTime.Now.AddDays(1), Name = "Jacket"},
                new Product {DateOfCreation = DateTime.Now.AddDays(3), Name = "Suit"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
                new Product{DateOfCreation = DateTime.Now.AddDays(2), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var checkProducts = service.GetLatestProducts().ToList();

            Assert.Equal(8, checkProducts.Count);
            Assert.Equal("Jacket", checkProducts.First().Name);
            Assert.Equal("T-shirt", checkProducts[1].Name);
            Assert.Equal("T-shirt", checkProducts.Last().Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetMostViewedProductsShouldOrderDescendingByViews()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {Views = 1, Name = "Jacket"},
                new Product {Views = 3, Name = "Suit"},
                new Product{Views = 2, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var checkProducts = service.GetMostViewedProducts().ToList();

            Assert.Equal(3, checkProducts.Count);
            Assert.Equal("Suit", checkProducts.First().Name);
            Assert.Equal("T-shirt", checkProducts[1].Name);
            Assert.Equal("Jacket", checkProducts.Last().Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetMostViewedProductsShouldOrderDescendingByViewsAndTakeEight()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {Views = 1, Name = "Jacket"},
                new Product {Views = 3, Name = "Suit"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"},
                new Product{Views = 2, Name = "T-shirt"}
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var checkProducts = service.GetMostViewedProducts().ToList();

            Assert.Equal(8, checkProducts.Count);
            Assert.Equal("Suit", checkProducts.First().Name);
            Assert.Equal("T-shirt", checkProducts[1].Name);
            Assert.Equal("T-shirt", checkProducts.Last().Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetMostOrderedProductsShouldOrderDescendingByOrdersCount()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {Orders = new List<Order>{new Order()}, Name = "Jacket"},
                new Product {Orders = new List<Order>{new Order(), new Order(), new Order()}, Name = "Suit"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var checkProducts = service.GetMostOrderedProducts().ToList();

            Assert.Equal(3, checkProducts.Count);
            Assert.Equal("Suit", checkProducts.First().Name);
            Assert.Equal("T-shirt", checkProducts[1].Name);
            Assert.Equal("Jacket", checkProducts.Last().Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetMostViewedProductsShouldOrderDescendingByOrdersCountAndTakeTwelve()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {Orders = new List<Order>{new Order()}, Name = "Jacket"},
                new Product {Orders = new List<Order>{new Order(), new Order(), new Order()}, Name = "Suit"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var checkProducts = service.GetMostOrderedProducts().ToList();

            Assert.Equal(12, checkProducts.Count);
            Assert.Equal("Suit", checkProducts.First().Name);
            Assert.Equal("T-shirt", checkProducts[1].Name);
            Assert.Equal("T-shirt", checkProducts.Last().Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetTopProductShouldWork()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var products = new List<Product>
            {
                new Product {Orders = new List<Order>{new Order()}, Name = "Jacket"},
                new Product {Orders = new List<Order>{new Order(), new Order(), new Order()}, Name = "Suit"},
                new Product{Orders = new List<Order>{new Order(), new Order()}, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var topProduct = service.GetTopProduct();

            Assert.NotNull(topProduct);
            Assert.Equal("Suit", topProduct.Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductShouldReturnCorrectProductAndIncreaseViews()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Views = 1, Name = "Jacket"},
                new Product {Id = productId, Views = 1, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Views = 1, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = await service.GetProduct(productId.ToString(), true);

            Assert.NotNull(product);
            Assert.Equal(2, product.Views);
            Assert.Equal("Suit", product.Name);
            productRepo.Verify(r => r.All(), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetProductShouldReturnNullForInvalidGuid()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Views = 1, Name = "Jacket"},
                new Product {Id = productId, Views = 1, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Views = 1, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = await service.GetProduct("123", true);

            Assert.Null(product);
            productRepo.Verify(r => r.All(), Times.Never);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetProductShouldReturnNullForNotFoundId()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Views = 1, Name = "Jacket"},
                new Product {Id = productId, Views = 1, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Views = 1, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = await service.GetProduct(Guid.NewGuid().ToString(), true);

            Assert.Null(product);
            productRepo.Verify(r => r.All(), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task GetProductShouldReturnProductButNotIncreaseViews()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Views = 1, Name = "Jacket"},
                new Product {Id = productId, Views = 1, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Views = 1, Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = await service.GetProduct(productId.ToString(), false);

            Assert.NotNull(product);
            Assert.Equal(1, product.Views);
            Assert.Equal("Suit", product.Name);
            productRepo.Verify(r => r.All(), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void GetProductForCartShouldReturnCorrectProduct()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = service.GetProductForCart(productId.ToString());

            Assert.NotNull(product);
            Assert.Equal("Suit", product.Name);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetProductForCartShouldReturnNullForInvalidGuid()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = service.GetProductForCart("123");

            Assert.Null(product);
            productRepo.Verify(r => r.All(), Times.Never);
        }

        [Fact]
        public void GetProductForCartShouldReturnNullForNotFoundId()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit"},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);
            var product = service.GetProductForCart(Guid.NewGuid().ToString());

            Assert.Null(product);
            productRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task EditProductShouldWork()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product
                {
                    Id = productId,
                    Name = "Suit",
                    CategoryId = Guid.NewGuid(),
                    Color = "black",
                    Creator = new User()
                    {
                        UserName = "stamat"
                    },
                    Description = "some",
                    Details = "some",
                    ImageUrls = "1, 2",
                    Sex = Sex.Men
                },
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);

            var editedProduct = new Product
            {
                Id = productId,
                Name = "Suit Edited",
                CategoryId = Guid.NewGuid(),
                Color = "black edited",
                Description = "some edited",
                Details = "some edited",
                Sex = Sex.Women
            };

            var result = await service.EditProduct(editedProduct, "stamat", new List<string> {"3", "4"});

            Assert.True(result);
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Id == editedProduct.Id);
            Assert.Contains(products, p => p.Name == editedProduct.Name);
            Assert.Contains(products, p => p.CategoryId == editedProduct.CategoryId);
            Assert.Contains(products, p => p.Color == editedProduct.Color);
            Assert.Contains(products, p => p.Description == editedProduct.Description);
            Assert.Contains(products, p => p.Details == editedProduct.Details);
            Assert.Contains(products, p => p.Sex == editedProduct.Sex);
            Assert.Contains(products, p => p.ImageUrls == "1, 2, 3, 4");
            productRepo.Verify(r => r.All(), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EditProductShouldReturnFalseIfCreatorIsNotTheSame()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product
                {
                    Id = productId,
                    Name = "Suit",
                    CategoryId = Guid.NewGuid(),
                    Color = "black",
                    Creator = new User()
                    {
                        UserName = "stamat"
                    },
                    Description = "some",
                    Details = "some",
                    ImageUrls = "1, 2",
                    Sex = Sex.Men
                },
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var service = new ProductsService(productRepo.Object, null, null);

            var editedProduct = new Product
            {
                Id = productId,
                Name = "Suit Edited",
                CategoryId = Guid.NewGuid(),
                Color = "black edited",
                Description = "some edited",
                Details = "some edited",
                Sex = Sex.Women
            };

            var result = await service.EditProduct(editedProduct, "nqkuv", new List<string> { "3", "4" });

            Assert.False(result);
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Id == editedProduct.Id);
            Assert.DoesNotContain(products, p => p.Name == editedProduct.Name);
            Assert.DoesNotContain(products, p => p.CategoryId == editedProduct.CategoryId);
            Assert.DoesNotContain(products, p => p.Color == editedProduct.Color);
            Assert.DoesNotContain(products, p => p.Description == editedProduct.Description);
            Assert.DoesNotContain(products, p => p.Details == editedProduct.Details);
            Assert.DoesNotContain(products, p => p.Sex == editedProduct.Sex);
            Assert.DoesNotContain(products, p => p.ImageUrls == "1, 2, 3, 4");
            productRepo.Verify(r => r.All(), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteProductShouldWork()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var productCreator = new User {UserName = "stamat"};
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit", Creator = productCreator},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());
            productRepo.Setup(r => r.Delete(It.IsAny<Product>())).Callback<Product>(p => products.Remove(p));

            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(r => r.All()).Returns(new List<User> { productCreator }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(r => r.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var service = new ProductsService(productRepo.Object, usersRepo.Object, userManager.Object);

            var result = await service.DeleteProduct(productId.ToString(), "stamat");

            Assert.True(result);
            Assert.Equal(2, products.Count);
            Assert.DoesNotContain(products, p => p.Id == productId);
            productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProductShouldWorkForCreator()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var productCreator = new User { UserName = "stamat" };
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit", Creator = productCreator},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());
            productRepo.Setup(r => r.Delete(It.IsAny<Product>())).Callback<Product>(p => products.Remove(p));

            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(r => r.All()).Returns(new List<User> { productCreator }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(r => r.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var service = new ProductsService(productRepo.Object, usersRepo.Object, userManager.Object);

            var result = await service.DeleteProduct(productId.ToString(), "stamat");

            Assert.True(result);
            Assert.Equal(2, products.Count);
            Assert.DoesNotContain(products, p => p.Id == productId);
            productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProductShouldWorkForAdminNotCreator()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var productCreator = new User { UserName = "stamat" };
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit", Creator = productCreator},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());
            productRepo.Setup(r => r.Delete(It.IsAny<Product>())).Callback<Product>(p => products.Remove(p));

            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(r => r.All()).Returns(new List<User> { productCreator, new User() {UserName = "nqkuv"} }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(r => r.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var service = new ProductsService(productRepo.Object, usersRepo.Object, userManager.Object);

            var result = await service.DeleteProduct(productId.ToString(), "nqkuv");

            Assert.True(result);
            Assert.Equal(2, products.Count);
            Assert.DoesNotContain(products, p => p.Id == productId);
            productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Once);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteProductShouldReturnFalseForUserNotAdminNorCreator()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var productCreator = new User { UserName = "stamat" };
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit", Creator = productCreator},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(r => r.All()).Returns(new List<User> { productCreator, new User {UserName = "nqkuv"} }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(r => r.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(false));

            var service = new ProductsService(productRepo.Object, usersRepo.Object, userManager.Object);

            var result = await service.DeleteProduct(Guid.NewGuid().ToString(), "nqkuv");

            Assert.False(result);
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Id == productId);
            productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteProductShouldReturnFalseForInvalidGuidId()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var productCreator = new User { UserName = "stamat" };
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit", Creator = productCreator},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(r => r.All()).Returns(new List<User> { productCreator }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(r => r.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var service = new ProductsService(productRepo.Object, usersRepo.Object, userManager.Object);

            var result = await service.DeleteProduct("123", "stamat");

            Assert.False(result);
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Id == productId);
            productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteProductShouldReturnFalseForNotFoundProduct()
        {
            var productRepo = new Mock<IRepository<Product>>();
            var productId = Guid.NewGuid();
            var productCreator = new User { UserName = "stamat" };
            var products = new List<Product>
            {
                new Product {Id = Guid.NewGuid(), Name = "Jacket"},
                new Product {Id = productId, Name = "Suit", Creator = productCreator},
                new Product{Id = Guid.NewGuid(), Name = "T-shirt"},
            };

            productRepo.Setup(r => r.All()).Returns(products.AsQueryable());

            var usersRepo = new Mock<IRepository<User>>();
            usersRepo.Setup(r => r.All()).Returns(new List<User> { productCreator }.AsQueryable());

            var userStore = new Mock<IUserStore<User>>();
            var userManager = new Mock<UserManager<User>>(userStore.Object, null, null, null, null, null, null, null, null);
            userManager.Setup(r => r.IsInRoleAsync(It.IsAny<User>(), It.IsAny<string>())).Returns(Task.FromResult(true));

            var service = new ProductsService(productRepo.Object, usersRepo.Object, userManager.Object);

            var result = await service.DeleteProduct(Guid.NewGuid().ToString(), "stamat");

            Assert.False(result);
            Assert.Equal(3, products.Count);
            Assert.Contains(products, p => p.Id == productId);
            productRepo.Verify(r => r.Delete(It.IsAny<Product>()), Times.Never);
            productRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
