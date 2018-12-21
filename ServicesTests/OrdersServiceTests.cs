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
    public class OrdersServiceTests
    {
        [Fact]
        public async Task CreateOrdersShouldWork()
        {
            var ordersRepo = new Mock<IRepository<Order>>();
            var productsRepo = new Mock<IRepository<Product>>();
            var usersRepo = new Mock<IRepository<User>>();

            usersRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User {Id = "1", UserName = "stamat"},
                new User {Id = "2", UserName = "pesho"}
            }.AsQueryable());

            var productId = Guid.NewGuid();
            var otherProductId = Guid.NewGuid();

            productsRepo.Setup(r => r.All()).Returns(new List<Product>
            {
                new Product
                {
                    Id = productId,
                    CreatorId = "2",
                    Sizes = new List<ProductSize>
                    {
                        new ProductSize { Quantity = 1, Size = new Size() }
                    }
                },
                new Product
                {
                    Id = otherProductId,
                    CreatorId = "2",
                    Sizes = new List<ProductSize>
                    {
                        new ProductSize { Quantity = 1, Size = new Size() }
                    }
                }
            }.AsQueryable());

            var orders = new List<Order>();
            ordersRepo.Setup(r => r.All()).Returns(orders.AsQueryable());
            ordersRepo.Setup(r => r.AddRangeAsync(It.IsAny<ICollection<Order>>())).Returns<ICollection<Order>>(Task.FromResult)
                .Callback<ICollection<Order>>(o => orders.AddRange(o));

            var service = new OrdersService(ordersRepo.Object, usersRepo.Object, productsRepo.Object);

            var ordersToAdd = new List<Order>
            {
                new Order{ProductId = productId, Quantity = 1},
                new Order{ProductId = otherProductId, Quantity = 1}
            };

            await service.CreateOrders(ordersToAdd, "stamat");

            Assert.Equal(2, orders.Count);
            
            ordersRepo.Verify(r => r.AddRangeAsync(ordersToAdd), Times.Once);
            ordersRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateOrdersShouldNotAllowPurchasingOfOwnProducts()
        {
            var ordersRepo = new Mock<IRepository<Order>>();
            var productsRepo = new Mock<IRepository<Product>>();
            var usersRepo = new Mock<IRepository<User>>();

            usersRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User {Id = "1", UserName = "stamat"},
                new User {Id = "2", UserName = "pesho"}
            }.AsQueryable());

            var productId = Guid.NewGuid();
            var otherProductId = Guid.NewGuid();

            productsRepo.Setup(r => r.All()).Returns(new List<Product>
            {
                new Product
                {
                    Id = productId,
                    CreatorId = "2",
                    Sizes = new List<ProductSize>
                    {
                        new ProductSize { Quantity = 1, Size = new Size() }
                    }
                },
                new Product
                {
                    Id = otherProductId,
                    CreatorId = "1",
                    Sizes = new List<ProductSize>
                    {
                        new ProductSize { Quantity = 1, Size = new Size() }
                    }
                }
            }.AsQueryable());

            var service = new OrdersService(ordersRepo.Object, usersRepo.Object, productsRepo.Object);

            var ordersToAdd = new List<Order>
            {
                new Order{ProductId = productId, Quantity = 1},
                new Order{ProductId = otherProductId, Quantity = 1}
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.CreateOrders(ordersToAdd, "stamat"));

            ordersRepo.Verify(r => r.AddRangeAsync(ordersToAdd), Times.Never);
            ordersRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task CreateOrdersShouldThrowIfProductsQuantityIsNotEnough()
        {
            var ordersRepo = new Mock<IRepository<Order>>();
            var productsRepo = new Mock<IRepository<Product>>();
            var usersRepo = new Mock<IRepository<User>>();

            usersRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User {Id = "1", UserName = "stamat"},
                new User {Id = "2", UserName = "pesho"}
            }.AsQueryable());

            var productId = Guid.NewGuid();
            var otherProductId = Guid.NewGuid();

            productsRepo.Setup(r => r.All()).Returns(new List<Product>
            {
                new Product
                {
                    Id = productId,
                    CreatorId = "2",
                    Sizes = new List<ProductSize>
                    {
                        new ProductSize { Quantity = 1, Size = new Size() }
                    }
                },
                new Product
                {
                    Id = otherProductId,
                    CreatorId = "2",
                    Sizes = new List<ProductSize>
                    {
                        new ProductSize { Quantity = 1, Size = new Size() }
                    }
                }
            }.AsQueryable());

            var service = new OrdersService(ordersRepo.Object, usersRepo.Object, productsRepo.Object);

            var ordersToAdd = new List<Order>
            {
                new Order{ProductId = productId, Quantity = 1},
                new Order{ProductId = otherProductId, Quantity = 2}
            };

            await Assert.ThrowsAsync<InvalidOperationException>(async () =>
                await service.CreateOrders(ordersToAdd, "stamat"));

            ordersRepo.Verify(r => r.AddRangeAsync(ordersToAdd), Times.Never);
            ordersRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void GetSellOrdersShouldWork()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User { UserName = "stamat", SellOrders = new List<Order> {new Order()}},
                new User { UserName = "gosho", SellOrders = new List<Order> {new Order(), new Order()}}
            }.AsQueryable());

            var service = new OrdersService(null, userRepo.Object, null);
            var orders = service.GetSellOrders("stamat");

            Assert.Equal(1, orders.Count);

            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetSellOrdersShouldOrderDescendingByDate()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    UserName = "stamat", SellOrders = new List<Order>
                    {
                        new Order {DateOfCreation = DateTime.Now, ProductName = "3"},
                        new Order {DateOfCreation = DateTime.Now.AddDays(3), ProductName = "1"},
                        new Order {DateOfCreation = DateTime.Now.AddDays(2), ProductName = "2"}
                    }
                },
                new User { UserName = "gosho", SellOrders = new List<Order> {new Order(), new Order()}}
            }.AsQueryable());

            var service = new OrdersService(null, userRepo.Object, null);
            var orders = service.GetSellOrders("stamat").ToList();

            Assert.Equal(3, orders.Count);
            Assert.Equal("1", orders.First().ProductName);
            Assert.Equal("2", orders[1].ProductName);
            Assert.Equal("3", orders.Last().ProductName);
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetPurchaseOrdersShouldWork()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User { UserName = "stamat", PurchaseOrders = new List<Order> {new Order()}},
                new User { UserName = "gosho", PurchaseOrders = new List<Order> {new Order(), new Order()}}
            }.AsQueryable());

            var service = new OrdersService(null, userRepo.Object, null);
            var orders = service.GetPurchaseOrders("stamat");

            Assert.Equal(1, orders.Count);

            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetPurchaseOrdersShouldOrderDescendingByDate()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    UserName = "stamat", PurchaseOrders = new List<Order>
                    {
                        new Order {DateOfCreation = DateTime.Now, ProductName = "3"},
                        new Order {DateOfCreation = DateTime.Now.AddDays(3), ProductName = "1"},
                        new Order {DateOfCreation = DateTime.Now.AddDays(2), ProductName = "2"}
                    }
                },
                new User { UserName = "gosho", SellOrders = new List<Order> {new Order(), new Order()}}
            }.AsQueryable());

            var service = new OrdersService(null, userRepo.Object, null);
            var orders = service.GetPurchaseOrders("stamat").ToList();

            Assert.Equal(3, orders.Count);
            Assert.Equal("1", orders.First().ProductName);
            Assert.Equal("2", orders[1].ProductName);
            Assert.Equal("3", orders.Last().ProductName);
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task AcceptOrderShouldWork()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = orderId,  ProductName = "suit", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Seller = new User {UserName = "gosho"}, IsAccepted = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.AcceptOrder(orderId.ToString(), "stamat");

            Assert.True(result);
            Assert.Contains(orders, o => o.IsAccepted);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AcceptOrderShouldReturnFalseForInvalidGuid()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = orderId,  ProductName = "suit", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Seller = new User {UserName = "gosho"}, IsAccepted = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.AcceptOrder("123", "stamat");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsAccepted);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AcceptOrderShouldReturnFalseForNotFoundOrder()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = orderId,  ProductName = "suit", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Seller = new User {UserName = "gosho"}, IsAccepted = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.AcceptOrder("a02ce201-e961-4419-928b-7214b27eb011", "stamat");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsAccepted);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task AcceptOrderShouldReturnFalseForWrongSeller()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = orderId,  ProductName = "suit", Seller = new User {UserName = "stamat"}, IsAccepted = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Seller = new User {UserName = "gosho"}, IsAccepted = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.AcceptOrder(orderId.ToString(), "gosho");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsAccepted);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ReceiveOrderShouldWork()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = orderId,  ProductName = "suit", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Buyer = new User {UserName = "gosho"}, IsAccepted = true, IsDelivered = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.ReceiveOrder(orderId.ToString(), "stamat");

            Assert.True(result);
            Assert.Contains(orders, o => o.IsDelivered);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task ReceiveOrderShouldReturnFalseForInvalidGuid()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = orderId,  ProductName = "suit", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Buyer = new User {UserName = "gosho"}, IsAccepted = true, IsDelivered = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.ReceiveOrder("123", "stamat");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsDelivered);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ReceiveOrderShouldReturnFalseForNotFoundOrder()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = orderId,  ProductName = "suit", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Buyer = new User {UserName = "gosho"}, IsAccepted = true, IsDelivered = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.ReceiveOrder("a02ce201-e961-4419-928b-7214b27eb011", "stamat");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsDelivered);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ReceiveOrderShouldReturnFalseForWrongSeller()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = orderId,  ProductName = "suit", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Buyer = new User {UserName = "gosho"}, IsAccepted = true, IsDelivered = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.ReceiveOrder(orderId.ToString(), "gosho");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsDelivered);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task ReceiveOrderShouldReturnFalseForNotAcceptedOrder()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket", Buyer = new User {UserName = "stamat"}, IsAccepted = true, IsDelivered = false},
                new Order {Id = orderId,  ProductName = "suit", Buyer = new User {UserName = "stamat"}, IsAccepted = false, IsDelivered = false},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else", Buyer = new User {UserName = "gosho"}, IsAccepted = true, IsDelivered = false}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);

            var result = await service.ReceiveOrder(orderId.ToString(), "stamat");

            Assert.False(result);
            Assert.DoesNotContain(orders, o => o.IsDelivered);

            orderRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void GetOrderDetailsShouldWork()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket"},
                new Order {Id = orderId,  ProductName = "suit"},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else"}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);
            var order = service.GetOrderDetails(orderId.ToString());

            Assert.NotNull(order);
            Assert.Equal(orderId, order.Id);
            Assert.Equal("suit", order.ProductName);
            
            orderRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetOrderDetailsShouldReturnNullForInvalidGuid()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket"},
                new Order {Id = orderId,  ProductName = "suit"},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else"}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);
            var order = service.GetOrderDetails("123");

            Assert.Null(order);

            orderRepo.Verify(r => r.All(), Times.Never);
        }

        [Fact]
        public void GetOrderDetailsShouldReturnNullForValidButNotFoundId()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orderId = Guid.NewGuid();

            var orders = new List<Order>
            {
                new Order {Id = Guid.NewGuid(), ProductName = "jacket"},
                new Order {Id = orderId,  ProductName = "suit"},
                new Order {Id = Guid.NewGuid(), ProductName = "someting else"}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);
            var order = service.GetOrderDetails("df430ca7-3f5e-4558-86c7-8a1541f33747");

            Assert.Null(order);

            orderRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetAllOrdersShouldWork()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orders = new List<Order>
            {
                new Order(),
                new Order(),
                new Order()
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);
            var checkOrders = service.GetAllOrders();

            Assert.Equal(3, checkOrders.Count);
            orderRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetAllOrdersShouldOrderDescendingByDate()
        {
            var orderRepo = new Mock<IRepository<Order>>();
            var orders = new List<Order>
            {
                new Order {DateOfCreation = DateTime.Now, ProductName = "3"},
                new Order {DateOfCreation = DateTime.Now.AddDays(3), ProductName = "1"},
                new Order {DateOfCreation = DateTime.Now.AddDays(2), ProductName = "2"}
            };

            orderRepo.Setup(r => r.All()).Returns(orders.AsQueryable());

            var service = new OrdersService(orderRepo.Object, null, null);
            var checkOrders = service.GetAllOrders().ToList();

            Assert.Equal(3, checkOrders.Count);
            Assert.Equal("1", checkOrders.First().ProductName);
            Assert.Equal("2", checkOrders[1].ProductName);
            Assert.Equal("3", checkOrders.Last().ProductName);
            orderRepo.Verify(r => r.All(), Times.Once);
        }
    }
}
