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
            var result = await service.RestrictUser("1");
            
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
            var result = await service.RestrictUser("1");

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
            var result = await service.UnRestrictUser("1");

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
            var result = await service.UnRestrictUser("1");

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
            var userInfo = new UserInfo {Id = Guid.NewGuid() };
            await service.UpdateUserInfo(userInfo, "stamat");

            userInfoRepo.Verify(r => r.Update(userInfo), Times.Once);
            userInfoRepo.Verify(r => r.SaveChangesAsync(), Times.Once);

            userRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateUserInfoShouldNotCallRepositoryUpdateOnEmptyGuid()
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
            await service.UpdateUserInfo(userInfo, "stamat");

            userInfoRepo.Verify(r => r.Update(userInfo), Times.Never);
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
    }
}
