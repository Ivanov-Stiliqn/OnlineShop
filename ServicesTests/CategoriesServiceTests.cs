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
    public class CategoriesServiceTests
    {
        [Fact]
        public async Task CreateCategoryShouldWork()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category(),
                new Category()
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());
            categoriesRepo.Setup(r => r.AddAsync(It.IsAny<Category>())).Returns<Category>(Task.FromResult)
                .Callback<Category>(c => categories.Add(c));

            var service = new CategoriesService(categoriesRepo.Object);

            var category = new Category {Name = "Jackets"};
            var result = await service.Create(category);

            Assert.True(result);
            Assert.Equal(3, categories.Count);
            Assert.Contains(categories, c => c.Name == category.Name);
            categoriesRepo.Verify(r => r.AddAsync(category), Times.Once);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateCategoryShouldReturnFalseForAlreadyExistingCategory()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets"},
                new Category()
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());
           
            var service = new CategoriesService(categoriesRepo.Object);

            var category = new Category {Name = "Jackets"};
            var result = await service.Create(category);

            Assert.False(result);
            Assert.Equal(2, categories.Count);
            categoriesRepo.Verify(r => r.AddAsync(category), Times.Never);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void GetCategoriesShouldReturnAllCategories()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category(),
                new Category(),
                new Category()
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var checkCategories = service.GetCategories();

            Assert.Equal(3, checkCategories.Count);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetTopCategoriesShouldOrderDescending()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Views = 1},
                new Category {Name = "Suits", Views = 3}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var checkCategories = service.GetTopCategories();

            Assert.Equal(2, checkCategories.Count);
            Assert.Equal("Suits", checkCategories.First().Name);
            Assert.Equal("Jackets", checkCategories.Last().Name);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetTopCategoriesShouldOrderDescendingAdnReturnTopTwo()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Views = 1},
                new Category {Name = "Suits", Views = 3},
                new Category {Name = "Something", Views = 2},
                new Category {Name = "T-shirts", Views = 2}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var checkCategories = service.GetTopCategories();

            Assert.Equal(2, checkCategories.Count);
            Assert.Equal("Suits", checkCategories.First().Name);
            Assert.Equal("Something", checkCategories.Last().Name);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithoutFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 0.0m;
            decimal maxPrice = 0.0m;
            Guid sizeId = Guid.Empty;


            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product(),
                        new Product(),
                        new Product(),
                        new Product(),
                        new Product(),
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, 0);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(5, productsTotalCount);
            Assert.Equal(3, products.Count);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldOrderAscendingAndIncreaseViewsWithoutFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 0.0m;
            decimal maxPrice = 0.0m;
            Guid sizeId = Guid.Empty;


            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product {DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1"},
                        new Product {DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1"},
                        new Product {DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3"},
                        new Product {DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4"},
                        new Product {DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5"},
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, 0);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(5, productsTotalCount);
            Assert.Equal(3, products.Count);
            Assert.Equal("test 5", products.First().Name);
            Assert.Equal("test 4", products.ToList()[1].Name);
            Assert.Equal("test 1", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithPriceFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 12.3m;
            decimal maxPrice = 22.1m;
            Guid sizeId = Guid.Empty;


            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product {DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1", Price = 10},
                        new Product {DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1", Price = 11},
                        new Product {DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3", Price = 12.4m},
                        new Product {DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4", Price = 14},
                        new Product {DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5", Price = 16},
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, 0);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(3, productsTotalCount);
            Assert.Equal(3, products.Count);
            Assert.Equal("test 5", products.First().Name);
            Assert.Equal("test 4", products.ToList()[1].Name);
            Assert.Equal("test 3", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithSizeFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 0.0m;
            decimal maxPrice = 0.0m;
            Guid sizeId = Guid.NewGuid();

            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}}
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}}
                        },
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, 0);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(3, productsTotalCount);
            Assert.Equal(3, products.Count);
            Assert.Equal("test 1", products.First().Name);
            Assert.Equal("test 1", products.ToList()[1].Name);
            Assert.Equal("test 3", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithSizeAndPriceFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 10.3m;
            decimal maxPrice = 20.2m;
            Guid sizeId = Guid.NewGuid();

            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}, Price = 10.4m
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}, Price = 10.5m
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}, Price = 30.4m
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}}, Price = 10.4m
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}}, Price = 10.4m
                        },
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, 0);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(2, productsTotalCount);
            Assert.Equal(2, products.Count);
            Assert.Equal("test 1", products.First().Name);
            Assert.Equal("test 1", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithSexFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 0.0m;
            decimal maxPrice = 0.0m;
            Guid sizeId = Guid.Empty;
            Sex sex = Sex.Men;

            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sex = Sex.Kids,

                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3",
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4",
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5",
                            Sex = Sex.Men
                        },
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, sex);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(4, productsTotalCount);
            Assert.Equal(3, products.Count);
            Assert.Equal("test 5", products.First().Name);
            Assert.Equal("test 4", products.ToList()[1].Name);
            Assert.Equal("test 1", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithSexAdnSizeFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 0.0m;
            decimal maxPrice = 0.0m;
            Guid sizeId = Guid.NewGuid();
            Sex sex = Sex.Men;

            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sex = Sex.Kids,
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}}

                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}},
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}},
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}},
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5",
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}},
                            Sex = Sex.Men
                        },
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, sex);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(2, productsTotalCount);
            Assert.Equal(2, products.Count);
            Assert.Equal("test 1", products.First().Name);
            Assert.Equal("test 3", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithSexAndPriceFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 10.1m;
            decimal maxPrice = 20.2m;
            Guid sizeId = Guid.Empty;
            Sex sex = Sex.Men;

            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sex = Sex.Kids,
                            Price = 10.5m

                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Price = 10.5m,
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3",
                            Price = 20.0m,
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4",
                            Price = 30.5m,
                            Sex = Sex.Men
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5",
                            Price = 310.5m,
                            Sex = Sex.Men
                        },
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, sex);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(2, productsTotalCount);
            Assert.Equal(2, products.Count);
            Assert.Equal("test 1", products.First().Name);
            Assert.Equal("test 3", products.Last().Name);
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task GetProductsByCategoryShouldWorkAndIncreaseViewsWithSizeSexAndPriceFilters()
        {
            Guid categoryId = Guid.NewGuid();
            int skip = 0;
            int take = 3;
            decimal minPrice = 10.1m;
            decimal maxPrice = 20.2m;
            Guid sizeId = Guid.NewGuid();
            Sex sex = Sex.Men;

            var categoriesRepo = new Mock<IRepository<Category>>();
            var categories = new List<Category>()
            {
                new Category
                {
                    Id = categoryId,
                    Name = "Jackets",
                    Views = 1,
                    Products = new List<Product>
                    {
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Sex = Sex.Kids,
                            Price = 10.5m,
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}},

                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(3), Name = "test 1",
                            Price = 10.5m,
                            Sex = Sex.Men,
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = sizeId}},
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(4), Name = "test 3",
                            Price = 20.0m,
                            Sex = Sex.Men,
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}},
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(2), Name = "test 4",
                            Price = 30.5m,
                            Sex = Sex.Men,
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}},
                        },
                        new Product
                        {
                            DateOfCreation = DateTime.Now.AddDays(1), Name = "test 5",
                            Price = 310.5m,
                            Sex = Sex.Men,
                            Sizes = new List<ProductSize> {new ProductSize {SizeId = Guid.NewGuid()}},
                        },
                    }
                },
                new Category {Id = Guid.NewGuid(), Name = "Suits", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "Something", Views = 1},
                new Category {Id = Guid.NewGuid(), Name = "T-shirts", Views = 1}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var products = await service.GetProductsByCategory(categoryId, skip, take, minPrice, maxPrice, sizeId, sex);
            var productsTotalCount = service.SearchedProductsCount;

            Assert.Equal(1, productsTotalCount);
            Assert.Equal(1, products.Count);
            Assert.Equal("test 1", products.First().Name);
       
            Assert.Contains(categories, c => c.Views > 1);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetCategoryShouldWork()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);
            var category = service.GetCategory(categoryId.ToString());

            Assert.NotNull(category);
            Assert.Equal("Jackets", category.Name);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetCategoryShouldReturnNullForInvalidGuid()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);
            var category = service.GetCategory("123");

            Assert.Null(category);
            categoriesRepo.Verify(r => r.All(), Times.Never);
        }

        [Fact]
        public void GetCategoryShouldReturnNullForNotFoundId()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);
            var category = service.GetCategory(Guid.NewGuid().ToString());

            Assert.Null(category);
            categoriesRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task EditCategoryShouldWork()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId, Image = "1"},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var categoryForEdit = new Category
            {
                Id = categoryId,
                Name = "Jackets Edited",
                Image = "2"
            };

            await service.EditCategory(categoryForEdit);

            Assert.Contains(categories, c => c.Name == categoryForEdit.Name);
            Assert.Contains(categories, c => c.Image == categoryForEdit.Image);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EditCategoryShouldNotWorkForEmptyImage()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId, Image = "1"},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var categoryForEdit = new Category
            {
                Id = categoryId,
                Name = "Jackets Edited",
                Image = ""
            };

            await service.EditCategory(categoryForEdit);

            Assert.Contains(categories, c => c.Name == categoryForEdit.Name);
            Assert.DoesNotContain(categories, c => c.Image == categoryForEdit.Image);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task EditCategoryShouldNotWorkForMissingImage()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId, Image = "1"},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var categoryForEdit = new Category
            {
                Id = categoryId,
                Name = "Jackets Edited"
            };

            await service.EditCategory(categoryForEdit);

            Assert.Contains(categories, c => c.Name == categoryForEdit.Name);
            Assert.Contains(categories, c => c.Image == "1");
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryShouldWork()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());
            categoriesRepo.Setup(r => r.Delete(It.IsAny<Category>())).Callback<Category>(c => categories.Remove(c));

            var service = new CategoriesService(categoriesRepo.Object);

            var result = await service.DeleteCategory(categoryId.ToString());

            Assert.True(result);
            Assert.Equal(3, categories.Count);
            Assert.DoesNotContain(categories, c => c.Id == categoryId);
            Assert.DoesNotContain(categories, c => c.Name == "Jackets");
            categoriesRepo.Verify(r => r.Delete(It.IsAny<Category>()), Times.Once);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCategoryShouldReturnFalseForInvalidGuid()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var result = await service.DeleteCategory("123");

            Assert.False(result);
            Assert.Equal(4, categories.Count);
            Assert.Contains(categories, c => c.Id == categoryId);
            Assert.Contains(categories, c => c.Name == "Jackets");
            categoriesRepo.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task DeleteCategoryShouldReturnFalseForNotFoundId()
        {
            var categoriesRepo = new Mock<IRepository<Category>>();
            var categoryId = Guid.NewGuid();
            var categories = new List<Category>()
            {
                new Category {Name = "Jackets", Id = categoryId},
                new Category {Name = "Suits", Id = Guid.NewGuid()},
                new Category {Name = "Something", Id = Guid.NewGuid()},
                new Category {Name = "T-shirts", Id = Guid.NewGuid()}
            };

            categoriesRepo.Setup(r => r.All()).Returns(categories.AsQueryable());

            var service = new CategoriesService(categoriesRepo.Object);

            var result = await service.DeleteCategory(Guid.NewGuid().ToString());

            Assert.False(result);
            Assert.Equal(4, categories.Count);
            Assert.Contains(categories, c => c.Id == categoryId);
            Assert.Contains(categories, c => c.Name == "Jackets");
            categoriesRepo.Verify(r => r.Delete(It.IsAny<Category>()), Times.Never);
            categoriesRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }
    }
}
