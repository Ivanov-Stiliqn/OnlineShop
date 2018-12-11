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
    public class UsersServiceTests
    {
        [Fact]
        public void AllUsersShouldReturnAllUsersExceptCurrentOne()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var users = service.AllUsers("stamat");

            Assert.Equal(2, users.Count);
            Assert.DoesNotContain(users, u => u.UserName == "stamat");
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void SearchByUsernameShouldNotReturnCurrentOne()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var users = service.SearchByName("stamat", "stamat");

            Assert.Equal(0, users.Count);
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void SearchByUsernameShouldReturnMatchedUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var users = service.SearchByName("stamat", "gosho");

            Assert.Equal(1, users.Count);
            Assert.Contains(users, u => u.UserName == "stamat");
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void SearchByUsernameShouldReturnMatchedUserCaseInsensitive()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var users = service.SearchByName("sTaMat", "gosho");

            Assert.Equal(1, users.Count);
            Assert.Contains(users, u => u.UserName == "stamat");
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task RestrictUserShouldWorkWithExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var result = await service.RestrictUser("1", "nqkuv");

            var users = service.AllUsers("4");

            Assert.Contains(users, u => u.IsRestricted);
            Assert.True(result);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RestrictUserShouldReturnFalseForNonExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "0",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var result = await service.RestrictUser("1", "nqkuv");
            var users = service.AllUsers("1");

            Assert.DoesNotContain(users, u => u.IsRestricted);
            Assert.False(result);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RestrictUserShouldReturnFalseOnTryingUnRestrictingCurrentUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = false
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var result = await service.RestrictUser("1", "stamat");

            var users = service.AllUsers("4");

            Assert.DoesNotContain(users, u => u.IsRestricted);
            Assert.False(result);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UnRestrictUserShouldWorkWithExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var result = await service.UnRestrictUser("1", "nqkuv");

            var users = service.AllUsers("4");

            Assert.Contains(users, u => !u.IsRestricted);
            Assert.True(result);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UnRestrictUserShouldReturnFalseForNonExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "0",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var result = await service.UnRestrictUser("1", "nqkuv");
            var users = service.AllUsers("1");

            Assert.DoesNotContain(users, u => !u.IsRestricted);
            Assert.False(result);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task UnRestrictUserShouldReturnFalseOnTryingUnRestrictingCurrentUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    IsRestricted = true
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var result = await service.UnRestrictUser("1", "stamat");

            var users = service.AllUsers("4");

            Assert.DoesNotContain(users, u => !u.IsRestricted);
            Assert.False(result);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void GetUserInfoShouldReturnCurrentUserInfo()
        {
            var userRepo = new Mock<IRepository<User>>();
            var userInfoId = Guid.NewGuid();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    UserInfo = new UserInfo() { Id = userInfoId }
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var userInfo = service.GetUserInfo("stamat");
            Assert.Equal(userInfoId, userInfo.Id);
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void GetUserInfoShouldReturnNullForNonExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            var userInfoId = Guid.NewGuid();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    UserInfo = new UserInfo() { Id = userInfoId }
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);
            var userInfo = service.GetUserInfo("nqkuv");
            Assert.Null(userInfo);
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoShouldCallRepositoryUpdateOnValidGuid()
        {
            var userRepo = new Mock<IRepository<User>>();
            var userInfoRepo = new Mock<IRepository<UserInfo>>();
            var seededUserInfo = new UserInfo() {Id = Guid.NewGuid(), FirstName = "Stamat"};

            userInfoRepo.Setup(r => r.All()).Returns(new List<UserInfo>
            {
                seededUserInfo
            }.AsQueryable());

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    UserInfo = seededUserInfo
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid(), FirstName = "Pesho"}
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid(), FirstName = "Gosho"}
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, userInfoRepo.Object, null);

            seededUserInfo.FirstName = "Not Stamat";
            await service.UpdateUserInfo(seededUserInfo, "stamat");
           
            var updatedUserInfo = service.GetUserInfo("stamat");
            Assert.Equal("Not Stamat", updatedUserInfo.FirstName);
            Assert.Equal(seededUserInfo.Id, updatedUserInfo.Id);
            userInfoRepo.Verify(r => r.Update(seededUserInfo), Times.Once);
            userInfoRepo.Verify(r => r.SaveChangesAsync(), Times.Once);

            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoShouldNotCallRepositoryUpdateOnEmptyGuid()
        {
            var userRepo = new Mock<IRepository<User>>();
            var userInfoRepo = new Mock<IRepository<UserInfo>>();
            var seededUserInfo = new UserInfo() { Id = Guid.Empty, FirstName = "Stamat" };

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat"
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid(), FirstName = "Pesho"}
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid(), FirstName = "Gosho"}
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, userInfoRepo.Object, null);

            seededUserInfo.FirstName = "Stamat";
            await service.UpdateUserInfo(seededUserInfo, "stamat");

            var updatedUserInfo = service.GetUserInfo("stamat");
            Assert.Equal("Stamat", updatedUserInfo.FirstName);

            userInfoRepo.Verify(r => r.Update(seededUserInfo), Times.Never);
            userInfoRepo.Verify(r => r.SaveChangesAsync(), Times.Never);

            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoShouldThrowErrorOnNotExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            var userInfoRepo = new Mock<IRepository<UserInfo>>();

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    UserInfo = new UserInfo() { Id = Guid.NewGuid() }
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, userInfoRepo.Object, null);
            var userInfo = new UserInfo { Id = Guid.Empty };

            await Assert.ThrowsAsync<NullReferenceException>(
                async () => await service.UpdateUserInfo(userInfo, "nqkuv"));
        }

        [Fact]
        public async Task AddToWishListShouldWorkAndCallSaveChanges()
        {
            var userRepo = new Mock<IRepository<User>>();
            var productsRepo = new Mock<IRepository<Product>>();

            var productId = Guid.NewGuid();
            var otherProductId = Guid.NewGuid();

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    Whishlist = otherProductId.ToString()
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            
            productsRepo.Setup(r => r.All()).Returns(new List<Product>
            {
                new Product() {Id = productId},
                new Product() {Id = otherProductId},
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, productsRepo.Object);

            await service.AddProductToWhishlist(productId.ToString(), "stamat");

            var wishlist = service.GetWishList("stamat");

            Assert.Equal(2, wishlist.Count);
            Assert.Contains(wishlist, p => p.Id == productId);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddingSameProductToWishListShouldCallDb()
        {
            var userRepo = new Mock<IRepository<User>>();
            var productsRepo = new Mock<IRepository<Product>>();

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    Whishlist = ""
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var productId = Guid.NewGuid();
            productsRepo.Setup(r => r.All()).Returns(new List<Product>
            {
                new Product() {Id = productId},
                new Product() {Id = Guid.NewGuid()},
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, productsRepo.Object);

            await service.AddProductToWhishlist(productId.ToString(), "stamat");
            await service.AddProductToWhishlist(productId.ToString(), "stamat");

            var wishlist = service.GetWishList("stamat");

            Assert.Equal(1 , wishlist.Count);
            Assert.Contains(wishlist, p => p.Id == productId);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task AddToWishListShouldThrowIfUserDoesNotExist()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.AddProductToWhishlist("1", "nqkuv"));

            userRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public async Task RemoveProductFromWishListShouldWork()
        {
            var userRepo = new Mock<IRepository<User>>();
            var productsRepo = new Mock<IRepository<Product>>();

            var productId = Guid.NewGuid();
            var otherProductId = Guid.NewGuid();

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>(),
                    Whishlist = productId + ", " + otherProductId
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());


            productsRepo.Setup(r => r.All()).Returns(new List<Product>
            {
                new Product() {Id = productId},
                new Product() {Id = otherProductId},
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, productsRepo.Object);

            await service.RemoveProductFromWishlist(otherProductId.ToString(), "stamat");

            var wishlist = service.GetWishList("stamat");

            Assert.Equal(1, wishlist.Count);
            Assert.DoesNotContain(wishlist, p => p.Id == otherProductId);
            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task RemoveProductShouldThrowIfUserDoesNotExist()
        {
            var userRepo = new Mock<IRepository<User>>();
            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>()
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);

            await Assert.ThrowsAsync<NullReferenceException>(async () => await service.RemoveProductFromWishlist("1", "nqkuv"));

            userRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
        }

        [Fact]
        public void UserProductsShouldReturnOnlyUserProducts()
        {
            var userRepo = new Mock<IRepository<User>>();
            var productId = Guid.NewGuid();

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>{ new Product { Id = productId} }
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>{ new Product { Id = Guid.NewGuid()}, new Product { Id = Guid.NewGuid() } }
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>{ new Product { Id = Guid.NewGuid()}, new Product { Id = Guid.NewGuid() } }
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);

            var userProducts = service.GetUserProducts("stamat");
            Assert.Equal(1, userProducts.Count);
            Assert.Contains(userProducts, p => p.Id == productId);
            userRepo.Verify(r => r.All(), Times.Once);
        }

        [Fact]
        public void UserProductsShouldReturnNullForNonExistingUser()
        {
            var userRepo = new Mock<IRepository<User>>();
            var productId = Guid.NewGuid();

            userRepo.Setup(r => r.All()).Returns(new List<User>
            {
                new User
                {
                    Id = "1",
                    UserName = "stamat",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>{ new Product { Id = productId} }
                },
                new User
                {
                    Id = "2",
                    UserName = "pesho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>{ new Product { Id = Guid.NewGuid()}, new Product { Id = Guid.NewGuid() } }
                },
                new User
                {
                    Id = "3",
                    UserName = "gosho",
                    PurchaseOrders = new List<Order>(),
                    Reports = new List<Report>(),
                    MyProducts = new List<Product>{ new Product { Id = Guid.NewGuid()}, new Product { Id = Guid.NewGuid() } }
                }
            }.AsQueryable());

            var service = new UsersService(userRepo.Object, null, null);

            var userProducts = service.GetUserProducts("nqkuv");
            Assert.Null(userProducts);
            userRepo.Verify(r => r.All(), Times.Once);
        }
    }
}
